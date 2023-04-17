// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularStringConverter : TabularFieldConverter<string>
{
    public override bool TryGetFormatBufferLength(string? value, out int result)
    {
        result = value?.Length ?? 0;

        return true;
    }

    public override bool TryFormat(string? value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        value.AsSpan().CopyTo(buffer);
        charsWritten = value?.Length ?? 0;

        return true;
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out string? value)
    {
        value = !buffer.IsEmpty ? new(buffer) : string.Empty;

        return true;
    }
}
