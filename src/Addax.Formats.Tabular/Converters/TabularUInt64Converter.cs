// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularUInt64Converter : TabularNumberConverter<ulong>
{
    public override int GetFormatBufferLength(ulong value)
    {
        return 32;
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
