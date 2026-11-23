---
uid: urn:topics:fundamentals
---

# Fundamentals

## Data types

The library has built-in support for the types in the table, each read and written in one fixed form, independent of the current culture.

| Type | Read Format | Write Format | Standard |
|:-|:-|:-|:-|
| `System.Boolean` | `true|false|1|0` | `true|false` | W3C CSVW |
| `System.Byte` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Char` | | | |
| `System.DateOnly` | `yyyy'-'MM'-'ddK` | `o` | ISO 8601-1:2019 |
| `System.DateTime` | `yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK` | `o` | ISO 8601-1:2019 |
| `System.DateTimeOffset` | `yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK` | `o` | ISO 8601-1:2019 |
| `System.Decimal` | `AllowLeadingSign`, `AllowDecimalPoint` | `g` | W3C CSVW |
| `System.Double` | `AllowLeadingSign`, `AllowDecimalPoint`, `AllowExponent` | `g` | W3C CSVW |
| `System.Guid` | `d` | `d` | RFC 9562 |
| `System.Half` | `AllowLeadingSign`, `AllowDecimalPoint`, `AllowExponent` | `g` | W3C CSVW |
| `System.Int128` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Int16` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Int32` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Int64` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Memory<System.Byte>` | `base16` / `base64` | `base16` / `base64` | RFC 4648 |
| `System.ReadOnlyMemory<System.Byte>` | `base16` / `base64` | `base16` / `base64` | RFC 4648 |
| `System.SByte` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Single` | `AllowLeadingSign`, `AllowDecimalPoint`, `AllowExponent` | `g` | W3C CSVW |
| `System.String` | | | |
| `System.TimeOnly` | `HH':'mm':'ss.FFFFFFFK` | `o` | ISO 8601-1:2019 |
| `System.TimeSpan` | `'P'd'DT'h'H'm'M's'.'FFFFFFF'S'` | `'P'd'DT'h'H'm'M's'.'fffffff'S'` | W3C CSVW |
| `System.UInt128` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.UInt16` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.UInt32` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.UInt64` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Uri` | | 3987/3.1, 3987/5.3.2.3 | RFC 3987 |
| `System.Numerics.BFloat16` | `AllowLeadingSign`, `AllowDecimalPoint`, `AllowExponent` | `g` | W3C CSVW |
| `System.Numerics.BigInteger` | `AllowLeadingSign` | `g` | W3C CSVW |
| `System.Numerics.Decimal128` | `AllowLeadingSign`, `AllowDecimalPoint`, `AllowExponent` | `g` | W3C CSVW |
| `System.Numerics.Decimal32` | `AllowLeadingSign`, `AllowDecimalPoint`, `AllowExponent` | `g` | W3C CSVW |
| `System.Numerics.Decimal64` | `AllowLeadingSign`, `AllowDecimalPoint`, `AllowExponent` | `g` | W3C CSVW |
| `System.Text.Rune` | | | |
| `System.Byte[]` | `base16` / `base64` | `base16` / `base64` | RFC 4648 |

A `System.Char` is one UTF-16 code unit, and a `System.Text.Rune` is one Unicode scalar value. Integers are read within the range of the type, and `System.Numerics.BigInteger` has no range limit.

A `System.TimeSpan` is read with an optional leading "`-`", and any of its components may be omitted as long as one remains, together with `T` when no time component remains, and a component may exceed its carry range, as in `PT36H`. A negative value is written with a leading "`-`".

A `System.DateTime` read with a zone is converted to UTC and has UTC kind, and one read without a zone has unspecified kind. A `System.DateTimeOffset` read without a zone has the offset `+00:00`. `System.DateOnly` and `System.TimeOnly` ignore the zone.

A `System.Uri` is read as an absolute or relative URI reference, and is written in its escaped form when it is absolute and as its original string when it is relative.

Hexadecimal digits are read in any case and written in lowercase. A Base64 value is read and written with padding, and white space inside it is ignored when it is read. An empty field reads as no bytes, and no bytes and `null` are written as an empty field. Binary data has no default form: the accessor, the write method or the converter names the encoding.

## Options

Readers and writers of both APIs take a <xref:Addax.Formats.Tabular.TabularOptions> instance, and three of its options change how field values are read and written.

