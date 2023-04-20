// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;

namespace Addax.Formats.Tabular.Primitives;

internal sealed class SequenceSegmentPool<T>
{
    public static readonly SequenceSegmentPool<T> Shared = new(checked(256 * Environment.ProcessorCount));

    private readonly ConcurrentQueue<SequenceSegment<T>> _segments = new();
    private readonly int _capacity;

    private int _count;

    private SequenceSegmentPool(int capacity)
    {
        _capacity = capacity;
    }

    public SequenceSegment<T> Rent(int size)
    {
        if (_segments.TryDequeue(out var segment))
        {
            var count = Interlocked.Decrement(ref _count);

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
        segment.Dispose();

        if (Interlocked.Increment(ref _count) <= _capacity)
        {
            _segments.Enqueue(segment);
        }
        else
        {
            var count = Interlocked.Decrement(ref _count);

            Debug.Assert(count >= 0);
        }
    }
}
