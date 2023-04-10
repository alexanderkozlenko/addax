// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularInt16Converter : TabularNumberConverter<short>
{
    public override int GetFormatBufferLength(short value)
    {
        return 8;
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
