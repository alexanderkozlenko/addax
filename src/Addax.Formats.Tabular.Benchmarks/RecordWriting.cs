using Addax.Formats.Tabular.Benchmarks.Records;
using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class RecordWriting
{
    private const int s_records = 16384;

    private readonly BuiltInRecord _builtInTypes;
    private readonly ConverterRecord _registeredConverter;
    private readonly int[] _int32Array;
    private readonly string?[] _stringArray;

    public RecordWriting()
    {
        TabularRegistry.Converters[typeof(Amount)] = new AmountConverter();

        _builtInTypes = new()
        {
            A = 123456789,
            B = 1234.5678,
            C = new(2024, 2, 3),
            D = "vvvv",
        };

        _registeredConverter = new()
        {
            A = new(123456789),
            B = new(123456789),
            C = new(123456789),
            D = new(123456789),
        };

        _int32Array = [123456789, 123456789, 123456789, 123456789];
        _stringArray = ["vvvv", "vvvv", "vvvv", "vvvv"];
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void BuiltInTypes()
    {
        Write(_builtInTypes);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void RegisteredConverter()
    {
        Write(_registeredConverter);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void Int32Array()
    {
        Write(_int32Array);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void StringArray()
    {
        Write(_stringArray);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask BuiltInTypesAsync()
    {
        return WriteAsync(_builtInTypes);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask RegisteredConverterAsync()
    {
        return WriteAsync(_registeredConverter);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask Int32ArrayAsync()
    {
        return WriteAsync(_int32Array);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask StringArrayAsync()
    {
        return WriteAsync(_stringArray);
    }

    private static void Write<T>(T record)
        where T : notnull
    {
        using var writer = new TabularWriter<T>(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            writer.WriteRecord(record);
        }
    }

    private static async ValueTask WriteAsync<T>(T record)
        where T : notnull
    {
        await using var writer = new TabularWriter<T>(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            await writer.WriteRecordAsync(record).ConfigureAwait(false);
        }
    }
}
