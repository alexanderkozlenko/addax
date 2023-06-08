// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Text;

namespace Addax.Formats.Tabular;

internal static class TabularFormatInfo
{
    public const int StackBufferLength = 128;

    public static readonly Encoding DefaultEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: false);
    public static readonly IFormatProvider DefaultFormatProvider = CreateFormatProvider();

    public static bool IsUnicodeMandatoryBreak(char value)
    {
        return value is
            '\u000a' or
            '\u000b' or
            '\u000c' or
            '\u000d' or
            '\u0085' or
            '\u2028' or
            '\u2029';
    }

    public static bool IsUnicodeMandatoryBreak(ReadOnlySpan<char> value)
    {
        return value is
            ['\u000a'] or
            ['\u000b'] or
            ['\u000c'] or
            ['\u000d'] or
            ['\u0085'] or
            ['\u2028'] or
            ['\u2029'] or
            ['\u000d', '\u000a'];
    }

    private static CultureInfo CreateFormatProvider()
    {
        var culture = CultureInfo.CreateSpecificCulture(string.Empty);

        culture.NumberFormat.PositiveInfinitySymbol = "INF";
        culture.NumberFormat.NegativeInfinitySymbol = "-INF";

        return CultureInfo.ReadOnly(culture);
    }
}
