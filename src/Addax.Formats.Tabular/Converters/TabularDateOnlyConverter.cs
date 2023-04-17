// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularDateOnlyConverter : TabularFieldConverter<DateOnly>
{
    private static readonly string[] _formats = CreateFormats();

    public override bool TryGetFormatBufferLength(DateOnly value, out int result)
    {
        result = 16;

        return true;
    }

    public override bool TryFormat(DateOnly value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        return value.TryFormat(buffer, out charsWritten, "yyyy'-'MM'-'dd", provider);
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out DateOnly value)
    {
        const DateTimeStyles styles =
            DateTimeStyles.AssumeUniversal |
            DateTimeStyles.AllowLeadingWhite |
            DateTimeStyles.AllowTrailingWhite;

        var status = DateTimeOffset.TryParseExact(buffer, _formats, provider, styles, out var dateTimeOffset);

        value = DateOnly.FromDateTime(dateTimeOffset.Date);

        return status;
    }

    private static string[] CreateFormats()
    {
        return new[]
        {
            "yyyy'-'MM'-'dd",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
        };
    }
}
