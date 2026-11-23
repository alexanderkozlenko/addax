# Overview

Addax is a high-throughput, forward-only framework for reading and writing tabular data in delimiter-separated formats of any dialect. The data model follows the W3C recommendation [Model for Tabular Data and Metadata on the Web](https://w3.org/TR/2015/REC-tabular-data-model-20151217) (2015). Typed values are read and written in fixed, culture-independent formats based on W3C CSVW, ISO 8601-1:2019, RFC 3987, RFC 4648 and RFC 9562. The library has two APIs. The high-level API consists of `TabularReader<T>` and `TabularWriter<T>`. They read or write one record per call and delegate the mapping between a record and a .NET object to a record handler, generated at compile time from attributes or written by hand. The low-level API consists of `TabularReader` and `TabularWriter`, on which the high-level API is built. They move forward through a stream one field at a time and read or write each field as a string, as a character sequence, or as a typed value.

- **High-level API** should be used when the records have a fixed structure that maps to a custom type, with or without a header, comments are only skipped, and each record is used as a whole.
- **Low-level API** should be used when the structure varies or is known only at run time, the stream contains comments to process, only some fields are needed, or a field is processed without creating a string.

The library and its source generator ship together in the `Addax.Formats.Tabular` NuGet package. Every reader and writer needs a `TabularDialect`, which defines the line terminator, the delimiter and the quote character, and optionally a separate escape character and a comment prefix. The examples use the RFC 4180 dialect: records end with `CRLF`, fields are separated by commas, and fields that need it are enclosed in double quotes. Both examples write the same two books to a file and read them back, first with the high-level API and then with the low-level API.

## Examples: High-level API

The source generator creates a record handler at compile time that maps the members of the type to fields. Write the records to a file, then read them back one record at a time. The writer writes one record per `TabularWriter<T>.WriteRecord()` call, and the reader moves to each record and returns it as a `Book`:

```cs
var books = new Book[]
{
    new("Lewis Carroll", "Alice's Adventures in Wonderland", new(1865, 11, 9)),
    new("H. G. Wells", "The Time Machine", new(1895, 5, 7))
};

var dialect = new TabularDialect("\r\n", ',', '"');

using (var stream = File.Create("books.csv"))
using (var writer = new TabularWriter<Book>(stream, dialect))
{
    foreach (var book in books)
    {
        writer.WriteRecord(book);
    }
}

using (var stream = File.OpenRead("books.csv"))
using (var reader = new TabularReader<Book>(stream, dialect))
{
    while (reader.TryReadRecord())
    {
        var book = reader.CurrentRecord;

        Console.WriteLine($"{book.Author}: {book.Title} ({book.Published})");
    }
}

[TabularRecord]
internal record Book(
    [property: TabularFieldOrder(0)] string Author,
    [property: TabularFieldOrder(1)] string Title,
    [property: TabularFieldOrder(2)] DateOnly Published);
```

## Examples: Low-level API

The same file, written and read one field at a time. The writer writes fields until `TabularWriter.WriteEndOfRecord()` ends the record, and the reader moves to each record and reads its fields in order:

```cs
var books = new[]
{
    new Book("Lewis Carroll", "Alice's Adventures in Wonderland", new(1865, 11, 9)),
    new Book("H. G. Wells", "The Time Machine", new(1895, 5, 7))
};

var dialect = new TabularDialect("\r\n", ',', '"');

using (var stream = File.Create("books.csv"))
using (var writer = new TabularWriter(stream, dialect))
{
    foreach (var book in books)
    {
        writer.WriteString(book.Author);
        writer.WriteString(book.Title);
        writer.WriteDateOnly(book.Published);
        writer.WriteEndOfRecord();
    }
}

using (var stream = File.OpenRead("books.csv"))
using (var reader = new TabularReader(stream, dialect))
{
    while (reader.TryMoveToNextRecord())
    {
        reader.TryReadField();
        reader.TryGetString(out var bookAuthor);
        reader.TryReadField();
        reader.TryGetString(out var bookTitle);
        reader.TryReadField();
        reader.TryGetDateOnly(out var bookPublished);

        Console.WriteLine($"{bookAuthor}: {bookTitle} ({bookPublished})");
    }
}
```

