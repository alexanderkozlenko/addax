// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.CompilerServices;
using System.Text;
using Addax.Formats.Tabular.Primitives;

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

    public ValueTask ReadAsync(CancellationToken cancellationToken)
    {
        if ((_bufferSource.Length > _examined) || _isCompleted)
        {
            return ValueTask.CompletedTask;
        }
        else
        {
            return ReadAsyncCore(cancellationToken);
        }

        [StackTraceHidden]
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask ReadAsyncCore(CancellationToken cancellationToken)
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

            Convert(textReadBuffer, _bufferSource, textReadResult.IsCompleted);

            _pipeReader.AdvanceTo(textReadBuffer.End);
            _isCompleted = textReadResult.IsCompleted;
        }

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

    private void Convert(in ReadOnlySequence<byte> bytes, IBufferWriter<char> writer, bool flush)
    {
        var reader = new SequenceReader<byte>(bytes);
        var completed = false;

        while (!reader.End)
        {
            var bytesFragment = reader.UnreadSpan[..Math.Min(reader.UnreadSpan.Length, 0x00100000)];
            var charsBuffer = writer.GetSpan(_encoding.GetMaxCharCount(bytesFragment.Length));

            _decoder.Convert(bytesFragment, charsBuffer, flush: false, out var bytesUsed, out var charsUsed, out completed);
            reader.Advance(bytesUsed);
            writer.Advance(charsUsed);
        }

        if (!completed && flush)
        {
            var charsBuffer = writer.GetSpan(_encoding.GetMaxCharCount(byteCount: 0));

            _decoder.Convert(bytes: ReadOnlySpan<byte>.Empty, charsBuffer, flush: true, out _, out var charsUsed, out completed);

            Debug.Assert(completed);

            writer.Advance(charsUsed);
        }
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
