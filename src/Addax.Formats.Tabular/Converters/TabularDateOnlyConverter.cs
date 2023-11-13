// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateOnly" /> value from or to a sequence of characters.</summary>
public class TabularDateOnlyConverter : TabularConverter<DateOnly>
{
    internal static readonly TabularDateOnlyConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularDateOnlyConverter" /> class with the specified format.</summary>
    /// <param name="format">The format to use for parsing and formatting.</param>
    public TabularDateOnlyConverter(string? format = "yyyy'-'MM'-'dd")
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(DateOnly value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format, provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateOnly value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

        if (DateTimeOffset.TryParseExact(source, _format, provider, styles, out var dateTimeOffset))
        {
            value = DateOnly.FromDateTime(dateTimeOffset.Date);

            return true;
        }

        value = default;

        return false;
    }
}
