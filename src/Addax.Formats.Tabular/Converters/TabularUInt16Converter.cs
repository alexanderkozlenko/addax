// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="ushort" /> value to or from a character sequence.</summary>
[CLSCompliant(false)]
public class TabularUInt16Converter : TabularConverter<ushort>
{
    /// <summary>Initializes a new instance of the <see cref="TabularUInt16Converter" /> class.</summary>
    public TabularUInt16Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(ushort value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out ushort value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign;

        return ushort.TryParse(source, styles, provider, out value);
    }
}
