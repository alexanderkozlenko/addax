// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;

namespace Addax.Formats.Tabular.Buffers;

internal struct ArrayBuilder<T> : IDisposable
{
    private T[] _array;
    private int _length;

    public ArrayBuilder(int capacity)
    {
        Debug.Assert(capacity >= 0);
        Debug.Assert(capacity <= Array.MaxLength);

        _array = ArrayFactory<T>.ArrayPool.Rent(capacity);
    }

    public readonly void Dispose()
    {
        ArrayFactory<T>.ArrayPool.Return(_array);
    }

    public void Add(T item)
    {
        if (_length == _array.Length)
        {
            Resize();
        }

        _array[_length++] = item;
    }

    public readonly T[] ToArray()
    {
        var array = GC.AllocateUninitializedArray<T>(_length);

        Array.Copy(_array, array, _length);

        return array;
    }

    private void Resize()
    {
        var arrayLength = (int)Math.Max(Math.Min(2 * (uint)_array.Length, (uint)Array.MaxLength), (uint)_array.Length + 1);
        var array = ArrayFactory<T>.ArrayPool.Rent(arrayLength);

        Array.Copy(_array, array, _length);
        ArrayFactory<T>.ArrayPool.Return(_array);

        _array = array;
    }
}
