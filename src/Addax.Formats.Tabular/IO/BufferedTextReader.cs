// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Text;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.IO;

internal sealed class BufferedTextReader : IDisposable
{
    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly Decoder _decoder;
    private readonly BufferWriter<char> _charBufferWriter;
    private readonly int _byteBufferSize;
    private readonly int _maxCharsPerByte;
    private readonly bool _isStartOfStream;

    private long _bytesDecoded;
    private bool _isPreambleConsumed;
    private bool _isEndOfStream;

    public BufferedTextReader(Stream stream, Encoding encoding, int bufferSize)
    {
        Debug.Assert(bufferSize > 0);

        _stream = stream;
        _encoding = encoding;
        _isStartOfStream = !stream.CanSeek || (stream.Position == 0);
        _decoder = encoding.GetDecoder();
        _byteBufferSize = Math.Min(bufferSize, encoding.GetMaxByteCount(0x00100000));
        _maxCharsPerByte = encoding.GetMaxCharCount(1);
        _charBufferWriter = new(encoding.GetMaxCharCount(Math.Min(bufferSize, 0x00100000)));
        _isPreambleConsumed = !_isStartOfStream || encoding.Preamble.IsEmpty;
    }

    public void Dispose()
    {
        _charBufferWriter.Dispose();
    }

    public bool TryRead()
    {
        var byteBufferSize = Math.Min(_isPreambleConsumed ? _byteBufferSize : Math.Max(_byteBufferSize, _encoding.Preamble.Length), _charBufferWriter.RemainingCount / _maxCharsPerByte);

        if (byteBufferSize == 0)
        {
            return false;
        }

        using var byteBuffer = new ArrayBuffer<byte>(byteBufferSize);

        var byteBufferUsedSize = _stream.ReadAtLeast(byteBuffer.AsSpan(), byteBufferSize, false);

        Decode(byteBuffer.AsSpan(byteBufferUsedSize), byteBufferUsedSize < byteBufferSize);

        return true;
    }

    public async ValueTask<bool> TryReadAsync(CancellationToken cancellationToken)
    {
        var byteBufferSize = Math.Min(_isPreambleConsumed ? _byteBufferSize : Math.Max(_byteBufferSize, _encoding.Preamble.Length), _charBufferWriter.RemainingCount / _maxCharsPerByte);

        if (byteBufferSize == 0)
        {
            return false;
        }

        using var byteBuffer = new ArrayBuffer<byte>(byteBufferSize);

        var byteBufferUsedSize = await _stream.ReadAtLeastAsync(byteBuffer.AsMemory(), byteBufferSize, false, cancellationToken).ConfigureAwait(false);

        Decode(byteBuffer.AsSpan(byteBufferUsedSize), byteBufferUsedSize < byteBufferSize);

        return true;
    }

    public void Advance(int consumed)
    {
        _charBufferWriter.Consume(consumed);
    }

    private void Decode(ReadOnlySpan<byte> byteBuffer, bool isEndOfStream)
    {
        _isEndOfStream = isEndOfStream;

        if (!_isPreambleConsumed)
        {
            if (byteBuffer.StartsWith(_encoding.Preamble))
            {
                byteBuffer = byteBuffer.Slice(_encoding.Preamble.Length);

                _bytesDecoded += _encoding.Preamble.Length;
            }

            _isPreambleConsumed = true;
        }

        var isCompleted = !isEndOfStream;

        while (!byteBuffer.IsEmpty || !isCompleted)
        {
            var byteBufferSlice = byteBuffer.Slice(0, Math.Min(byteBuffer.Length, 0x00100000));
            var charBuffer = _charBufferWriter.GetSpan(_encoding.GetMaxCharCount(byteBuffer.Length));
            var flushSlice = isEndOfStream && (byteBufferSlice.Length == byteBuffer.Length);

            _decoder.Convert(byteBufferSlice, charBuffer, flushSlice, out var bytesUsed, out var charsUsed, out isCompleted);
            _charBufferWriter.Advance(charsUsed);
            _bytesDecoded += bytesUsed;

            byteBuffer = byteBuffer.Slice(bytesUsed);
        }
    }

    public ArraySegment<char> BufferSegment
    {
        get
        {
            return _charBufferWriter.WrittenSegment;
        }
    }

    public long BytesConsumed
    {
        get
        {
            return _bytesDecoded;
        }
    }

    public bool IsStartOfStream
    {
        get
        {
            return _isStartOfStream;
        }
    }

    public bool IsEndOfStream
    {
        get
        {
            return _isEndOfStream;
        }
    }
}
