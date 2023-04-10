// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularHalfConverter : TabularNumberConverter<Half>
{
    public override int GetFormatBufferLength(Half value)
    {
        return 16;
    }

    public override int GetParseBufferLength()
    {
        return Array.MaxLength;
    }

    protected override NumberStyles Styles
    {
        get
        {
            return NumberStyles.Float | NumberStyles.AllowThousands;
        }
    }
}
