// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1812

namespace Addax.Formats.Tabular.Internal;

internal sealed class EmptyAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    public static EmptyAsyncEnumerable<T> Instance = new();

    private EmptyAsyncEnumerable()
    {
    }

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        return EmptyAsyncEnumerator<T>.Instance;
    }
}
