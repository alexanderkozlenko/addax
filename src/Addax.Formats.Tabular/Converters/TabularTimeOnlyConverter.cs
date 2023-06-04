// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularTimeOnlyConverter : TabularFieldConverter<TimeOnly>
{
    private static readonly string[] _formats = CreateFormats();

    public override bool TryGetFormatBufferLength(TimeOnly value, out int result)
    {
        result = 16;

        return true;
    }

    public override bool TryFormat(TimeOnly value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        var result = value.TryFormat(buffer, out charsWritten, "HH':'mm':'ss.FFFFFFF", provider);

        Debug.Assert(result);

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out TimeOnly value)
    {
        const DateTimeStyles styles =
            DateTimeStyles.AssumeUniversal |
            DateTimeStyles.AllowLeadingWhite |
            DateTimeStyles.AllowTrailingWhite;

        var status = DateTimeOffset.TryParseExact(buffer, _formats, provider, styles, out var dateTimeOffset);

        value = TimeOnly.FromTimeSpan(dateTimeOffset.TimeOfDay);

        return status;
    }

    private static string[] CreateFormats()
    {
        return new[]
        {
            "HH':'mm':'ss.FFFFFFF",
            "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK",
        };
    }
}
