## Addax - Value Converters

<p />

### A custom converter

<p />

A complete example of a custom value converter that handles values of the `System.DateTime` type represented as Unix timestamps:

<p />

```cs
internal sealed class UnixDateTimeConverter : TabularConverter<DateTime>
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

<p />

# [High-level API](#tab/high-level-api)

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
    
    [TabularConverter(typeof(UnixDateTimeConverter))]
    [TabularFieldOrder(2)]
    public DateTime? Published;
}
```

# [Low-level API](#tab/low-level-api)

```cs
using Addax.Formats.Tabular;

var converter = new UnixDateTimeConverter();
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
        reader.TryGetString(out var author);
        reader.TryReadField();
        reader.TryGetString(out var title);
        reader.TryReadField();
        reader.TryGet(converter, out var published);

        Console.WriteLine($"{author} '{title}' ({published})");
    }
}
```

---

<p />

### Standard date and time converters 

<p />

The following converters provide an option to customize the standard behavior by accepting a format specifier in the constructor:

<p />

- `Addax.Formats.Tabular.Converters.TabularDateOnlyConverter`
- `Addax.Formats.Tabular.Converters.TabularDateTimeConverter`
- `Addax.Formats.Tabular.Converters.TabularDateTimeOffsetConverter`
- `Addax.Formats.Tabular.Converters.TabularTimeOnlyConverter`