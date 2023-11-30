// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="UInt128" /> value from or to a character sequence.</summary>
[CLSCompliant(false)]
public class TabularUInt128Converter : TabularConverter<UInt128>
{
    internal static readonly TabularUInt128Converter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularUInt128Converter" /> class.</summary>
    public TabularUInt128Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(UInt128 value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out UInt128 value)
    {
        const NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowThousands;

        return UInt128.TryParse(source, styles, provider, out value) || TabularNumber<UInt128>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
