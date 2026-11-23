namespace Addax.Formats.Tabular.Benchmarks.Records;

public readonly struct Amount
{
    public Amount(long value)
    {
        Value = value;
    }

    public long Value
    {
        get;
    }
}
