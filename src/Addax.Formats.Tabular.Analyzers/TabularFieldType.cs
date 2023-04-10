// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Analyzers;

internal enum TabularFieldType : byte
{
    None,
    Char,
    Rune,
    String,
    Boolean,
    SByte,
    Byte,
    Int16,
    UInt16,
    Int32,
    UInt32,
    Int64,
    UInt64,
    Int128,
    UInt128,
    BigInteger,
    Half,
    Single,
    Double,
    Decimal,
    Complex,
    TimeSpan,
    TimeOnly,
    DateOnly,
    DateTime,
    DateTimeOffset,
    Guid,
}
