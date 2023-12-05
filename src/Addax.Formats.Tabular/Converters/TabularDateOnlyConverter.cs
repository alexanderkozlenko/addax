// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateOnly" /> value from or to a character sequence.</summary>
public class TabularDateOnlyConverter : TabularConverter<DateOnly>
{
    internal static readonly TabularDateOnlyConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularDateOnlyConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom date format string.</param>
    public TabularDateOnlyConverter([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format = null)
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(DateOnly value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format ?? "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateOnly value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite;

        return DateOnly.TryParseExact(source, _format ?? "yyyy'-'MM'-'dd", provider, styles, out value);
    }
}
