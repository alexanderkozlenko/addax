// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="TimeOnly" /> value from or to a character sequence.</summary>
public class TabularTimeOnlyConverter : TabularConverter<TimeOnly>
{
    internal static readonly TabularTimeOnlyConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularTimeOnlyConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom time format string.</param>
    public TabularTimeOnlyConverter([StringSyntax(StringSyntaxAttribute.TimeOnlyFormat)] string? format = null)
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
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite;

        return TimeOnly.TryParseExact(source, _format ?? "HH':'mm':'ss.FFFFFFF", provider, styles, out value);
    }
}
