using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularWriterTests
{
    [TestMethod]
    [DataRow("ndqe", "", "")]
    [DataRow("ndqq", "", "")]
    [DataRow("rdqe", "", "")]
    [DataRow("rdqq", "", "")]
    [DataRow("ndqe", "v", "v")]
    [DataRow("ndqq", "v", "v")]
    [DataRow("rdqe", "v", "v")]
    [DataRow("rdqq", "v", "v")]
    [DataRow("ndqe", "e", "e")]
    [DataRow("rdqe", "e", "e")]
    [DataRow("ndqe", "d", "qdq")]
    [DataRow("ndqq", "d", "qdq")]
    [DataRow("rdqe", "d", "qdq")]
    [DataRow("rdqq", "d", "qdq")]
    [DataRow("ndqe", "n", "qnq")]
    [DataRow("ndqq", "n", "qnq")]
    [DataRow("rdqe", "rn", "qrnq")]
    [DataRow("rdqq", "rn", "qrnq")]
    [DataRow("rdqe", "r", "qrq")]
    [DataRow("rdqq", "r", "qrq")]
    [DataRow("rdqe", "n", "qnq")]
    [DataRow("rdqq", "n", "qnq")]
    [DataRow("ndqe", "q", "qeqq")]
    [DataRow("ndqq", "q", "qqqq")]
    [DataRow("rdqe", "q", "qeqq")]
    [DataRow("rdqq", "q", "qqqq")]
    [DataRow("ndqe", "qe", "qeqeeq")]
    [DataRow("rdqe", "qe", "qeqeeq")]
    [DataRow("ndqe", ".", "d")]
    [DataRow("ndqq", ".", "d")]
    [DataRow("rdqe", ".", "d")]
    [DataRow("rdqq", ".", "d")]
    [DataRow("ndqe", "v.", "vd")]
    [DataRow("ndqq", "v.", "vd")]
    [DataRow("rdqe", "v.", "vd")]
    [DataRow("rdqq", "v.", "vd")]
    [DataRow("ndqe", "e.", "ed")]
    [DataRow("rdqe", "e.", "ed")]
    [DataRow("ndqe", "d.", "qdqd")]
    [DataRow("ndqq", "d.", "qdqd")]
    [DataRow("rdqe", "d.", "qdqd")]
    [DataRow("rdqq", "d.", "qdqd")]
    [DataRow("ndqe", "n.", "qnqd")]
    [DataRow("ndqq", "n.", "qnqd")]
    [DataRow("rdqe", "rn.", "qrnqd")]
    [DataRow("rdqq", "rn.", "qrnqd")]
    [DataRow("rdqe", "r.", "qrqd")]
    [DataRow("rdqq", "r.", "qrqd")]
    [DataRow("rdqe", "n.", "qnqd")]
    [DataRow("rdqq", "n.", "qnqd")]
    [DataRow("ndqe", "q.", "qeqqd")]
    [DataRow("ndqq", "q.", "qqqqd")]
    [DataRow("rdqe", "q.", "qeqqd")]
    [DataRow("rdqq", "q.", "qqqqd")]
    [DataRow("ndqe", "qe.", "qeqeeqd")]
    [DataRow("rdqe", "qe.", "qeqeeqd")]
    [DataRow("ndqe", ":", "n")]
    [DataRow("ndqq", ":", "n")]
    [DataRow("rdqe", ":", "rn")]
    [DataRow("rdqq", ":", "rn")]
    [DataRow("ndqe", "v:", "vn")]
    [DataRow("ndqq", "v:", "vn")]
    [DataRow("rdqe", "v:", "vrn")]
    [DataRow("rdqq", "v:", "vrn")]
    [DataRow("ndqe", "e:", "en")]
    [DataRow("rdqe", "e:", "ern")]
    [DataRow("ndqe", "d:", "qdqn")]
    [DataRow("ndqq", "d:", "qdqn")]
    [DataRow("rdqe", "d:", "qdqrn")]
    [DataRow("rdqq", "d:", "qdqrn")]
    [DataRow("ndqe", "n:", "qnqn")]
    [DataRow("ndqq", "n:", "qnqn")]
    [DataRow("rdqe", "rn:", "qrnqrn")]
    [DataRow("rdqq", "rn:", "qrnqrn")]
    [DataRow("rdqe", "r:", "qrqrn")]
    [DataRow("rdqq", "r:", "qrqrn")]
    [DataRow("rdqe", "n:", "qnqrn")]
    [DataRow("rdqq", "n:", "qnqrn")]
    [DataRow("ndqe", "q:", "qeqqn")]
    [DataRow("ndqq", "q:", "qqqqn")]
    [DataRow("rdqe", "q:", "qeqqrn")]
    [DataRow("rdqq", "q:", "qqqqrn")]
    [DataRow("ndqe", "qe:", "qeqeeqn")]
    [DataRow("rdqe", "qe:", "qeqeeqrn")]
    [DataRow("ndqea", "a", "qaq")]
    [DataRow("ndqqa", "a", "qaq")]
    [DataRow("rdqea", "a", "qaq")]
    [DataRow("rdqqa", "a", "qaq")]
    [DataRow("ndqea", "av", "qavq")]
    [DataRow("ndqqa", "av", "qavq")]
    [DataRow("rdqea", "av", "qavq")]
    [DataRow("rdqqa", "av", "qavq")]
    [DataRow("ndqea", "ae", "qaeeq")]
    [DataRow("rdqea", "ae", "qaeeq")]
    [DataRow("ndqea", "ad", "qadq")]
    [DataRow("ndqqa", "ad", "qadq")]
    [DataRow("rdqea", "ad", "qadq")]
    [DataRow("rdqqa", "ad", "qadq")]
    [DataRow("rdqea", "ar", "qarq")]
    [DataRow("rdqqa", "ar", "qarq")]
    [DataRow("rdqea", "an", "qanq")]
    [DataRow("rdqqa", "an", "qanq")]
    [DataRow("ndqea", "aq", "qaeqq")]
    [DataRow("ndqqa", "aq", "qaqqq")]
    [DataRow("rdqea", "aq", "qaeqq")]
    [DataRow("rdqqa", "aq", "qaqqq")]
    [DataRow("ndqea", "va", "va")]
    [DataRow("ndqqa", "va", "va")]
    [DataRow("rdqea", "va", "va")]
    [DataRow("rdqqa", "va", "va")]
    [DataRow("ndqea", "ea", "ea")]
    [DataRow("rdqea", "ea", "ea")]
    [DataRow("ndqea", "da", "qdaq")]
    [DataRow("ndqqa", "da", "qdaq")]
    [DataRow("rdqea", "da", "qdaq")]
    [DataRow("rdqqa", "da", "qdaq")]
    [DataRow("rdqea", "ra", "qraq")]
    [DataRow("rdqqa", "ra", "qraq")]
    [DataRow("rdqea", "na", "qnaq")]
    [DataRow("rdqqa", "na", "qnaq")]
    [DataRow("ndqea", "qa", "qeqaq")]
    [DataRow("ndqqa", "qa", "qqqaq")]
    [DataRow("rdqea", "qa", "qeqaq")]
    [DataRow("rdqqa", "qa", "qqqaq")]
    [DataRow("ndqea", "a.", "qaqd")]
    [DataRow("ndqqa", "a.", "qaqd")]
    [DataRow("rdqea", "a.", "qaqd")]
    [DataRow("rdqqa", "a.", "qaqd")]
    [DataRow("ndqea", "a:", "qaqn")]
    [DataRow("ndqqa", "a:", "qaqn")]
    [DataRow("rdqea", "a:", "qaqrn")]
    [DataRow("rdqqa", "a:", "qaqrn")]
    public void CommitStringPositive(string dialectScript, string structureScript, string content)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structureScript.Split(':'))
        {
            foreach (var field in record.Split('.'))
            {
                writer.WriteString(field);
            }

            writer.FinishRecord();
        }

        writer.Flush();

        Assert.AreEqual(content, Encoding.ASCII.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ndqea", "a", "a")]
    [DataRow("ndqqa", "a", "a")]
    [DataRow("rdqea", "a", "a")]
    [DataRow("rdqqa", "a", "a")]
    [DataRow("ndqea", "ad", "ad")]
    [DataRow("ndqqa", "ad", "ad")]
    [DataRow("rdqea", "ad", "ad")]
    [DataRow("rdqqa", "ad", "ad")]
    [DataRow("rdqea", "ar", "ar")]
    [DataRow("rdqqa", "ar", "ar")]
    [DataRow("rdqea", "an", "an")]
    [DataRow("rdqqa", "an", "an")]
    [DataRow("ndqea", "a.", "an")]
    [DataRow("ndqqa", "a.", "an")]
    [DataRow("rdqea", "a.", "arn")]
    [DataRow("rdqqa", "a.", "arn")]
    [DataRow("ndqea", "a:", "an")]
    [DataRow("ndqqa", "a:", "an")]
    [DataRow("rdqea", "a:", "arn")]
    [DataRow("rdqqa", "a:", "arn")]
    public void CommitAnnotationPositive(string dialectScript, string structureScript, string content)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structureScript.Split(':'))
        {
            foreach (var field in record.Split('.'))
            {
                if (field.StartsWith('a'))
                {
                    writer.WriteAnnotation(field.AsSpan(1));
                }
                else
                {
                    writer.WriteString(field);
                }
            }

            writer.FinishRecord();
        }

        writer.Flush();

        Assert.AreEqual(content, Encoding.ASCII.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ndqe", "a")]
    [DataRow("ndqq", "a")]
    [DataRow("rdqe", "a")]
    [DataRow("rdqq", "a")]
    [DataRow("ndqea", "an")]
    [DataRow("ndqqa", "an")]
    [DataRow("rdqea", "arn")]
    [DataRow("rdqqa", "arn")]
    public void CommitAnnotationNegative(string dialectScript, string structureScript)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        if (!dialect.AnnotationPrefix.HasValue)
        {
            Assert.ThrowsException<InvalidOperationException>(() => WriteStream(writer, structureScript));
        }
        else
        {
            Assert.ThrowsException<FormatException>(() => WriteStream(writer, structureScript));
        }

        static void WriteStream(TabularWriter writer, string structureScript)
        {
            foreach (var record in structureScript.Split(':'))
            {
                foreach (var field in record.Split('.'))
                {
                    if (field.StartsWith('a'))
                    {
                        writer.WriteAnnotation(field.AsSpan(1));
                    }
                    else
                    {
                        writer.WriteString(field);
                    }
                }

                writer.FinishRecord();
            }
        }
    }

    [TestMethod]
    [DataRow("ndqe", "")]
    [DataRow("ndqq", "")]
    [DataRow("rdqe", "")]
    [DataRow("rdqq", "")]
    [DataRow("xdqe", "")]
    [DataRow("xdqq", "")]
    public void WriteEmpty(string dialectScript, string content)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteEmpty();
        writer.Flush();

        Assert.AreEqual(content, Encoding.ASCII.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ndqe", "h1.h2:v11.v12:v21.v22", "h1dh2nv11dv12nv21dv22")]
    [DataRow("ndqq", "h1.h2:v11.v12:v21.v22", "h1dh2nv11dv12nv21dv22")]
    [DataRow("rdqe", "h1.h2:v11.v12:v21.v22", "h1dh2rnv11dv12rnv21dv22")]
    [DataRow("rdqq", "h1.h2:v11.v12:v21.v22", "h1dh2rnv11dv12rnv21dv22")]
    [DataRow("ndqea", "h1.h2:v11.v12:v21.v22", "h1dh2nv11dv12nv21dv22")]
    [DataRow("ndqqa", "h1.h2:v11.v12:v21.v22", "h1dh2nv11dv12nv21dv22")]
    [DataRow("rdqea", "h1.h2:v11.v12:v21.v22", "h1dh2rnv11dv12rnv21dv22")]
    [DataRow("rdqqa", "h1.h2:v11.v12:v21.v22", "h1dh2rnv11dv12rnv21dv22")]
    public void WriteRecord(string dialectScript, string structureScript, string content)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        foreach (var record in structureScript.Split(':'))
        {
            writer.WriteRecord(record.Split('.'));
        }

        writer.Flush();

        Assert.AreEqual(content, Encoding.ASCII.GetString(stream.ToArray()));
    }

    private static TabularDialect CreateDialect(string script)
    {
        return script.Length switch
        {
            4 => new(script[0] == 'n' ? "n" : "rn", script[1], script[2], script[3]),
            5 => new(script[0] == 'n' ? "n" : "rn", script[1], script[2], script[3], script[4]),
            _ => throw new NotSupportedException(),
        };
    }
}
