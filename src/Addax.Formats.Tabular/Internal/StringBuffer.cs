// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular.Internal;

internal readonly ref struct StringBuffer
{
    private readonly char[]? _array;
    private readonly Span<char> _span;

    public StringBuffer(Span<char> source)
    {
        _span = source;
    }

    public StringBuffer(int length)
    {
        var source = ArrayPool<char>.Shared.Rent(length);

        _array = source;
        _span = source;
    }

    public void Dispose()
    {
        if (_array is not null)
        {
            ArrayPool<char>.Shared.Return(_array);
        }
    }

    public Span<char> AsSpan()
    {
        return _span;
    }
}
