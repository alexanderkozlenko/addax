// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularInt128Converter : TabularNumberConverter<Int128>
{
    public override bool TryGetFormatBufferLength(Int128 value, out int result)
    {
        result = 64;

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
