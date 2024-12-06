---
uid: urn:topics:features
---

# Addax - Features

<p />

## Tabular Fields

<p />

The framework has built-in support for interpreting tabular fields as values of the following types:

<p />

|Type|Format (Read)|Format (Write)|Standard|
|:-|:-|:-|:-|
|`System.Boolean`|`"true" | "false" | "1" | "0"` |`"true" | "false"`|W3C CSVW|
|`System.Byte`|`g | g%`|`g`|W3C CSVW|
|`System.Char`||||
|`System.DateOnly`|`yyyy'-'MM'-'dd`|`o`|ISO 8601-1:2019, RFC 3339|
|`System.DateTime`|`yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK`|`o`|ISO 8601-1:2019, RFC 3339|
|`System.DateTimeOffset`|`yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK`|`o`|ISO 8601-1:2019, RFC 3339|
|`System.Decimal`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Double`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Guid`|`d`|`d`|RFC 4122|
|`System.Half`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Int16`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Int32`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Int64`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Int128`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.SByte`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Single`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.String`||||
|`System.TimeOnly`|`HH':'mm':'ss.FFFFFFF`|`o`|ISO 8601-1:2019, RFC 3339|
|`System.TimeSpan`|`[-]'P'd'DT'h'H'm'M's.FFFFFFF'S'` __\*__|`[-]'P'd'DT'h'H'm'M's.fffffff'S'`|ISO 8601-1:2019, RFC 3339|
|`System.UInt16`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.UInt32`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.UInt64`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.UInt128`|`g | g% | g‰`|`g`|W3C CSVW|
|`System.Uri`||||
|`System.Numerics.BigInteger`|`g`|`g`|W3C CSVW|
|`System.Byte[]`|`base16 | base64` __\*\*__|`base16 | base64` __\*\*__|RFC 4648|
|`System.Text.Rune`||||

<p />

__\*__ - The complete list of formats supported for reading `System.TimeSpan` values is the following:

<p />

- `'P'd'DT'h'H'm'M's'.'FFFFFFF'S'`
- `'P'd'DT'h'H'm'M's'S'`
- `'P'd'DT'h'H'm'M'`
- `'P'd'DT'h'H's'.'FFFFFFF'S'`
- `'P'd'DT'h'H's'S'`
- `'P'd'DT'h'H'`
- `'P'd'DT'm'M's'.'FFFFFFF'S'`
- `'P'd'DT'm'M's'S'`
- `'P'd'DT'm'M'`
- `'P'd'DT's'.'FFFFFFF'S'`
- `'P'd'DT's'S'`
- `'P'd'D'`
- `'PT'h'H'm'M's'.'FFFFFFF'S'`
- `'PT'h'H'm'M's'S'`
- `'PT'h'H'm'M'`
- `'PT'h'H's'.'FFFFFFF'S'`
- `'PT'h'H's'S'`
- `'PT'h'H'`
- `'PT'm'M's'.'FFFFFFF'S'`
- `'PT'm'M's'S'`
- `'PT'm'M'`
- `'PT's'.'FFFFFFF'S'`
- `'PT's'S'`

<p />

__\*\*__ - The binary data values require the encoding specified explicitly by the corresponding API or converter:

<p />

|Type|Encoding|Low-Level API|High-Level API|Converter|
|:-|:-:|:-:|:-:|:-|
|`System.Byte[]`|base16|Supported|Converter|[TabularBase16ArrayConverter](xref:Addax.Formats.Tabular.Converters.TabularBase16ArrayConverter)|
|`System.Byte[]`|base64|Supported|Converter|[TabularBase64ArrayConverter](xref:Addax.Formats.Tabular.Converters.TabularBase64ArrayConverter)|
|`System.Memory<System.Byte>`|base16|Converter|Converter|[TabularBase16MemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase16MemoryConverter)|
|`System.Memory<System.Byte>`|base64|Converter|Converter|[TabularBase64MemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase64MemoryConverter)|
|`System.ReadOnlyMemory<System.Byte>`|base16|Converter|Converter|[TabularBase16ReadOnlyMemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase16ReadOnlyMemoryConverter)|
|`System.ReadOnlyMemory<System.Byte>`|base64|Converter|Converter|[TabularBase64ReadOnlyMemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase64ReadOnlyMemoryConverter)|

<p />

The default behavior of the built-in date and time value converters can be modified by using a custom format string:

<p />

