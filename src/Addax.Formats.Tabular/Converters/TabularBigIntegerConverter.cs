// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularBigIntegerConverter : TabularNumberConverter<BigInteger>
{
    public override int GetFormatBufferLength(BigInteger value)
    {
        if ((long.MinValue <= value) && (value <= long.MaxValue))
        {
            return 32;
        }

        try
        {
            var bufferLength = checked((int)Math.Ceiling(BigInteger.Log10(BigInteger.Abs(value)) + 1) + 1);

            if (bufferLength > Array.MaxLength)
            {
                return -1;
            }

            return bufferLength;
        }
        catch (ArgumentOutOfRangeException)
        {
            return -1;
        }
        catch (OverflowException)
        {
            return -1;
        }
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
