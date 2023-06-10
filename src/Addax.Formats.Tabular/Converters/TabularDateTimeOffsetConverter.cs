// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularDateTimeOffsetConverter : TabularFieldConverter<DateTimeOffset>
{
    private static readonly string[] _formats = CreateFormats();

    public override bool TryGetFormatBufferSize(DateTimeOffset value, out int result)
    {
        result = 64;

        return true;
    }

    public override bool TryFormat(DateTimeOffset value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        var result = value.TryFormat(buffer, out charsWritten, "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", provider);

        Debug.Assert(result);

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out DateTimeOffset value)
    {
        const DateTimeStyles styles =
            DateTimeStyles.AssumeUniversal |
            DateTimeStyles.AllowLeadingWhite |
            DateTimeStyles.AllowTrailingWhite;

        return DateTimeOffset.TryParseExact(buffer, _formats, provider, styles, out value);
    }

    private static string[] CreateFormats()
    {
        return new[]
        {
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
            "yyyy'-'MM'-'dd",
        };
    }
}
