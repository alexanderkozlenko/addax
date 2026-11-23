---
uid: urn:topics:high-level-api
---

# High-level API

<xref:Addax.Formats.Tabular.TabularReader`1> reads each value record of a stream as a `T`, and <xref:Addax.Formats.Tabular.TabularWriter`1> writes each `T` as a record, through a record handler. Both take the same stream, dialect and options as the low-level API, and every member that reads or writes has an asynchronous counterpart with a cancellation token, for example `TryReadRecordAsync` for `TryReadRecord`. A record handler, a <xref:Addax.Formats.Tabular.TabularHandler`1>, converts between one record and one `T`. The constructor of the reader or writer selects it once:

1. The entry for `typeof(T)` in [TabularOptions.Handlers](xref:Addax.Formats.Tabular.TabularOptions.Handlers) of the options.
2. Otherwise the entry for `typeof(T)` in [TabularRegistry.Handlers](xref:Addax.Formats.Tabular.TabularRegistry.Handlers).
3. Otherwise the constructor throws an `InvalidOperationException`.

The registry holds the built-in array handlers, the handlers generated for `[TabularRecord]` types, and any handler stored in it. A reader or writer keeps the handler it selected, so later changes to the registry do not affect it.

## Reading records

[TryReadRecord](xref:Addax.Formats.Tabular.TabularReader%601.TryReadRecord) reads the next value record into [CurrentRecord](xref:Addax.Formats.Tabular.TabularReader%601.CurrentRecord), and [TrySkipRecord](xref:Addax.Formats.Tabular.TabularReader%601.TrySkipRecord) moves past it without calling the handler. Both skip comment records. [CurrentRecordRef](xref:Addax.Formats.Tabular.TabularReader%601.CurrentRecordRef) returns a read-only reference to the record instead of a copy. The samples read and write `observations.csv`:

```text
city,lat,lon,date,temperature
Kyiv,50.4501,30.5234,2024-02-03,-2.5
Ottawa,45.4215,-75.6972,2024-02-03,-11.5
```

```cs
var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.OpenRead("observations.csv");
using var reader = new TabularReader<Observation>(stream, dialect);

while (reader.TryReadRecord())
{
    var observation = reader.CurrentRecord;

    Console.WriteLine($"{observation.City}: {observation.Temperature}");
}

[TabularRecord]
internal record Observation(
    [property: TabularFieldOrder(0), TabularFieldName("city")] string City,
    [property: TabularFieldOrder(1), TabularFieldName("lat")] double Latitude,
    [property: TabularFieldOrder(2), TabularFieldName("lon")] double Longitude,
    [property: TabularFieldOrder(3), TabularFieldName("date")] DateOnly Date,
    [property: TabularFieldOrder(4), TabularFieldName("temperature")] double Temperature);
```

When the handler's `Header` is not `null`, the first value record of the stream is the header, and the first `TryReadRecord` or `TrySkipRecord` skips it. The header is identified by position only: its fields are not compared with the names in `Header`, and fields map to members by their order. An exception from the handler leaves the reader usable, and the next call continues with the next record. This includes the `TabularException` that the generated and array handlers throw for a missing field or a field that cannot be converted. A failure of the field reader underneath faults the reader.

## Writing records

[WriteRecord](xref:Addax.Formats.Tabular.TabularWriter%601.WriteRecord*) writes one record. When the handler's `Header` is not `null`, the writer writes the header before the first record, or on disposal when no record was written. The writer buffers and flushes like the low-level writer. The sample writes `observations.csv` with the `Observation` type:

```cs
var observations = new Observation[]
{
    new("Kyiv", 50.4501, 30.5234, new(2024, 2, 3), -2.5),
    new("Ottawa", 45.4215, -75.6972, new(2024, 2, 3), -11.5)
};

var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.Create("observations.csv");
using var writer = new TabularWriter<Observation>(stream, dialect);

foreach (var observation in observations)
{
    writer.WriteRecord(observation);
}

[TabularRecord]
internal record Observation(
    [property: TabularFieldOrder(0), TabularFieldName("city")] string City,
    [property: TabularFieldOrder(1), TabularFieldName("lat")] double Latitude,
    [property: TabularFieldOrder(2), TabularFieldName("lon")] double Longitude,
    [property: TabularFieldOrder(3), TabularFieldName("date")] DateOnly Date,
    [property: TabularFieldOrder(4), TabularFieldName("temperature")] double Temperature);
