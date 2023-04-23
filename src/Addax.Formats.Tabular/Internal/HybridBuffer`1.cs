// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular.Internal;

internal readonly ref struct HybridBuffer<T>
{
    private readonly T[]? _array;
    private readonly Span<T> _span;

    private HybridBuffer(T[]? array, Span<T> span)
    {
        _array = array;
        _span = span;
    }

    public void Dispose()
    {
        if (_array is not null)
        {
            ArrayPool<T>.Shared.Return(_array);
        }
    }

    public static HybridBuffer<T> Create(Span<T> span)
    {
        return new(null, span);
    }

    public static HybridBuffer<T> Create(int length)
    {
        var array = ArrayPool<T>.Shared.Rent(length);

        return new(array, new(array, 0, length));
    }

    public Span<T> AsSpan()
    {
        return _span;
    }
}
