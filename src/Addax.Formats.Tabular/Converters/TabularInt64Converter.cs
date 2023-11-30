// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="long" /> value from or to a character sequence.</summary>
public class TabularInt64Converter : TabularConverter<long>
{
    internal static readonly TabularInt64Converter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularInt64Converter" /> class.</summary>
    public TabularInt64Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(long value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out long value)
    {
        const NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowThousands;

        return long.TryParse(source, styles, provider, out value) || TabularNumber<long>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
