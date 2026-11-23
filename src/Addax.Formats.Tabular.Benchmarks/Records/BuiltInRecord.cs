namespace Addax.Formats.Tabular.Benchmarks.Records;

[TabularRecord]
public sealed class BuiltInRecord
{
    [TabularFieldOrder(0)]
    public int A
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    public double B
    {
        get;
        set;
    }

    [TabularFieldOrder(2)]
    public DateOnly C
    {
        get;
        set;
    }

    [TabularFieldOrder(3)]
    public string? D
    {
        get;
        set;
    }
}
