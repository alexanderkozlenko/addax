// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="DateTimeOffset" /> value from or to a character sequence.</summary>
public class TabularDateTimeOffsetConverter : TabularConverter<DateTimeOffset>
{
    internal static readonly TabularDateTimeOffsetConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularDateTimeOffsetConverter" /> class.</summary>
    public TabularDateTimeOffsetConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(DateTimeOffset value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "o", provider);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateTimeOffset value)
    {
        const DateTimeStyles styles = DateTimeStyles.AllowLeadingWhite | DateTimeStyles.AllowTrailingWhite | DateTimeStyles.AssumeUniversal;

        return DateTimeOffset.TryParseExact(source, "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", provider, styles, out value);
    }
}
