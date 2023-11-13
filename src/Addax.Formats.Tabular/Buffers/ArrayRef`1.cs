// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;

namespace Addax.Formats.Tabular.Buffers;

internal readonly struct ArrayRef<T> : IDisposable
{
    private readonly T[]? _array;
    private readonly int _offset;
    private readonly int _length;
    private readonly bool _isUntracked;

    public ArrayRef(T[] array, int offset, int length, bool isUntracked)
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
        _isUntracked = isUntracked;
    }

    public void Dispose()
    {
        if (!IsUntracked && (_array is not null))
        {
            ArrayFactory<T>.ArrayPool.Return(_array);
        }
    }

    public Memory<T> AsMemory()
    {
        return new(_array, _offset, _length);
    }

    public Span<T> AsSpan()
    {
        return new(_array, _offset, _length);
    }

    public ReadOnlyMemory<T> AsReadOnlyMemory(int offset, int length)
    {
        Debug.Assert(offset >= 0);
        Debug.Assert(offset <= Array.MaxLength);
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);
        Debug.Assert(offset + length <= _length);

        return new(_array, _offset + offset, length);
    }

    public ReadOnlyMemory<T> AsReadOnlyMemory()
    {
        return new(_array, _offset, _length);
    }

    public ReadOnlySpan<T> AsReadOnlySpan(int offset, int length)
    {
        Debug.Assert(offset >= 0);
        Debug.Assert(offset <= Array.MaxLength);
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);
        Debug.Assert(offset + length <= _length);

        return new(_array, _offset + offset, length);
    }

    public ReadOnlySpan<T> AsReadOnlySpan()
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
