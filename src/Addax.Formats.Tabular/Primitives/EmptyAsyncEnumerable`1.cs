// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

internal sealed class EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    public static readonly EmptyAsyncEnumerable<T> Instance = new();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return EmptyAsyncEnumerator<T>.Instance;
    }
}
