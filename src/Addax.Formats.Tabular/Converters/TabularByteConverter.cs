// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularByteConverter : TabularNumberConverter<byte>
{
    public override int GetFormatBufferLength(byte value)
    {
        return 4;
    }

    public override int GetParseBufferLength()
    {
        return Array.MaxLength;
    }

    protected override NumberStyles Styles
    {
        get
        {
            return NumberStyles.Integer | NumberStyles.AllowThousands;
        }
    }
}
