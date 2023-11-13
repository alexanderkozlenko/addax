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

The primary approach for using a custom record handler is to specify it explicitly:

<p />

```cs
var handler = new PointHandler();

var writer = new TabularWriter<(double, double)>(stream1, dialect, handler: handler);
var reader = new TabularReader<(double, double)>(stream2, dialect, handler: handler);
```

<p />

Additonally, it can be added to the `TabularRegistry.Handlers` shared collection with generated record handlers:

<p />

```cs
TabularRegistry.Handlers[typeof((double, double))] = new PointHandler();
```