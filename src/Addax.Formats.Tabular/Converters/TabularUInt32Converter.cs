// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="uint" /> value from or to a character sequence.</summary>
[CLSCompliant(false)]
public class TabularUInt32Converter : TabularConverter<uint>
{
    internal static readonly TabularUInt32Converter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularUInt32Converter" /> class.</summary>
    public TabularUInt32Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(uint value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out uint value)
    {
        const NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowThousands;

        return uint.TryParse(source, styles, provider, out value) || TabularNumber<uint>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
