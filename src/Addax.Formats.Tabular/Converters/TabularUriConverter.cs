// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Uri" /> instance from or to a sequence of characters.</summary>
public class TabularUriConverter : TabularConverter<Uri>
{
    internal static readonly TabularUriConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularUriConverter" /> class.</summary>
    public TabularUriConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Uri? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        if (value is null)
        {
            charsWritten = 0;

            return true;
        }

        return value.TryFormat(destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Uri? value)
    {
        var uriString = source.IsEmpty ? string.Empty : new(source);

        return Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out value);
    }
}
