// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

internal abstract class TabularFieldConverter<T>
    where T : struct
{
    public abstract int GetParseBufferLength();

    public abstract int GetFormatBufferLength(T value);

    public abstract bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out T result);

    public abstract bool TryFormat(T value, Span<char> buffer, IFormatProvider provider, out int charsWritten);
}
