using System.Text;

namespace Addax.Formats.Tabular.UnitTests;

internal static class TabularTestingFactory
{
    public static IEnumerable<object[]> ExpandDynamicData(IReadOnlyCollection<object[]> source, IReadOnlyCollection<string> items)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(items);

        foreach (var dataRow in source)
        {
            foreach (var item in items)
            {
                yield return dataRow.Append(item).ToArray();
            }
        }
    }

    public static Encoding CreateEncoding(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        return name switch
        {
            "utf-8" => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true),
            "utf-8-bom" => new UTF8Encoding(encoderShouldEmitUTF8Identifier: true, throwOnInvalidBytes: true),
            "utf-16" => new UnicodeEncoding(bigEndian: false, byteOrderMark: false, throwOnInvalidBytes: true),
            "utf-16-bom" => new UnicodeEncoding(bigEndian: false, byteOrderMark: true, throwOnInvalidBytes: true),
            _ => throw new NotSupportedException(),
        };
    }

    public static TabularDataDialect CreateDialect(string script)
    {
        ArgumentException.ThrowIfNullOrEmpty(script);

        var segments = script.Split('-');

        return segments.Length switch
        {
            3 => new(
                DecodeLineTerminator(segments[0]),
                (char)Convert.ToUInt16(segments[1], 16),
                (char)Convert.ToUInt16(segments[2], 16)),
            4 => new(
                DecodeLineTerminator(segments[0]),
                (char)Convert.ToUInt16(segments[1], 16),
                (char)Convert.ToUInt16(segments[2], 16),
                (char)Convert.ToUInt16(segments[3], 16)),
            5 => new(
                DecodeLineTerminator(segments[0]),
                (char)Convert.ToUInt16(segments[1], 16),
                (char)Convert.ToUInt16(segments[2], 16),
                (char)Convert.ToUInt16(segments[3], 16),
                (char)Convert.ToUInt16(segments[4], 16)),
            _ => throw new NotSupportedException(),
        };

        static string DecodeLineTerminator(string script)
        {
            return script switch
            {
                "lf" => "\u000a",
                "vt" => "\u000b",
                "ff" => "\u000c",
                "cr" => "\u000d",
                "nl" => "\u0085",
                "ls" => "\u2028",
                "ps" => "\u2029",
                "crlf" => "\u000d\u000a",
                _ => throw new NotSupportedException(),
            };
        }
    }

    public static string DecodeContentScript(TabularDataDialect dialect, string script)
    {
        ArgumentNullException.ThrowIfNull(dialect);
        ArgumentNullException.ThrowIfNull(script);

        var contentBuilder = new StringBuilder();

        foreach (var scriptSymbol in script)
        {
            if (scriptSymbol is 'l')
            {
                contentBuilder.Append(dialect.LineTerminator[0]);
            }
            else if (scriptSymbol is 't')
            {
                if (dialect.LineTerminator.Length > 1)
                {
                    contentBuilder.Append(dialect.LineTerminator[1]);
                }
            }
            else if (scriptSymbol is 'd')
            {
                contentBuilder.Append(dialect.Delimiter);
            }
            else if (scriptSymbol is 'q')
            {
                contentBuilder.Append(dialect.QuoteChar);
            }
            else if (scriptSymbol is 'e')
            {
                contentBuilder.Append(dialect.EscapeChar);
            }
            else if (scriptSymbol is 'c')
            {
                contentBuilder.Append(dialect.CommentPrefix);
            }
            else if (scriptSymbol is 'v')
            {
                contentBuilder.Append('v');
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        return contentBuilder.ToString();
    }
}
