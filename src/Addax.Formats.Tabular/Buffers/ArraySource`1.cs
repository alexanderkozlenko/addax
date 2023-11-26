// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular.Buffers;

internal static class ArraySource<T>
{
    public static readonly ArrayPool<T> ArrayPool = ArrayPool<T>.Create(Array.MaxLength, 32);
}
