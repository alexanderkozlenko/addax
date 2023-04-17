// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularBigIntegerConverter : TabularNumberConverter<BigInteger>
{
    public override bool TryGetFormatBufferLength(BigInteger value, out int result)
    {
        if ((long.MinValue <= value) && (value <= long.MaxValue))
        {
            result = 32;

            return true;
        }

        try
        {
            var bufferLength = checked((int)Math.Ceiling(BigInteger.Log10(BigInteger.Abs(value)) + 1) + 1);

            if (bufferLength > Array.MaxLength)
            {
                result = 0;

                return false;
            }

            result = bufferLength;

            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = 0;

            return false;
        }
        catch (OverflowException)
        {
            result = 0;

            return false;
        }
    }

    protected override NumberStyles Styles
    {
        get
        {
            return NumberStyles.Integer | NumberStyles.AllowThousands;
        }
    }
}
