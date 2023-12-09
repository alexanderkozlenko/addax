// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;

namespace Addax.Formats.Tabular.Buffers;

internal sealed class BufferWriter<T> : IBufferWriter<T>, IDisposable
{
    private readonly int _minimumSize;

    private T[] _array;
    private int _offset;
    private int _length;

    public BufferWriter(int minimumSize)
    {
        Debug.Assert(minimumSize > 0);
        Debug.Assert(minimumSize <= Array.MaxLength);

        _minimumSize = minimumSize;
        _array = ArraySource<T>.ArrayPool.Rent(minimumSize);
    }

    public void Dispose()
    {
        ArraySource<T>.ArrayPool.Return(_array);
    }

    public Memory<T> GetMemory(int sizeHint = 0)
    {
        Debug.Assert(sizeHint >= 0);
        Debug.Assert(sizeHint <= Array.MaxLength);

        if (sizeHint == 0)
        {
            sizeHint = _minimumSize;
        }

        if (_offset + _length + sizeHint > _array.Length)
        {
            Resize(sizeHint);
        }

        return _array.AsMemory(_offset + _length);
    }

    public Span<T> GetSpan(int sizeHint = 0)
    {
        Debug.Assert(sizeHint >= 0);
        Debug.Assert(sizeHint <= Array.MaxLength);

        if (sizeHint == 0)
        {
            sizeHint = _minimumSize;
        }

        if (_offset + _length + sizeHint > _array.Length)
        {
            Resize(sizeHint);
        }

        return _array.AsSpan(_offset + _length);
    }

    public void Advance(int count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert(count <= _array.Length - _offset - _length);

        _length += count;
    }

    public void Truncate(int count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert(count <= _length);

        if (count != _length)
        {
            _length -= count;
            _offset += count;
        }
        else
        {
            _offset = 0;
            _length = 0;
        }
    }

    private void Resize(int count)
    {
        var arrayLength = (int)Math.Max(Math.Min(2 * (uint)_length, (uint)Array.MaxLength), (uint)_length + (uint)count);
        var array = ArraySource<T>.ArrayPool.Rent(arrayLength);

        if (_length != 0)
        {
            Array.Copy(_array, _offset, array, 0, _length);
        }

        ArraySource<T>.ArrayPool.Return(_array);

        _array = array;
        _offset = 0;
    }

    public ReadOnlyMemory<T> WrittenMemory
    {
        get
        {
            return new(_array, _offset, _length);
        }
    }

    public ReadOnlySpan<T> WrittenSpan
    {
        get
        {
            return new(_array, _offset, _length);
        }
    }

    public int WrittenCount
    {
        get
        {
            return _length;
        }
    }

    public int FreeCapacity
    {
        get
        {
            return Array.MaxLength - _length;
        }
    }

    public bool HasCapacity
    {
        get
        {
            return _length < Array.MaxLength;
        }
    }
}
