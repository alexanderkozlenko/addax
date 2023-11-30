// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Int128" /> value from or to a character sequence.</summary>
public class TabularInt128Converter : TabularConverter<Int128>
{
    internal static readonly TabularInt128Converter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularInt128Converter" /> class.</summary>
    public TabularInt128Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Int128 value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Int128 value)
    {
        const NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowThousands;

        return Int128.TryParse(source, styles, provider, out value) || TabularNumber<Int128>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
