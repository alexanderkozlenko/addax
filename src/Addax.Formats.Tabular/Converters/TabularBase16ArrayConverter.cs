// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts binary data encoded with "base16" ("hex") encoding from or to a character sequence.</summary>
public class TabularBase16ArrayConverter : TabularConverter<byte[]?>
{
    internal static readonly TabularBase16ArrayConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularBase16ArrayConverter" /> class.</summary>
    public TabularBase16ArrayConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(byte[]? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return TabularBinary.TryFormatBase16(value, destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, [NotNullWhen(true)] out byte[]? value)
    {
        return TabularBinary.TryParseBase16(source, out value);
    }
}
