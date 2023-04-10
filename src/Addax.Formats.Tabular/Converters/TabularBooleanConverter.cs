// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularBooleanConverter : TabularFieldConverter<bool>
{
    public override int GetFormatBufferLength(bool value)
    {
        return 8;
    }

    public override int GetParseBufferLength()
    {
        return Array.MaxLength;
    }

    public override bool TryFormat(bool value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        var result = value.TryFormat(buffer, out charsWritten);

        Debug.Assert(result);

        buffer[0] += (char)0x20;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out bool value)
    {
        if (bool.TryParse(buffer, out value))
        {
            return true;
        }

        buffer = buffer.Trim();

        if (buffer is ['1'])
        {
            value = true;

            return true;
        }
        if (buffer is ['0'])
        {
            value = false;

            return true;
        }

        return false;
    }
}
