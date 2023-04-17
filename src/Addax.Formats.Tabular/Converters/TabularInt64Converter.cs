// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularInt64Converter : TabularNumberConverter<long>
{
    public override bool TryGetFormatBufferLength(long value, out int result)
    {
        result = 32;

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
