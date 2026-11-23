// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts an <see cref="sbyte" /> value to or from a character sequence.</summary>
[CLSCompliant(false)]
public class TabularSByteConverter : TabularConverter<sbyte>
{
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
        const NumberStyles styles = NumberStyles.AllowLeadingSign;

        return sbyte.TryParse(source, styles, provider, out value);
    }
}
