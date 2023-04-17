// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularGuidConverter : TabularFieldConverter<Guid>
{
    public override bool TryGetFormatBufferLength(Guid value, out int result)
    {
        result = 64;

        return true;
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
