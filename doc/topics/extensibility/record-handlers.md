---
uid: urn:topics:extensibility:record-handlers
---

## Addax - Record Handlers

<p />

### A custom handler

<p />

A simple example of a custom record handler that interprets a record as a tuple with point coordinates:

<p />

# [C#](#tab/cs)

```cs
internal class PointHandler : TabularHandler<(double, double)>
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

# [F#](#tab/fs)

```fs
type internal PointHandler() =
    inherit TabularHandler<(double * double)>()

        override this.Read(reader) =
            let mutable item1 = Unchecked.defaultof<double>
            let mutable item2 = Unchecked.defaultof<double>

            reader.TryReadField () |> ignore
            reader.TryGetDouble &item1 |> ignore
            reader.TryReadField () |> ignore
            reader.TryGetDouble &item2 |> ignore

            new TabularRecord<(double * double)>((item1, item2))

        override this.Write(writer, record) =
            let (item1, item2) = record

            writer.WriteDouble item1
            writer.WriteDouble item2
```

---

<p />

# [High-level API (C#)](#tab/api-hl/cs)

The primary approach is to specify the record handler for reader or writer explicitly:

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

Additonally, it can be added to the [TabularRegistry.Handlers](xref:Addax.Formats.Tabular.TabularRegistry.Handlers) collection with generated record handlers:

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

# [High-level API (F#)](#tab/api-hl/fs)

The primary approach is to specify the record handler for reader or writer explicitly:

<p />

```fs
let private handler = new PointHandler()
let dialect = new TabularDialect("\r\n", ',', '\"')

using (new TabularWriter<(double * double)>(File.Create "points.csv", dialect, handler = handler)) (fun writer ->
    let point1 = (double 50.4501, double 30.5234)
    let point2 = (double 45.4215, double 75.6972)

    writer.WriteRecord &point1
    writer.WriteRecord &point2
)

using (new TabularReader<(double * double)>(File.OpenRead "points.csv", dialect, handler = handler)) (fun reader ->
    while reader.TryReadRecord () do
        let (lat, lon) = reader.CurrentRecord

        printfn $"{lat} N, {lon} W"
)
```

<p />

Additonally, it can be added to the [TabularRegistry.Handlers](xref:Addax.Formats.Tabular.TabularRegistry.Handlers) collection with generated record handlers:

<p />

```fs
TabularRegistry.Handlers.Add(typeof<(double * double)>, new PointHandler())

let dialect = new TabularDialect("\r\n", ',', '\"')

using (new TabularWriter<(double * double)>(File.Create "points.csv", dialect)) (fun writer ->
    let point1 = (double 50.4501, double 30.5234)
    let point2 = (double 45.4215, double 75.6972)

    writer.WriteRecord &point1
    writer.WriteRecord &point2
)

using (new TabularReader<(double * double)>(File.OpenRead "points.csv", dialect)) (fun reader ->
    while reader.TryReadRecord () do
        let (lat, lon) = reader.CurrentRecord

        printfn $"{lat} N, {lon} W"
)
```

---
