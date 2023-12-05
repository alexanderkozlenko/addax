---
uid: urn:topics:features
---

## Addax - Features

<p />

### Tabular Fields

<p />

The framework has built-in support for interpreting tabular fields as values of the following types:

<p />

|Type|String Format|Standard|
|:-|:-|:-|
|`System.Boolean`|Lexical space: `"true" | "false" | "1" | "0"`|W3C XSD 1.1 P2|
|`System.Byte`|Format specifier: `"g"`||
|`System.Char`|One UTF-16 code unit||
|`System.DateOnly`|Format: `"yyyy'-'MM'-'dd"`|RFC 3339 / ISO 8601-1:2019|
|`System.DateTime`|Format: `"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"`|RFC 3339 / ISO 8601-1:2019|
|`System.DateTimeOffset`|Format: `"yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"`|RFC 3339 / ISO 8601-1:2019|
|`System.Decimal`|Format specifier: `"g"`||
|`System.Double`|Format specifier: `"g"`||
|`System.Guid`|Format: `"00000000-0000-0000-0000-000000000000"`|RFC 4122|
|`System.Half`|Format specifier: `"g"`||
|`System.Int16`|Format specifier: `"g"`||
|`System.Int32`|Format specifier: `"g"`||
|`System.Int64`|Format specifier: `"g"`||
|`System.Int128`|Format specifier: `"g"`||
|`System.SByte`|Format specifier: `"g"`||
|`System.Single`|Format specifier: `"g"`||
|`System.String`|Up to 2,147,483,591 UTF-16 code units||
|`System.TimeOnly`|Format: `"HH':'mm':'ss.FFFFFFF"`|RFC 3339 / ISO 8601-1:2019|
|`System.TimeSpan`|Format: `"[-]'P'd'DT'h'H'm'M's.FFFFFFF'S'"`|RFC 3339 / ISO 8601-1:2019|
|`System.UInt16`|Format specifier: `"g"`||
|`System.UInt32`|Format specifier: `"g"`||
|`System.UInt64`|Format specifier: `"g"`||
|`System.UInt128`|Format specifier: `"g"`||
|`System.Uri`|See Appx. A "Collected ABNF for URI"|RFC 3986|
|`System.Numerics.BigInteger`|Format specifier: `"g"`||
|`System.Byte[]`|Encoding: "base16" ("hex") or "base64"|RFC 4648|

<p />

Each date and time value converter can be instantiated or derived using a custom format string:

<p />

- [TabularDateOnlyConverter](xref:Addax.Formats.Tabular.Converters.TabularDateOnlyConverter)
- [TabularDateTimeConverter](xref:Addax.Formats.Tabular.Converters.TabularDateTimeConverter)
- [TabularDateTimeOffsetConverter](xref:Addax.Formats.Tabular.Converters.TabularDateTimeOffsetConverter)
- [TabularTimeOnlyConverter](xref:Addax.Formats.Tabular.Converters.TabularTimeOnlyConverter)

<p />

A generated record handler implicitly supports type members of the `Nullable<T>` type with any supported value type as the underlying type. However, a generated record handler requires an explicitly specified value converter for every base16 or base64 binary field mapped:

<p />

- [TabularBase16ArrayConverter](xref:Addax.Formats.Tabular.Converters.TabularBase16ArrayConverter)
- [TabularBase64ArrayConverter](xref:Addax.Formats.Tabular.Converters.TabularBase64ArrayConverter)
- [TabularBase16MemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase16MemoryConverter)
- [TabularBase64MemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase64MemoryConverter)
- [TabularBase16ReadOnlyMemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase16ReadOnlyMemoryConverter)
- [TabularBase64ReadOnlyMemoryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase64ReadOnlyMemoryConverter)

<p />

### Tabular Records

<p />

The framework has built-in support for interpreting tabular records as single-dimensional arrays `T[]` or `Nullable<T>[]` of any supported type (except binary data). For example, tabular records of any file can be interpreted as string arrays, even if the number of fields is not fixed:

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

- [TabularArrayHandler\<T\>](xref:Addax.Formats.Tabular.Handlers.TabularArrayHandler`1)
- [TabularSparseArrayHandler\<T\>](xref:Addax.Formats.Tabular.Handlers.TabularSparseArrayHandler`1)

<p />

### Dialect Inferrence

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

### Performance

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
