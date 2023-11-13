using System;
using System.Numerics;
using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord]
public struct MyType
{
    [TabularFieldOrder(0)]
    public string? MyValue0;

    [TabularFieldOrder(1)]
    public char MyValue1;

    [TabularFieldOrder(2)]
    public bool MyValue2;

    [TabularFieldOrder(3)]
    public sbyte MyValue3;

    [TabularFieldOrder(4)]
    public byte MyValue4;

    [TabularFieldOrder(5)]
    public short MyValue5;

    [TabularFieldOrder(6)]
    public ushort MyValue6;

    [TabularFieldOrder(7)]
    public int MyValue7;

    [TabularFieldOrder(8)]
    public uint MyValue8;

    [TabularFieldOrder(9)]
    public long MyValue9;

    [TabularFieldOrder(10)]
    public ulong MyValue10;

    [TabularFieldOrder(11)]
    public Int128 MyValue11;

    [TabularFieldOrder(12)]
    public UInt128 MyValue12;

    [TabularFieldOrder(13)]
    public BigInteger MyValue13;

    [TabularFieldOrder(14)]
    public Half MyValue14;

    [TabularFieldOrder(15)]
    public float MyValue15;

    [TabularFieldOrder(16)]
    public double MyValue16;

    [TabularFieldOrder(17)]
    public decimal MyValue17;

    [TabularFieldOrder(18)]
    public TimeSpan MyValue18;

    [TabularFieldOrder(19)]
    public TimeOnly MyValue19;

    [TabularFieldOrder(20)]
    public DateOnly MyValue20;

    [TabularFieldOrder(21)]
    public DateTime MyValue21;

    [TabularFieldOrder(22)]
    public DateTimeOffset MyValue22;

    [TabularFieldOrder(23)]
    public Guid MyValue23;

    [TabularFieldOrder(24)]
    public Uri? MyValue24;
}
