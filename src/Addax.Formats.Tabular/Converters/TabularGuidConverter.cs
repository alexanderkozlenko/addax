// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Guid" /> value from or to a character sequence.</summary>
public class TabularGuidConverter : TabularConverter<Guid>
{
    internal static readonly TabularGuidConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularGuidConverter" /> class.</summary>
    public TabularGuidConverter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularGuidConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom GUID format string.</param>
    protected TabularGuidConverter([StringSyntax(StringSyntaxAttribute.GuidFormat)] string? format)
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(Guid value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format ?? "d");
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Guid value)
    {
        return Guid.TryParseExact(source, _format ?? "d", out value);
    }
}
