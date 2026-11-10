// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Decimal64" /> value from or to a character sequence.</summary>
public class TabularDecimal64Converter : TabularConverter<Decimal64>
{
    internal static readonly TabularDecimal64Converter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularDecimal64Converter" /> class.</summary>
    public TabularDecimal64Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Decimal64 value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Decimal64 value)
    {
        const NumberStyles styles = NumberStyles.AllowThousands | NumberStyles.Float;

        return Decimal64.TryParse(source, styles, provider, out value) || TabularNumber<Decimal64>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
