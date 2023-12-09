// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;

namespace Addax.Formats.Tabular;

internal sealed class TabularFormatter
{
    private readonly string _tokenT;
    private readonly char _tokenD;
    private readonly char _tokenQ;
    private readonly char _tokenE;
    private readonly char? _tokenA;

    private readonly TabularSearchValues _searchValues;

    public TabularFormatter(TabularDialect dialect)
    {
        Debug.Assert(dialect is not null);

        _tokenT = dialect.LineTerminator;
        _tokenD = dialect.Delimiter;
        _tokenQ = dialect.QuoteSymbol;
        _tokenE = dialect.EscapeSymbol;
        _tokenA = dialect.AnnotationPrefix;

        _searchValues = dialect.SearchValues;
    }

    public void WriteDelimiter(IBufferWriter<char> writer)
    {
        Debug.Assert(writer is not null);

        var target = writer.GetSpan(1);

        target[0] = _tokenD;
        writer.Advance(1);
    }

    public void WriteLineTerminator(IBufferWriter<char> writer)
    {
        Debug.Assert(writer is not null);

        var tokenT = _tokenT.AsSpan();
        var target = writer.GetSpan(_tokenT.Length);

        tokenT.CopyTo(target);
        writer.Advance(tokenT.Length);
    }

    public void WriteValue(ReadOnlySpan<char> source, IBufferWriter<char> writer, TabularTextInfo textInfo)
    {
        Debug.Assert(writer is not null);

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
        Debug.Assert(writer is not null);
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
