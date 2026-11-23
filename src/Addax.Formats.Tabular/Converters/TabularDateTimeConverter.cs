// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateTime" /> value to or from a character sequence.</summary>
/// <remarks>When parsing, a value with a time zone offset is converted to Coordinated Universal Time (UTC) and has the <see cref="DateTimeKind.Utc" /> kind; a value without an offset has the <see cref="DateTimeKind.Unspecified" /> kind.</remarks>
public class TabularDateTimeConverter : TabularConverter<DateTime>
{
    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeConverter" /> class.</summary>
    public TabularDateTimeConverter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom date and time format string.</param>
    public TabularDateTimeConverter([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format)
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
        const DateTimeStyles styles = DateTimeStyles.AdjustToUniversal;

        if (_format is null)
        {
            return DateTime.TryParseExact(source, "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", CultureInfo.InvariantCulture, styles, out value);
        }

        return DateTime.TryParseExact(source, _format, provider, styles, out value);
    }
}
