// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

internal sealed class TabularStreamWriter : IBufferWriter<char>, IDisposable, IAsyncDisposable
{
    private readonly SequenceSource<byte> _byteBuffer;
    private readonly SequenceSource<char> _charBuffer;
    private readonly Stream _stream;
    private readonly Encoding _encoding;
    private readonly Encoder _encoder;
    private readonly bool _leaveOpen;

    private bool _isPreambleCommitted;

    public TabularStreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
    {
        _byteBuffer = new(bufferSize);
        _charBuffer = new(GetCharBufferSize(encoding, bufferSize));
        _stream = stream;
        _encoding = encoding;
        _encoder = encoding.GetEncoder();
        _leaveOpen = leaveOpen;
        _isPreambleCommitted = encoding.Preamble.IsEmpty;
    }

    public void Dispose()
    {
        Flush(CancellationToken.None);

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

        if (!_leaveOpen)
        {
            var disposeTask = _stream.DisposeAsync();

            if (!disposeTask.IsCompletedSuccessfully)
            {
                await disposeTask.ConfigureAwait(false);
            }
        }
    }

    public Memory<char> GetMemory(int sizeHint = 0)
    {
        return _charBuffer.GetMemory(sizeHint);
    }

    public Span<char> GetSpan(int sizeHint = 0)
    {
        return _charBuffer.GetSpan(sizeHint);
    }

    public void Advance(int count)
    {
        _charBuffer.Advance(count);
    }

    public void Flush(CancellationToken cancellationToken)
    {
        if (_charBuffer.Length == 0)
        {
            return;
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (!_isPreambleCommitted)
        {
            _stream.Write(_encoding.Preamble);
            _isPreambleCommitted = true;
        }

        try
        {
            var chars = _charBuffer.ToSequence();

            foreach (var segment in chars)
            {
                cancellationToken.ThrowIfCancellationRequested();

                Transcoder.Convert(_encoding, _encoder, segment.Span, _byteBuffer);
            }

            Transcoder.Flush(_encoding, _encoder, _byteBuffer);

            var bytes = _byteBuffer.ToSequence();

            foreach (var segment in bytes)
            {
                cancellationToken.ThrowIfCancellationRequested();

                _stream.Write(segment.Span);
            }

            _stream.Flush();
            _charBuffer.Clear();
        }
        finally
        {
            _byteBuffer.Clear();
        }
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        if (_charBuffer.Length == 0)
        {
            return ValueTask.CompletedTask;
        }
        else
        {
            return FlushAsyncCore(cancellationToken);
        }

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask FlushAsyncCore(CancellationToken cancellationToken)
        {
            if (!_isPreambleCommitted)
            {
                var preambleBufferLength = _encoding.Preamble.Length;
                var preambleBuffer = ArrayPool<byte>.Shared.Rent(preambleBufferLength);

                try
                {
                    var preambleBufferMemory = preambleBuffer.AsMemory(0, preambleBufferLength);

                    _encoding.Preamble.CopyTo(preambleBufferMemory.Span);

                    await _stream.WriteAsync(preambleBufferMemory, cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(preambleBuffer);
                }

                _isPreambleCommitted = true;
            }

            try
            {
                var chars = _charBuffer.ToSequence();

                foreach (var segment in chars)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Transcoder.Convert(_encoding, _encoder, segment.Span, _byteBuffer);
                }

                Transcoder.Flush(_encoding, _encoder, _byteBuffer);

                var bytes = _byteBuffer.ToSequence();

                foreach (var segment in bytes)
                {
                    await _stream.WriteAsync(segment, cancellationToken).ConfigureAwait(false);
                }

                await _stream.FlushAsync(cancellationToken).ConfigureAwait(false);

                _charBuffer.Clear();
            }
            finally
            {
                _byteBuffer.Clear();
            }
        }
    }

    private static int GetCharBufferSize(Encoding encoding, int bufferSize)
    {
        return Math.Max(1, encoding.GetMaxCharCount(bufferSize));
    }

    public long UnflushedChars
    {
        get
        {
            return _charBuffer.Length;
        }
    }
}
