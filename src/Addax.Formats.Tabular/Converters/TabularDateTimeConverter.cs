// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateTime" /> value from or to a character sequence.</summary>
public class TabularDateTimeConverter : TabularConverter<DateTime>
{
    internal static readonly TabularDateTimeConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeConverter" /> class.</summary>
    public TabularDateTimeConverter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom date and time format string.</param>
    protected TabularDateTimeConverter([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format)
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(DateTime value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format ?? "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateTime value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AdjustToUniversal;

        return DateTime.TryParseExact(source, _format ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", provider, styles, out value);
    }
}
