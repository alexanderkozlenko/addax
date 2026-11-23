// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Decimal128" /> value to or from a character sequence.</summary>
public class TabularDecimal128Converter : TabularConverter<Decimal128>
{
    /// <summary>Initializes a new instance of the <see cref="TabularDecimal128Converter" /> class.</summary>
    public TabularDecimal128Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Decimal128 value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Decimal128 value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        return Decimal128.TryParse(source, styles, provider, out value);
    }
}
