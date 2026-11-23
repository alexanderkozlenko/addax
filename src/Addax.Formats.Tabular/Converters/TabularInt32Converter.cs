// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts an <see cref="int" /> value to or from a character sequence.</summary>
public class TabularInt32Converter : TabularConverter<int>
{
    /// <summary>Initializes a new instance of the <see cref="TabularInt32Converter" /> class.</summary>
    public TabularInt32Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(int value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out int value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign;

        return int.TryParse(source, styles, provider, out value);
    }
}
