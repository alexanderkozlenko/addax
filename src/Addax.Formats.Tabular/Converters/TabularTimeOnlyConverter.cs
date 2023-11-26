// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="TimeOnly" /> value from or to a sequence of characters.</summary>
public class TabularTimeOnlyConverter : TabularConverter<TimeOnly>
{
    internal static readonly TabularTimeOnlyConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularTimeOnlyConverter" /> class.</summary>
    public TabularTimeOnlyConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(TimeOnly value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out TimeOnly value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

        if (DateTimeOffset.TryParseExact(source, "HH':'mm':'ss.FFFFFFF", provider, styles, out var dateTimeOffset))
        {
            value = TimeOnly.FromTimeSpan(dateTimeOffset.TimeOfDay);

            return true;
        }

        value = default;

        return false;
    }
}
