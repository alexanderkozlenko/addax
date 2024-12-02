// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Rune" /> value from or to a character sequence.</summary>
public class TabularRuneConverter : TabularConverter<Rune>
{
    internal static readonly TabularRuneConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularRuneConverter" /> class.</summary>
    public TabularRuneConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Rune value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryEncodeToUtf16(destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Rune value)
    {
        if (source.Length == 2)
        {
            return Rune.TryCreate(source[0], source[1], out value);
        }
        else if (source.Length == 1)
        {
            return Rune.TryCreate(source[0], out value);
        }

        value = default;

        return false;
    }
}
