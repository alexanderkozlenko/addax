// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="decimal" /> value from or to a character sequence.</summary>
public class TabularDecimalConverter : TabularConverter<decimal>
{
    internal static readonly TabularDecimalConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularDecimalConverter" /> class.</summary>
    public TabularDecimalConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(decimal value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out decimal value)
    {
        const NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands;

        return decimal.TryParse(source, styles, provider, out value) || TabularNumber<decimal>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
