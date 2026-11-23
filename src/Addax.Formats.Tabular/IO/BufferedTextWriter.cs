// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Text;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.IO;

internal sealed class BufferedTextWriter : IDisposable
{
    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly Encoder _encoder;
    private readonly BufferWriter<char> _charBufferWriter;
    private readonly int _byteBufferSize;
    private readonly int _maxBytesPerChar;
    private readonly int _flushThreshold;
    private readonly bool _isStartOfStream;

    private long _bytesEncoded;
    private bool _isPreambleCommitted;

    public BufferedTextWriter(Stream stream, Encoding encoding, int bufferSize)
    {
        Debug.Assert(bufferSize > 0);

        _stream = stream;
        _encoding = encoding;
        _isStartOfStream = !stream.CanSeek || (stream.Position == 0);
        _encoder = encoding.GetEncoder();
        _byteBufferSize = bufferSize;
        _maxBytesPerChar = encoding.GetMaxByteCount(1);
        _flushThreshold = encoding.GetMaxCharCount(Math.Min(bufferSize, 0x00100000));
        _charBufferWriter = new(_flushThreshold);
        _isPreambleCommitted = !_isStartOfStream || encoding.Preamble.IsEmpty;
    }

    public void Dispose()
    {
        _charBufferWriter.Dispose();
    }

    public void Flush()
    {
        FlushBuffer();

        _stream.Flush();
    }

    public async ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
        await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    public void FlushBuffer()
    {
        if ((_charBufferWriter.WrittenCount == 0) && _isPreambleCommitted)
        {
            return;
        }

        if (!_isPreambleCommitted)
        {
            _stream.Write(_encoding.Preamble);
            _isPreambleCommitted = true;
            _bytesEncoded += _encoding.Preamble.Length;
        }

        var charBuffer = _charBufferWriter.WrittenMemory.Span;
        var completed = true;

        while (!charBuffer.IsEmpty || !completed)
        {
            var charBufferSlice = charBuffer.Slice(0, Math.Min(charBuffer.Length, 0x00100000));
            var byteBufferSize = Math.Max(Math.Min(_byteBufferSize, _encoding.GetMaxByteCount(charBufferSlice.Length)), _maxBytesPerChar);
            var flush = charBufferSlice.Length == charBuffer.Length;

            using var byteBuffer = new ArrayBuffer<byte>(byteBufferSize);

            _encoder.Convert(charBufferSlice, byteBuffer.AsSpan(), flush, out var charsUsed, out var bytesUsed, out completed);

            var byteBufferUsed = byteBuffer.AsSpan(bytesUsed);

            _stream.Write(byteBufferUsed);
            _charBufferWriter.Consume(charsUsed);
            _bytesEncoded += bytesUsed;

            charBuffer = charBuffer.Slice(charsUsed);
        }
    }

    public ValueTask FlushBufferAsync(CancellationToken cancellationToken)
    {
        if ((_charBufferWriter.WrittenCount == 0) && _isPreambleCommitted)
        {
            return ValueTask.CompletedTask;
        }

        return FlushCoreAsync(cancellationToken);

        async ValueTask FlushCoreAsync(CancellationToken cancellationToken)
        {
            if (!_isPreambleCommitted)
            {
                using (var byteBuffer = new ArrayBuffer<byte>(_encoding.Preamble.Length))
                {
                    _encoding.Preamble.CopyTo(byteBuffer.AsSpan());

                    await _stream.WriteAsync(byteBuffer.AsMemory(), cancellationToken).ConfigureAwait(false);
                }

                _isPreambleCommitted = true;
                _bytesEncoded += _encoding.Preamble.Length;
            }

            var charBuffer = _charBufferWriter.WrittenMemory;
            var isCompleted = true;

            while (!charBuffer.IsEmpty || !isCompleted)
            {
                var charBufferSlice = charBuffer.Slice(0, Math.Min(charBuffer.Length, 0x00100000));
                var byteBufferSize = Math.Max(Math.Min(_byteBufferSize, _encoding.GetMaxByteCount(charBufferSlice.Length)), _maxBytesPerChar);
                var flushSlice = charBufferSlice.Length == charBuffer.Length;

                using var byteBuffer = new ArrayBuffer<byte>(byteBufferSize);

                _encoder.Convert(charBufferSlice.Span, byteBuffer.AsSpan(), flushSlice, out var charsUsed, out var bytesUsed, out isCompleted);

                var byteBufferUsed = byteBuffer.AsMemory(bytesUsed);

                await _stream.WriteAsync(byteBufferUsed, cancellationToken).ConfigureAwait(false);

                _charBufferWriter.Consume(charsUsed);
                _bytesEncoded += bytesUsed;

                charBuffer = charBuffer.Slice(charsUsed);
            }
        }
    }

    public BufferWriter<char> BufferWriter
    {
        get
        {
            return _charBufferWriter;
        }
    }

    public long BytesCommitted
    {
        get
        {
            return _bytesEncoded;
        }
    }

    public int RemainingCount
    {
        get
        {
            return _charBufferWriter.RemainingCount;
        }
    }

    public bool IsStartOfStream
    {
        get
        {
            return _isStartOfStream;
        }
    }

    public bool IsFull
    {
        get
        {
            return _charBufferWriter.WrittenCount >= _flushThreshold;
        }
    }
}
