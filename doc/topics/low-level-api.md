---
uid: urn:topics:low-level-api
---

# Low-level API

<xref:Addax.Formats.Tabular.TabularReader> reads a stream one field at a time, and <xref:Addax.Formats.Tabular.TabularWriter> writes a stream one field at a time. Every member that reads from or writes to the stream has an asynchronous counterpart with a cancellation token, for example `TryReadFieldAsync` for `TryReadField`.

Invalid arguments and failed conversions throw without changing the state of a reader or writer, so the next call can continue. A failure of the stream itself faults the reader or writer for good: content outside the grammar, which the reader reports with a <xref:Addax.Formats.Tabular.TabularContentException>, a stream or encoding exception, or cancellation. After that, every further operation throws an `InvalidOperationException`, and a faulted writer discards its buffered text on disposal.

## Reading fields

[TryMoveToNextRecord](xref:Addax.Formats.Tabular.TabularReader.TryMoveToNextRecord) moves to the next record and skips the unread fields of the current one. [TryReadField](xref:Addax.Formats.Tabular.TabularReader.TryReadField) reads the next field of the record and returns `false` at the end of the record. The samples read and write `observations.csv`:

```text
city,lat,lon,date,temperature
Kyiv,50.4501,30.5234,2024-02-03,-2.5
Ottawa,45.4215,-75.6972,2024-02-03,-11.5
```

```cs
var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.OpenRead("observations.csv");
using var reader = new TabularReader(stream, dialect);

reader.TryMoveToNextRecord();

while (reader.TryMoveToNextRecord())
{
    reader.TryReadField();
    reader.TryGetString(out var observationCity);
    reader.TryReadField();
    reader.TryGetDouble(out var observationLatitude);
    reader.TryReadField();
    reader.TryGetDouble(out var observationLongitude);
    reader.TryReadField();
    reader.TryGetDateOnly(out var observationDate);
    reader.TryReadField();
    reader.TryGetDouble(out var observationTemperature);

    Console.WriteLine($"{observationCity}: {observationTemperature}");
}
```

[CurrentField](xref:Addax.Formats.Tabular.TabularReader.CurrentField) holds the value of the last read field. It is a region of the reader's buffer, not a copy, and stays valid only until the next call that advances the reader. To keep a field, copy it, for example with `TryGetString` or `CurrentField.ToArray()`. `TryGetString` and the typed accessors (`TryGetInt32`, `TryGetDateTime`, …) convert the current field and return `false` when it does not have the form the type reads, while the `Get` forms (`GetInt32`, …) throw a `FormatException` instead. The typed accessors parse directly from the buffer, without creating a string. [TryGet](xref:Addax.Formats.Tabular.TabularReader.TryGet*) and [Get](xref:Addax.Formats.Tabular.TabularReader.Get*) convert with a given converter. The forms of every type are listed in [Data types](xref:urn:topics:fundamentals#data-types). [TrySkipField](xref:Addax.Formats.Tabular.TabularReader.TrySkipField) moves past a field without decoding it, and `TryMoveToNextRecord` skips the rest of a record the same way, so a reader that needs some fields decodes only those. The sample reads only the cities and temperatures of `observations.csv`:

```text
city,lat,lon,date,temperature
Kyiv,50.4501,30.5234,2024-02-03,-2.5
Ottawa,45.4215,-75.6972,2024-02-03,-11.5
```

```cs
var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.OpenRead("observations.csv");
using var reader = new TabularReader(stream, dialect);

reader.TryMoveToNextRecord();

while (reader.TryMoveToNextRecord())
{
    reader.TryReadField();
    reader.TryGetString(out var observationCity);
    reader.TrySkipField();
    reader.TrySkipField();
    reader.TrySkipField();
    reader.TryReadField();
    reader.TryGetDouble(out var observationTemperature);

    Console.WriteLine($"{observationCity}: {observationTemperature}");
}
```

## Writing fields

Each write method appends one field to the current record: [WriteString](xref:Addax.Formats.Tabular.TabularWriter.WriteString*), [WriteEmpty](xref:Addax.Formats.Tabular.TabularWriter.WriteEmpty), the typed methods (`WriteInt32`, `WriteDateTime`, …) and [Write](xref:Addax.Formats.Tabular.TabularWriter.Write*) with a converter. [WriteEndOfRecord](xref:Addax.Formats.Tabular.TabularWriter.WriteEndOfRecord) ends the record. The sample writes `observations.csv`:

```cs
var observations = new (string City, double Latitude, double Longitude, DateOnly Date, double Temperature)[]
{
    new("Kyiv", 50.4501, 30.5234, new(2024, 2, 3), -2.5),
    new("Ottawa", 45.4215, -75.6972, new(2024, 2, 3), -11.5)
};

var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.Create("observations.csv");
using var writer = new TabularWriter(stream, dialect);

writer.WriteString("city");
writer.WriteString("lat");
writer.WriteString("lon");
writer.WriteString("date");
writer.WriteString("temperature");
writer.WriteEndOfRecord();

foreach (var observation in observations)
{
    writer.WriteString(observation.City);
    writer.WriteDouble(observation.Latitude);
    writer.WriteDouble(observation.Longitude);
    writer.WriteDateOnly(observation.Date);
    writer.WriteDouble(observation.Temperature);
    writer.WriteEndOfRecord();
}
```

