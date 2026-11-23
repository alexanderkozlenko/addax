// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Text.RegularExpressions;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="TimeSpan" /> value to or from a character sequence.</summary>
public partial class TabularTimeSpanConverter : TabularConverter<TimeSpan>
{
    private static readonly long[] s_ticksPerDigit =
    [
        00000000,
        01000000,
        00100000,
        00010000,
        00001000,
        00000100,
        00000010,
        00000001,
    ];

    /// <summary>Initializes a new instance of the <see cref="TabularTimeSpanConverter" /> class.</summary>
    public TabularTimeSpanConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(TimeSpan value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        var sign = value < TimeSpan.Zero ? "-" : "";

        var days = Math.Abs(value.Days);
        var hours = Math.Abs(value.Hours);
        var minutes = Math.Abs(value.Minutes);
        var seconds = Math.Abs(value.Seconds) + (Math.Abs(value.Milliseconds) * 1e-3) + (Math.Abs(value.Microseconds) * 1e-6) + (Math.Abs(value.Nanoseconds) * 1e-9);

        return destination.TryWrite(CultureInfo.InvariantCulture, $"{sign}P{days:d}DT{hours:d}H{minutes:d}M{seconds:f7}S", out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out TimeSpan value)
    {
        if (!DurationRegex.IsMatch(source))
        {
            value = default;

            return false;
        }

        var ticks = Int128.Zero;

        foreach (var match in ComponentRegex.EnumerateMatches(source))
        {
            var component = source.Slice(match.Index, match.Length - 1);

            var ticksPerUnit = source[match.Index + match.Length - 1] switch
            {
                'D' => TimeSpan.TicksPerDay,
                'H' => TimeSpan.TicksPerHour,
                'M' => TimeSpan.TicksPerMinute,
                _ => TimeSpan.TicksPerSecond,
            };

            var pointIndex = component.IndexOf('.');

            if (pointIndex >= 0)
            {
                var fraction = component.Slice(pointIndex + 1);

                ticks += long.Parse(fraction, NumberStyles.None, CultureInfo.InvariantCulture) * s_ticksPerDigit[fraction.Length];
                component = component.Slice(0, pointIndex);
            }

            if (!ulong.TryParse(component, NumberStyles.None, CultureInfo.InvariantCulture, out var count))
            {
                value = default;

                return false;
            }

            ticks += (Int128)count * ticksPerUnit;
        }

        if (source[0] == '-')
        {
            ticks = -ticks;
        }

        if ((ticks < long.MinValue) || (ticks > long.MaxValue))
        {
            value = default;

            return false;
        }

        value = new((long)ticks);

        return true;
    }

    [GeneratedRegex(@"\A-?P(?=[0-9]|T[0-9])(?:[0-9]+D)?(?:T(?=[0-9])(?:[0-9]+H)?(?:[0-9]+M)?(?:[0-9]+(?:\.[0-9]{1,7})?S)?)?\z", RegexOptions.CultureInvariant)]
    private static partial Regex DurationRegex
    {
        get;
    }

    [GeneratedRegex(@"[0-9]+(?:\.[0-9]+)?[DHMS]", RegexOptions.CultureInvariant)]
    private static partial Regex ComponentRegex
    {
        get;
    }
}
