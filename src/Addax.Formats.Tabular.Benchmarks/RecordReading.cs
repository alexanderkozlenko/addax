using Addax.Formats.Tabular.Benchmarks.Records;
using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class RecordReading
{
    private const int s_records = 16384;

    private readonly MemoryStream _builtInTypes;
    private readonly MemoryStream _registeredConverter;
    private readonly MemoryStream _int32Array;
    private readonly MemoryStream _stringArray;

    public RecordReading()
    {
        TabularRegistry.Converters[typeof(Amount)] = new AmountConverter();

        _builtInTypes = BenchmarkData.CreateStream(s_records, static x =>
        {
            x.WriteInt32(123456789);
            x.WriteDouble(1234.5678);
            x.WriteDateOnly(new(2024, 2, 3));
            x.WriteString("vvvv");
        });

        _registeredConverter = BenchmarkData.CreateStream(s_records, static x =>
        {
            x.WriteInt64(123456789);
            x.WriteInt64(123456789);
            x.WriteInt64(123456789);
            x.WriteInt64(123456789);
        });

        _int32Array = BenchmarkData.CreateStream(s_records, static x =>
        {
            x.WriteInt32(123456789);
            x.WriteInt32(123456789);
            x.WriteInt32(123456789);
            x.WriteInt32(123456789);
        });

        _stringArray = BenchmarkData.CreateStream(s_records, static x =>
        {
            x.WriteString("vvvv");
            x.WriteString("vvvv");
            x.WriteString("vvvv");
            x.WriteString("vvvv");
        });
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void BuiltInTypes()
    {
        Read<BuiltInRecord>(_builtInTypes);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void RegisteredConverter()
    {
        Read<ConverterRecord>(_registeredConverter);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void Int32Array()
    {
        Read<int[]>(_int32Array);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public void StringArray()
    {
        Read<string?[]>(_stringArray);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask BuiltInTypesAsync()
    {
        return ReadAsync<BuiltInRecord>(_builtInTypes);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask RegisteredConverterAsync()
    {
        return ReadAsync<ConverterRecord>(_registeredConverter);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask Int32ArrayAsync()
    {
        return ReadAsync<int[]>(_int32Array);
    }

    [Benchmark(OperationsPerInvoke = s_records)]
    public ValueTask StringArrayAsync()
    {
        return ReadAsync<string?[]>(_stringArray);
    }

    private static void Read<T>(MemoryStream stream)
        where T : notnull
    {
        stream.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader<T>(stream, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryReadRecord())
        {
        }
    }

    private static async ValueTask ReadAsync<T>(MemoryStream stream)
        where T : notnull
    {
        stream.Seek(0, SeekOrigin.Begin);

        await using var reader = new TabularReader<T>(stream, BenchmarkData.Dialect, BenchmarkData.Options);

        while (await reader.TryReadRecordAsync().ConfigureAwait(false))
        {
        }
    }
}
