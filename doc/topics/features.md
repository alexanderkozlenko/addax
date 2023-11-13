## Addax - Features

<p />

### Value Types

<p />

The framework has built-in support for working with tabular fields as values of the following types: 

<p />

|Type|Format|Standard|
|-|-|-|
|`System.Boolean`|Lexical space: `true`, `false`, `1`, `0`|W3C XMLSCHEMA11-2|
|`System.Byte`|Format specifier: `g`||
|`System.Char`|One UTF-16 code unit||
|`System.DateOnly`|`yyyy'-'MM'-'dd`|RFC 3339 / ISO 8601-1:2019|
|`System.DateTime`|`yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK`|RFC 3339 / ISO 8601-1:2019|
|`System.DateTimeOffset`|`yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK`|RFC 3339 / ISO 8601-1:2019|
|`System.Decimal`|Format specifier: `g`||
|`System.Double`|Format specifier: `g`||
|`System.Guid`|`00000000-0000-0000-0000-000000000000`|RFC 4122|
|`System.Half`|Format specifier: `g`||
|`System.Int16`|Format specifier: `g`||
|`System.Int32`|Format specifier: `g`||
|`System.Int64`|Format specifier: `g`||
|`System.Int128`|Format specifier: `g`||
|`System.SByte`|Format specifier: `g`||
|`System.Single`|Format specifier: `g`||
|`System.String`|Up to 2,147,483,591 UTF-16 code units||
|`System.TimeOnly`|`HH':'mm':'ss.FFFFFFF`|RFC 3339 / ISO 8601-1:2019|
|`System.TimeSpan`|`[-]'P'd'DT'h'H'm'M's'.'FFFFFFF'S'`|RFC 3339 / ISO 8601-1:2019|
|`System.UInt16`|Format specifier: `g`||
|`System.UInt32`|Format specifier: `g`||
|`System.UInt64`|Format specifier: `g`||
|`System.UInt128`|Format specifier: `g`||
|`System.Uri`|See Appx. A "Collected ABNF for URI"|RFC 3986|
|`System.Numerics.BigInteger`|Format specifier: `g`||
|`System.Byte[]`|Encoding: "base16" ("hex") or "base64"|RFC 4648|

<p />

Any generated record handler also implicitly supports type members of the `System.Nullable<T>` type, where `T` can be any supported value type.

<p />

> [!NOTE]
>  To use a type member of the `System.Byte[]` type with a generated record handler, one of the available converters must be specified explicitly:
>
> <p />
>
> - `Addax.Formats.Tabular.Converters.TabularBase16BinaryConverter`
> - `Addax.Formats.Tabular.Converters.TabularBase64BinaryConverter`

<p />

### Performance

<p />

The field and record readers can advance through tabular data without reading it completely, potentially decreasing overall processing time:

<p />

```cs
public sealed class TabularReader
{
    public bool TrySkipField();
    public ValueTask<bool> TrySkipFieldAsync(CancellationToken cancellationToken);
}

public sealed class TabularReader<T>
{
    public bool TrySkipRecord();
    public ValueTask<bool> TrySkipRecordAsync(CancellationToken cancellationToken);
}
```

<p />

### Memory Usage

<p />

The field reader provides access to the last read field in a way that allows reading the field without additional string allocations:

<p />

```cs
public sealed class TabularReader
{
    public ReadOnlyMemory<char> CurrentField { get; }
}
```

<p />

The default string factory supports a mode with a thread-safe pool based on hash codes, reducing allocations when reading fields as strings:

<p />

```cs
var options = new TabularOptions { StringFactory = new(maxLength: 128) };
```
