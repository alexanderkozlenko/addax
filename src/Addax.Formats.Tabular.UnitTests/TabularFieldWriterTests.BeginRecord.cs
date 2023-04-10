#pragma warning disable IDE0025
#pragma warning disable IDE1006

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

public partial class TabularFieldWriterTests
{
    public static IEnumerable<object[]> BeginNewLineData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "lf-1a-1b-1c" },
            new object[] { "lf-1a-1b-1b" },
            new object[] { "crlf-1a-1b-1c" },
            new object[] { "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(BeginNewLineData))]
    public async Task BeginRecord(string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();
        writer.BeginRecord();

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, encoding);
        var result = await reader.ReadToEndAsync(CancellationToken);

        Assert.AreEqual(dialect.LineTerminator, result);
    }
}
