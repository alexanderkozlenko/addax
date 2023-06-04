// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;

namespace Addax.Formats.Tabular.Converters;

internal abstract class TabularNumberConverter<T> : TabularFieldConverter<T>
    where T : struct, INumber<T>
{
    public sealed override bool TryFormat(T value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        var result = value.TryFormat(buffer, out charsWritten, "g", provider);

        Debug.Assert(result);

        return true;
    }

    public sealed override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out T value)
    {
        if (T.TryParse(buffer, Styles, provider, out value))
        {
            return true;
        }

        buffer = buffer.TrimEnd();

        if (buffer is [.., '%' or '\u2030'])
        {
            if (T.TryParse(buffer[..^1], Styles, provider, out value))
            {
                if (buffer[^1] is '%')
                {
                    value /= T.CreateChecked(100);
                }
                else
                {
                    // The division operation for an 8-bit number types should not throw an exception.

                    if (T.CreateSaturating(1000) / T.CreateChecked(100) == T.CreateChecked(10))
                    {
                        value /= T.CreateChecked(1000);
                    }
                    else
                    {
                        value = T.Zero;
                    }
                }

                return true;
            }
        }

        return false;
    }

    protected abstract NumberStyles Styles
    {
        get;
    }
}
