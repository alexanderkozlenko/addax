// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="ulong" /> value to or from a character sequence.</summary>
[CLSCompliant(false)]
public class TabularUInt64Converter : TabularConverter<ulong>
{
    /// <summary>Initializes a new instance of the <see cref="TabularUInt64Converter" /> class.</summary>
    public TabularUInt64Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(ulong value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out ulong value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign;

        return ulong.TryParse(source, styles, provider, out value);
    }
}
