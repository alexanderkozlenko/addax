// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularSingleConverter : TabularNumberConverter<float>
{
    public override bool TryGetFormatBufferSize(float value, out int result)
    {
        result = 16;

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
