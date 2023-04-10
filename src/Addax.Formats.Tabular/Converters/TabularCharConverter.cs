// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularCharConverter : TabularFieldConverter<char>
{
    public override int GetFormatBufferLength(char value)
    {
        return 1;
    }

    public override int GetParseBufferLength()
    {
        return 1;
    }

    public override bool TryFormat(char value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        buffer[0] = value;
        charsWritten = 1;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out char value)
    {
        value = buffer[0];

        return true;
    }
}
