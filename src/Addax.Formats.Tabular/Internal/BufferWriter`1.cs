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

    public Span<T> WriteBuffer
    {
        get
        {
            return _buffer[_writtenCount..];
        }
    }

    public int WrittenCount
    {
        get
        {
            return _writtenCount;
        }
    }
}
