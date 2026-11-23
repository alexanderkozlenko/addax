using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class FieldWriting
{
    private const int s_records = 1024;
    private const int s_fields = 64;
    private const int s_operations = s_records * s_fields;

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Empty()
    {
        Write("");
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Regular()
    {
        Write("vvvv");
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Escaped()
    {
        Write("^^^^");
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public ValueTask EmptyAsync()
    {
        return WriteAsync("");
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public ValueTask RegularAsync()
    {
        return WriteAsync("vvvv");
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public ValueTask EscapedAsync()
    {
        return WriteAsync("^^^^");
    }

    private static void Write(string value)
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteString(value);
            }

            writer.WriteEndOfRecord();
        }
    }

    private static async ValueTask WriteAsync(string value)
    {
        var memory = value.AsMemory();

        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteStringAsync(memory).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }
}
