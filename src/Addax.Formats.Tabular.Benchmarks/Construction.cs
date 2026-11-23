using Addax.Formats.Tabular.Benchmarks.Records;
using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class Construction
{
    private const int s_instances = 256;

    [Benchmark(OperationsPerInvoke = s_instances)]
    public void FieldReader()
    {
        for (var i = 0; i < s_instances; i++)
        {
            using var reader = new TabularReader(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);
        }
    }

    [Benchmark(OperationsPerInvoke = s_instances)]
    public void FieldWriter()
    {
        for (var i = 0; i < s_instances; i++)
        {
            using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);
        }
    }

    [Benchmark(OperationsPerInvoke = s_instances)]
    public void RecordReader()
    {
        for (var i = 0; i < s_instances; i++)
        {
            using var reader = new TabularReader<BuiltInRecord>(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);
        }
    }

    [Benchmark(OperationsPerInvoke = s_instances)]
    public void RecordWriter()
    {
        for (var i = 0; i < s_instances; i++)
        {
            using var writer = new TabularWriter<BuiltInRecord>(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);
        }
    }
}
