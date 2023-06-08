// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

internal sealed class TabularRecordAsyncEnumerable<T> : IAsyncEnumerable<TabularRecord<T>>
{
    private readonly TabularFieldReader _reader;
    private readonly TabularRecordReaderContext _context;
    private readonly TabularRecordConverter<T> _converter;
    private readonly CancellationToken _cancellationToken;

    public TabularRecordAsyncEnumerable(TabularFieldReader reader, TabularRecordReaderContext context, TabularRecordConverter<T> converter, CancellationToken cancellationToken)
    {
        _reader = reader;
        _context = context;
        _converter = converter;
        _cancellationToken = cancellationToken;
    }

    public IAsyncEnumerator<TabularRecord<T>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        if (_reader.PositionType is TabularPositionType.EndOfStream)
        {
            return EmptyAsyncEnumerator<TabularRecord<T>>.Instance;
        }

        if (cancellationToken == default)
        {
            cancellationToken = _cancellationToken;
        }

        return new TabularRecordAsyncEnumerator<T>(this, cancellationToken);
    }

    public ValueTask<bool> MoveNextRecordAsync(CancellationToken cancellationToken)
    {
        return _reader.MoveNextRecordAsync(cancellationToken);
    }

    public ValueTask<TabularRecord<T>> ReadRecordAsync(CancellationToken cancellationToken)
    {
        return _converter.ReadRecordAsync(_reader, _context, cancellationToken);
    }
}
