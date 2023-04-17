# Addax

[![NuGet: Addax.Formats.Tabular](https://img.shields.io/nuget/vpre/Addax.Formats.Tabular.svg?style=flat&label=Addax.Formats.Tabular)](https://www.nuget.org/packages/Addax.Formats.Tabular)
[![NuGet: Addax.Formats.Tabular.Analyzers](https://img.shields.io/nuget/vpre/Addax.Formats.Tabular.svg?style=flat&label=Addax.Formats.Tabular.Analyzers)](https://www.nuget.org/packages/Addax.Formats.Tabular.Analyzers)

Addax is a high-performance and low-allocating framework for producing and consuming tabular data, represented as delimiter-separated values. The implementation is based on W3C draft [Model for Tabular Data and Metadata on the Web](https://w3c.github.io/csvw/syntax). The framework uses [System.IO.Pipelines](https://learn.microsoft.com/en-us/dotnet/standard/io/pipelines) for efficient I/O and is fully compatible with [native AOT](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot).

## Quick Start

Reading from and writing to a CSV file, formatted according to [RFC 4180](https://ietf.org/rfc/rfc4180.html):

```cs
var dialect = new TabularDataDialect("\r\n", ',', '\"');

await using var stream = File.OpenRead("example.csv");
await using var reader = new TabularRecordReader(stream, dialect);

await foreach (var record in reader.ReadRecordsAsync<string[]>())
{
    Console.WriteLine(string.Join(',', record.Content!));
}
```
```cs
var dialect = new TabularDataDialect("\r\n", ',', '\"');

await using var stream = File.OpenWrite("example.csv");
await using var writer = new TabularRecordWriter(stream, dialect);

await writer.WriteRecordAsync(new[] { "Hello World!" });
```

## Overview

The framework has built-in support for several primitive and core .NET types:
|Category|Types|
|-|-|
|String|`System.String`, `System.Char`, `System.Text.Rune`, `System.ReadOnlySpan<char>`, `System.ReadOnlySequence<char>`|
|Number|`System.SByte`, `System.Byte`, `System.Int16`, `System.UInt16`, `System.Int32`, `System.UInt32`, `System.Int64`, `System.UInt64`, `System.Int128`, `System.UInt128`, `System.Numerics.BigInteger`, `System.Half`, `System.Single`, `System.Double`, `System.Decimal`, `System.Numerics.Complex`|
|Time|`System.TimeSpan`, `System.TimeOnly`, `System.DateOnly`, `System.DateTime`, `System.DateTimeOffset`|
|Other|`System.Boolean`, `System.Guid`|

The framework supports the following standard line terminators:
- `<U+000A>` - Line Feed (LF).
- `<U+000B>` - Vertical Tab (VT).
- `<U+000C>` - Form Feed (FF).
- `<U+000D>` - Carriage Return (CR).
- `<U+2028>` - Line Separator (LS).
- `<U+2029>` - Paragraph Separator (PS).
- `<U+000D, U+000A>` - Carriage Return followed by Line Feed (CR+LF).

## Working with records

Reading and writing records can be done using `TabularRecordReader` and `TabularRecordWriter` types in the following ways:
- __Using the record converter source generator__:
  - Add reference to `Addax.Formats.Tabular.Analyzers` package.
  - Annotate the required type with `TabularRecordAttribute` attribute.
  - Annotate the required type members with `TabularFieldAttribute` attribute.
- __Using a custom implementation__:
  - Implement a converter by deriving from `TabularRecordConverter<T>`.
  - Provide an instance to reader or writer directly or via `TabularRecordConverterRegistry.Shared`.
- __Using the built-in implementations__:
  - Use auxiliary converters for `string[]` and `IEnumerable<string>` types.

Additional features of record generator:
- Automatically handles fields of `System.Nullable<T>` type.
- Supports records with non-consecutive field indices.

An example of working with records using a generated converter:

```cs
[TabularRecord(strict: false)]
internal struct Experiment
{
    [TabularField(index: 0)]
    public double? Result;
}
```
```cs
await using var reader = new TabularRecordReader(stream, dialect);

await foreach (var record in reader.ReadRecordsAsync<Experiment>())
{
    Console.WriteLine(record.Result);
}
```
```cs
await using var writer = new TabularRecordWriter(stream, dialect);

var record = new Experiment { Result = .9 };

await writer.WriteRecordAsync(record);
```

The annotation attributes can be removed during app trimming using the standard approach:

```xml
<Project>
  <ItemGroup>
    <EmbeddedResource Include="ILLink.LinkAttributes.xml">
      <LogicalName>ILLink.LinkAttributes.xml</LogicalName>
    </EmbeddedResource>
  </ItemGroup>
</Project>
```
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

Reading and writing fields can be done using `TabularFieldReader` and `TabularFieldWriter` types in the following ways:
- __Using specialized methos for standard types__:
  - Read and write fields using `TryGet***(...)`, `Get***(...)`, and `Write***(...)` method groups.
- __Using generic methods for non-standard types__:
  - Implement a converter by deriving from `TabularFieldConverter<T>`.
  - Provide an instance to reader or writer directly or via `TabularFieldConverterRegistry.Shared`.
  - Read and write fields using`TryGet<T>(...)`, `Get<T>(...)`, and `Write<T>(...)` method groups.

The important aspects:
- _Specialized methods is the primary way to work with standard types due to optimized implementation._
- _Behavior of all specialized methos except for `string` type can be overriden with custom converters_.

An example of working with fields:

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

An example of a custom field converter:

```cs
class MyConverter : TabularFieldConverter<DateTime>
{
    public override bool TryFormat(DateTime value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        return value.TryFormat(buffer, out charsWritten, "MM/dd/yyyy", provider);
    }

    public override bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out DateTime value)
    {
        return DateTime.TryParseExact(buffer, "MM/dd/yyyy", provider, default, out value);
    }
}
```

An example of a custom field converter used for generating a record converter:

```cs
[TabularRecord]
internal struct Experiment
{
    [TabularField(index: 0, converter: typeof(MyConverter))]
    public DateTime Timestamp;
}
```

## References

- [W3C XML Schema Definition Language (XSD) 1.1 Part 2: Datatypes](https://w3.org/tr/2012/rec-xmlschema11-2-20120405)
- [Unicode Standard Annex #14: Unicode Line Breaking Algorithm](https://www.unicode.org/reports/tr14/tr14-49.html)