|Type|Converter|
|:-|:-|
|`System.DateOnly`|[TabularDateOnlyConverter](xref:Addax.Formats.Tabular.Converters.TabularDateOnlyConverter)|
|`System.DateTime`|[TabularDateTimeConverter](xref:Addax.Formats.Tabular.Converters.TabularDateTimeConverter)|
|`System.DateTimeOffset`|[TabularDateTimeOffsetConverter](xref:Addax.Formats.Tabular.Converters.TabularDateTimeOffsetConverter)|
|`System.TimeOnly`|[TabularTimeOnlyConverter](xref:Addax.Formats.Tabular.Converters.TabularTimeOnlyConverter)|

<p />

A generated record handler implicitly supports type members of the `System.Nullable<T>` type with any supported value type as the underlying type.

<p />

## Tabular Records

<p />

The framework has built-in support for interpreting tabular records as single-dimensional arrays `T[]` or `System.Nullable<T>[]` of any supported type (except binary data). For example, tabular records of any file can be interpreted as string arrays, even if the number of fields is not fixed:

<p />

# [C#](#tab/cs)

```cs
var dialect = new TabularDialect("\r\n", ',', '\"');

using (var reader = new TabularReader<string?[]>(File.OpenRead("data.csv"), dialect))
{
    while (reader.TryReadRecord())
    {
        Console.WriteLine(string.Join('|', reader.CurrentRecord));
    }
}
```

# [F#](#tab/fs)

```fs
let dialect = new TabularDialect("\r\n", ',', '\"')

using (new TabularReader<array<string>>(File.OpenRead "books.csv", dialect)) (fun reader ->
    while reader.TryReadRecord ()) do
        printfn "%s" (String.concat "|" reader.CurrentRecord)
)
```

---

<p />

The framework also provides generic record handlers for working with tabular records as single-dimensional arrays of any type:

<p />

|Type|Handler|
|:-|:-|
|`T`|[TabularArrayHandler\<T\>](xref:Addax.Formats.Tabular.Handlers.TabularArrayHandler`1)|
|`System.Nullable<T>`|[TabularSparseArrayHandler\<T\>](xref:Addax.Formats.Tabular.Handlers.TabularSparseArrayHandler`1)|

<p />

## Dialect Inferrence

<p />

A tabular dialect can be inferred from a stream based on frequency of the eligible token values:

<p />

# [C#](#tab/cs)

```cs
var dialect = TabularData.InferDialect(File.OpenRead("books.csv"), ["\n", "\r\n"], [','], ['"']);
```

# [F#](#tab/fs)

```fs
let dialect = TabularData.InferDialect(File.OpenRead "books.csv", [ "\n"; "\r\n" ], [ ',' ], [ '"' ])
```

---

<p />

## Performance

<p />

The field and record readers can advance through tabular data without reading it completely, potentially decreasing overall processing time:

<p />

# [C#](#tab/cs)

```cs
public class TabularReader
{
    public bool TrySkipField();
    public ValueTask<bool> TrySkipFieldAsync(CancellationToken cancellationToken);
}

public class TabularReader<T>
{
    public bool TrySkipRecord();
    public ValueTask<bool> TrySkipRecordAsync(CancellationToken cancellationToken);
}
```

# [F#](#tab/fs)

```fs
type TabularReader =
    member TrySkipField: unit -> bool
    member TrySkipFieldAsync: CancellationToken -> ValueTask<bool>

type TabularReader<'T> =
    member TrySkipRecord: unit -> bool
    member TrySkipRecordAsync: CancellationToken -> ValueTask<bool>
```

---

<p />

The field reader provides access to the last read field in a way that allows reading the field without additional string allocations:

<p />

# [C#](#tab/cs)

```cs
public class TabularReader
{
    public ReadOnlyMemory<char> CurrentField
    {
        get;
    }
}
```

# [F#](#tab/fs)

```fs
type TabularReader =
    member CurrentField: ReadOnlyMemory<char>
        with get()
```

---

<p />

The default string factory supports a mode with a thread-safe pool based on hash codes, reducing allocations when reading fields as strings:

<p />

# [C#](#tab/cs)

```cs
var options = new TabularOptions
{
    StringFactory = new(maxLength: 128)
};
```

# [F#](#tab/fs)

```fs
let options = new TabularOptions (
    StringFactory = new TabularStringFactory(maxLength = 128)
)
```

---

<p />

## Guidelines

<p />

- **DO** reuse instances of the [TabularDialect](xref:Addax.Formats.Tabular.TabularDialect) type as they contain pre-calculated data for parsing and formatting.
