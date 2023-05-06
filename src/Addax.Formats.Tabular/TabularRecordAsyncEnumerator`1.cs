// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular;

internal sealed class TabularRecordAsyncEnumerator<T> : IAsyncEnumerator<TabularRecord<T>>
{
    private readonly TabularRecordAsyncEnumerable<T> _enumerable;
    private readonly CancellationToken _cancellationToken;

    private TabularRecord<T> _current;

    public TabularRecordAsyncEnumerator(TabularRecordAsyncEnumerable<T> enumerable, CancellationToken cancellationToken)
    {
        _enumerable = enumerable;
        _cancellationToken = cancellationToken;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public async ValueTask<bool> MoveNextAsync()
    {
        if (await _enumerable.MoveNextRecordAsync(_cancellationToken).ConfigureAwait(false))
        {
            _current = await _enumerable.ReadRecordAsync(_cancellationToken).ConfigureAwait(false);

            return true;
        }
        else
        {
            _current = default;

            return false;
        }
    }

    public TabularRecord<T> Current
    {
        get
        {
            return _current;
        }
    }
}
