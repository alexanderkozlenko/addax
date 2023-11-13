// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="TimeOnly" /> value from or to a sequence of characters.</summary>
public class TabularTimeOnlyConverter : TabularConverter<TimeOnly>
{
    internal static readonly TabularTimeOnlyConverter Instance = new();

    private readonly string? _format;

    /// <summary>Initializes a new instance of the <see cref="TabularTimeOnlyConverter" /> class with the specified format.</summary>
    /// <param name="format">The format to use for parsing and formatting.</param>
    public TabularTimeOnlyConverter(string? format = "HH':'mm':'ss.FFFFFFF")
    {
        _format = format;
    }

    /// <inheritdoc />
    public override bool TryFormat(TimeOnly value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, _format, provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out TimeOnly value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

        if (DateTimeOffset.TryParseExact(source, _format, provider, styles, out var dateTimeOffset))
        {
            value = TimeOnly.FromTimeSpan(dateTimeOffset.TimeOfDay);

            return true;
        }

        value = default;

        return false;
    }
}
