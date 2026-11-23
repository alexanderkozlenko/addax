// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Decimal32" /> value to or from a character sequence.</summary>
public class TabularDecimal32Converter : TabularConverter<Decimal32>
{
    /// <summary>Initializes a new instance of the <see cref="TabularDecimal32Converter" /> class.</summary>
    public TabularDecimal32Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Decimal32 value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Decimal32 value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        return Decimal32.TryParse(source, styles, provider, out value);
    }
}
