// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

internal sealed class TabularStreamReader : IDisposable, IAsyncDisposable
{
    private readonly SequenceSource<char> _charBuffer;
    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly Decoder _decoder;
    private readonly int _byteBufferSize;
    private readonly int _charBufferSize;
    private readonly bool _leaveOpen;

    private long _examinedChars;
    private bool _isPreambleConsumed;
    private bool _isCompleted;

    public TabularStreamReader(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
    {
        var charBufferSize = GetCharBufferSize(encoding, bufferSize);

        _byteBufferSize = bufferSize;
        _charBufferSize = charBufferSize;
        _charBuffer = new(charBufferSize);
        _stream = stream;
        _encoding = encoding;
        _decoder = encoding.GetDecoder();
        _leaveOpen = leaveOpen;
        _isPreambleConsumed = encoding.Preamble.IsEmpty;
    }

    public void Dispose()
    {
        _charBuffer.Clear();

        if (!_leaveOpen)
        {
            _stream.Dispose();
        }
    }

    public ValueTask DisposeAsync()
    {
        _charBuffer.Clear();

        if (!_leaveOpen)
        {
            return _stream.DisposeAsync();
        }
        else
        {
            return ValueTask.CompletedTask;
        }
    }

    public void Read(CancellationToken cancellationToken)
    {
        if ((_charBuffer.Length - _examinedChars > _charBufferSize / 4) || _isCompleted)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var isCompleted = false;

        if (!_isPreambleConsumed)
        {
            var readBufferLength = _encoding.Preamble.Length;
            var readBuffer = ArrayPool<byte>.Shared.Rent(readBufferLength);

            try
            {
                var readBufferMemory = readBuffer.AsMemory(0, readBufferLength);
                var readDataLength = _stream.ReadAtLeast(readBufferMemory.Span, readBufferLength, throwOnEndOfStream: false);

                isCompleted = readDataLength < readBufferLength;
                readBufferMemory = readBufferMemory[..readDataLength];

                if (!readBufferMemory.Span.SequenceEqual(_encoding.Preamble))
                {
                    Transcoder.Convert(_encoding, _decoder, readBufferMemory.Span, _charBuffer);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(readBuffer);
            }

            _isPreambleConsumed = true;
        }

        if (!isCompleted)
        {
            var readBufferLength = _byteBufferSize;
            var readBuffer = ArrayPool<byte>.Shared.Rent(readBufferLength);

            try
            {
                var readBufferMemory = readBuffer.AsMemory(0, readBufferLength);
                var readDataLength = _stream.ReadAtLeast(readBufferMemory.Span, readBufferLength, throwOnEndOfStream: false);

                isCompleted = readDataLength < readBufferLength;
                readBufferMemory = readBufferMemory[..readDataLength];

                Transcoder.Convert(_encoding, _decoder, readBufferMemory.Span, _charBuffer);

                if (isCompleted)
                {
                    Transcoder.Flush(_encoding, _decoder, _charBuffer);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(readBuffer);
            }
        }

        _isCompleted = isCompleted;
    }

    public ValueTask ReadAsync(CancellationToken cancellationToken)
    {
        if ((_charBuffer.Length - _examinedChars > _charBufferSize / 4) || _isCompleted)
        {
            return ValueTask.CompletedTask;
        }
        else
        {
            return ReadAsyncCore(cancellationToken);
        }

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask ReadAsyncCore(CancellationToken cancellationToken)
        {
            var isCompleted = false;

            if (!_isPreambleConsumed)
            {
                var readBufferLength = _encoding.Preamble.Length;
                var readBuffer = ArrayPool<byte>.Shared.Rent(readBufferLength);

                try
                {
                    var readBufferMemory = readBuffer.AsMemory(0, readBufferLength);
                    var readDataLength = await _stream.ReadAtLeastAsync(readBufferMemory, readBufferLength, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(false);

                    isCompleted = readDataLength < readBufferLength;
                    readBufferMemory = readBufferMemory[..readDataLength];

                    if (!readBufferMemory.Span.SequenceEqual(_encoding.Preamble))
                    {
                        Transcoder.Convert(_encoding, _decoder, readBufferMemory.Span, _charBuffer);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(readBuffer);
                }

                _isPreambleConsumed = true;
            }

            if (!isCompleted)
            {
                var readBufferLength = _byteBufferSize;
                var readBuffer = ArrayPool<byte>.Shared.Rent(readBufferLength);

                try
                {
                    var readBufferMemory = readBuffer.AsMemory(0, readBufferLength);
                    var readDataLength = await _stream.ReadAtLeastAsync(readBufferMemory, readBufferLength, throwOnEndOfStream: false, cancellationToken).ConfigureAwait(false);

                    isCompleted = readDataLength < readBufferLength;
                    readBufferMemory = readBufferMemory[..readDataLength];

                    Transcoder.Convert(_encoding, _decoder, readBufferMemory.Span, _charBuffer);

                    if (isCompleted)
                    {
                        Transcoder.Flush(_encoding, _decoder, _charBuffer);
                    }
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(readBuffer);
                }
            }

            _isCompleted = isCompleted;
        }
    }

    public void Advance(long consumed)
    {
        Advance(consumed, consumed);
    }

    public void Advance(long consumed, long examined)
    {
        Debug.Assert(consumed >= 0);
        Debug.Assert(consumed <= _charBuffer.Length);
        Debug.Assert(examined >= consumed);
        Debug.Assert(examined >= _examinedChars);

        if (consumed != 0)
        {
            _charBuffer.Release(consumed);
        }

        _examinedChars = examined - consumed;
    }

    private static int GetCharBufferSize(Encoding encoding, int bufferSize)
    {
        return Math.Max(1, encoding.GetMaxCharCount(bufferSize));
    }

    public ReadOnlySequence<char> Buffer
    {
        get
        {
            return _charBuffer.ToSequence();
        }
    }

    public long ExaminedChars
    {
        get
        {
            return _examinedChars;
        }
    }

    public bool IsCompleted
    {
        get
        {
            return _isCompleted;
        }
    }
}
