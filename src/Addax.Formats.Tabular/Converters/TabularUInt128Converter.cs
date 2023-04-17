// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularUInt128Converter : TabularNumberConverter<UInt128>
{
    public override bool TryGetFormatBufferLength(UInt128 value, out int result)
    {
        result = 64;

        return true;
    }

    protected override NumberStyles Styles
    {
        get
        {
            return NumberStyles.Integer | NumberStyles.AllowThousands;
        }
    }
}