### TrimWhitespace

With [TabularOptions.TrimWhitespace](xref:Addax.Formats.Tabular.TabularOptions.TrimWhitespace) set to `true`, the reader trims every field after decoding, so the quoted field `" Alice "` reads as `"Alice"` and a field of only white space reads as an empty field, and the converters receive the trimmed field. The writer trims every value and comment before quoting it.

### FormatProvider

Readers and writers pass the converters a format provider, which is the invariant culture unless [TabularOptions.FormatProvider](xref:Addax.Formats.Tabular.TabularOptions.FormatProvider) sets another one, and the format provider affects only:

- Numbers: the sign symbols, the decimal separator, and the symbols of infinity and `NaN`.
- Dates and times with a custom format: the names of months and days, the separators and the calendar.

### StringFactory

Readers create the strings of fields through [TabularOptions.StringFactory](xref:Addax.Formats.Tabular.TabularOptions.StringFactory). A <xref:Addax.Formats.Tabular.TabularStringFactory> created with a maximum length pools strings: a field of at most that length returns the pooled string instance for the same characters. The pool holds a fixed number of strings, 4,096 by default, where a new string replaces the pooled string it collides with, so it suits fields with few distinct values that repeat, such as codes, categories or names:

```csharp
using var factory = new TabularStringFactory(maxLength: 32);

var dialect = new TabularDialect("\r\n", ',', '"');
var options = new TabularOptions { StringFactory = factory };

using var stream = File.OpenRead("books.csv");
using var reader = new TabularReader(stream, dialect, options);
```

By default, the strings behind URIs are not pooled, because the built-in `Uri` converter does not use the option. A `TabularUriConverter` created with the factory pools them when it replaces the built-in converter, whether it is stored in the options or the registry, passed to `TryGet`, `Get` and `Write`, or named in a `[TabularConverter<T>]` attribute through a derived class.

## Standards

The BNF [grammar](https://github.com/alexanderkozlenko/addax/blob/main/eng/grammar.bnf) follows section 8, "Parsing Tabular Data", of the W3C recommendation "Model for Tabular Data and Metadata on the Web".
RFC 4180 corresponds to the dialect `{ "\r\n", ',', '"' }`. The reader also accepts any Unicode characters in fields, a lone `CR` or `LF` in an unquoted field, and records with different numbers of fields. The CSVW dialect description maps to the library as follows:

| CSVW | Addax | Purpose |
|:-|:-|:-|
| `commentPrefix` | `TabularDialect.CommentPrefix` | The character that starts a comment record, if the dialect has comment records. |
| `delimiter` | `TabularDialect.Delimiter` | The character that separates the fields of a value record. |
| `doubleQuote` | `TabularDialect.EscapeChar` | Whether a quote character is escaped by doubling it. |
| `encoding` | `TabularOptions.Encoding` | The encoding of the stream (UTF-8 by default). |
| `lineTerminators` | `TabularDialect.LineTerminator` | The single sequence of one or two characters that ends a record. |
| `quoteChar` | `TabularDialect.QuoteChar` | The character that encloses a quoted field, which every dialect requires. |
| `trim` | `TabularOptions.TrimWhitespace` | Whether field values are trimmed, at both ends, of the whitespace characters. |

## References

- [ISO 8601-1:2019, Date and time, Part 1: Basic rules](https://iso.org/standard/70907.html)
- [RFC 3987, Internationalized Resource Identifiers (IRIs)](https://rfc-editor.org/rfc/rfc3987)
- [RFC 4180, Common Format and MIME Type for Comma-Separated Values (CSV) Files](https://rfc-editor.org/rfc/rfc4180)
- [RFC 4648, The Base16, Base32, and Base64 Data Encodings](https://rfc-editor.org/rfc/rfc4648)
- [RFC 9562, Universally Unique IDentifiers (UUIDs)](https://rfc-editor.org/rfc/rfc9562)
- [W3C, Model for Tabular Data and Metadata on the Web](https://w3.org/TR/2015/REC-tabular-data-model-20151217)
- [W3C, Metadata Vocabulary for Tabular Data](https://w3.org/TR/2015/REC-tabular-metadata-20151217)
