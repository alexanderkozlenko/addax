## About

A high-throughput, allocation-free, forward-only framework that reads and writes tabular data in delimiter-separated formats of any dialect, with values in the standard, culture-independent formats of the W3C recommendation "Model for Tabular Data and Metadata on the Web", ISO 8601-1:2019, RFC 3987, RFC 4648 and RFC 9562. Also provides mapping of records to custom types through source-generated or hand-written record handlers.

## Features

- Selective reading that skips unneeded fields and entire records without decoding them, so only the data in use is processed.
- Typed reading and writing of numeric, date and time, GUID, URI, Unicode scalar and binary (Base16, Base64) values, extensible with custom value converters.
- Record mapping generated at compile time from attributes or provided by custom record handlers, with per-reader and per-writer overrides of converters and handlers.
- Optional string pooling that returns one shared instance for recurring field values, avoiding repeated allocations and duplicate strings in memory.
- Synchronous and asynchronous APIs with cancellation support, compatible with trimming and native AOT.

## Documentation

- [W3C - Model for Tabular Data and Metadata on the Web](https://w3.org/TR/2015/REC-tabular-data-model-20151217)
- [Addax - Documentation](https://alexanderkozlenko.github.io/addax)
