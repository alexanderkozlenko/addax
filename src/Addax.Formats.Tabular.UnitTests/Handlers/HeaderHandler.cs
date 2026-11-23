namespace Addax.Formats.Tabular.UnitTests.Handlers;

internal sealed class HeaderHandler : TabularHandler<string[]>
{
    public override string[] Read(TabularReader reader)
    {
        var values = new List<string>();

        while (reader.TryReadField())
        {
            values.Add(reader.GetString());
        }

        return [.. values];
    }

    public override async ValueTask<string[]> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        var values = new List<string>();

        while (await reader.TryReadFieldAsync(cancellationToken))
        {
            values.Add(reader.GetString());
        }

        return [.. values];
    }

    public override void Write(TabularWriter writer, string[] record)
    {
        foreach (var value in record)
        {
            writer.WriteString(value);
        }
    }

    public override async ValueTask WriteAsync(TabularWriter writer, string[] record, CancellationToken cancellationToken)
    {
        foreach (var value in record)
        {
            await writer.WriteStringAsync(value, cancellationToken);
        }
    }

    public override IEnumerable<string?>? Header
    {
        get
        {
            return ["h1", "h2"];
        }
    }
}
