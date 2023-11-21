using System.Runtime.CompilerServices;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class ReadBenchmark
{
    private const int s_count = 1024;

    private static readonly TabularDialect s_dialect = new("t", 'd', 'q');
    private static readonly TabularOptions s_options = new() { Encoding = Encoding.ASCII, LeaveOpen = true };

    private readonly MemoryStream _stream0 = CreateStream("");
    private readonly MemoryStream _stream1 = CreateStream("vvvv");
    private readonly MemoryStream _stream2 = CreateStream("dddd");

    [Benchmark(Description = "read field: empty")]
    public void Read0()
    {
        _stream0.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_stream0, s_dialect, s_options);

        while (reader.TryPickRecord())
        {
            while (reader.TryReadField())
            {
            }
        }
    }

    [Benchmark(Description = "read field: regular")]
    public void Read1()
    {
        _stream1.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_stream1, s_dialect, s_options);

        while (reader.TryPickRecord())
        {
            while (reader.TryReadField())
            {
            }
        }
    }

    [Benchmark(Description = "read field: escaped")]
    public void Read2()
    {
        _stream2.Seek(0, SeekOrigin.Begin);

        using var reader = new TabularReader(_stream2, s_dialect, s_options);

        while (reader.TryPickRecord())
        {
            while (reader.TryReadField())
            {
            }
        }
    }

    [Benchmark(Description = "read field: empty (async)")]
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Read0Async()
    {
        _stream0.Seek(0, SeekOrigin.Begin);

        await using var reader = new TabularReader(_stream0, s_dialect, s_options);

        while (reader.TryPickRecord())
        {
            while (await reader.TryReadFieldAsync().ConfigureAwait(false))
            {
            }
        }
    }

    [Benchmark(Description = "read field: regular (async)")]
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Read1Async()
    {
        _stream1.Seek(0, SeekOrigin.Begin);

        await using var reader = new TabularReader(_stream1, s_dialect, s_options);

        while (reader.TryPickRecord())
        {
            while (await reader.TryReadFieldAsync().ConfigureAwait(false))
            {
            }
        }
    }

    [Benchmark(Description = "read field: escaped (async)")]
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask Read2Async()
    {
        _stream2.Seek(0, SeekOrigin.Begin);

        await using var reader = new TabularReader(_stream2, s_dialect, s_options);

        while (reader.TryPickRecord())
        {
            while (await reader.TryReadFieldAsync().ConfigureAwait(false))
            {
            }
        }
    }

    private static MemoryStream CreateStream(string value)
    {
        var stream = new MemoryStream();

        using (var writer = new StreamWriter(stream, s_options.Encoding, leaveOpen: true))
        {
            for (var i = 0; i < s_count; i++)
            {
                if (i > 0)
                {
                    writer.Write(s_dialect.LineTerminator);
                }

                for (var j = 0; j < s_count; j++)
                {
                    if (j > 0)
                    {
                        writer.Write(s_dialect.Delimiter);
                    }

                    writer.Write(value);
                }
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}
