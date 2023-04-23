// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1812

namespace Addax.Formats.Tabular.Internal;

internal sealed class EmptyAsyncEnumerator<T> : IAsyncEnumerator<T>
{
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
