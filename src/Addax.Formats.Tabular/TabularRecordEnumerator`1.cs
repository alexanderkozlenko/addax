// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections;

namespace Addax.Formats.Tabular;

internal sealed class TabularRecordEnumerator<T> : IEnumerator<TabularRecord<T>>
{
    private readonly TabularRecordEnumerable<T> _enumerable;

    private TabularRecord<T> _current;

    public TabularRecordEnumerator(TabularRecordEnumerable<T> enumerable, CancellationToken cancellationToken)
    {
        _enumerable = enumerable;
    }

    public void Dispose()
    {
    }

    public bool MoveNext()
    {
        if (_enumerable.MoveNextRecord())
        {
            _current = _enumerable.ReadRecord();

            return true;
        }
        else
        {
            _current = default;

            return false;
        }
    }

    public void Reset()
    {
    }

    public TabularRecord<T> Current
    {
        get
        {
            return _current;
        }
    }

    object IEnumerator.Current
    {
        get
        {
            return Current;
        }
    }
}
