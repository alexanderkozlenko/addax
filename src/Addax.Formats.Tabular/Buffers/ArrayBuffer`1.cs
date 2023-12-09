// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular.Buffers;

internal readonly struct ArrayBuffer<T> : IDisposable
{
    private readonly T[]? _array;
    private readonly int _offset;
    private readonly int _length;
    private readonly bool _isUntracked;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayBuffer(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);

        _array = ArraySource<T>.ArrayPool.Rent(length);
        _offset = 0;
        _length = length;
        _isUntracked = false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArrayBuffer(T[] array, int offset, int length)
    {
        Debug.Assert(array is not null);
        Debug.Assert(offset >= 0);
        Debug.Assert(offset <= Array.MaxLength);
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);
        Debug.Assert(offset + length <= array.Length);

        _array = array;
        _offset = offset;
        _length = length;
        _isUntracked = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (!IsUntracked && (_array is not null))
        {
            ArraySource<T>.ArrayPool.Return(_array);
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

    public bool IsUntracked
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _isUntracked;
        }
    }
}
