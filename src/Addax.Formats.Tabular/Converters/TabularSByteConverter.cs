// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="sbyte" /> value from or to a sequence of characters.</summary>
[CLSCompliant(false)]
public class TabularSByteConverter : TabularConverter<sbyte>
{
    internal static readonly TabularSByteConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularSByteConverter" /> class.</summary>
    public TabularSByteConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(sbyte value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out sbyte value)
    {
        const NumberStyles styles = NumberStyles.Integer;

        return sbyte.TryParse(source, styles, provider, out value) || TabularNumber<sbyte>.TryParseAsPartsPer(source, styles, provider, out value);
    }
}
