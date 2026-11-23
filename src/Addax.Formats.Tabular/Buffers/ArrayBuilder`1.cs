// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular.Buffers;

internal struct ArrayBuilder<T> : IDisposable
{
    private T[] _array;
    private int _length;

    public ArrayBuilder(int capacity)
    {
        _array = ArrayPool<T>.Shared.Rent(capacity);
    }

    public readonly void Dispose()
    {
        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _array.AsSpan(0, _length).Clear();
        }

        ArrayPool<T>.Shared.Return(_array);
    }

    public void Add(T item)
    {
        if (_length == _array.Length)
        {
            Resize();
        }

        _array[_length++] = item;
    }

    public readonly T[] Build()
    {
        var array = GC.AllocateUninitializedArray<T>(_length);

        Array.Copy(_array, array, _length);

        return array;
    }

    private void Resize()
    {
        var arrayLength = (int)Math.Max(Math.Min(2 * (uint)_array.Length, (uint)Array.MaxLength), (uint)_array.Length + 1);
        var array = ArrayPool<T>.Shared.Rent(arrayLength);

        Array.Copy(_array, array, _length);

        if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
        {
            _array.AsSpan(0, _length).Clear();
        }

        ArrayPool<T>.Shared.Return(_array);

        _array = array;
    }
}
