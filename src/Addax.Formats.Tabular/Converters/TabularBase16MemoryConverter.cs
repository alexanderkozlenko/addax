// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts binary data encoded with "base16" ("hex") encoding from or to a character sequence.</summary>
public class TabularBase16MemoryConverter : TabularConverter<Memory<byte>>
{
    /// <summary>Initializes a new instance of the <see cref="TabularBase16MemoryConverter" /> class.</summary>
    public TabularBase16MemoryConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Memory<byte> value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return TabularBinary.TryFormatBase16(value.Span, destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Memory<byte> value)
    {
        if (TabularBinary.TryParseBase16(source, out var array))
        {
            value = array;

            return true;
        }

        value = default;

        return false;
    }
}
