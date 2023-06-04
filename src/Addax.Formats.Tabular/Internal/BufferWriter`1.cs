// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Internal;

internal ref struct BufferWriter<T>
{
    private readonly Span<T> _buffer;

    private int _written;

    public BufferWriter(Span<T> buffer)
    {
        _buffer = buffer;
    }

    public void Advance(int count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert(count <= _buffer.Length - _written);

        _written += count;
    }

    public void Write(T value)
    {
        _buffer[_written++] = value;
    }

    public readonly Span<T> FreeBuffer
    {
        get
        {
            return _buffer[_written..];
        }
    }

    public readonly int Written
    {
        get
        {
            return _written;
        }
    }
}
