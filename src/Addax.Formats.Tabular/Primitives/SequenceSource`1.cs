// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA2213

using System.Buffers;

namespace Addax.Formats.Tabular.Primitives;

internal sealed class SequenceSource<T> : IBufferWriter<T>, IDisposable
{
    private readonly int _minimumSegmentSize;

    private SequenceSegment<T>? _sequenceHead;
    private SequenceSegment<T>? _sequenceTail;
    private int _sequenceHeadStart;

    public SequenceSource(int minimumSegmentSize)
    {
        Debug.Assert(minimumSegmentSize > 0);

        _minimumSegmentSize = minimumSegmentSize;
    }

    public void Dispose()
    {
        var currentSegment = _sequenceHead;

        _sequenceTail = null;
        _sequenceHead = null;
        _sequenceHeadStart = 0;

        while (currentSegment is not null)
        {
            var nextSegment = (SequenceSegment<T>?)currentSegment.Next;

            SequenceSegmentPool<T>.Shared.Return(currentSegment);

            currentSegment = nextSegment;
        }
    }

    public ReadOnlySequence<T> CreateSequence()
    {
        var sequenceTail = _sequenceTail;

        if (sequenceTail is null)
        {
            return ReadOnlySequence<T>.Empty;
        }

        var sequenceHead = _sequenceHead;

        return new(sequenceHead!, _sequenceHeadStart, sequenceTail, sequenceTail.Memory.Length);
    }

    public Memory<T> GetMemory(int sizeHint = 0)
    {
        Debug.Assert(sizeHint >= 0);

        if (sizeHint == 0)
        {
            sizeHint = _minimumSegmentSize;
        }
        if (_sequenceTail?.WriteBuffer.Length >= sizeHint)
        {
            return _sequenceTail.WriteBuffer;
        }

        var sequenceTail = SequenceSegmentPool<T>.Shared.Rent(Math.Max(_minimumSegmentSize, sizeHint));

        if (_sequenceTail is not null)
        {
            _sequenceTail.AppendNext(sequenceTail);
        }
        else
        {
            _sequenceHead = sequenceTail;
        }

        _sequenceTail = sequenceTail;

        return sequenceTail.WriteBuffer;
    }

    public Span<T> GetSpan(int sizeHint = 0)
    {
        return GetMemory(sizeHint).Span;
    }

    public void Advance(int count)
    {
        Debug.Assert(count >= 0);

        if (count != 0)
        {
            _sequenceTail!.Advance(count);
        }
    }

    public void Release(long count)
    {
        Debug.Assert(count >= 0);

        var currentSegment = _sequenceHead;
        var currentSegmentStart = _sequenceHeadStart + count;

        while (currentSegment is not null)
        {
            var currentSegmentLength = currentSegment.Memory.Length;

            if (currentSegmentLength > currentSegmentStart)
            {
                currentSegment.ResetRunningIndex();

                break;
            }

            var nextSegment = (SequenceSegment<T>?)currentSegment.Next;

            SequenceSegmentPool<T>.Shared.Return(currentSegment);

            currentSegment = nextSegment;
            currentSegmentStart -= currentSegmentLength;
        }

        _sequenceHead = currentSegment;
        _sequenceHeadStart = (int)currentSegmentStart;

        if (currentSegment is null)
        {
            _sequenceTail = null;
        }
    }

    public long Length
    {
        get
        {
            var sequenceTail = _sequenceTail;

            if (sequenceTail is null)
            {
                return 0;
            }

            return sequenceTail.Memory.Length + sequenceTail.RunningIndex - _sequenceHeadStart;
        }
    }
}
