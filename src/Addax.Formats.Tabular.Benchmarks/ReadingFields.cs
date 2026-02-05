using BenchmarkDotNet.Attributes;

namespace Addax.Formats.Tabular.Benchmarks;

public class ReadingFields
{
    private const int s_count = 1024;

    private static readonly TabularDialect s_dialect = new("t", 'd', 'q');
    private static readonly TabularOptions s_options = new() { LeaveOpen = true };

    private readonly MemoryStream _stream0 = CreateStream("");
    private readonly MemoryStream _stream1 = CreateStream("vvvv");
    private readonly MemoryStream _stream2 = CreateStream("qddd");

    [Benchmark(Description = "reading field: sync, empty")]
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

    [Benchmark(Description = "reading field: sync, regular")]
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

    [Benchmark(Description = "reading field: sync, escaped")]
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

    private static MemoryStream CreateStream(string value)
    {
        var stream = new MemoryStream();

        using (var writer = new TabularWriter(stream, s_dialect, s_options))
        {
            for (var i = 0; i < s_count; i++)
            {
                for (var j = 0; j < s_count; j++)
                {
                    writer.WriteString(value);
                }

                writer.FinishRecord();
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}
