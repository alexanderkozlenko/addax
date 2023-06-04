// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularByteConverter : TabularNumberConverter<byte>
{
    public override bool TryGetFormatBufferLength(byte value, out int result)
    {
        result = 4;

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
