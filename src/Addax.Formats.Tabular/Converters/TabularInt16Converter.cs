// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularInt16Converter : TabularNumberConverter<short>
{
    public override bool TryGetFormatBufferLength(short value, out int result)
    {
        result = 8;

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
