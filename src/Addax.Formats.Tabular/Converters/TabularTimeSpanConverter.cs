// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="TimeSpan" /> value from or to a sequence of characters.</summary>
public class TabularTimeSpanConverter : TabularConverter<TimeSpan>
{
    internal static readonly TabularTimeSpanConverter Instance = new();

    private static readonly string[] s_formats =
    [
        "'PT's'S'",
        "'PT'm'M's'S'",
        "'PT'h'H'm'M's'S'",
        "'P'd'DT'h'H'm'M's'S'",

        "'PT's'.'FFFFFFF'S'",
        "'PT'm'M's'.'FFFFFFF'S'",
        "'PT'h'H'm'M's'.'FFFFFFF'S'",
        "'P'd'DT'h'H'm'M's'.'FFFFFFF'S'",

        "'PT'm'M'",
        "'PT'h'H'",
        "'PT'h'H'm'M'",
        "'PT'h'H's'S'",
        "'P'd'D'",
        "'P'd'DT'h'H'",
        "'P'd'DT'h'H'm'M'",
        "'P'd'DT'h'H's'S'",
        "'P'd'DT'm'M'",
        "'P'd'DT'm'M's'S'",
        "'P'd'DT's'S'",

        "'PT'h'H's'.'FFFFFFF'S'",
        "'P'd'DT'h'H's'.'FFFFFFF'S'",
        "'P'd'DT'm'M's'.'FFFFFFF'S'",
        "'P'd'DT's'.'FFFFFFF'S'",
    ];

    /// <summary>Initializes a new instance of the <see cref="TabularTimeSpanConverter" /> class.</summary>
    public TabularTimeSpanConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(TimeSpan value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        charsWritten = 0;

        var writer = new RegionWriter<char>(destination, ref charsWritten);

        if (value < TimeSpan.Zero)
        {
            var formatInfo = NumberFormatInfo.GetInstance(provider);

            if (!formatInfo.NegativeSign.TryCopyTo(writer.UnusedRegion))
            {
                return false;
            }

            charsWritten += formatInfo.NegativeSign.Length;
            value = value.Duration();
        }

        if (!writer.TryWrite('P'))
        {
            return false;
        }

        if (value.Days > 0)
        {
            if (!value.Days.TryFormat(writer.UnusedRegion, out var charsWrittenD, "g", provider))
            {
                return false;
            }

            charsWritten += charsWrittenD;

            if (!writer.TryWrite('D'))
            {
                return false;
            }
        }

        var seconds = value.Seconds + (value.Milliseconds / 1e+3) + (value.Microseconds / 1e+6) + (value.Nanoseconds / 1e+9);

        if ((value.Hours != 0) || (value.Minutes != 0) || (seconds != 0) || (value == TimeSpan.Zero))
        {
            if (!writer.TryWrite('T'))
            {
                return false;
            }
        }

        if ((value.Hours != 0) || (value == TimeSpan.Zero))
        {
            if (!value.Hours.TryFormat(writer.UnusedRegion, out var charsWrittenH, "g", provider))
            {
                return false;
            }

            charsWritten += charsWrittenH;

            if (!writer.TryWrite('H'))
            {
                return false;
            }
        }

        if (value.Minutes != 0)
        {
            if (!value.Minutes.TryFormat(writer.UnusedRegion, out var charsWrittenM, "g", provider))
            {
                return false;
            }

            charsWritten += charsWrittenM;

            if (!writer.TryWrite('M'))
            {
                return false;
            }
        }

        if (seconds != 0)
        {
            if (!seconds.TryFormat(writer.UnusedRegion, out var charsWrittenS, "0.#######", provider))
            {
                return false;
            }

            charsWritten += charsWrittenS;

            if (!writer.TryWrite('S'))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out TimeSpan value)
    {
        source = source.Trim();

        var styles = TimeSpanStyles.None;
        var formatInfo = NumberFormatInfo.GetInstance(provider);

        if (source.StartsWith(formatInfo.NegativeSign))
        {
            source = source.Slice(formatInfo.NegativeSign.Length);
            styles = TimeSpanStyles.AssumeNegative;
        }

        return TimeSpan.TryParseExact(source, s_formats, provider, styles, out value);
    }
}
