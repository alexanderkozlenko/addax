#pragma warning disable CA1001

using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class WritingFields
{
    private const int s_count = 1024;

    private static readonly TabularDialect s_dialect = new("t", 'd', 'q');
    private static readonly TabularOptions s_options = new() { LeaveOpen = true };

    private const string s_value0 = "";
    private const string s_value1 = "vvvv";
    private const string s_value2 = "dddd";

    private readonly MemoryStream _stream = new();

    [Benchmark(Description = "writing field: sync, empty")]
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

    [Benchmark(Description = "writing field: sync, regular")]
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

    [Benchmark(Description = "writing field: sync, escaped")]
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
}
