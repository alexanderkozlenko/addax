// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="TimeOnly" /> value to or from a character sequence.</summary>
/// <remarks>When parsing with the default format, an optional time zone offset is accepted and ignored.</remarks>
public class TabularTimeOnlyConverter : TabularConverter<TimeOnly>
{
    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularTimeOnlyConverter" /> class.</summary>
    public TabularTimeOnlyConverter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularTimeOnlyConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom time format string.</param>
    public TabularTimeOnlyConverter([StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string? format)
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(TimeOnly value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format ?? "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out TimeOnly value)
    {
        const DateTimeStyles styles = DateTimeStyles.None;

        if (_format is null)
        {
            if (DateTimeOffset.TryParseExact(source, "HH':'mm':'ss.FFFFFFFK", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out var result))
            {
                value = TimeOnly.FromTimeSpan(result.TimeOfDay);

                return true;
            }

            value = default;

            return false;
        }

        return TimeOnly.TryParseExact(source, _format, provider, styles, out value);
    }
}
