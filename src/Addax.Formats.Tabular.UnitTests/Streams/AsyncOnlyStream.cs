namespace Addax.Formats.Tabular.UnitTests.Streams;

internal sealed class AsyncOnlyStream : Stream
{
    private readonly MemoryStream _target;

    public AsyncOnlyStream(MemoryStream target)
    {
        _target = target;
    }

    public override void Flush()
    {
        throw new InvalidOperationException();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) : Task.CompletedTask;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new InvalidOperationException();
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        throw new InvalidOperationException();
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        _target.Write(buffer, offset, count);

        return Task.CompletedTask;
    }

    public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return ValueTask.FromCanceled(cancellationToken);
        }

        _target.Write(buffer.Span);

        return ValueTask.CompletedTask;
    }

    public override bool CanRead
    {
        get
        {
            return false;
        }
    }

    public override bool CanSeek
    {
        get
        {
            return false;
        }
    }

    public override bool CanWrite
    {
        get
        {
            return true;
        }
    }

    public override long Length
    {
        get
        {
            throw new NotSupportedException();
        }
    }

    public override long Position
    {
        get
        {
            throw new NotSupportedException();
        }
        set
        {
            throw new NotSupportedException();
        }
    }
}