```text
city,lat,lon,date,temperature
Kyiv,50.4501,30.5234,2024-02-03,-2.5
Ottawa,45.4215,-75.6972,2024-02-03,-11.5
```

The writer collects text in its buffer and writes a full buffer to the stream without flushing the stream. Only [Flush](xref:Addax.Formats.Tabular.TabularWriter.Flush), [FlushAsync](xref:Addax.Formats.Tabular.TabularWriter.FlushAsync*) and disposal flush the stream. Disposal also ends a record that has fields. Readers and writers never close or dispose the stream.

## Value converters

A value converter, a <xref:Addax.Formats.Tabular.TabularConverter`1>, parses the text of a field into a .NET value and formats a value into text. The typed accessors and write methods of the low-level API and the generated record handlers use the converter that the reader or writer resolves for the type. [TabularReader.TryGet](xref:Addax.Formats.Tabular.TabularReader.TryGet*), [TabularReader.Get](xref:Addax.Formats.Tabular.TabularReader.Get*) and [TabularWriter.Write](xref:Addax.Formats.Tabular.TabularWriter.Write*) take any converter. A reader or writer resolves the converter for a type the first time it needs it, and keeps it for the rest of its lifetime:

1. The entry for the type in [TabularOptions.Converters](xref:Addax.Formats.Tabular.TabularOptions.Converters) of the options the reader or writer was created with.
2. Otherwise the entry for the type in [TabularRegistry.Converters](xref:Addax.Formats.Tabular.TabularRegistry.Converters).
3. Otherwise the call that needs the converter throws an `InvalidOperationException`.

The registry holds the built-in converters for every type in [Data types](xref:urn:topics:fundamentals#data-types) except `string` and binary data, and is shared by the whole process. The sample stores a date converter with a custom format in the registry, so every reader and writer of the process reads and writes dates in that format, also through `TryGetDateOnly` and `WriteDateOnly`:

```cs
TabularRegistry.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");
```

The same converter stored in the options of one reader or writer applies to that reader or writer only. The converters for `DateOnly`, `TimeOnly`, `DateTime`, `DateTimeOffset` and `Guid` have a constructor that takes a .NET format string: <xref:Addax.Formats.Tabular.Converters.TabularDateOnlyConverter>, <xref:Addax.Formats.Tabular.Converters.TabularTimeOnlyConverter>, <xref:Addax.Formats.Tabular.Converters.TabularDateTimeConverter>, <xref:Addax.Formats.Tabular.Converters.TabularDateTimeOffsetConverter>, <xref:Addax.Formats.Tabular.Converters.TabularGuidConverter>. Values are written with the format and read with an exact match of it. The sample reads the dates of `observations.csv` written as `03.02.2024`, passing the converter explicitly:

```cs
var converter = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");
var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.OpenRead("observations.csv");
using var reader = new TabularReader(stream, dialect);

reader.TryMoveToNextRecord();

while (reader.TryMoveToNextRecord())
{
    reader.TryReadField();
    reader.TryGetString(out var observationCity);
    reader.TrySkipField();
    reader.TrySkipField();
    reader.TryReadField();
    reader.TryGet(converter, out var observationDate);
    reader.TrySkipField();

    Console.WriteLine($"{observationCity}: {observationDate}");
}
```

An attribute of the high-level API names a converter by type, so a custom format there needs a class derived from the converter that passes the format in its public parameterless constructor.

A converter in the options or the registry serves every reader and writer that resolves it, and a converter named in a `[TabularConverter<T>]` attribute is shared by every reader and writer of the record type, on any thread, so a converter must be stateless or immutable.

A type without a built-in converter, such as an enumeration, needs a converter of its own. The sample converts the `City` enumeration by the names of its values and reads the `city` column of `observations.csv` as a `City` value:

```cs
var converter = new CityConverter();
var dialect = new TabularDialect("\r\n", ',', '"');

using var stream = File.OpenRead("observations.csv");
using var reader = new TabularReader(stream, dialect);

reader.TryMoveToNextRecord();

while (reader.TryMoveToNextRecord())
{
    reader.TryReadField();
    reader.TryGet(converter, out var observationCity);

    Console.WriteLine(observationCity);
}

internal class CityConverter : TabularConverter<City>
{
    public override bool TryFormat(City value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return Enum.TryFormat(value, destination, out charsWritten);
    }

    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out City value)
    {
        return Enum.TryParse(source, out value);
    }
}

internal enum City
{
    Kyiv,
    Ottawa,
}
```

The level at which the converter is declared decides where it applies:

- One call: `reader.TryGet(converter, out var city)` and `writer.Write(city, converter)`.
- The readers and writers created with an options instance: `options.Converters[typeof(City)] = converter`.
- Every reader and writer of the process: `TabularRegistry.Converters[typeof(City)] = converter`.
- One member of a record type: `[TabularConverter<CityConverter>]`.
- Every `City` member of an assembly: `[assembly: TabularConverter<CityConverter>]`.
