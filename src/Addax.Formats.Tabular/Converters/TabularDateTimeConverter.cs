// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateTime" /> value from or to a sequence of characters.</summary>
public class TabularDateTimeConverter : TabularConverter<DateTime>
{
    internal static readonly TabularDateTimeConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeConverter" /> class with the specified format.</summary>
    /// <param name="format">The format to use for parsing and formatting.</param>
    public TabularDateTimeConverter(string? format = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK")
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(DateTime value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format, provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateTime value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

        if (DateTimeOffset.TryParseExact(source, _format, provider, styles, out var dateTimeOffset))
        {
            value = dateTimeOffset.UtcDateTime;

            return true;
        }

        value = default;

        return false;
    }
}
