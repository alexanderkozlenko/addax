#pragma warning disable CA1001

using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class WriteBenchmark
{
    private const int s_count = 1024;

    private static readonly TabularDialect s_dialect = new("t", 'd', 'q');
    private static readonly TabularOptions s_options = new() { Encoding = Encoding.ASCII, LeaveOpen = true };

    private const string s_value0 = "";
    private const string s_value1 = "vvvv";
    private const string s_value2 = "dddd";

    private readonly MemoryStream _stream = new();

    [Benchmark(Description = "write field: empty")]
    public void Write0()
    {
        _stream.Seek(0, SeekOrigin.Begin);

        var value = s_value0.AsSpan();

        using var writer = new TabularWriter(_stream, s_dialect, s_options);

        for (var i = 0; i < s_count; i++)
        {
            for (var j = 0; j < s_count; j++)
            {
                writer.WriteString(value);
            }

            writer.FinishRecord();
        }
    }

    [Benchmark(Description = "write field: regular")]
    public void Write1()
    {
        _stream.Seek(0, SeekOrigin.Begin);

        var value = s_value1.AsSpan();

        using var writer = new TabularWriter(_stream, s_dialect, s_options);

        for (var i = 0; i < s_count; i++)
        {
            for (var j = 0; j < s_count; j++)
            {
                writer.WriteString(value);
            }

            writer.FinishRecord();
        }
    }

    [Benchmark(Description = "write field: escaped")]
    public void Write2()
    {
        _stream.Seek(0, SeekOrigin.Begin);

        var value = s_value2.AsSpan();

        using var writer = new TabularWriter(_stream, s_dialect, s_options);

        for (var i = 0; i < s_count; i++)
        {
            for (var j = 0; j < s_count; j++)
            {
                writer.WriteString(value);
            }

            writer.FinishRecord();
        }
    }

    [Benchmark(Description = "write field: empty (async)")]
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Write0Async()
    {
        _stream.Seek(0, SeekOrigin.Begin);

        var value = s_value0.AsMemory();

        await using var writer = new TabularWriter(_stream, s_dialect, s_options);

        for (var i = 0; i < s_count; i++)
        {
            for (var j = 0; j < s_count; j++)
            {
                await writer.WriteStringAsync(value).ConfigureAwait(false);
            }

            writer.FinishRecord();
        }
    }

    [Benchmark(Description = "write field: regular (async)")]
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Write1Async()
    {
        _stream.Seek(0, SeekOrigin.Begin);

        var value = s_value1.AsMemory();

        await using var writer = new TabularWriter(_stream, s_dialect, s_options);

        for (var i = 0; i < s_count; i++)
        {
            for (var j = 0; j < s_count; j++)
            {
                await writer.WriteStringAsync(value).ConfigureAwait(false);
            }

            writer.FinishRecord();
        }
    }

    [Benchmark(Description = "write field: escaped (async)")]
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Write2Async()
    {
        _stream.Seek(0, SeekOrigin.Begin);

        var value = s_value2.AsMemory();

        await using var writer = new TabularWriter(_stream, s_dialect, s_options);

        for (var i = 0; i < s_count; i++)
        {
            for (var j = 0; j < s_count; j++)
            {
                await writer.WriteStringAsync(value).ConfigureAwait(false);
            }

            writer.FinishRecord();
        }
    }
}
