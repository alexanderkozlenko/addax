// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;

namespace Addax.Formats.Tabular.Buffers;

internal readonly struct ArrayBuffer<T> : IDisposable
{
    private readonly T[]? _array;
    private readonly int _offset;
    private readonly int _length;
    private readonly bool _isUntracked;

    public ArrayBuffer(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);

        _array = ArraySource<T>.ArrayPool.Rent(length);
        _offset = 0;
        _length = length;
        _isUntracked = false;
    }

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

    public void Dispose()
    {
        if (!IsUntracked && (_array is not null))
        {
            ArraySource<T>.ArrayPool.Return(_array);
        }
    }

    public Memory<T> AsMemory(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);

        return new(_array, _offset, length);
    }

    public Memory<T> AsMemory()
    {
        return new(_array, _offset, _length);
    }

    public Span<T> AsSpan(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);

        return new(_array, _offset, length);
    }

    public Span<T> AsSpan()
    {
        return new(_array, _offset, _length);
    }

    public bool IsUntracked
    {
        get
        {
            return _isUntracked;
        }
    }
}
