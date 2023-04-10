// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularGuidConverter : TabularFieldConverter<Guid>
{
    public override int GetFormatBufferLength(Guid value)
    {
        return 64;
    }

    public override int GetParseBufferLength()
    {
        return Array.MaxLength;
    }

    public override bool TryFormat(Guid value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        return value.TryFormat(buffer, out charsWritten, "d");
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out Guid value)
    {
        return Guid.TryParseExact(buffer, "d", out value);
    }
}
