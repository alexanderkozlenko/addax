// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Addax.Formats.Tabular;

internal static class TabularFormatInfo
{
    public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);
    public static readonly CultureInfo DefaultCulture = CreateDefaultCulture();

    public static bool IsSupportedLineTerminator(string value)
    {
        Debug.Assert(value is not null);

        return value.Length switch
        {
            1 => true,
            2 => value[0] != value[1],
            _ => false,
        };
    }

    private static CultureInfo CreateDefaultCulture()
    {
        var culture = CultureInfo.CreateSpecificCulture(string.Empty);

        culture.NumberFormat.PositiveInfinitySymbol = "INF";
        culture.NumberFormat.NegativeInfinitySymbol = "-INF";

        return CultureInfo.ReadOnly(culture);
    }
}
