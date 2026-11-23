using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class FieldReading
{
    private const int s_records = 1024;
    private const int s_fields = 64;
    private const int s_operations = s_records * s_fields;

    private readonly MemoryStream _empty;
    private readonly MemoryStream _regular;
    private readonly MemoryStream _escaped;

    public FieldReading()
    {
        _empty = CreateStream("");
        _regular = CreateStream("vvvv");
        _escaped = CreateStream("'^^^");
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Empty()
    {
        Read(_empty);
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Regular()
    {
        Read(_regular);
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Escaped()
    {
        Read(_escaped);
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public ValueTask EmptyAsync()
    {
        return ReadAsync(_empty);
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public ValueTask RegularAsync()
    {
        return ReadAsync(_regular);
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public ValueTask EscapedAsync()
    {
        return ReadAsync(_escaped);
    }

    private static void Read(MemoryStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(stream, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }
    }

    private static async ValueTask ReadAsync(MemoryStream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);

        await using var reader = new TabularReader(stream, BenchmarkData.Dialect, BenchmarkData.Options);

        while (await reader.TryMoveToNextRecordAsync().ConfigureAwait(false))
        {
            while (await reader.TryReadFieldAsync().ConfigureAwait(false))
            {
            }
        }
    }

    private static MemoryStream CreateStream(string value)
    {
        return BenchmarkData.CreateStream(s_records, x =>
        {
            for (var i = 0; i < s_fields; i++)
            {
                x.WriteString(value);
            }
        });
    }
}
