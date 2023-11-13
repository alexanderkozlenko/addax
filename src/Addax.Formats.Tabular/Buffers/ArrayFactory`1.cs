// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;

namespace Addax.Formats.Tabular.Buffers;

internal static class ArrayFactory<T>
{
    public static readonly ArrayPool<T> ArrayPool = ArrayPool<T>.Create(Array.MaxLength, 32);

    public static ArrayRef<T> Create(int length)
    {
        Debug.Assert(length >= 0);
        Debug.Assert(length <= Array.MaxLength);

        return new(ArrayPool.Rent(length), 0, length, false);
    }
}
