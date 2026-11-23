// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateOnly" /> value to or from a character sequence.</summary>
/// <remarks>When parsing with the default format, an optional time zone offset is accepted and ignored.</remarks>
public class TabularDateOnlyConverter : TabularConverter<DateOnly>
{
    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularDateOnlyConverter" /> class.</summary>
    public TabularDateOnlyConverter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDateOnlyConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom date format string.</param>
    public TabularDateOnlyConverter([StringSyntax(StringSyntaxAttribute.DateOnlyFormat)] string? format)
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
        const DateTimeStyles styles = DateTimeStyles.None;

        if (_format is null)
        {
            if (DateTimeOffset.TryParseExact(source, "yyyy'-'MM'-'ddK", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
            {
                value = DateOnly.FromDateTime(result.DateTime);

                return true;
            }

            value = default;

            return false;
        }

        return DateOnly.TryParseExact(source, _format, provider, styles, out value);
    }
}
