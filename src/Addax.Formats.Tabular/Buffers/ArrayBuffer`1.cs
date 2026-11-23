// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;

namespace Addax.Formats.Tabular.Buffers;

internal readonly struct ArrayBuffer<T> : IDisposable
{
    private readonly T[]? _array;
    private readonly int _length;

    public ArrayBuffer(int length)
    {
        _array = ArrayPool<T>.Shared.Rent(length);
        _length = length;
    }

    public void Dispose()
    {
        if (_array is not null)
        {
            ArrayPool<T>.Shared.Return(_array);
        }
    }

    public Memory<T> AsMemory(int length)
    {
        Debug.Assert(length <= _length);

        return new(_array, 0, length);
    }

    public Memory<T> AsMemory()
    {
        return new(_array, 0, _length);
    }

    public Span<T> AsSpan(int length)
    {
        Debug.Assert(length <= _length);

        return new(_array, 0, length);
    }

    public Span<T> AsSpan()
    {
        return new(_array, 0, _length);
    }
}
