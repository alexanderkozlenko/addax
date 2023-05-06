// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1812

using System.Collections;

namespace Addax.Formats.Tabular.Internal;

internal sealed class EmptyEnumerable<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator()
    {
        return Singleton<EmptyEnumerator<T>>.Instance;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
