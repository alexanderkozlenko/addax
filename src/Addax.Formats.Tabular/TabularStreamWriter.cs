// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.IO.Pipelines;
using System.Text;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

internal sealed class TabularStreamWriter : IBufferWriter<char>, IAsyncDisposable
{
    private readonly SequenceSource<char> _bufferSource;
    private readonly PipeWriter _pipeWriter;
    private readonly Encoding _encoding;
    private readonly Encoder _encoder;

    private bool _isPreambleCommitted;

    public TabularStreamWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen)
    {
        _bufferSource = new(Math.Max(1, encoding.GetMaxCharCount(bufferSize)));
        _pipeWriter = PipeWriter.Create(stream, new(pool: null, bufferSize, leaveOpen));
        _encoding = encoding;
        _encoder = encoding.GetEncoder();
        _isPreambleCommitted = encoding.Preamble.IsEmpty;
    }

    public async ValueTask DisposeAsync()
    {
        await FlushAsync(default).ConfigureAwait(false);
        await _pipeWriter.CompleteAsync().ConfigureAwait(false);
    }

    public Memory<char> GetMemory(int sizeHint = 0)
    {
        return _bufferSource.GetMemory(sizeHint);
    }

    public Span<char> GetSpan(int sizeHint = 0)
    {
        return _bufferSource.GetSpan(sizeHint);
    }

    public void Advance(int count)
    {
        _bufferSource.Advance(count);
    }

    public ValueTask FlushAsync(CancellationToken cancellationToken)
    {
        if (_bufferSource.Length == 0)
        {
            return ValueTask.CompletedTask;
        }

        if (!_isPreambleCommitted)
        {
            _encoding.Preamble.CopyTo(_pipeWriter.GetSpan(_encoding.Preamble.Length));
            _pipeWriter.Advance(_encoding.Preamble.Length);
            _isPreambleCommitted = true;
        }

        Transcoder.Convert(_encoding, _encoder, _bufferSource.CreateSequence(), _pipeWriter, flush: true);

        _bufferSource.Clear();

        var flushTask = _pipeWriter.FlushAsync(cancellationToken);

        return flushTask.IsCompletedSuccessfully ? ValueTask.CompletedTask : new(flushTask.AsTask());
    }

    public long UnflushedChars
    {
        get
        {
            return _bufferSource.Length;
        }
    }
}
