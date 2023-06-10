// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularDecimalConverter : TabularNumberConverter<decimal>
{
    public override bool TryGetFormatBufferSize(decimal value, out int result)
    {
        result = 32;

        return true;
    }

    protected override NumberStyles Styles
    {
        get
        {
            return
                NumberStyles.Integer |
                NumberStyles.AllowDecimalPoint |
                NumberStyles.AllowThousands;
        }
    }
}
