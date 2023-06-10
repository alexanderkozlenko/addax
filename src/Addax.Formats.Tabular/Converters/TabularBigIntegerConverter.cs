// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularBigIntegerConverter : TabularNumberConverter<BigInteger>
{
    public override bool TryGetFormatBufferSize(BigInteger value, out int result)
    {
        if ((value >= long.MinValue) &&
            (value <= long.MaxValue))
        {
            result = 32;

            return true;
        }

        try
        {
            result = checked((int)Math.Ceiling(BigInteger.Log10(BigInteger.Abs(value)) + 1) + 1);

            if (result <= Array.MaxLength)
            {
                return true;
            }

            return false;
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
            return
                NumberStyles.Integer |
                NumberStyles.AllowThousands;
        }
    }
}
