// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularDoubleConverter : TabularNumberConverter<double>
{
    public override int GetFormatBufferLength(double value)
    {
        return 32;
    }

    public override int GetParseBufferLength()
    {
        return Array.MaxLength;
    }

    protected override NumberStyles Styles
    {
        get
        {
            return NumberStyles.Float | NumberStyles.AllowThousands;
        }
    }
}
