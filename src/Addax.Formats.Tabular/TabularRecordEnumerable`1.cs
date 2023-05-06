// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

internal sealed class TabularRecordEnumerable<T> : IEnumerable<TabularRecord<T>>
{
    private readonly TabularFieldReader _reader;
    private readonly TabularRecordReaderContext _context;
    private readonly TabularRecordConverter<T> _converter;
    private readonly CancellationToken _cancellationToken;

    public TabularRecordEnumerable(TabularFieldReader reader, TabularRecordReaderContext context, TabularRecordConverter<T> converter, CancellationToken cancellationToken)
    {
        _reader = reader;
        _context = context;
        _converter = converter;
        _cancellationToken = cancellationToken;
    }

    public IEnumerator<TabularRecord<T>> GetEnumerator()
    {
        if (_reader.PositionType is TabularPositionType.EndOfStream)
        {
            return Singleton<EmptyEnumerator<TabularRecord<T>>>.Instance;
        }

        return new TabularRecordEnumerator<T>(this, _cancellationToken);
    }

    public bool MoveNextRecord()
    {
        return _reader.MoveNextRecord(_cancellationToken);
    }

    public TabularRecord<T> ReadRecord()
    {
        return _converter.ReadRecord(_reader, _context, _cancellationToken);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
