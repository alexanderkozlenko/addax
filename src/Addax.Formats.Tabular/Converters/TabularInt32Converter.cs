// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularInt32Converter : TabularNumberConverter<int>
{
    public override bool TryGetFormatBufferSize(int value, out int result)
    {
        result = 16;

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
