// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular;

internal static class TabularNumber<T>
    where T : struct, INumberBase<T>
{
    private static readonly bool s_isPerMilleSupported = T.CreateSaturating(1000) / T.CreateChecked(100) == T.CreateChecked(10);

    public static bool TryParseAsPartsPer(ReadOnlySpan<char> source, NumberStyles styles, IFormatProvider? provider, out T value)
    {
        source = source.TrimEnd();

        if (!source.IsEmpty)
        {
            var formatInfo = NumberFormatInfo.GetInstance(provider);

            if (TryParseAsPartsPer(source, styles, provider, formatInfo.PercentSymbol, 100, out value))
            {
                return true;
            }

            if (s_isPerMilleSupported && TryParseAsPartsPer(source, styles, provider, formatInfo.PerMilleSymbol, 1000, out value))
            {
                return true;
            }
        }

        value = default;

        return false;
    }

    private static bool TryParseAsPartsPer(ReadOnlySpan<char> source, NumberStyles styles, IFormatProvider? provider, ReadOnlySpan<char> symbol, int denominator, out T value)
    {
        if (source.EndsWith(symbol))
        {
            source = source.Slice(0, source.Length - symbol.Length);

            if (T.TryParse(source, styles, provider, out value))
            {
                value /= T.CreateChecked(denominator);

                return true;
            }
        }

        value = default;

        return false;
    }
}
