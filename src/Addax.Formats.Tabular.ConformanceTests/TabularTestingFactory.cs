namespace Addax.Formats.Tabular.ConformanceTests;

internal static class TabularTestingFactory
{
    public static TabularDataDialect CreateDialect(string script)
    {
        ArgumentException.ThrowIfNullOrEmpty(script);

        var segments = script.Split('-');

        return new(
            segments[0] switch
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
            },
            (char)Convert.ToUInt16(segments[1], 16),
            (char)Convert.ToUInt16(segments[2], 16),
            (char)Convert.ToUInt16(segments[3], 16));
    }
}
