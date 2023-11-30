// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="char" /> value from or to a character sequence.</summary>
public class TabularCharConverter : TabularConverter<char>
{
    internal static readonly TabularCharConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularCharConverter" /> class.</summary>
    public TabularCharConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(char value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        if (!destination.IsEmpty)
        {
            destination[0] = value;
            charsWritten = 1;

            return true;
        }

        charsWritten = 0;

        return false;
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out char value)
    {
        if (source.Length == 1)
        {
            value = source[0];

            return true;
        }

        value = default;

        return false;
    }
}
