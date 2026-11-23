// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="short" /> value to or from a character sequence.</summary>
public class TabularInt16Converter : TabularConverter<short>
{
    /// <summary>Initializes a new instance of the <see cref="TabularInt16Converter" /> class.</summary>
    public TabularInt16Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(short value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out short value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign;

        return short.TryParse(source, styles, provider, out value);
    }
}
