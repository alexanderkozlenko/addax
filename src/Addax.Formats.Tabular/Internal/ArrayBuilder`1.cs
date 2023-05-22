// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular.Internal;

internal struct ArrayBuilder<T> : IDisposable
{
    private T[] _array;
    private int _length;

    private ArrayBuilder(T[] array)
    {
        _array = array;
    }

    public readonly void Dispose()
    {
        ArrayPool<T>.Shared.Return(_array);
    }

    public static ArrayBuilder<T> Create(int capacity)
    {
        return new(ArrayPool<T>.Shared.Rent(capacity));
    }

    public void Add(T item)
    {
        if (_length == _array.Length)
        {
            EnsureCapacity();
        }

        _array[_length++] = item;
    }

    public readonly T[] ToArray()
    {
        if (_length != 0)
        {
            var array = GC.AllocateUninitializedArray<T>(_length);

            Array.Copy(_array, array, _length);

            return array;
        }
        else
        {
            return Array.Empty<T>();
        }
    }

    private void EnsureCapacity()
    {
        var capacity = Math.Max(_length + 1, 2 * _length);

        if ((uint)capacity > (uint)Array.MaxLength)
        {
            capacity = Math.Max(_length + 1, Array.MaxLength);
        }

        var array = ArrayPool<T>.Shared.Rent(capacity);

        Array.Copy(_array, array, _length);
        ArrayPool<T>.Shared.Return(_array);

        _array = array;
    }
}
