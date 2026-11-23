// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;
using Addax.Formats.Tabular.Buffers;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

internal sealed class TabularFormatter
{
    private readonly string _lineTerminator;
    private readonly char _lineTerminatorStart;
    private readonly char _lineTerminatorEnd;
    private readonly char _delimiter;
    private readonly char _quoteChar;
    private readonly char _escapeChar;
    private readonly char? _commentPrefix;
    private readonly SearchValues<char> _charsRequiringQuotes;
    private readonly SearchValues<char> _escapableChars;

    public TabularFormatter(TabularDialect dialect)
    {
        _lineTerminator = dialect.LineTerminator;
        _lineTerminatorStart = dialect.LineTerminator[0];
        _lineTerminatorEnd = dialect.LineTerminator[^1];
        _delimiter = dialect.Delimiter;
        _quoteChar = dialect.QuoteChar;
        _escapeChar = dialect.EscapeChar;
        _commentPrefix = dialect.CommentPrefix;
        _charsRequiringQuotes = dialect.SearchValues.CharsRequiringQuotes;
        _escapableChars = dialect.SearchValues.EscapableChars;
    }

    public void WriteLineTerminator(BufferWriter<char> writer)
    {
        var target = writer.GetSpan(_lineTerminator.Length);

        target[0] = _lineTerminatorStart;

        if (_lineTerminator.Length > 1)
        {
            target[1] = _lineTerminatorEnd;
        }

        writer.Advance(_lineTerminator.Length);
    }

    public void WriteValue(ReadOnlySpan<char> source, BufferWriter<char> writer, TabularTextInfo textInfo, bool isDelimited)
    {
        var charsRequired = textInfo.CharsRequired + (isDelimited ? 1 : 0);

        if (charsRequired == 0)
        {
            return;
        }

        var target = writer.GetSpan(charsRequired);

        if (isDelimited)
        {
            target[0] = _delimiter;
            target = target.Slice(1);
        }

        if (!textInfo.HasQuoting)
        {
            source.CopyTo(target);
        }
        else
        {
            target[0] = _quoteChar;
            target = target.Slice(1);

            while (!source.IsEmpty)
            {
                var index = source.IndexOfAny(_escapableChars);

                if (index >= 0)
                {
                    source.Slice(0, index).CopyTo(target);
                    target[index + 0] = _escapeChar;
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

            target[0] = _quoteChar;
        }

        writer.Advance(charsRequired);
    }

    public void WriteComment(ReadOnlySpan<char> source, BufferWriter<char> writer)
    {
        Debug.Assert(_commentPrefix.HasValue);

        var targetSize = source.Length + 1;
        var target = writer.GetSpan(targetSize);

        target[0] = _commentPrefix.Value;
        source.CopyTo(target.Slice(1));
        writer.Advance(targetSize);
    }

    public TabularTextInfo GetTextInfo(ReadOnlySpan<char> source)
    {
        if (source.IsEmpty)
        {
            return default;
        }

        var charsRequired = source.Length;

        var hasQuoting =
            (source.IndexOfAny(_charsRequiringQuotes) >= 0) ||
            (_commentPrefix.HasValue && (source[0] == _commentPrefix.Value));

        if (hasQuoting)
        {
            charsRequired += source.CountAny(_escapableChars) + 2;
        }

        return new(charsRequired, hasQuoting);
    }

    public bool CanWriteComment(ReadOnlySpan<char> source)
    {
        return source.IndexOf(_lineTerminator.AsSpan()) < 0;
    }

    public int LineTerminatorLength
    {
        get
        {
            return _lineTerminator.Length;
        }
    }

    public bool SupportsComments
    {
        get
        {
            return _commentPrefix.HasValue;
        }
    }
}
