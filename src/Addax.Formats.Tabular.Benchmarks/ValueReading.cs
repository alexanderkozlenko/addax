using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class ValueReading
{
    private const int s_records = 1024;
    private const int s_fields = 16;
    private const int s_operations = s_records * s_fields;

    private readonly MemoryStream _int32;
    private readonly MemoryStream _int64;
    private readonly MemoryStream _double;
    private readonly MemoryStream _decimal;
    private readonly MemoryStream _dateOnly;
    private readonly MemoryStream _dateTimeOffset;
    private readonly MemoryStream _guid;
    private readonly MemoryStream _string;
    private readonly MemoryStream _base64Array;

    public ValueReading()
    {
        _int32 = CreateStream(static x => x.WriteInt32(123456789));
        _int64 = CreateStream(static x => x.WriteInt64(1234567890123456789));
        _double = CreateStream(static x => x.WriteDouble(1234.5678));
        _decimal = CreateStream(static x => x.WriteDecimal(1234.5678m));
        _dateOnly = CreateStream(static x => x.WriteDateOnly(new(2024, 2, 3)));
        _dateTimeOffset = CreateStream(static x => x.WriteDateTimeOffset(new(2024, 2, 3, 4, 5, 6, TimeSpan.FromHours(2))));
        _guid = CreateStream(static x => x.WriteGuid(new("123e4567-e89b-12d3-a456-426614174000")));
        _string = CreateStream(static x => x.WriteString("vvvv"));
        _base64Array = CreateStream(static x => x.WriteBase64Array(new byte[16]));
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Int32()
    {
        _int32.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_int32, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetInt32(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Int64()
    {
        _int64.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_int64, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetInt64(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Double()
    {
        _double.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_double, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetDouble(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Decimal()
    {
        _decimal.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_decimal, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetDecimal(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void DateOnly()
    {
        _dateOnly.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_dateOnly, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetDateOnly(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void DateTimeOffset()
    {
        _dateTimeOffset.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_dateTimeOffset, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetDateTimeOffset(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Guid()
    {
        _guid.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_guid, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetGuid(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void String()
    {
        _string.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_string, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetString(out _);
            }
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Base64Array()
    {
        _base64Array.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_base64Array, BenchmarkData.Dialect, BenchmarkData.Options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
                reader.TryGetBase64Array(out _);
            }
        }
    }

    private static MemoryStream CreateStream(Action<TabularWriter> writeValue)
    {
        return BenchmarkData.CreateStream(s_records, x =>
        {
            for (var i = 0; i < s_fields; i++)
            {
                writeValue(x);
            }
        });
    }
}
