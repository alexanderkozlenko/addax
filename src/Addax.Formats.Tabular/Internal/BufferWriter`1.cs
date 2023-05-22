// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Internal;

internal ref struct BufferWriter<T>
{
    private readonly Span<T> _buffer;

    private int _writtenCount;

    public BufferWriter(Span<T> buffer)
    {
        _buffer = buffer;
    }

    public void Advance(int count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert(count <= _buffer.Length - _writtenCount);

        _writtenCount += count;
    }

    public void Write(T value)
    {
        _buffer[_writtenCount++] = value;
    }

    public readonly Span<T> WriteBuffer
    {
        get
        {
            return _buffer[_writtenCount..];
        }
    }

    public readonly int WrittenCount
    {
        get
        {
            return _writtenCount;
        }
    }
}
