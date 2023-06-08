// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularTimeSpanConverter : TabularFieldConverter<TimeSpan>
{
    private static readonly string[] _formats = CreateFormats();

    public override bool TryGetFormatBufferLength(TimeSpan value, out int result)
    {
        result = 64;

        return true;
    }

    public override bool TryFormat(TimeSpan value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        var writer = new StringBufferWriter(buffer);

        if (value < TimeSpan.Zero)
        {
            value = value.Duration();
            writer.Write('-');
        }

        writer.Write('P');

        if (value.Days > 0)
        {
            var result = value.Days.TryFormat(writer.FreeBuffer, out var charsWrittenD, "g", provider);

            Debug.Assert(result);

            writer.Advance(charsWrittenD);
            writer.Write('D');
        }

        var seconds = value.Seconds + (value.Milliseconds / 1e+3) + (value.Microseconds / 1e+6) + (value.Nanoseconds / 1e+9);

        if ((value.Hours != 0) || (value.Minutes != 0) || (seconds != 0) || (value == TimeSpan.Zero))
        {
            writer.Write('T');
        }
        if ((value.Hours != 0) || (value == TimeSpan.Zero))
        {
            var result = value.Hours.TryFormat(writer.FreeBuffer, out var charsWrittenH, "g", provider);

            Debug.Assert(result);

            writer.Advance(charsWrittenH);
            writer.Write('H');
        }
        if (value.Minutes != 0)
        {
            var result = value.Minutes.TryFormat(writer.FreeBuffer, out var charsWrittenM, "g", provider);

            Debug.Assert(result);

            writer.Advance(charsWrittenM);
            writer.Write('M');
        }
        if (seconds != 0)
        {
            var result = seconds.TryFormat(writer.FreeBuffer, out var charsWrittenS, "0.#######", provider);

            Debug.Assert(result);

            writer.Advance(charsWrittenS);
            writer.Write('S');
        }

        charsWritten = writer.Written;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out TimeSpan value)
    {
        buffer = buffer.Trim();

        var styles = TimeSpanStyles.None;

        if (buffer is ['-', ..])
        {
            buffer = buffer[1..];
            styles = TimeSpanStyles.AssumeNegative;
        }

        return TimeSpan.TryParseExact(buffer, _formats, provider, styles, out value);
    }

    private static string[] CreateFormats()
    {
        return new[]
        {
            "'PT'h'H'm'M's'.'FFFFFFF'S'",
            "'PT'h'H'm'M's'S'",
            "'PT'h'H'm'M'",
            "'PT'h'H's'.'FFFFFFF'S'",
            "'PT'h'H's'S'",
            "'PT'h'H'",
            "'PT'm'M's'.'FFFFFFF'S'",
            "'PT'm'M's'S'",
            "'PT'm'M'",
            "'PT's'.'FFFFFFF'S'",
            "'PT's'S'",
            "'P'd'DT'h'H'm'M's'.'FFFFFFF'S'",
            "'P'd'DT'h'H'm'M's'S'",
            "'P'd'DT'h'H'm'M'",
            "'P'd'DT'h'H's'.'FFFFFFF'S'",
            "'P'd'DT'h'H's'S'",
            "'P'd'DT'h'H'",
            "'P'd'DT'm'M's'.'FFFFFFF'S'",
            "'P'd'DT'm'M's'S'",
            "'P'd'DT'm'M'",
            "'P'd'DT's'.'FFFFFFF'S'",
            "'P'd'DT's'S'",
            "'P'd'D'",
        };
    }
}
