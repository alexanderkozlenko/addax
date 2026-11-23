// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts binary data encoded with "base64" encoding to or from a character sequence.</summary>
public class TabularBase64ReadOnlyMemoryConverter : TabularConverter<ReadOnlyMemory<byte>>
{
    internal static readonly TabularBase64ReadOnlyMemoryConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularBase64ReadOnlyMemoryConverter" /> class.</summary>
    public TabularBase64ReadOnlyMemoryConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(ReadOnlyMemory<byte> value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return Convert.TryToBase64Chars(value.Span, destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out ReadOnlyMemory<byte> value)
    {
        if (TabularConvert.TryParseBase64(source, out var array))
        {
            value = array;

            return true;
        }

        value = default;

        return false;
    }
}
