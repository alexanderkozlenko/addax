// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Text;

namespace Addax.Formats.Tabular;

internal static class TabularFormatInfo
{
    public static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);
    public static readonly CultureInfo DefaultCulture = CreateDefaultCulture();

    private static CultureInfo CreateDefaultCulture()
    {
        var culture = CultureInfo.CreateSpecificCulture(string.Empty);

        culture.NumberFormat.PositiveInfinitySymbol = "INF";
        culture.NumberFormat.NegativeInfinitySymbol = "-INF";

        return CultureInfo.ReadOnly(culture);
    }
}
