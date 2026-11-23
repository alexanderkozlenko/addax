namespace Addax.Formats.Tabular.Benchmarks;

internal static class BenchmarkData
{
    public static readonly TabularDialect Dialect = new("$@", '^', '\'');
    public static readonly TabularOptions Options = new();

    public static MemoryStream CreateStream(int records, Action<TabularWriter> writeRecord)
    {
        var stream = new MemoryStream();

        using (var writer = new TabularWriter(stream, Dialect, Options))
        {
            for (var i = 0; i < records; i++)
            {
                writeRecord(writer);
                writer.WriteEndOfRecord();
            }
        }

        stream.Seek(0, SeekOrigin.Begin);

        return stream;
    }
}
