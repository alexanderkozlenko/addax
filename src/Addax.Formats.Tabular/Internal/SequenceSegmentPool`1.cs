// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Addax.Formats.Tabular.Internal;

internal sealed class SequenceSegmentPool<T>
{
    public static readonly SequenceSegmentPool<T> Shared = new(GetPoolCapacity());

    private readonly ConcurrentQueue<SequenceSegment<T>> _segments = new();
    private readonly int _capacity;

    private int _size;

    private SequenceSegmentPool(int capacity)
    {
        _capacity = capacity;
    }

    public SequenceSegment<T> Rent(int size)
    {
        if (_segments.TryDequeue(out var segment))
        {
            var count = Interlocked.Decrement(ref _size);

            Debug.Assert(count >= 0);
        }
        else
        {
            segment = new();
        }

        segment.EnsureCapacity(size);

        return segment;
    }

    public void Return(SequenceSegment<T> segment)
    {
        segment.Clear();

        if (Interlocked.Increment(ref _size) <= _capacity)
        {
            _segments.Enqueue(segment);
        }
        else
        {
            var count = Interlocked.Decrement(ref _size);

            Debug.Assert(count >= 0);
        }
    }

    private static int GetPoolCapacity()
    {
        if ((AppContext.GetData("Addax.Formats.Tabular.SequenceSegmentPoolCapacity") is not int poolCapacity) || (poolCapacity < 0))
        {
            poolCapacity = 256 * Environment.ProcessorCount;
        }

        return poolCapacity;
    }
}
