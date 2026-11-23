namespace Addax.Formats.Tabular.UnitTests.Handlers;

internal sealed class SyncOnlyHandler : TabularHandler<string[]>
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

    public override void Write(TabularWriter writer, string[] record)
    {
        foreach (var value in record)
        {
            writer.WriteString(value);
        }
    }
}
