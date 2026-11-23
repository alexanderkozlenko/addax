namespace Addax.Formats.Tabular.UnitTests.Handlers;

internal sealed class AsyncOnlyHandler : TabularHandler<string[]>
{
    public override async ValueTask<string[]> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        var values = new List<string>();

        while (await reader.TryReadFieldAsync(cancellationToken))
        {
            values.Add(reader.GetString());
        }

        return [.. values];
    }

    public override async ValueTask WriteAsync(TabularWriter writer, string[] record, CancellationToken cancellationToken)
    {
        foreach (var value in record)
        {
            await writer.WriteStringAsync(value, cancellationToken);
        }
    }
}
