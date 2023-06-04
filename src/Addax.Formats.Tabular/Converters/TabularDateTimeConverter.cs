// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularDateTimeConverter : TabularFieldConverter<DateTime>
{
    private static readonly string[] _formats = CreateFormats();

    public override bool TryGetFormatBufferLength(DateTime value, out int result)
    {
        result = 32;

        return true;
    }

    public override bool TryFormat(DateTime value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        var result = value.TryFormat(buffer, out charsWritten, "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", provider);

        Debug.Assert(result);

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out DateTime value)
    {
        const DateTimeStyles styles =
            DateTimeStyles.AdjustToUniversal |
            DateTimeStyles.AllowLeadingWhite |
            DateTimeStyles.AllowTrailingWhite;

        return DateTime.TryParseExact(buffer, _formats, provider, styles, out value);
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
