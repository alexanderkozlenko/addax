// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

internal sealed class TabularStreamReader : IAsyncDisposable
{
    private readonly SequenceSource<char> _bufferSource;
    private readonly PipeReader _pipeReader;
    private readonly Encoding _encoding;
    private readonly Decoder _decoder;

    private long _examined;
    private bool _isPreambleConsumed;
    private bool _isCompleted;

    public TabularStreamReader(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
    {
        _bufferSource = new(Math.Max(1, encoding.GetMaxCharCount(bufferSize)));
        _pipeReader = PipeReader.Create(stream, new(pool: null, bufferSize, Math.Max(1, bufferSize / 4), leaveOpen));
        _encoding = encoding;
        _decoder = encoding.GetDecoder();
        _isPreambleConsumed = encoding.Preamble.IsEmpty;
    }

    public ValueTask DisposeAsync()
    {
        _bufferSource.Dispose();

        return _pipeReader.CompleteAsync();
    }

    public bool TryRead()
    {
        return (_bufferSource.Length > _examined) || _isCompleted;
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask ReadAsync(CancellationToken cancellationToken)
    {
        if (!_isPreambleConsumed)
        {
            var preambleReadResult = await _pipeReader.ReadAtLeastAsync(_encoding.Preamble.Length, cancellationToken).ConfigureAwait(false);
            var preambleReadBuffer = preambleReadResult.Buffer;

            if (BufferStartsWith(preambleReadBuffer, _encoding.Preamble))
            {
                _pipeReader.AdvanceTo(preambleReadBuffer.GetPosition(_encoding.Preamble.Length));
            }

            _isPreambleConsumed = true;
        }

        var textReadResult = await _pipeReader.ReadAsync(cancellationToken).ConfigureAwait(false);
        var textReadBuffer = textReadResult.Buffer;

        Transcoder.Convert(_encoding, _decoder, textReadBuffer, _bufferSource, textReadResult.IsCompleted);

        _pipeReader.AdvanceTo(textReadBuffer.End);
        _isCompleted = textReadResult.IsCompleted;

        static bool BufferStartsWith(in ReadOnlySequence<byte> buffer, ReadOnlySpan<byte> value)
        {
            var reader = new SequenceReader<byte>(buffer);

            return reader.IsNext(value);
        }
    }

    public void Advance(long consumed, long examined)
    {
        Debug.Assert(consumed >= 0);
        Debug.Assert(examined >= consumed);
        Debug.Assert(examined >= _examined);
        Debug.Assert(examined <= _bufferSource.Length);

        if (consumed != 0)
        {
            _bufferSource.Release(consumed);
        }

        _examined = examined - consumed;
    }

    public ReadOnlySequence<char> Buffer
    {
        get
        {
            return _bufferSource.CreateSequence();
        }
    }

    public long Examined
    {
        get
        {
            return _examined;
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
