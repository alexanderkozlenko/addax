namespace Addax.Formats.Tabular.UnitTests.Streams;

internal sealed class FailingFlushStream : MemoryStream
{
    public override void Flush()
    {
        throw new IOException();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return Task.FromException(new IOException());
    }
}
