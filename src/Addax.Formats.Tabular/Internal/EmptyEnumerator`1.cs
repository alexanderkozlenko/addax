// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1812

using System.Collections;

namespace Addax.Formats.Tabular.Internal;

internal sealed class EmptyEnumerator<T> : IEnumerator<T>
{
    public static EmptyEnumerator<T> Instance = new();

    private EmptyEnumerator()
    {
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        return false;
    }

    public void Reset()
    {
    }

    public T Current
    {
        get
        {
            return default!;
        }
    }

    object? IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }
}
