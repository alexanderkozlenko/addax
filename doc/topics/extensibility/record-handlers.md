## Addax - Record Handlers

<p />

### A custom handler

<p />

A simple example of a custom record handler that interprets a record as the `(double, double)` type:

<p />

```cs
internal sealed class PointHandler : TabularHandler<(double, double)>
{
    public override TabularRecord<(double, double)> Read(TabularReader reader)
    {
        reader.TryReadField();
        reader.TryGetDouble(out var item0);
        reader.TryReadField();
        reader.TryGetDouble(out var item1);

        return new((item0, item1));
    }

    public override void Write(TabularWriter writer, (double, double) record)
    {
        writer.WriteDouble(record.Item1);
        writer.WriteDouble(record.Item2);
    }
}
```

<p />

# [High-level API](#tab/high-level-api)

The primary approach is to specify the required record handler for reader or writer explicitly:

<p />

```cs
var handler = new PointHandler();
var dialect = new TabularDialect("\r\n", ',', '\"');

using (var writer = new TabularWriter<(double, double)>(File.Create("points.csv"), dialect, handler: handler))
{
    writer.WriteRecord((50.4501, 30.5234));
    writer.WriteRecord((45.4215, 75.6972));
}

using (var reader = new TabularReader<(double, double)>(File.OpenRead("points.csv"), dialect, handler: handler))
{
    while (reader.TryReadRecord())
    {
        var (lat, lon) = reader.CurrentRecord;

        Console.WriteLine($"{lat} N, {lon} W");
    }
}
```

<p />

Additonally, it can be added to the `TabularRegistry.Handlers` shared collection with generated record handlers:

<p />

```cs
TabularRegistry.Handlers[typeof((double, double))] = new PointHandler();

var dialect = new TabularDialect("\r\n", ',', '\"');

using (var writer = new TabularWriter<(double, double)>(File.Create("points.csv"), dialect))
{
    writer.WriteRecord((50.4501, 30.5234));
    writer.WriteRecord((45.4215, 75.6972));
}

using (var reader = new TabularReader<(double, double)>(File.OpenRead("points.csv"), dialect))
{
    while (reader.TryReadRecord())
    {
        var (lat, lon) = reader.CurrentRecord;

        Console.WriteLine($"{lat} N, {lon} W");
    }
}
```

# [Low-level API](#tab/low-level-api)

N/A

---