```

```text
city,lat,lon,date,temperature
Kyiv,50.4501,30.5234,2024-02-03,-2.5
Ottawa,45.4215,-75.6972,2024-02-03,-11.5
```

Any exception from `WriteRecord` or `Flush` faults the writer: every further call throws an `InvalidOperationException`, and disposal writes no header. The fields that a failed handler had written stay in the output as a record.

## Generated record handlers

For every class, struct, record class and record struct marked with <xref:Addax.Formats.Tabular.TabularRecordAttribute>, the source generator creates a record handler at compile time, with no reflection at run time. A module initializer of the assembly that declares the type stores the handler in [TabularRegistry.Handlers](xref:Addax.Formats.Tabular.TabularRegistry.Handlers), so every reader and writer for the type finds it. Types the generator cannot map are reported with generator diagnostics.

| Attribute | Target | Effect |
|:-|:-|:-|
| `[TabularRecord]` | class, struct | Generates a record handler for the type. |
| `[TabularFieldOrder(n)]` | field, property | Maps the member to the field at the zero-based position `n` of the record. |
| `[TabularFieldName("name")]` | field, property | Gives the field a name, used in the header. |
| `[TabularConverter<TConverter>]` | field, property | Converts the member's values with `TConverter`. |
| `[assembly: TabularConverter<TConverter>]` | assembly | Converts every value of the type that `TConverter` converts with `TConverter`, in every mapped member of the assembly without a converter attribute of its own. |

Only a member with <xref:Addax.Formats.Tabular.TabularFieldOrderAttribute> is mapped: a field or a property, declared or inherited, that is accessible from any code of the assembly that declares the record type. The handler reads a field into a member it can set, including through an `init` accessor, and writes a field from a member it can get. A generated handler supports reading when the type is not abstract, at least one mapped member can be set, and the handler can construct the type in one of two ways:

- With a parameterless constructor and an object initializer, so `init` accessors and `required` members are supported. Every `required` member must be a mapped member that can be set. The implicit parameterless constructor of a struct counts only when the struct declares no constructor with parameters.
- Otherwise, with the primary constructor of a positional record. Every parameter is mapped with `[property: TabularFieldOrder(n)]`, every mapped member that can be set is such a parameter, the parameters have no modifiers or default values, and the type declares no `required` members.

It supports writing when at least one mapped member can be read and the orders of these members are 0, 1, …, n−1, without gaps. Otherwise `TryReadRecord` or `WriteRecord` throws a `NotSupportedException`. On read, the handler reads the fields of the members that can be set, in field order, and skips the other fields before the last of them. Fields after the last one are ignored. A missing field or a field that cannot be converted throws a `TabularException` that names the record, the field and the member. A generated handler has a header when at least one mapped member has `[TabularFieldName]`. The header has one name for every field order from 0 to the highest mapped order, with an empty name for an unnamed member and for an order without a member. For the `Observation` type with names only on `City` and `Temperature`, the header is `city,,,,temperature`:

```cs
[TabularRecord]
internal record Observation(
    [property: TabularFieldOrder(0), TabularFieldName("city")] string City,
    [property: TabularFieldOrder(1)] double Latitude,
    [property: TabularFieldOrder(2)] double Longitude,
    [property: TabularFieldOrder(3)] DateOnly Date,
    [property: TabularFieldOrder(4), TabularFieldName("temperature")] double Temperature);
