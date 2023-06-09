# Addax

Addax is a high-performance and low-allocating framework for producing and consuming tabular data, represented as delimiter-separated values. The implementation is based on W3C draft recommendation "Model for Tabular Data and Metadata on the Web".

## Packages

|Name|Version|
|:-|:-|
|Addax.Formats.Tabular|[![NuGet](https://img.shields.io/nuget/vpre/Addax.Formats.Tabular.svg?style=flat)](https://www.nuget.org/packages/Addax.Formats.Tabular)|
|Addax.Formats.Tabular.Analyzers|[![NuGet](https://img.shields.io/nuget/vpre/Addax.Formats.Tabular.Analyzers.svg?style=flat)](https://www.nuget.org/packages/Addax.Formats.Tabular.Analyzers)|

## Overview

The main features are:

- The framework supports data in any tabular dialect and stream encoding.
- The framework supports working with tabular fields longer than 2,147,483,591 characters.
- The framework and any generated code are fully compatible with native AOT compilation.

An example of reading from and writing to a CSV file formatted according to RFC 4180:

```cs
var dialect = new TabularDataDialect("\r\n", ',', '\"');

await using var stream = File.OpenRead("example.csv");
await using var reader = new TabularRecordReader(stream, dialect);

await foreach (var record in reader.ReadRecordsAsync<string[]>())
{
    Console.WriteLine(string.Join(',', record.Content));
}
```
```cs
var dialect = new TabularDataDialect("\r\n", ',', '\"');

await using var stream = File.OpenWrite("example.csv");
await using var writer = new TabularRecordWriter(stream, dialect);

await writer.WriteRecordAsync(new[] { "Hello World!" });
```

## Working with records

An example of working with records using a generated converter:

```cs
[TabularRecord(strict: false)]
public struct Experiment
{
    [TabularField(index: 0)]
    public double? Result;
}
```
```cs
await using var reader = new TabularRecordReader(stream, dialect);

var record = await reader.ReadRecordAsync<Experiment>();
```
```cs
await using var writer = new TabularRecordWriter(stream, dialect);

var record = new Experiment { Result = .9 };

await writer.WriteRecordAsync(record);
```

An example of `ILLink.LinkAttributes.xml` for trimming annotation attributes:

```xml
<linker>
  <assembly fullname="MyAssembly">
    <type fullname="Addax.Formats.Tabular.TabularFieldAttribute">
      <attribute internal="RemoveAttributeInstances" />
    </type>
    <type fullname="Addax.Formats.Tabular.TabularRecordAttribute">
      <attribute internal="RemoveAttributeInstances" />
    </type>
  </assembly>
</linker>
```

## Working with fields

An example of working with fields using a built-in converter:

```cs
await using var reader = new TabularFieldReader(stream, dialect);

while (await reader.MoveNextRecordAsync())
{
    while (await reader.ReadFieldAsync())
    {
        reader.TryGetDouble(out var result);
    }
}
```
```cs
await using var writer = new TabularFieldWriter(stream, dialect);

writer.BeginRecord();
writer.WriteDouble(.9);

await writer.FlushAsync();
```

An example of defining a record converter with a custom field converter:

```cs
[TabularRecord]
public struct Experiment
{
    [TabularField(0, typeof(MyConverter))]
    public DateTime Timestamp;
}
```
```cs
public class MyConverter : TabularFieldConverter<DateTime>
{
    public override bool TryFormat(DateTime v, Span<char> s, IFormatProvider p, out int n)
    {
        return v.TryFormat(s, out n, "MM/dd/yyyy", p);
    }

    public override bool TryParse(ReadOnlySpan<char> s, IFormatProvider p, out DateTime v)
    {
        return DateTime.TryParseExact(s, "MM/dd/yyyy", p, default, out v);
    }
}
```

## Supported runtime types

The framework has built-in support for the following types:

|Category|Runtime Types|
|:-|:-|
|Text|`System.String`</br>`System.Char`</br>`System.Text.Rune`</br>`System.ReadOnlySpan<char>`</br>`System.ReadOnlySequence<char>`|
|Number|`System.SByte`</br>`System.Int16`</br>`System.Int32`</br>`System.Int64`</br>`System.Int128`</br>`System.Byte`</br>`System.UInt16`</br>`System.UInt32`</br>`System.UInt64`</br>`System.UInt128`</br>`System.Half`</br>`System.Single`</br>`System.Double`</br>`System.Decimal`</br>`System.Numerics.BigInteger`</br>`System.Numerics.Complex`|
|Date and Time|`System.TimeSpan`</br>`System.TimeOnly`</br>`System.DateOnly`</br>`System.DateTime`</br>`System.DateTimeOffset`|
||`System.Boolean`</br>`System.Guid`|

## Supported line terminators

The framework supports the standard Unicode line terminators according to UAX #14:

|Name|Characters|Description|
|:-|:-|:-|
|`LF`|`<U+000A>`|Line Feed|
|`VT`|`<U+000B>`|Vertical Tab|
|`FF`|`<U+000C>`|Form Feed|
|`CR`|`<U+000D>`|Carriage Return|
|`LS`|`<U+2028>`|Line Separator|
|`PS`|`<U+2029>`|Paragraph Separator|
|`CR+LF`|`<U+000D, U+000A>`|Carriage Return followed by Line Feed|

## References

- [W3C Model for Tabular Data and Metadata on the Web](https://w3c.github.io/csvw/syntax)
- [W3C XML Schema Definition Language (XSD) 1.1 Part 2: Datatypes](https://w3.org/tr/2012/rec-xmlschema11-2-20120405)
- [Unicode Standard Annex #14: Unicode Line Breaking Algorithm](https://www.unicode.org/reports/tr14/tr14-49.html)
