// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateOnly" /> value from or to a sequence of characters.</summary>
public class TabularDateOnlyConverter : TabularConverter<DateOnly>
{
    internal static readonly TabularDateOnlyConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularDateOnlyConverter" /> class.</summary>
    public TabularDateOnlyConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(DateOnly value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateOnly value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

        if (DateTimeOffset.TryParseExact(source, "yyyy'-'MM'-'dd", provider, styles, out var dateTimeOffset))
        {
            value = DateOnly.FromDateTime(dateTimeOffset.Date);

            return true;
        }

        value = default;

        return false;
    }
}