```

The values of a mapped member are converted with the first of:

1. The converter named by the member's `[TabularConverter<TConverter>]` attribute.
2. The converter named by an `[assembly: TabularConverter<TConverter>]` attribute for the member type, or for `T` of a `Nullable<T>` member.
3. The converter that the reader or writer resolves for the type, as described in [Value converters](xref:urn:topics:low-level-api#value-converters).

A converter named by an attribute is chosen at compile time, and the handler creates one instance of it per member. It must derive from `TabularConverter<T>` for the member type and have a public parameterless constructor. A member of a type without a built-in converter, including enumerations and `byte[]`, needs a converter from an attribute, the options or the registry. Empty fields and `null` depend on the member type:

| Member type | Empty field on read | `null` on write |
|:-|:-|:-|
| `Nullable<T>`, for example `int?` | `null` | empty field |
| nullable reference type, for example `string?`, `Uri?`, `byte[]?` | `null` | passed to the converter, and the built-in converters write an empty field |
| `string` | empty string | empty field |
| any other type | passed to the converter, which rejects it for most value types with a `TabularException` | |

## Custom record handlers

A custom handler derives from `TabularHandler<T>` and overrides the members it supports, since `Read`, `ReadAsync`, `Write` and `WriteAsync` throw a `NotSupportedException` by default. The synchronous methods of the record reader and writer call `Read` and `Write`, and the asynchronous methods call `ReadAsync` and `WriteAsync`.

- `Read` is called with the field reader at the start of a value record. It may leave fields unread: the record reader skips the rest of the record.
- `Write` writes the fields of one record. The record writer ends the record.
- `Header` returns the names of the header, or `null` for no header.

A handler is stored in [TabularOptions.Handlers](xref:Addax.Formats.Tabular.TabularOptions.Handlers) or [TabularRegistry.Handlers](xref:Addax.Formats.Tabular.TabularRegistry.Handlers) with the record type as the key. One handler instance serves every reader and writer that selects it, on any thread, so a handler must be stateless or thread-safe. The sample writes `observations.csv` with a handler written by hand for the `Observation` type:

```cs
var observations = new Observation[]
{
    new("Kyiv", 50.4501, 30.5234, new(2024, 2, 3), -2.5),
    new("Ottawa", 45.4215, -75.6972, new(2024, 2, 3), -11.5)
};

TabularRegistry.Handlers[typeof(Observation)] = new ObservationHandler();

var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.Create("observations.csv");
using var writer = new TabularWriter<Observation>(stream, dialect);

foreach (var observation in observations)
{
    writer.WriteRecord(observation);
}

internal class ObservationHandler : TabularHandler<Observation>
{
    public override void Write(TabularWriter writer, Observation record)
    {
        writer.WriteString(record.City);
        writer.WriteDouble(record.Latitude);
        writer.WriteDouble(record.Longitude);
        writer.WriteDateOnly(record.Date);
        writer.WriteDouble(record.Temperature);
    }

    public override IEnumerable<string?>? Header =>
        ["city", "lat", "lon", "date", "temperature"];
}

internal record Observation(
    string City,
    double Latitude,
    double Longitude,
    DateOnly Date,
    double Temperature);
```

```text
city,lat,lon,date,temperature
Kyiv,50.4501,30.5234,2024-02-03,-2.5
Ottawa,45.4215,-75.6972,2024-02-03,-11.5
```

## Arrays as record types

An array type can be used as the record type without a handler of its own: the registry has built-in handlers that read a record into an array with one element per field and write an array as a record with one field per element. The length of the array follows the record, so records with different numbers of fields read as arrays of different lengths. These handlers have no header.

The supported record types are `string?[]`, `Uri?[]`, and `T[]` and `T?[]` for every `T` of `bool`, `char`, `Rune`, `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `Int128`, `UInt128`, `BigInteger`, `Half`, `BFloat16`, `float`, `double`, `decimal`, `Decimal32`, `Decimal64`, `Decimal128`, `TimeSpan`, `DateOnly`, `TimeOnly`, `DateTime`, `DateTimeOffset` and `Guid`. A `byte[]` record holds one number per field, not base16 or base64 binary data.

The elements are read and written with the converter that the reader or writer resolves for the element type, so a converter stored in the options or the registry changes the form of the elements. In `string?[]`, `Uri?[]` and `T?[]`, an empty field reads as `null` and a `null` element is written as an empty field. In `T[]`, an empty field is passed to the converter, which rejects it for most types. A field that cannot be converted throws a `TabularException` that leaves the reader usable. The sample reads every record of `observations.csv`, the header included, as an array of five strings:

```text
city,lat,lon,date,temperature
Kyiv,50.4501,30.5234,2024-02-03,-2.5
Ottawa,45.4215,-75.6972,2024-02-03,-11.5
```

```cs
var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.OpenRead("observations.csv");
using var reader = new TabularReader<string?[]>(stream, dialect);

while (reader.TryReadRecord())
{
    var observation = reader.CurrentRecord;

    Console.WriteLine($"{observation[0]}: {observation[4]}");
}
```
