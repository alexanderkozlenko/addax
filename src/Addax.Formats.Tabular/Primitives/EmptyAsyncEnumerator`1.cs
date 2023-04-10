// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Primitives;

internal sealed class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>
{
    public static readonly EmptyAsyncEnumerator<T> Instance = new();

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask<bool> MoveNextAsync()
    {
        return new(false);
    }

    public T Current
    {
        get
        {
            return default!;
        }
    }
}
