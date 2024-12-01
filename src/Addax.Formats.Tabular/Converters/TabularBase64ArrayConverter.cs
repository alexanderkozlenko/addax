// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts binary data encoded with "base64" encoding from or to a character sequence.</summary>
public class TabularBase64ArrayConverter : TabularConverter<byte[]>
{
    internal static readonly TabularBase64ArrayConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularBase64ArrayConverter" /> class.</summary>
    public TabularBase64ArrayConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(byte[]? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return TabularBinary.TryFormatBase64(value, destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, [NotNullWhen(true)] out byte[]? value)
    {
        return TabularBinary.TryParseBase64(source, out value);
    }
}
