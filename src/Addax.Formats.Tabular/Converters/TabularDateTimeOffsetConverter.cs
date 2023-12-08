// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateTimeOffset" /> value from or to a character sequence.</summary>
public class TabularDateTimeOffsetConverter : TabularConverter<DateTimeOffset>
{
    internal static readonly TabularDateTimeOffsetConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeOffsetConverter" /> class.</summary>
    public TabularDateTimeOffsetConverter()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeOffsetConverter" /> class with the specified format.</summary>
    /// <param name="format">A standard or custom date and time format string.</param>
    protected TabularDateTimeOffsetConverter([StringSyntax(StringSyntaxAttribute.DateTimeFormat)] string? format)
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(DateTimeOffset value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format ?? "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateTimeOffset value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

        return DateTimeOffset.TryParseExact(source, _format ?? "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", provider, styles, out value);
    }
}
