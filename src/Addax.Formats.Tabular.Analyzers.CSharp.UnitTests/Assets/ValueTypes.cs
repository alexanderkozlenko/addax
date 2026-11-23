using System;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public struct TextTypesRecord
{
    [TabularFieldOrder(0)]
    public string? MyValue0;

    [TabularFieldOrder(1)]
    public char MyValue1;

    [TabularFieldOrder(2)]
    public Rune MyValue2;
}

[TabularRecord]
public struct NullableTextTypesRecord
{
    [TabularFieldOrder(0)]
    public string? MyValue0;

    [TabularFieldOrder(1)]
    public char? MyValue1;

    [TabularFieldOrder(2)]
    public Rune? MyValue2;
}

[TabularRecord]
public struct IntegerTypesRecord
{
    [TabularFieldOrder(0)]
    public sbyte MyValue0;

    [TabularFieldOrder(1)]
    public byte MyValue1;

    [TabularFieldOrder(2)]
    public short MyValue2;

    [TabularFieldOrder(3)]
    public ushort MyValue3;

    [TabularFieldOrder(4)]
    public int MyValue4;

    [TabularFieldOrder(5)]
    public uint MyValue5;

    [TabularFieldOrder(6)]
    public long MyValue6;

    [TabularFieldOrder(7)]
    public ulong MyValue7;

    [TabularFieldOrder(8)]
    public Int128 MyValue8;

    [TabularFieldOrder(9)]
    public UInt128 MyValue9;

    [TabularFieldOrder(10)]
    public BigInteger MyValue10;
}

[TabularRecord]
public struct NullableIntegerTypesRecord
{
    [TabularFieldOrder(0)]
    public sbyte? MyValue0;

    [TabularFieldOrder(1)]
    public byte? MyValue1;

    [TabularFieldOrder(2)]
    public short? MyValue2;

    [TabularFieldOrder(3)]
    public ushort? MyValue3;

    [TabularFieldOrder(4)]
    public int? MyValue4;

    [TabularFieldOrder(5)]
    public uint? MyValue5;

    [TabularFieldOrder(6)]
    public long? MyValue6;

    [TabularFieldOrder(7)]
    public ulong? MyValue7;

    [TabularFieldOrder(8)]
    public Int128? MyValue8;

    [TabularFieldOrder(9)]
    public UInt128? MyValue9;

    [TabularFieldOrder(10)]
    public BigInteger? MyValue10;
}

[TabularRecord]
public struct BinaryFloatingPointTypesRecord
{
    [TabularFieldOrder(0)]
    public Half MyValue0;

    [TabularFieldOrder(1)]
    public BFloat16 MyValue1;

    [TabularFieldOrder(2)]
    public float MyValue2;

    [TabularFieldOrder(3)]
    public double MyValue3;
}

[TabularRecord]
public struct NullableBinaryFloatingPointTypesRecord
{
    [TabularFieldOrder(0)]
    public Half? MyValue0;

    [TabularFieldOrder(1)]
    public BFloat16? MyValue1;

    [TabularFieldOrder(2)]
    public float? MyValue2;

    [TabularFieldOrder(3)]
    public double? MyValue3;
}

[TabularRecord]
public struct DecimalTypesRecord
{
    [TabularFieldOrder(0)]
    public decimal MyValue0;

    [TabularFieldOrder(1)]
    public Decimal32 MyValue1;

    [TabularFieldOrder(2)]
    public Decimal64 MyValue2;

    [TabularFieldOrder(3)]
    public Decimal128 MyValue3;
}

[TabularRecord]
public struct NullableDecimalTypesRecord
{
    [TabularFieldOrder(0)]
    public decimal? MyValue0;

    [TabularFieldOrder(1)]
    public Decimal32? MyValue1;

    [TabularFieldOrder(2)]
    public Decimal64? MyValue2;

    [TabularFieldOrder(3)]
    public Decimal128? MyValue3;
}

[TabularRecord]
public struct TimeTypesRecord
{
    [TabularFieldOrder(0)]
    public TimeSpan MyValue0;

    [TabularFieldOrder(1)]
    public TimeOnly MyValue1;

    [TabularFieldOrder(2)]
    public DateOnly MyValue2;

    [TabularFieldOrder(3)]
    public DateTime MyValue3;

    [TabularFieldOrder(4)]
    public DateTimeOffset MyValue4;
}

[TabularRecord]
public struct NullableTimeTypesRecord
{
    [TabularFieldOrder(0)]
    public TimeSpan? MyValue0;

    [TabularFieldOrder(1)]
    public TimeOnly? MyValue1;

    [TabularFieldOrder(2)]
    public DateOnly? MyValue2;

    [TabularFieldOrder(3)]
    public DateTime? MyValue3;

    [TabularFieldOrder(4)]
    public DateTimeOffset? MyValue4;
}

[TabularRecord]
public struct OtherTypesRecord
{
    [TabularFieldOrder(0)]
    public bool MyValue0;

    [TabularFieldOrder(1)]
    public Guid MyValue1;

    [TabularFieldOrder(2)]
    public Uri? MyValue2;
}

[TabularRecord]
public struct NullableOtherTypesRecord
{
    [TabularFieldOrder(0)]
    public bool? MyValue0;

    [TabularFieldOrder(1)]
    public Guid? MyValue1;

    [TabularFieldOrder(2)]
    public Uri? MyValue2;
}

[TabularRecord]
public struct RegisteredTypesRecord
{
    [TabularFieldOrder(0)]
    public Memory<byte> MyValue0;

    [TabularFieldOrder(1)]
    public byte[]? MyValue1;

    [TabularFieldOrder(2)]
    public ReadOnlyMemory<byte>? MyValue2;
}
