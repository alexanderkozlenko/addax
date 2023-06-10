// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularUInt16Converter : TabularNumberConverter<ushort>
{
    public override bool TryGetFormatBufferSize(ushort value, out int result)
    {
        result = 8;

        return true;
    }

    protected override NumberStyles Styles
    {
        get
        {
            return
                NumberStyles.Integer |
                NumberStyles.AllowThousands;
        }
    }
}
