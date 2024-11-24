// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Collections;

internal sealed class PooledQueue<T> : IDisposable
{
    private T[] _array;
    private int _length;
    private int _head;
    private int _tail;

    public PooledQueue(int capacity)
    {
        Debug.Assert(capacity >= 0);
        Debug.Assert(capacity <= Array.MaxLength);

        _array = BufferSource<T>.ArrayPool.Rent(capacity);
    }

    public void Dispose()
    {
        BufferSource<T>.ArrayPool.Return(_array);
    }

    public void Enqueue(in T item)
    {
        Debug.Assert(_length < Array.MaxLength);

        if (_length == _array.Length)
        {
            Resize();
        }

        _array[_tail] = item;
        _tail = _tail < _array.Length - 1 ? _tail + 1 : 0;
        _length++;
    }

    public T Dequeue()
    {
        Debug.Assert(_length > 0);

        var item = _array[_head];

        _head = _head < _array.Length - 1 ? _head + 1 : 0;
        _length--;

        return item;
    }

    private void Resize()
    {
        var arrayLength = (int)Math.Max(Math.Min(2 * (uint)_array.Length, (uint)Array.MaxLength), (uint)_array.Length + 1);
        var array = BufferSource<T>.ArrayPool.Rent(arrayLength);

        if (_length != 0)
        {
            if (_head < _tail)
            {
                Array.Copy(_array, _head, array, 0, _length);
            }
            else
            {
                Array.Copy(_array, _head, array, 0, _array.Length - _head);
                Array.Copy(_array, 0, array, _array.Length - _head, _tail);
            }
        }

        BufferSource<T>.ArrayPool.Return(_array);

        _array = array;
        _head = 0;
        _tail = _length == arrayLength ? 0 : _length;
    }

    public bool IsEmpty
    {
        get
        {
            return _length == 0;
        }
    }
}
