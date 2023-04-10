using System;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular;

namespace MyNamespace;

[TabularRecord(strict: true)]
internal sealed class MyType
{
    [TabularField(index: 0)]
    public string? MyProperty0 { get; set; }
    [TabularField(index: 1)]
    public sbyte? MyProperty1 { get; set; }
    [TabularField(index: 2)]
    public byte? MyProperty2 { get; set; }
    [TabularField(index: 3)]
    public short? MyProperty3 { get; set; }
    [TabularField(index: 4)]
    public ushort? MyProperty4 { get; set; }
    [TabularField(index: 5)]
    public int? MyProperty5 { get; set; }
    [TabularField(index: 6)]
    public uint? MyProperty6 { get; set; }
    [TabularField(index: 7)]
    public long? MyProperty7 { get; set; }
    [TabularField(index: 8)]
    public ulong? MyProperty8 { get; set; }
    [TabularField(index: 9)]
    public Int128? MyProperty9 { get; set; }
    [TabularField(index: 10)]
    public UInt128? MyProperty10 { get; set; }
    [TabularField(index: 11)]
    public BigInteger? MyProperty11 { get; set; }
    [TabularField(index: 12)]
    public Half? MyProperty12 { get; set; }
    [TabularField(index: 13)]
    public float? MyProperty13 { get; set; }
    [TabularField(index: 14)]
    public double? MyProperty14 { get; set; }
    [TabularField(index: 15)]
    public decimal? MyProperty15 { get; set; }
    [TabularField(index: 16)]
    public Complex? MyProperty16 { get; set; }
    [TabularField(index: 17)]
    public TimeSpan? MyProperty17 { get; set; }
    [TabularField(index: 18)]
    public TimeOnly? MyProperty18 { get; set; }
    [TabularField(index: 19)]
    public DateOnly? MyProperty19 { get; set; }
    [TabularField(index: 20)]
    public DateTime? MyProperty20 { get; set; }
    [TabularField(index: 21)]
    public DateTimeOffset? MyProperty21 { get; set; }
    [TabularField(index: 22)]
    public bool? MyProperty22 { get; set; }
    [TabularField(index: 23)]
    public char? MyProperty23 { get; set; }
    [TabularField(index: 24)]
    public Rune? MyProperty24 { get; set; }
    [TabularField(index: 25)]
    public Guid? MyProperty25 { get; set; }
}
