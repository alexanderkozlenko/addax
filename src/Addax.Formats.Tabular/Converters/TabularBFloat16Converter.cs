// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="BFloat16" /> value to or from a character sequence.</summary>
public class TabularBFloat16Converter : TabularConverter<BFloat16>
{
    /// <summary>Initializes a new instance of the <see cref="TabularBFloat16Converter" /> class.</summary>
    public TabularBFloat16Converter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(BFloat16 value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "g", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out BFloat16 value)
    {
        const NumberStyles styles = NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        return BFloat16.TryParse(source, styles, provider, out value);
    }
}
