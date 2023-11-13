// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.IO;

internal sealed class LiteTextReader : IDisposable, IAsyncDisposable
{
    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly Decoder _decoder;
    private readonly BufferWriter<char> _charBufferWriter;
    private readonly int _byteBufferSize;
    private readonly bool _leaveOpen;
    private readonly bool _isAppending;

    private long _bytesDecoded;
    private bool _isPreambleConsumed;
    private bool _isEndOfStream;

    public LiteTextReader(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
    {
        Debug.Assert(stream is not null);
        Debug.Assert(encoding is not null);
        Debug.Assert(bufferSize > 0);
        Debug.Assert(bufferSize <= Array.MaxLength);

        _stream = stream;
        _encoding = encoding;
        _leaveOpen = leaveOpen;

        _decoder = encoding.GetDecoder();
        _byteBufferSize = bufferSize;
        _charBufferWriter = new(encoding.GetMaxCharCount(Math.Min(bufferSize, 0x00100000)));
        _isAppending = stream.CanSeek && (stream.Position > 0);
        _isPreambleConsumed = encoding.Preamble.IsEmpty || _isAppending;
    }

    public void Dispose()
    {
        _charBufferWriter.Dispose();

        if (!_leaveOpen)
        {
            _stream.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        _charBufferWriter.Dispose();

        if (!_leaveOpen)
        {
            return _stream.DisposeAsync();
        }

        return ValueTask.CompletedTask;
    }

    public bool TryRead()
    {
        if (!_charBufferWriter.HasCapacity)
        {
            return false;
        }

        var byteBufferSize = Math.Min(_byteBufferSize, _encoding.GetMaxByteCount(Math.Min(_charBufferWriter.UnusedSize, 0x00100000)));

        if (!_isPreambleConsumed)
        {
            byteBufferSize = Math.Max(byteBufferSize, _encoding.Preamble.Length);
        }

        using (var byteBuffer = ArrayFactory<byte>.Create(byteBufferSize))
        {
            var byteBufferUsedSize = _stream.ReadAtLeast(byteBuffer.AsSpan(), byteBufferSize, false);
            var byteBufferUsed = byteBuffer.AsReadOnlySpan(0, byteBufferUsedSize);

            _isEndOfStream = byteBufferUsedSize < byteBufferSize;

            if (!_isPreambleConsumed)
            {
                if (byteBufferUsed.StartsWith(_encoding.Preamble))
                {
                    byteBufferUsed = byteBufferUsed.Slice(_encoding.Preamble.Length);
                }

                _isPreambleConsumed = true;
            }

            Decode(byteBufferUsed, _charBufferWriter, _decoder, _isEndOfStream, ref _bytesDecoded);
        }

        return true;
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public async ValueTask<bool> TryReadAsync(CancellationToken cancellationToken)
    {
        if (!_charBufferWriter.HasCapacity)
        {
            return false;
        }

        var byteBufferSize = Math.Min(_byteBufferSize, _encoding.GetMaxByteCount(Math.Min(_charBufferWriter.UnusedSize, 0x00100000)));

        if (!_isPreambleConsumed)
        {
            byteBufferSize = Math.Max(byteBufferSize, _encoding.Preamble.Length);
        }

        using (var byteBuffer = ArrayFactory<byte>.Create(byteBufferSize))
        {
            var byteBufferUsedSize = await _stream.ReadAtLeastAsync(byteBuffer.AsMemory(), byteBufferSize, false, cancellationToken).ConfigureAwait(false);
            var byteBufferUsed = byteBuffer.AsReadOnlyMemory(0, byteBufferUsedSize);

            _isEndOfStream = byteBufferUsedSize < byteBufferSize;

            if (!_isPreambleConsumed)
            {
                if (byteBufferUsed.Span.StartsWith(_encoding.Preamble))
                {
                    byteBufferUsed = byteBufferUsed.Slice(_encoding.Preamble.Length);
                }

                _isPreambleConsumed = true;
            }

            Decode(byteBufferUsed.Span, _charBufferWriter, _decoder, _isEndOfStream, ref _bytesDecoded);
        }

        return true;
    }

    public void Advance(int consumed)
    {
        Debug.Assert(consumed >= 0);
        Debug.Assert(consumed <= Array.MaxLength);
        Debug.Assert(consumed <= _charBufferWriter.WrittenSize);

        _charBufferWriter.Truncate(consumed);
    }

    private static void Decode(ReadOnlySpan<byte> byteBuffer, BufferWriter<char> charBufferWriter, Decoder decoder, bool flush, ref long decodedBytes)
    {
        var isCompleted = true;

        while (!byteBuffer.IsEmpty || !isCompleted)
        {
            var byteBufferSlice = byteBuffer.Slice(0, Math.Min(byteBuffer.Length, 0x00100000));
            var charBuffer = charBufferWriter.GetSpan();
            var flushSlice = flush && byteBufferSlice.Length == byteBuffer.Length;

            decoder.Convert(byteBufferSlice, charBuffer, flushSlice, out var bytesUsed, out var charsUsed, out isCompleted);
            charBufferWriter.Advance(charsUsed);
            byteBuffer = byteBuffer.Slice(bytesUsed);
            decodedBytes += bytesUsed;
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

    public ReadOnlySpan<char> BufferSpan
    {
        get
        {
            return _charBufferWriter.WrittenSpan;
        }
    }

    public ReadOnlyMemory<char> BufferMemory
    {
        get
        {
            return _charBufferWriter.WrittenMemory;
        }
    }

    public bool IsEndOfStream
    {
        get
        {
            return _isEndOfStream;
        }
    }

    public bool IsAppending
    {
        get
        {
            return _isAppending;
        }
    }

    public long BytesConsumed
    {
        get
        {
            return _bytesDecoded - GetByteCount(_charBufferWriter.WrittenSpan, _encoding);
        }
    }
}
