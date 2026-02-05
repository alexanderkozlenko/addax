#pragma warning disable CA1001

using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class WritingFieldsAsync
{
    private const int s_count = 1024;

    private static readonly TabularDialect s_dialect = new("t", 'd', 'q');
    private static readonly TabularOptions s_options = new() { LeaveOpen = true };

    private const string s_value0 = "";
    private const string s_value1 = "vvvv";
    private const string s_value2 = "dddd";

    private readonly MemoryStream _stream = new();

    [Benchmark(Description = "writing field: async, empty")]
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

    [Benchmark(Description = "writing field: async, regular")]
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

    [Benchmark(Description = "writing field: async, escaped")]
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
