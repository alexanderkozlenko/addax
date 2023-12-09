---
uid: urn:topics:extensibility:value-converters
---

## Addax - Value Converters

<p />

### A custom converter

<p />

A complete example of a custom value converter that handles `System.DateTime` values represented as Unix timestamps:

<p />

# [C#](#tab/cs)

```cs
internal class UnixEpochDateTimeConverter : TabularConverter<DateTime>
{
    public override bool TryFormat(DateTime value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        var seconds = (long)(value.ToUniversalTime() - DateTime.UnixEpoch).TotalSeconds;

        return seconds.TryFormat(destination, out charsWritten, "g", provider);
    }

    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out DateTime value)
    {
        if (long.TryParse(source, NumberStyles.Integer, provider, out var seconds))
        {
            value = DateTime.UnixEpoch.AddSeconds(seconds);

            return true;
        }
        else
        {
            value = default;

            return false;
        }
    }
}
```

# [F#](#tab/fs)

```fs
type internal UnixEpochDateTimeConverter() =
    inherit TabularConverter<DateTime>()

        override this.TryFormat(value, destination, provider, charsWritten) =
            let seconds = int64 (value.ToUniversalTime () - DateTime.UnixEpoch).TotalSeconds

            seconds.TryFormat(destination, &charsWritten, "g", provider)

        override this.TryParse(source, provider, value) =
            let mutable seconds = Unchecked.defaultof<int64>

            if Int64.TryParse(source, NumberStyles.Integer, provider, &seconds) then
                value <- DateTime.UnixEpoch.AddSeconds (float seconds)
                true
            else
                value <- Unchecked.defaultof<DateTime>
                false
```

---

<p />

# [High-level API (C#)](#tab/api-hl/cs)

```cs
var dialect = new TabularDialect("\r\n", ',', '\"');

using (var writer = new TabularWriter<Book>(File.Create("books.csv"), dialect))
{
    var book1 = new Book
    {
        Author = "Lewis Carroll",
        Title = "Alice's Adventures in Wonderland",
        Published = new(1865, 11, 09, 0, 0, 0, DateTimeKind.Utc)
    };

    writer.WriteRecord(book1);

    var book2 = new Book
    {
        Author = "H. G. Wells",
        Title = "The Time Machine",
        Published = new(1894, 03, 17, 0, 0, 0, DateTimeKind.Utc)
    };

    writer.WriteRecord(book2);
}

using (var reader = new TabularReader<Book>(File.OpenRead("books.csv"), dialect))
{
    while (reader.TryReadRecord())
    {
        var book = reader.CurrentRecord;

        Console.WriteLine($"{book.Author} '{book.Title}' ({book.Published})");
    }
}

[TabularRecord]
internal struct Book
{
    [TabularFieldOrder(0)]
    public string? Author;

    [TabularFieldOrder(1)]
    public string? Title;

    [TabularConverter<UnixEpochDateTimeConverter>]
    [TabularFieldOrder(2)]
    public DateTime? Published;
}
```

# [Low-level API (C#)](#tab/api-ll/cs)

```cs
using Addax.Formats.Tabular;

var converter = new UnixEpochDateTimeConverter();
var dialect = new TabularDialect("\r\n", ',', '\"');

using (var writer = new TabularWriter(File.Create("books.csv"), dialect))
{
    writer.WriteString("Lewis Carroll");
    writer.WriteString("Alice's Adventures in Wonderland");
    writer.Write(new(1865, 11, 09, 0, 0, 0, DateTimeKind.Utc), converter);
    writer.FinishRecord();
    writer.WriteString("H. G. Wells");
    writer.WriteString("The Time Machine");
    writer.Write(new(1894, 03, 17, 0, 0, 0, DateTimeKind.Utc), converter);
    writer.FinishRecord();
}

using (var reader = new TabularReader(File.OpenRead("books.csv"), dialect))
{
    while (reader.TryPickRecord())
    {
        reader.TryReadField();
        reader.TryGetString(out var field0);
        reader.TryReadField();
        reader.TryGetString(out var field1);
        reader.TryReadField();
        reader.TryGet(converter, out var field2);

        Console.WriteLine($"{field0} '{field1}' ({field2})");
    }
}
```

<p />

Consider adding extension methods for using a custom value converter with the low-level API:

<p />

```cs
internal static class TabularUnixDateTimeExtensions
{
    private static readonly UnixEpochDateTimeConverter s_converter = new();

    public static bool TryGetUnixDateTime(this TabularReader reader, out DateTime value)
    {
        return reader.TryGet(s_converter, out value);
    }

    public static DateTime GetUnixDateTime(this TabularReader reader)
    {
        return reader.Get(s_converter);
    }

    public static void WriteUnixDateTime(this TabularWriter writer, DateTime value)
    {
        writer.Write(value, s_converter);
    }

    public static ValueTask WriteUnixDateTimeAsync(this TabularWriter writer, DateTime value, CancellationToken cancellationToken)
    {
        return writer.WriteAsync(value, s_converter, cancellationToken);
    }
}
```

# [High-level API (F#)](#tab/api-hl/fs)

> [!NOTE]
> Using a custom value converter in the high-level API with F# requires a custom record handler.

# [Low-level API (F#)](#tab/api-ll/fs)

```fs
let private converter = new UnixEpochDateTimeConverter()
let dialect = new TabularDialect("\r\n", ',', '\"')

using (new TabularWriter(File.Create "books.csv", dialect)) (fun writer ->
    writer.WriteString "Lewis Carroll"
    writer.WriteString "Alice's Adventures in Wonderland"
    writer.Write (new DateTime(1865, 11, 09, 0, 0, 0, DateTimeKind.Utc), converter)
    writer.FinishRecord ()
    writer.WriteString "H. G. Wells"
    writer.WriteString "The Time Machine"
    writer.Write (new DateTime(1894, 03, 17, 0, 0, 0, DateTimeKind.Utc), converter)
    writer.FinishRecord ()
)

using (new TabularReader(File.OpenRead "books.csv", dialect)) (fun reader ->
    while reader.TryPickRecord () do
        let mutable field0 = Unchecked.defaultof<string>
        let mutable field1 = Unchecked.defaultof<string>
        let mutable field2 = Unchecked.defaultof<DateTime>

        reader.TryReadField () |> ignore
        reader.TryGetString &field0 |> ignore
        reader.TryReadField () |> ignore
        reader.TryGetString &field1 |> ignore
        reader.TryReadField () |> ignore
        reader.TryGet (converter, &field2) |> ignore

        printfn $"{field0} '{field1}' ({field2})"
)
```

---
