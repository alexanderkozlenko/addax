// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts binary data encoded with "base16" ("hex") encoding from or to a sequence of characters.</summary>
public class TabularBase16BinaryConverter : TabularConverter<byte[]>
{
    internal static readonly TabularBase16BinaryConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularBase16BinaryConverter" /> class.</summary>
    public TabularBase16BinaryConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(byte[]? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        if ((value is null) || (value.Length == 0))
        {
            charsWritten = 0;

            return true;
        }

        if (destination.Length >= value.Length * 2)
        {
            for (var i = 0; i < value.Length; i++)
            {
                value[i].TryFormat(destination, out _, "x2", provider);
                destination = destination.Slice(2);
            }

            charsWritten = value.Length * 2;

            return true;
        }

        charsWritten = 0;

        return false;
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out byte[]? value)
    {
        source = source.Trim();

        if (source.Length % 2 == 0)
        {
            try
            {
                value = Convert.FromHexString(source);

                return true;
            }
            catch (FormatException)
            {
            }
        }

        value = default;

        return false;
    }
}
