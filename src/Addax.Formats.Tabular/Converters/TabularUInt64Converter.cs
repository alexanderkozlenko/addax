// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularUInt64Converter : TabularNumberConverter<ulong>
{
    public override bool TryGetFormatBufferLength(ulong value, out int result)
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
                NumberStyles.AllowThousands;
        }
    }
}
