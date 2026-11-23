// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts binary data encoded with "base64" encoding to or from a character sequence.</summary>
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
        return Convert.TryToBase64Chars(value, destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, [NotNullWhen(true)] out byte[]? value)
    {
        return TabularConvert.TryParseBase64(source, out value);
    }
}
