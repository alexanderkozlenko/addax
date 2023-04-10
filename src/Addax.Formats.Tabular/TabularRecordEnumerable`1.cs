// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

internal sealed class TabularRecordEnumerable<T> : IAsyncEnumerable<TabularRecord<T>>
{
    private readonly TabularFieldReader _reader;
    private readonly TabularRecordReaderContext _context;
    private readonly TabularRecordConverter<T> _converter;
    private readonly Predicate<TabularRecord<T>>? _predicate;
    private readonly CancellationToken _cancellationToken;

    public TabularRecordEnumerable(TabularFieldReader reader, TabularRecordReaderContext context, TabularRecordConverter<T> converter, Predicate<TabularRecord<T>>? predicate, long skip, long take, CancellationToken cancellationToken)
    {
        _reader = reader;
        _context = context;
        _converter = converter;
        _predicate = predicate;
        _cancellationToken = cancellationToken;

        Skip = skip;
        Take = take;
    }

    public IAsyncEnumerator<TabularRecord<T>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        if ((Take == 0) || (_reader.PositionType is TabularPositionType.EndOfStream))
        {
            return EmptyAsyncEnumerator<TabularRecord<T>>.Instance;
        }

        if (cancellationToken == default)
        {
            cancellationToken = _cancellationToken;
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new TabularRecordEnumerator<T>(_reader, _context, _converter, this, cancellationToken);
    }

    public bool MeetsCriteria(TabularRecord<T> record)
    {
        Debug.Assert(_predicate is not null);

        return _predicate.Invoke(record);
    }

    public bool HasCriteria
    {
        get
        {
            return _predicate is not null;
        }
    }

    public long Skip
    {
        get;
        set;
    }

    public long Take
    {
        get;
        set;
    }
}
