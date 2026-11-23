using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class ValueWriting
{
    private const int s_records = 1024;
    private const int s_fields = 16;
    private const int s_operations = s_records * s_fields;

    private const int s_int32 = 123456789;
    private const long s_int64 = 1234567890123456789;
    private const double s_double = 1234.5678;
    private const decimal s_decimal = 1234.5678m;
    private const string s_string = "vvvv";

    private static readonly DateOnly s_dateOnly = new(2024, 2, 3);
    private static readonly DateTimeOffset s_dateTimeOffset = new(2024, 2, 3, 4, 5, 6, TimeSpan.FromHours(2));
    private static readonly Guid s_guid = new("123e4567-e89b-12d3-a456-426614174000");
    private static readonly ReadOnlyMemory<char> s_stringMemory = s_string.AsMemory();
    private static readonly byte[] s_base64Array = new byte[16];

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Int32()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteInt32(s_int32);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Int64()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteInt64(s_int64);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Double()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteDouble(s_double);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Decimal()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteDecimal(s_decimal);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void DateOnly()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteDateOnly(s_dateOnly);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void DateTimeOffset()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteDateTimeOffset(s_dateTimeOffset);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Guid()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteGuid(s_guid);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void String()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteString(s_string);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public void Base64Array()
    {
        using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                writer.WriteBase64Array(s_base64Array);
            }

            writer.WriteEndOfRecord();
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask Int32Async()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteInt32Async(s_int32).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask Int64Async()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteInt64Async(s_int64).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask DoubleAsync()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteDoubleAsync(s_double).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask DecimalAsync()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteDecimalAsync(s_decimal).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask DateOnlyAsync()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteDateOnlyAsync(s_dateOnly).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask DateTimeOffsetAsync()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteDateTimeOffsetAsync(s_dateTimeOffset).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask GuidAsync()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteGuidAsync(s_guid).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask StringAsync()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteStringAsync(s_stringMemory).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }

    [Benchmark(OperationsPerInvoke = s_operations)]
    public async ValueTask Base64ArrayAsync()
    {
        await using var writer = new TabularWriter(Stream.Null, BenchmarkData.Dialect, BenchmarkData.Options);

        for (var i = 0; i < s_records; i++)
        {
            for (var j = 0; j < s_fields; j++)
            {
                await writer.WriteBase64ArrayAsync(s_base64Array).ConfigureAwait(false);
            }

            await writer.WriteEndOfRecordAsync().ConfigureAwait(false);
        }
    }
}
