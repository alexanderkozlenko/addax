---
uid: urn:topics:features
---

## Addax - Features

<p />

### Value Types

<p />

The framework has built-in support for working with tabular fields as values of the following types: 

<p />

|Runtime Type|Representation|Standard|
|-|-|-|
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
|`System.String`|Up to `2,147,483,591` UTF-16 code units||
|`System.TimeOnly`|Format: `"HH':'mm':'ss.FFFFFFF"`|RFC 3339 / ISO 8601-1:2019|
|`System.TimeSpan`|Format: `"[-]'P'd'DT'h'H'm'M's'.'FFFFFFF'S'"`|RFC 3339 / ISO 8601-1:2019|
|`System.UInt16`|Format specifier: `"g"`||
|`System.UInt32`|Format specifier: `"g"`||
|`System.UInt64`|Format specifier: `"g"`||
|`System.UInt128`|Format specifier: `"g"`||
|`System.Uri`|See Appx. A "Collected ABNF for URI"|RFC 3986|
|`System.Numerics.BigInteger`|Format specifier: `"g"`||
|`System.Byte[]`|Encoding: "base16" ("hex") or "base64"|RFC 4648|

<p />

Any generated record handler also supports type members of the `System.Nullable<T>` type with any supported value type as the underlying type.

<p />

To use a type member of the `System.Byte[]` type with a generated record handler, one of the available converters must be specified explicitly:

<p />

- [TabularBase16BinaryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase16BinaryConverter)
- [TabularBase64BinaryConverter](xref:Addax.Formats.Tabular.Converters.TabularBase64BinaryConverter)

<p />

### Dialect Inferrence

<p />

> [!NOTE]
> The section describes a preview feature that is available in the latest pre-release package.

<p />

A dialect can be inferred from a stream based on frequency of the eligible token values:

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

### Memory Usage

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

### References

<p />

- [W3C - Model for Tabular Data and Metadata on the Web](https://w3.org/TR/2015/REC-tabular-data-model-20151217)
