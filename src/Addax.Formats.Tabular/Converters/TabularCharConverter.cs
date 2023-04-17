// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularCharConverter : TabularFieldConverter<char>
{
    public override bool TryGetFormatBufferLength(char value, out int result)
    {
        result = 1;

        return true;
    }

    public override bool TryGetParseBufferLength(out int result)
    {
        result = 1;

        return true;
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
