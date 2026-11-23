namespace Addax.Formats.Tabular.Benchmarks.Records;

[TabularRecord]
public sealed class ConverterRecord
{
    [TabularFieldOrder(0)]
    public Amount A
    {
        get;
        set;
    }

    [TabularFieldOrder(1)]
    public Amount B
    {
        get;
        set;
    }

    [TabularFieldOrder(2)]
    public Amount C
    {
        get;
        set;
    }

    [TabularFieldOrder(3)]
    public Amount D
    {
        get;
        set;
    }
}
