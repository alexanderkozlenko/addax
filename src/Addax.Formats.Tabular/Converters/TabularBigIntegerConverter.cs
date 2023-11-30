// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="BigInteger" /> value from or to a character sequence.</summary>
public class TabularBigIntegerConverter : TabularConverter<BigInteger>
{
    internal static readonly TabularBigIntegerConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularBigIntegerConverter" /> class.</summary>
    public TabularBigIntegerConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(BigInteger value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out BigInteger value)
    {
        const NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowThousands;

        return BigInteger.TryParse(source, styles, provider, out value) || TabularNumber<BigInteger>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
