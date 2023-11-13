// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;

namespace Addax.Formats.Tabular;

internal sealed class TabularFormatter(TabularDialect dialect)
{
    private readonly TabularSearchValues _searchValues = dialect.SearchValues;

    private readonly string _tokenT = dialect.LineTerminator;
    private readonly char _tokenD = dialect.Delimiter;
    private readonly char _tokenQ = dialect.QuoteSymbol;
    private readonly char _tokenE = dialect.EscapeSymbol;
    private readonly char? _tokenA = dialect.AnnotationPrefix;

    public void WriteDelimiter(IBufferWriter<char> writer)
    {
        var target = writer.GetSpan(1);

        target[0] = _tokenD;
        writer.Advance(1);
    }

    public void WriteLineTerminator(IBufferWriter<char> writer)
    {
        var tokenT = _tokenT.AsSpan();
        var target = writer.GetSpan(_tokenT.Length);

        tokenT.CopyTo(target);
        writer.Advance(tokenT.Length);
    }

    public void WriteValue(ReadOnlySpan<char> source, IBufferWriter<char> writer, TabularTextInfo textInfo)
    {
        if (source.IsEmpty)
        {
            return;
        }

        var target = writer.GetSpan(textInfo.CharsRequired);

        if (!textInfo.HasUnsafeChars)
        {
            source.CopyTo(target);
        }
        else
        {
            var search = _tokenQ != _tokenE ? _searchValues.ValuesQE : _searchValues.ValuesQ;

            target[0] = _tokenQ;
            target = target.Slice(1);

            while (!source.IsEmpty)
            {
                var index = source.IndexOfAny(search);

                if (index >= 0)
                {
                    source.Slice(0, index).CopyTo(target);
                    target[index + 0] = _tokenE;
                    target[index + 1] = source[index];
                    target = target.Slice(index + 2);
                    source = source.Slice(index + 1);
                }
                else
                {
                    source.CopyTo(target);
                    target = target.Slice(source.Length);
                    source = default;
                }
            }

            target[0] = _tokenQ;
        }

        writer.Advance(textInfo.CharsRequired);
    }

    public void WriteAnnotation(ReadOnlySpan<char> source, IBufferWriter<char> writer)
    {
        Debug.Assert(_tokenA.HasValue);

        var targetSize = source.Length + 1;
        var target = writer.GetSpan(targetSize);

        target[0] = _tokenA.Value;
        source.CopyTo(target.Slice(1));
        writer.Advance(targetSize);
    }

    public TabularTextInfo GetTextInfo(ReadOnlySpan<char> source)
    {
        var charsRequired = source.Length;

        var hasUnsafeChars =
            (source.IndexOfAny(_searchValues.ValuesT0T1DQ) >= 0) ||
            (_tokenA.HasValue && !source.IsEmpty && (source[0] == _tokenA.Value));

        if (hasUnsafeChars)
        {
            charsRequired += GetEscapeCount(source) + 2;
        }

        return new(charsRequired, hasUnsafeChars);
    }

    public bool CanWriteAnnotation(ReadOnlySpan<char> source)
    {
        return _tokenT.Length == 1 ?
            (source.IndexOfAny(_searchValues.ValuesT0) < 0) :
            (source.IndexOf(_tokenT) < 0);
    }

    private int GetEscapeCount(ReadOnlySpan<char> source)
    {
        var result = 0;
        var search = _tokenQ != _tokenE ? _searchValues.ValuesQE : _searchValues.ValuesQ;

        while (!source.IsEmpty)
        {
            var index = source.IndexOfAny(search);

            if (index >= 0)
            {
                result++;
                source = source.Slice(index + 1);
            }
            else
            {
                source = default;
            }
        }

        return result;
    }

    public int LineTerminatorLength
    {
        get
        {
            return _tokenT.Length;
        }
    }

    public bool SupportsAnnotations
    {
        get
        {
            return _tokenA.HasValue;
        }
    }
}
