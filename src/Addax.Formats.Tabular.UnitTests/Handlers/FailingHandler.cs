namespace Addax.Formats.Tabular.UnitTests.Handlers;

internal sealed class FailingHandler : TabularHandler<string[]>
{
    public override string[] Read(TabularReader reader)
    {
        var values = new List<string>();

        while (reader.TryReadField())
        {
            var value = reader.GetString();

            if (value == "?")
            {
                throw new FormatException();
            }

            values.Add(value);
        }

        return [.. values];
    }

    public override async ValueTask<string[]> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        var values = new List<string>();

        while (await reader.TryReadFieldAsync(cancellationToken))
        {
            var value = reader.GetString();

            if (value == "?")
            {
                throw new FormatException();
            }

            values.Add(value);
        }

        return [.. values];
    }

    public override void Write(TabularWriter writer, string[] record)
    {
        foreach (var value in record)
        {
            if (value == "?")
            {
                throw new FormatException();
            }

            writer.WriteString(value);
        }
    }

    public override async ValueTask WriteAsync(TabularWriter writer, string[] record, CancellationToken cancellationToken)
    {
        foreach (var value in record)
        {
            if (value == "?")
            {
                throw new FormatException();
            }

            await writer.WriteStringAsync(value, cancellationToken);
        }
    }
}
