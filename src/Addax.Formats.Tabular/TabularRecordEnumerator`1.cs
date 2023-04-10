// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular;

internal sealed class TabularRecordEnumerator<T> : IAsyncEnumerator<TabularRecord<T>>
{
    private readonly TabularFieldReader _reader;
    private readonly TabularRecordReaderContext _context;
    private readonly TabularRecordConverter<T> _converter;
    private readonly TabularRecordEnumerable<T> _enumerable;
    private readonly CancellationToken _cancellationToken;

    private TabularRecord<T> _current;

    public TabularRecordEnumerator(TabularFieldReader reader, TabularRecordReaderContext context, TabularRecordConverter<T> converter, TabularRecordEnumerable<T> enumerable, CancellationToken cancellationToken)
    {
        _reader = reader;
        _context = context;
        _converter = converter;
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
        if (_enumerable.Take == 0)
        {
            return false;
        }

        var cancellationToken = _cancellationToken;

        if (!_enumerable.HasCriteria)
        {
            while ((_enumerable.Skip > 0) && await _reader.MoveNextRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                _enumerable.Skip -= 1;
            }

            while (await _reader.MoveNextRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                var record = await _converter.ReadRecordAsync(_reader, _context, cancellationToken).ConfigureAwait(false);

                if (_enumerable.Take > 0)
                {
                    _enumerable.Take -= 1;
                }

                _current = record;

                return true;
            }
        }
        else
        {
            while (await _reader.MoveNextRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                var record = await _converter.ReadRecordAsync(_reader, _context, cancellationToken).ConfigureAwait(false);

                if (!_enumerable.MeetsCriteria(record))
                {
                    continue;
                }

                if (_enumerable.Skip > 0)
                {
                    _enumerable.Skip -= 1;

                    continue;
                }

                if (_enumerable.Take > 0)
                {
                    _enumerable.Take -= 1;
                }

                _current = record;

                return true;
            }
        }

        return false;
    }

    public TabularRecord<T> Current
    {
        get
        {
            return _current;
        }
    }
}
