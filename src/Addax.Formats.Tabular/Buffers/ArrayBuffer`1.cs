// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular.Buffers;

internal readonly struct ArrayBuffer<T> : IDisposable
{
    private readonly T[]? _array;
    private readonly int _offset;
    private readonly int _length;
    private readonly bool _isSegment;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayBuffer(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);

        _array = BufferSource<T>.ArrayPool.Rent(length);
        _length = length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayBuffer(ArraySegment<T> segment)
    {
        _array = segment.Array;
        _offset = segment.Offset;
        _length = segment.Count;
        _isSegment = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (!_isSegment && (_array is not null))
        {
            BufferSource<T>.ArrayPool.Return(_array);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<T> AsMemory(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= _length);

        return new(_array, _offset, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<T> AsMemory()
    {
        return new(_array, _offset, _length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= _length);

        return new(_array, _offset, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> AsSpan()
    {
        return new(_array, _offset, _length);
    }

    public bool IsPooled
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return !_isSegment;
        }
    }
}
