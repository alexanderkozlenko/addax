// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="bool" /> value from or to a sequence of characters.</summary>
public class TabularBooleanConverter : TabularConverter<bool>
{
    internal static readonly TabularBooleanConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularBooleanConverter" /> class.</summary>
    public TabularBooleanConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(bool value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        if (value.TryFormat(destination, out charsWritten))
        {
            destination[0] = char.ToLowerInvariant(destination[0]);

            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out bool value)
    {
        return bool.TryParse(source, out value) || TryParseAsNumber(source, out value);
    }

    private static bool TryParseAsNumber(ReadOnlySpan<char> source, out bool value)
    {
        source = source.Trim();

        if (source.Length == 1)
        {
            var symbol = source[0];

            if (symbol == '1')
            {
                value = true;

                return true;
            }

            if (symbol == '0')
            {
                value = false;

                return true;
            }
        }

        value = default;

        return false;
    }
}
