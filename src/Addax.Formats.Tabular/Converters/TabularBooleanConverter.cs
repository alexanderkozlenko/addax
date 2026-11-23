// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="bool" /> value to or from a character sequence.</summary>
public class TabularBooleanConverter : TabularConverter<bool>
{
    /// <summary>Initializes a new instance of the <see cref="TabularBooleanConverter" /> class.</summary>
    public TabularBooleanConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(bool value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        var literal = value ? "true" : "false";

        if (literal.TryCopyTo(destination))
        {
            charsWritten = literal.Length;

            return true;
        }

        charsWritten = 0;

        return false;
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out bool value)
    {
        if (source is "true" or "1")
        {
            value = true;

            return true;
        }

        if (source is "false" or "0")
        {
            value = false;

            return true;
        }

        value = default;

        return false;
    }
}
