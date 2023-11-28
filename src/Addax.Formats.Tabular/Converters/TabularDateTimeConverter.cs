// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateTime" /> value from or to a sequence of characters.</summary>
public class TabularDateTimeConverter : TabularConverter<DateTime>
{
    internal static readonly TabularDateTimeConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeConverter" /> class.</summary>
    public TabularDateTimeConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(DateTime value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateTime value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AdjustToUniversal;

        return DateTime.TryParseExact(source, "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", provider, styles, out value);
    }
}
