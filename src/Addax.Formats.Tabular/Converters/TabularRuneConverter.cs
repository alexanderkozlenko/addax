// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularRuneConverter : TabularFieldConverter<Rune>
{
    public override int GetFormatBufferLength(Rune value)
    {
        return 2;
    }

    public override int GetParseBufferLength()
    {
        return 2;
    }

    public override bool TryFormat(Rune value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        return value.TryEncodeToUtf16(buffer, out charsWritten);
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out Rune value)
    {
        return buffer.Length == 1 ?
            Rune.TryCreate(buffer[0], out value) :
            Rune.TryCreate(buffer[0], buffer[1], out value);
    }
}
