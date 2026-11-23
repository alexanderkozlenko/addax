// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="double" /> value to or from a character sequence.</summary>
public class TabularDoubleConverter : TabularConverter<double>
{
    /// <summary>Initializes a new instance of the <see cref="TabularDoubleConverter" /> class.</summary>
    public TabularDoubleConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(double value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out double value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        return double.TryParse(source, styles, provider, out value);
    }
}
