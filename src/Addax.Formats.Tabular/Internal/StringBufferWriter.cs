// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Internal;

internal ref struct StringBufferWriter
{
    private readonly Span<char> _buffer;

    private int _written;

    public StringBufferWriter(Span<char> buffer)
    {
        _buffer = buffer;
    }

    public void Advance(int count)
    {
        Debug.Assert(count >= 0);
        Debug.Assert(count <= _buffer.Length - _written);

        _written += count;
    }

    public void Write(char value)
    {
        _buffer[_written++] = value;
    }

    public readonly Span<char> FreeBuffer
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
