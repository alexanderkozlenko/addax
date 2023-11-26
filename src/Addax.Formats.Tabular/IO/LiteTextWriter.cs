// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.IO;

internal sealed class LiteTextWriter : IDisposable, IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly Encoder _encoder;
    private readonly BufferWriter<char> _charBufferWriter;
    private readonly int _byteBufferSize;
    private readonly int _flushThreshold;
    private readonly bool _leaveOpen;
    private readonly bool _isStartOfStream;

    private long _bytesEncoded;
    private bool _isPreambleCommitted;

    public LiteTextWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
    {
        Debug.Assert(stream is not null);
        Debug.Assert(encoding is not null);
        Debug.Assert(bufferSize > 0);
        Debug.Assert(bufferSize <= Array.MaxLength);

        _stream = stream;
        _encoding = encoding;
        _leaveOpen = leaveOpen;

        _encoder = encoding.GetEncoder();
        _byteBufferSize = bufferSize;
        _charBufferWriter = new(encoding.GetMaxCharCount(Math.Min(bufferSize, 0x00100000)));
        _flushThreshold = encoding.GetMaxCharCount(Math.Min(bufferSize, 0x00100000));
        _isStartOfStream = !stream.CanSeek || (stream.Position == 0);
        _isPreambleCommitted = _isStartOfStream && encoding.Preamble.IsEmpty;
    }

    public void Dispose()
    {
        Flush();

        _charBufferWriter.Dispose();

        if (!_leaveOpen)
        {
            _stream.Dispose();
        }
    }

    public async ValueTask DisposeAsync()
    {
        var flushTask = FlushAsync(CancellationToken.None);

        if (!flushTask.IsCompletedSuccessfully)
        {
            await flushTask.ConfigureAwait(false);
        }

        _charBufferWriter.Dispose();

        if (!_leaveOpen)
        {
            var disposeTask = _stream.DisposeAsync();

            if (!disposeTask.IsCompletedSuccessfully)
            {
                await disposeTask.ConfigureAwait(false);
            }
        }
    }

    public void Flush()
    {
        if ((_charBufferWriter.WrittenCount == 0) && _isPreambleCommitted)
        {
            return;
        }

        if (!_isPreambleCommitted)
        {
            _stream.Write(_encoding.Preamble);
            _isPreambleCommitted = true;
        }

        var charBuffer = _charBufferWriter.WrittenMemory.Span;
        var completed = true;

        while (!charBuffer.IsEmpty || !completed)
        {
            var charBufferSlice = charBuffer.Slice(0, Math.Min(charBuffer.Length, 0x00100000));
            var byteBufferSize = Math.Min(_byteBufferSize, _encoding.GetMaxByteCount(charBufferSlice.Length));
            var flush = charBufferSlice.Length == charBuffer.Length;

            using (var byteBuffer = new ArrayBuffer<byte>(byteBufferSize))
            {
                _encoder.Convert(charBufferSlice, byteBuffer.AsSpan(), flush, out var charsUsed, out var bytesUsed, out completed);

                var byteBufferUsed = byteBuffer.AsSpan(bytesUsed);

                _stream.Write(byteBufferUsed);
                _charBufferWriter.Truncate(charsUsed);
                charBuffer = charBuffer.Slice(charsUsed);
                _bytesEncoded += bytesUsed;
            }
        }

        _stream.Flush();
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        if ((_charBufferWriter.WrittenCount == 0) && _isPreambleCommitted)
        {
            return ValueTask.CompletedTask;
        }

        return FlushCoreAsync(cancellationToken);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
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
            }

            var charBuffer = _charBufferWriter.WrittenMemory;
            var isCompleted = true;

            while (!charBuffer.IsEmpty || !isCompleted)
            {
                var charBufferSlice = charBuffer.Slice(0, Math.Min(charBuffer.Length, 0x00100000));
                var byteBufferSize = Math.Min(_byteBufferSize, _encoding.GetMaxByteCount(charBufferSlice.Length));
                var flushSlice = charBufferSlice.Length == charBuffer.Length;

                using (var byteBuffer = new ArrayBuffer<byte>(byteBufferSize))
                {
                    _encoder.Convert(charBufferSlice.Span, byteBuffer.AsSpan(), flushSlice, out var charsUsed, out var bytesUsed, out isCompleted);

                    var byteBufferUsed = byteBuffer.AsMemory(bytesUsed);

                    await _stream.WriteAsync(byteBufferUsed, cancellationToken).ConfigureAwait(false);

                    _charBufferWriter.Truncate(charsUsed);
                    charBuffer = charBuffer.Slice(charsUsed);
                    _bytesEncoded += bytesUsed;
                }
            }

            await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private static int GetByteCount(ReadOnlySpan<char> charBuffer, Encoding encoding)
    {
        var byteCount = 0;

        while (!charBuffer.IsEmpty)
        {
            var charBufferSlice = charBuffer.Slice(0, Math.Min(charBuffer.Length, 0x00100000));

            byteCount += encoding.GetByteCount(charBufferSlice);
            charBuffer = charBuffer.Slice(charBufferSlice.Length);
        }

        return byteCount;
    }

    public int FlushThreshold
    {
        get
        {
            return _flushThreshold;
        }
    }

    public IBufferWriter<char> BufferWriter
    {
        get
        {
            return _charBufferWriter;
        }
    }

    public int WrittenBufferSize
    {
        get
        {
            return _charBufferWriter.WrittenCount;
        }
    }

    public int FreeBufferSize
    {
        get
        {
            return _charBufferWriter.FreeCapacity;
        }
    }

    public bool IsStartOfStream
    {
        get
        {
            return _isStartOfStream;
        }
    }

    public long BytesCommitted
    {
        get
        {
            return _bytesEncoded + GetByteCount(_charBufferWriter.WrittenSpan, _encoding);
        }
    }
}
