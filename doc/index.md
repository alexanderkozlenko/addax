## Addax - Overview

<p />

### Introduction

<p />

The framework defines several fundamental types, however, the core types are the following:

<p />

- `TabularReader` - designed for reading tabular fields as strings or typed values.
- `TabularWriter` - designed for writing tabular fields as strings or typed values.
- `TabularReader<T>` - designed for reading tabular records as instances of the specified type.
- `TabularWriter<T>` - designed for writing tabular records as instances of the specified type.

<p />

All variants of readers and writers provide forward-only access to tabular data. The non-generic reader and writer can process tabular data of any structure and represent the low-level framework API. The generic reader and writer are built on top of the low-level API and represent the high-level framework API. A straightforward value converter abstraction provides an ability to work with tabular fields as typed values. On top of that, a record handler abstraction provides an ability to work with tabular records as instances of the specified type defining the complete reading and writing workflows. Even though record handlers can be created manually, the main approach is to generate record handlers automatically by the built-in source generator based on metadata defined with attributes. Here are a few common examples of practical framework usage:

<p />

### Example #1

<p />

The following example shows how to display the first ten records from a file with an unknown structure, formatted according to the RFC 4180:

<p />

# [High-level API](#tab/high-level-api)

```cs
using Addax.Formats.Tabular;

var stream = File.OpenRead("data.csv");
var dialect = new TabularDialect("\r\n", ',', '\"');

using (var reader = new TabularReader<string?[]>(stream, dialect))
{
    while (reader.TryReadRecord() && (reader.RecordsRead <= 10))
    {
        Console.WriteLine(string.Join('|', reader.CurrentRecord));
    }
}
```

# [Low-level API](#tab/low-level-api)

```cs
using Addax.Formats.Tabular;

var stream = File.OpenRead("data.csv");
var dialect = new TabularDialect("\r\n", ',', '\"');

using (var reader = new TabularReader(stream, dialect))
{
    while (reader.TryPickRecord() && (reader.RecordsRead <= 10))
    {
        while (reader.TryReadField())
        {
            Console.Write(reader.GetString());
            Console.Write('|');
        }

        Console.WriteLine();
    }
}
```

---

<p />

### Example #2

<p />

The following example shows how to process tabular data with a known structure:

<p />

# [High-level API](#tab/high-level-api)

```cs
using Addax.Formats.Tabular;

var dialect = new TabularDialect("\n", ',', '\"');

using (var writer = new TabularWriter<Book>(File.Create("books.csv"), dialect))
{
    var book1 = new Book
    {
        Author = "Lewis Carroll",
        Title = "Alice's Adventures in Wonderland",
        Published = new(1865, 11, 09)
    };

    writer.WriteRecord(book1);

    var book2 = new Book
    {
        Author = "H. G. Wells",
        Title = "The Time Machine",
        Published = new(1894, 03, 17)
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
    
    [TabularFieldOrder(2)]
    public DateOnly? Published;
}
```

# [Low-level API](#tab/low-level-api)

```cs
using Addax.Formats.Tabular;

var dialect = new TabularDialect("\n", ',', '\"');

using (var writer = new TabularWriter(File.Create("books.csv"), dialect))
{
    writer.WriteString("Lewis Carroll");
    writer.WriteString("Alice's Adventures in Wonderland");
    writer.WriteDateOnly(new(1865, 11, 09));
    writer.FinishRecord();
    writer.WriteString("H. G. Wells");
    writer.WriteString("The Time Machine");
    writer.WriteDateOnly(new(1894, 03, 17));
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
        reader.TryGetDateOnly(out var published);

        Console.WriteLine($"{author} '{title}' ({published})");
    }
}
```

---

<p />

### Example #3

<p />

The following example shows how to process tabular data with a header:

<p />

# [High-level API](#tab/high-level-api)

```cs
using Addax.Formats.Tabular;

var dialect = new TabularDialect("\n", ',', '\"');

using (var writer = new TabularWriter<Book>(File.Create("books.csv"), dialect))
{
    var book1 = new Book
    {
        Author = "Lewis Carroll",
        Title = "Alice's Adventures in Wonderland",
        Published = new(1865, 11, 09)
    };

    writer.WriteRecord(book1);

    var book2 = new Book
    {
        Author = "H. G. Wells",
        Title = "The Time Machine",
        Published = new(1894, 03, 17)
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
    [TabularFieldName("author")]
    [TabularFieldOrder(0)]
    public string? Author;
    
    [TabularFieldName("title")]
    [TabularFieldOrder(1)]
    public string? Title;
    
    [TabularFieldName("published")]
    [TabularFieldOrder(2)]
    public DateOnly? Published;
}
```

# [Low-level API](#tab/low-level-api)

```cs
using Addax.Formats.Tabular;

var dialect = new TabularDialect("\n", ',', '\"');

using (var writer = new TabularWriter(File.Create("books.csv"), dialect))
{
    writer.WriteString("author");
    writer.WriteString("title");
    writer.WriteString("published");
    writer.FinishRecord();
    writer.WriteString("Lewis Carroll");
    writer.WriteString("Alice's Adventures in Wonderland");
    writer.WriteDateOnly(new(1865, 11, 09));
    writer.FinishRecord();
    writer.WriteString("H. G. Wells");
    writer.WriteString("The Time Machine");
    writer.WriteDateOnly(new(1894, 03, 17));
    writer.FinishRecord();
}

using (var reader = new TabularReader(File.OpenRead("books.csv"), dialect))
{
    if (reader.TryPickRecord())
    {
        while (reader.TryReadField())
        {
        }
    }

    while (reader.TryPickRecord())
    {
        reader.TryReadField();
        reader.TryGetString(out var author);
        reader.TryReadField();
        reader.TryGetString(out var title);
        reader.TryReadField();
        reader.TryGetDateOnly(out var published);

        Console.WriteLine($"{author} '{title}' ({published})");
    }
}
```

---
