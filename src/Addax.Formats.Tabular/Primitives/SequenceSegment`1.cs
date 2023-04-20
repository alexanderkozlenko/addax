// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;

namespace Addax.Formats.Tabular.Primitives;

internal sealed class SequenceSegment<T> : ReadOnlySequenceSegment<T>, IDisposable
{
    private T[]? _buffer;

    public void Dispose()
    {
        Debug.Assert(_buffer is not null);

        ArrayPool<T>.Shared.Return(_buffer);

        _buffer = null;

        Memory = ReadOnlyMemory<T>.Empty;
        Next = null;
        RunningIndex = 0;
    }

    public void EnsureCapacity(int size)
    {
        Debug.Assert(_buffer is null);

        _buffer = ArrayPool<T>.Shared.Rent(size);
    }

    public void Advance(int count)
    {
        Debug.Assert(count >= 0);

        Memory = new(_buffer, 0, Memory.Length + count);
    }

    public void AppendNext(SequenceSegment<T> segment)
    {
        Debug.Assert(Next is null);

        segment.RunningIndex = Memory.Length + RunningIndex;

        Next = segment;
    }

    public void ResetRunningIndex()
    {
        var runningIndex = RunningIndex;

        if (runningIndex != 0)
        {
            var segment = this;

            do
            {
                segment.RunningIndex -= runningIndex;
                segment = (SequenceSegment<T>?)segment.Next;
            }
            while (segment is not null);
        }
    }

    public Memory<T> FreeBuffer
    {
        get
        {
            return _buffer.AsMemory(Memory.Length);
        }
    }
}
