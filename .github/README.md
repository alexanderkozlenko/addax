# Addax

[![NuGet: Addax.Formats.Tabular](https://img.shields.io/nuget/vpre/Addax.Formats.Tabular.svg?style=flat&label=Addax.Formats.Tabular)](https://www.nuget.org/packages/Addax.Formats.Tabular)
[![NuGet: Addax.Formats.Tabular.Analyzers](https://img.shields.io/nuget/vpre/Addax.Formats.Tabular.svg?style=flat&label=Addax.Formats.Tabular.Analyzers)](https://www.nuget.org/packages/Addax.Formats.Tabular.Analyzers)

Addax is a high-performance and low-allocating framework for producing and consuming tabular data, represented as delimiter-separated values. The implementation is based on W3C draft [Model for Tabular Data and Metadata on the Web](https://w3c.github.io/csvw/syntax).

## Quick Start

The following code works on a record level with a CSV file, formatted according to [RFC 4180](https://ietf.org/rfc/rfc4180.html):

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

The framework consists of two NuGet packages:
- `Addax.Formats.Tabular` - contains core types that provide low-level access to data.
- `Addax.Formats.Tabular.Analyzers` - contains source generator for converters that provide high-level access to data.

The main types in the framework are:
- `TabularDataDialect` - defines separators for tabular data.
- `TabularFieldReader` - provides read-only access to tabular data on field level.
- `TabularFieldWriter` - provides write-only access to tabular data on field level.
- `TabularRecordReader` - provides read-only access to tabular data on record level.
- `TabularRecordWriter` - provides write-only access to tabular data on record level.

The framework supports the following standard line terminators:
- `<U+000A>` - Line Feed (LF).
- `<U+000B>` - Vertical Tab (VT).
- `<U+000C>` - Form Feed (FF).
- `<U+000D>` - Carriage Return (CR).
- `<U+2028>` - Line Separator (LS).
- `<U+2029>` - Paragraph Separator (PS).
- `<U+000D, U+000A>` - Carriage Return followed by Line Feed (CR+LF).

The framework has built-in support for several primitive and core .NET types:
|Category|Types|
|-|-|
|String|`System.String`, `System.Char`, `System.Text.Rune`, `System.ReadOnlySpan<char>`, `System.ReadOnlySequence<char>`|
|Number|`System.SByte`, `System.Byte`, `System.Int16`, `System.UInt16`, `System.Int32`, `System.UInt32`, `System.Int64`, `System.UInt64`, `System.Int128`, `System.UInt128`, `System.Numerics.BigInteger`, `System.Half`, `System.Single`, `System.Double`, `System.Decimal`, `System.Numerics.Complex`|
|Time|`System.TimeSpan`, `System.TimeOnly`, `System.DateOnly`, `System.DateTime`, `System.DateTimeOffset`|
|Other|`System.Boolean`, `System.Guid`|

## High-Level API

Tabular records can be represented as classes, structures, record classes, and record structures; tabular fields can be represented as either fields or properties. Automatic generation of record converters is adaptive to type definitions and has two modes: non-strict (default) and strict. The non-strict mode is designed to tolerate various issues, such as tabular structure errors, missing values, or an extra new line at the end of a file. The framework also has a built-in record converter, where a record is represented as `string[]` (however, it is not recommended for memory-critical code, as it allocates an array per record during reading).

Reading data from a tabular file as records using a generated converter:

```cs
await using var reader = new TabularRecordReader(stream, dialect);

await foreach (var record in reader.ReadRecordsAsync<Experiment>())
{
    Console.WriteLine(record.Result);
}

...

[TabularRecord(strict: false)]
internal struct Experiment
{
    [TabularField(index: 0)]
    public double? Result;
}
```

Writing data to a tabular file as records using a generated converter:

```cs
await using var writer = new TabularRecordWriter(stream, dialect);

var record = new Experiment { Result = .9 };

await writer.WriteRecordAsync(record);

...

[TabularRecord(strict: false)]
internal struct Experiment
{
    [TabularField(index: 0)]
    public double? Result;
}
```

## Low-Level API

Reading data from a tabular file as fields:

```cs
await using var reader = new TabularFieldReader(stream, dialect);

while (await reader.MoveNextRecordAsync(cancellationToken))
{
    while (await reader.ReadFieldAsync(cancellationToken))
    {
        reader.TryGetDouble(out var result);
    }
}
```

Writing data to a tabular file as fields:

```cs
await using var writer = new TabularFieldWriter(stream, dialect);

writer.BeginRecord();
writer.WriteDouble(.9);

await writer.FlushAsync();
```

Using a custom format for fields (supported for reading or writing):

```cs
await using var reader = new TabularFieldReader(stream, dialect);

await reader.MoveNextRecordAsync();
await reader.ReadFieldAsync();

var value = reader.Get(static (s, p) => DateTime.ParseExact(s, "MM/dd/yyyy", p));
```

Removing framework attributes during app trimming:

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

## References

- [W3C XML Schema Definition Language (XSD) 1.1 Part 2: Datatypes](https://w3.org/tr/2012/rec-xmlschema11-2-20120405)
- [Unicode Standard Annex #14: Unicode Line Breaking Algorithm](https://www.unicode.org/reports/tr14/tr14-49.html)
