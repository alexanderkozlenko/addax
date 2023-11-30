// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Half" /> value from or to a character sequence.</summary>
public class TabularHalfConverter : TabularConverter<Half>
{
    internal static readonly TabularHalfConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularHalfConverter" /> class.</summary>
    public TabularHalfConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Half value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Half value)
    {
        const NumberStyles styles = NumberStyles.AllowThousands | NumberStyles.Float;

        return Half.TryParse(source, styles, provider, out value) || TabularNumber<Half>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
