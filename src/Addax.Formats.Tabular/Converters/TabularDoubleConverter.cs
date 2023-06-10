// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularDoubleConverter : TabularNumberConverter<double>
{
    public override bool TryGetFormatBufferSize(double value, out int result)
    {
        result = 32;

        return true;
    }

    protected override NumberStyles Styles
    {
        get
        {
            return
                NumberStyles.Float |
                NumberStyles.AllowThousands;
        }
    }
}
