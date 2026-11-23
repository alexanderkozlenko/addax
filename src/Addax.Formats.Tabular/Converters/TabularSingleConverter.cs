// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="float" /> value to or from a character sequence.</summary>
public class TabularSingleConverter : TabularConverter<float>
{
    /// <summary>Initializes a new instance of the <see cref="TabularSingleConverter" /> class.</summary>
    public TabularSingleConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(float value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out float value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        return float.TryParse(source, styles, provider, out value);
    }
}
