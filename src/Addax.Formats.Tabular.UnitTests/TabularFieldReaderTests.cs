#pragma warning disable IDE0025
#pragma warning disable IDE1006

using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularFieldReaderTests
{
    public static IEnumerable<object[]> ConsumeStreamData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", "[]", -1, "lf-1a-1b-1c" },
            new object[] { "", "[]", -1, "lf-1a-1b-1b" },
            new object[] { "", "[]", -1, "crlf-1a-1b-1c" },
            new object[] { "", "[]", -1, "crlf-1a-1b-1b" },
            new object[] { "v", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "v", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "v", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "v", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "vq", "", 1, "lf-1a-1b-1c" },
            new object[] { "vq", "", 1, "lf-1a-1b-1b" },
            new object[] { "vq", "", 1, "crlf-1a-1b-1c" },
            new object[] { "vq", "", 1, "crlf-1a-1b-1b" },
            new object[] { "ve", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "ve", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "vd", "[v:]", -1, "lf-1a-1b-1c" },
            new object[] { "vd", "[v:]", -1, "lf-1a-1b-1b" },
            new object[] { "vd", "[v:]", -1, "crlf-1a-1b-1c" },
            new object[] { "vd", "[v:]", -1, "crlf-1a-1b-1b" },
            new object[] { "vdc", "[v:v]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "vdc", "[v:v]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "vl", "[v][]", -1, "lf-1a-1b-1c" },
            new object[] { "vl", "[v][]", -1, "lf-1a-1b-1b" },
            new object[] { "vl", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "vl", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "vt", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "vt", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "vlt", "[v][]", -1, "crlf-1a-1b-1c" },
            new object[] { "vlt", "[v][]", -1, "crlf-1a-1b-1b" },
            new object[] { "vlv", "[v][v]", -1, "lf-1a-1b-1c" },
            new object[] { "vlv", "[v][v]", -1, "lf-1a-1b-1b" },
            new object[] { "vlv", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "vlv", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "q", "", 0, "lf-1a-1b-1c" },
            new object[] { "q", "", 0, "lf-1a-1b-1b" },
            new object[] { "q", "", 0, "crlf-1a-1b-1c" },
            new object[] { "q", "", 0, "crlf-1a-1b-1b" },
            new object[] { "qv", "", 1, "lf-1a-1b-1c" },
            new object[] { "qv", "", 1, "lf-1a-1b-1b" },
            new object[] { "qv", "", 1, "crlf-1a-1b-1c" },
            new object[] { "qv", "", 1, "crlf-1a-1b-1b" },
            new object[] { "qq", "[]", -1, "lf-1a-1b-1c" },
            new object[] { "qq", "[]", -1, "lf-1a-1b-1b" },
            new object[] { "qq", "[]", -1, "crlf-1a-1b-1c" },
            new object[] { "qq", "[]", -1, "crlf-1a-1b-1b" },
            new object[] { "qe", "", 1, "lf-1a-1b-1c" },
            new object[] { "qe", "", 1, "crlf-1a-1b-1c" },
            new object[] { "qvq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qvq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qvq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qvq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qevq", "", 2, "lf-1a-1b-1c" },
            new object[] { "qevq", "", 2, "lf-1a-1b-1b" },
            new object[] { "qevq", "", 2, "crlf-1a-1b-1c" },
            new object[] { "qevq", "", 2, "crlf-1a-1b-1b" },
            new object[] { "qeqq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qeqq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qeqq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qeqq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qeeq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qeeq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qeeq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qeeq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qeqvq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qeqvq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qeqvq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qeqvq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qeevq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qeevq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qeevq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qeevq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qveqq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qveqq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qveqq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qveqq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qveeq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qveeq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qveeq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qveeq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qveqvq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qveqvq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qveqvq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qveqvq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qveevq", "[v]", -1, "lf-1a-1b-1c" },
            new object[] { "qveevq", "[v]", -1, "lf-1a-1b-1b" },
            new object[] { "qveevq", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "qveevq", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "qqv", "", 2, "lf-1a-1b-1c" },
            new object[] { "qqv", "", 2, "lf-1a-1b-1b" },
            new object[] { "qqv", "", 2, "crlf-1a-1b-1c" },
            new object[] { "qqv", "", 2, "crlf-1a-1b-1b" },
            new object[] { "qqd", "[:]", -1, "lf-1a-1b-1c" },
            new object[] { "qqd", "[:]", -1, "lf-1a-1b-1b" },
            new object[] { "qqd", "[:]", -1, "crlf-1a-1b-1c" },
            new object[] { "qqd", "[:]", -1, "crlf-1a-1b-1b" },
            new object[] { "qql", "[][]", -1, "lf-1a-1b-1c" },
            new object[] { "qql", "[][]", -1, "lf-1a-1b-1b" },
            new object[] { "qql", "", 2, "crlf-1a-1b-1c" },
            new object[] { "qql", "", 2, "crlf-1a-1b-1b" },
            new object[] { "qqlt", "[][]", -1, "crlf-1a-1b-1c" },
            new object[] { "qqlt", "[][]", -1, "crlf-1a-1b-1b" },
            new object[] { "qqlv", "", 3, "crlf-1a-1b-1c" },
            new object[] { "qqlv", "", 3, "crlf-1a-1b-1b" },
            new object[] { "d", "[:]", -1, "lf-1a-1b-1c" },
            new object[] { "d", "[:]", -1, "lf-1a-1b-1b" },
            new object[] { "d", "[:]", -1, "crlf-1a-1b-1c" },
            new object[] { "d", "[:]", -1, "crlf-1a-1b-1b" },
            new object[] { "l", "[][]", -1, "lf-1a-1b-1c" },
            new object[] { "l", "[][]", -1, "lf-1a-1b-1b" },
            new object[] { "l", "[v]", -1, "crlf-1a-1b-1c" },
            new object[] { "l", "[v]", -1, "crlf-1a-1b-1b" },
            new object[] { "lt", "[][]", -1, "crlf-1a-1b-1c" },
            new object[] { "lt", "[][]", -1, "crlf-1a-1b-1b" },
            new object[] { "c", "[c]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "c", "[c]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "cv", "[c]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "cv", "[c]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "cd", "[c]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "cd", "[c]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "cq", "[c]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "cq", "[c]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "ce", "[c]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "ce", "[c]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "cc", "[c]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "cc", "[c]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "cl", "[c][]", -1, "lf-1a-1b-1c-1d" },
            new object[] { "cl", "[c]", -1, "crlf-1a-1b-1c-1d" },
            new object[] { "clt", "[c][]", -1, "crlf-1a-1b-1c-1d" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(ConsumeStreamData))]
    public async Task ConsumeStream(string streamScript, string outputScript, long errorPosition, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream();

        stream.Write(encoding.Preamble);
        stream.Write(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        stream.Position = 0;

        await using var reader = new TabularFieldReader(stream, dialect, new(encoding, bufferSize: 1));

        var scriptBuilder = new StringBuilder();
        var recordBuilder = new StringBuilder();

        try
        {
            while (await reader.MoveNextRecordAsync(CancellationToken))
            {
                scriptBuilder.Append('[');
                var recordHasContent = false;

                while (await reader.ReadFieldAsync(CancellationToken))
                {
                    if (recordHasContent)
                    {
                        recordBuilder.Append(':');
                    }

                    if (reader.FieldType is TabularFieldType.Comment)
                    {
                        recordBuilder.Append('c');
                    }
                    else if (reader.Value.Length != 0)
                    {
                        recordBuilder.Append('v');
                    }

                    recordHasContent = true;
                }

                scriptBuilder.Append(recordBuilder);
                scriptBuilder.Append(']');
                recordBuilder.Clear();
            }

            scriptBuilder.Replace(dialect.LineTerminator[^1], 't');
            scriptBuilder.Replace(dialect.LineTerminator[0], 'l');
            scriptBuilder.Replace(dialect.QuoteChar, 'q');
            scriptBuilder.Replace(dialect.EscapeChar, 'e');

            Assert.AreEqual(outputScript, scriptBuilder.ToString());
        }
        catch (TabularDataException exception)
        {
            Assert.AreEqual(errorPosition, exception.Position);
        }
    }

    private CancellationToken CancellationToken
    {
        get
        {
            return TestContext?.CancellationTokenSource?.Token ?? default;
        }
    }

    public TestContext? TestContext
    {
        get;
        set;
    }
}
