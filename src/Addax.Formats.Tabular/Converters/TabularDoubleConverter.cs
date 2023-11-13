// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="double" /> value from or to a sequence of characters.</summary>
public class TabularDoubleConverter : TabularConverter<double>
{
    internal static readonly TabularDoubleConverter Instance = new();

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
        const NumberStyles styles = NumberStyles.AllowThousands | NumberStyles.Float;

        return double.TryParse(source, styles, provider, out value) || TabularNumber<double>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
