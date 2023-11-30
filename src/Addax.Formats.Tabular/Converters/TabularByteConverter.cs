// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="byte" /> value from or to a character sequence.</summary>
public class TabularByteConverter : TabularConverter<byte>
{
    internal static readonly TabularByteConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularByteConverter" /> class.</summary>
    public TabularByteConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(byte value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out byte value)
    {
        const NumberStyles styles = NumberStyles.Integer;

        return byte.TryParse(source, styles, provider, out value) || TabularNumber<byte>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
