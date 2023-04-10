// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Numerics;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularComplexConverter : TabularFieldConverter<Complex>
{
    public override int GetFormatBufferLength(Complex value)
    {
        return 64;
    }

    public override int GetParseBufferLength()
    {
        return Array.MaxLength;
    }

    public override bool TryFormat(Complex value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        var writer = new BufferWriter<char>(buffer);

        if ((value.Real != 0) || (value.Imaginary == 0))
        {
            var result = value.Real.TryFormat(writer.WriteBuffer, out var charsWrittenR, "g", provider);

            Debug.Assert(result);

            writer.Advance(charsWrittenR);
        }
        if (value.Imaginary != 0)
        {
            if ((value.Real != 0) || (value.Imaginary < 0))
            {
                writer.Write(value.Imaginary < 0 ? '-' : '+');
            }
            if (double.Abs(value.Imaginary) != 1)
            {
                var result = double.Abs(value.Imaginary).TryFormat(writer.WriteBuffer, out var charsWrittenI, "g", provider);

                Debug.Assert(result);

                writer.Advance(charsWrittenI);
            }

            writer.Write('i');
        }

        charsWritten = writer.WrittenCount;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out Complex value)
    {
        buffer = buffer.Trim();

        if (buffer.IsEmpty)
        {
            value = default;

            return false;
        }

        var splitIndex = -1;
        var searchIndex = 0;

        while (searchIndex < buffer.Length)
        {
            var symbolIndex = buffer[searchIndex..].IndexOfAny('-', '+');

            if (symbolIndex < 0)
            {
                break;
            }

            searchIndex += symbolIndex;

            if ((searchIndex != 0) && (buffer[searchIndex - 1] is not ('e' or 'E')))
            {
                splitIndex = searchIndex;

                break;
            }

            searchIndex++;
        }

        const NumberStyles styles =
            NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowExponent |
            NumberStyles.AllowThousands;

        if (splitIndex > 0)
        {
            if (buffer is [.., 'i' or 'I'])
            {
                var fragmentR = buffer[..splitIndex];

                if (double.TryParse(fragmentR, styles, provider, out var valueR))
                {
                    var fragmentI = buffer[splitIndex..^1];

                    if (fragmentI is ['-'])
                    {
                        value = new(valueR, -1);

                        return true;
                    }
                    if (fragmentI is ['+'])
                    {
                        value = new(valueR, +1);

                        return true;
                    }
                    if (double.TryParse(fragmentI, styles, provider, out var valueI))
                    {
                        value = new(valueR, valueI);

                        return true;
                    }
                }
            }
        }
        else
        {
            if (buffer is [.., 'i' or 'I'])
            {
                buffer = buffer[..^1];

                if (buffer is [])
                {
                    value = new(0, +1);

                    return true;
                }
                if (buffer is ['-'])
                {
                    value = new(0, -1);

                    return true;
                }
                if (buffer is ['+'])
                {
                    value = new(0, +1);

                    return true;
                }
                if (double.TryParse(buffer, styles, provider, out var valueI))
                {
                    value = new(0, valueI);

                    return true;
                }
            }
            else
            {
                if (double.TryParse(buffer, styles, provider, out var valueR))
                {
                    value = new(valueR, 0);

                    return true;
                }
            }
        }

        value = default;

        return false;
    }
}
