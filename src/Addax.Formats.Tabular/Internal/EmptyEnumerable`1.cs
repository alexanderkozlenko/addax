// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1812

using System.Collections;

namespace Addax.Formats.Tabular.Internal;

internal sealed class EmptyEnumerable<T> : IEnumerable<T>
{
    public static EmptyEnumerable<T> Instance = new();

    private EmptyEnumerable()
    {
    }

    public IEnumerator<T> GetEnumerator()
    {
        return EmptyEnumerator<T>.Instance;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
