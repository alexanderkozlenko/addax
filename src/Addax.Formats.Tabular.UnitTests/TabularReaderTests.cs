using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularReaderTests
{
    [DataTestMethod]
    [DataRow("ndqe", "", "")]
    [DataRow("ndqq", "", "")]
    [DataRow("rdqe", "", "")]
    [DataRow("rdqq", "", "")]
    [DataRow("ndqe", "v", "v")]
    [DataRow("ndqq", "v", "v")]
    [DataRow("rdqe", "v", "v")]
    [DataRow("rdqq", "v", "v")]
    [DataRow("rdqe", "r", "r")]
    [DataRow("rdqq", "r", "r")]
    [DataRow("rdqe", "rv", "rv")]
    [DataRow("rdqq", "rv", "rv")]
    [DataRow("rdqe", "n", "n")]
    [DataRow("rdqq", "n", "n")]
    [DataRow("ndqe", "e", "e")]
    [DataRow("rdqe", "e", "e")]
    [DataRow("ndqe", "qq", "")]
    [DataRow("ndqq", "qq", "")]
    [DataRow("rdqe", "qq", "")]
    [DataRow("rdqq", "qq", "")]
    [DataRow("ndqe", "qvq", "v")]
    [DataRow("ndqq", "qvq", "v")]
    [DataRow("rdqe", "qvq", "v")]
    [DataRow("rdqq", "qvq", "v")]
    [DataRow("rdqe", "qrq", "r")]
    [DataRow("rdqq", "qrq", "r")]
    [DataRow("rdqe", "qnq", "n")]
    [DataRow("rdqq", "qnq", "n")]
    [DataRow("ndqe", "qeqq", "q")]
    [DataRow("ndqq", "qqqq", "q")]
    [DataRow("rdqe", "qeqq", "q")]
    [DataRow("rdqq", "qqqq", "q")]
    [DataRow("ndqe", "qeeq", "e")]
    [DataRow("rdqe", "qeeq", "e")]
    [DataRow("ndqe", "d", ".")]
    [DataRow("ndqq", "d", ".")]
    [DataRow("rdqe", "d", ".")]
    [DataRow("rdqq", "d", ".")]
    [DataRow("ndqe", "vd", "v.")]
    [DataRow("ndqq", "vd", "v.")]
    [DataRow("rdqe", "vd", "v.")]
    [DataRow("rdqq", "vd", "v.")]
    [DataRow("rdqe", "rd", "r.")]
    [DataRow("rdqq", "rd", "r.")]
    [DataRow("rdqe", "nd", "n.")]
    [DataRow("rdqq", "nd", "n.")]
    [DataRow("ndqe", "ed", "e.")]
    [DataRow("rdqe", "ed", "e.")]
    [DataRow("ndqe", "qqd", ".")]
    [DataRow("ndqq", "qqd", ".")]
    [DataRow("rdqe", "qqd", ".")]
    [DataRow("rdqq", "qqd", ".")]
    [DataRow("ndqe", "qvqd", "v.")]
    [DataRow("ndqq", "qvqd", "v.")]
    [DataRow("rdqe", "qvqd", "v.")]
    [DataRow("rdqq", "qvqd", "v.")]
    [DataRow("rdqe", "qrqd", "r.")]
    [DataRow("rdqq", "qrqd", "r.")]
    [DataRow("rdqe", "qnqd", "n.")]
    [DataRow("rdqq", "qnqd", "n.")]
    [DataRow("ndqe", "qeqqd", "q.")]
    [DataRow("ndqq", "qqqqd", "q.")]
    [DataRow("rdqe", "qeqqd", "q.")]
    [DataRow("rdqq", "qqqqd", "q.")]
    [DataRow("ndqe", "qeeqd", "e.")]
    [DataRow("rdqe", "qeeqd", "e.")]
    [DataRow("ndqe", "n", ":")]
    [DataRow("ndqq", "n", ":")]
    [DataRow("rdqe", "rn", ":")]
    [DataRow("rdqq", "rn", ":")]
    [DataRow("ndqe", "vn", "v:")]
    [DataRow("ndqq", "vn", "v:")]
    [DataRow("rdqe", "vrn", "v:")]
    [DataRow("rdqq", "vrn", "v:")]
    [DataRow("rdqe", "rrn", "r:")]
    [DataRow("rdqq", "rrn", "r:")]
    [DataRow("rdqe", "nrn", "n:")]
    [DataRow("rdqq", "nrn", "n:")]
    [DataRow("ndqe", "en", "e:")]
    [DataRow("rdqe", "ern", "e:")]
    [DataRow("ndqe", "qqn", ":")]
    [DataRow("ndqq", "qqn", ":")]
    [DataRow("rdqe", "qqrn", ":")]
    [DataRow("rdqq", "qqrn", ":")]
    [DataRow("ndqe", "qvqn", "v:")]
    [DataRow("ndqq", "qvqn", "v:")]
    [DataRow("rdqe", "qvqrn", "v:")]
    [DataRow("rdqq", "qvqrn", "v:")]
    [DataRow("rdqe", "qrqrn", "r:")]
    [DataRow("rdqq", "qrqrn", "r:")]
    [DataRow("rdqe", "qnqrn", "n:")]
    [DataRow("rdqq", "qnqrn", "n:")]
    [DataRow("ndqe", "qeqqn", "q:")]
    [DataRow("ndqq", "qqqqn", "q:")]
    [DataRow("rdqe", "qeqqrn", "q:")]
    [DataRow("rdqq", "qqqqrn", "q:")]
    [DataRow("ndqe", "qeeqn", "e:")]
    [DataRow("rdqe", "qeeqrn", "e:")]
    [DataRow("ndqea", "a", "")]
    [DataRow("ndqqa", "a", "")]
    [DataRow("rdqea", "a", "")]
    [DataRow("rdqqa", "a", "")]
    [DataRow("ndqea", "ad", "d")]
    [DataRow("ndqqa", "ad", "d")]
    [DataRow("rdqea", "ad", "d")]
    [DataRow("rdqqa", "ad", "d")]
    [DataRow("ndqea", "an", ":")]
    [DataRow("ndqqa", "an", ":")]
    [DataRow("rdqea", "arn", ":")]
    [DataRow("rdqqa", "arn", ":")]
    [DataRow("rdqea", "ar", "r")]
    [DataRow("rdqqa", "ar", "r")]
    [DataRow("rdqea", "an", "n")]
    [DataRow("rdqqa", "an", "n")]
    public void ConsumePositive(string dialectScript, string content, string structureScript)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var structureBuilder = new StringBuilder();
        var streamStart = true;

        while (reader.TryPickRecord())
        {
            if (!streamStart)
            {
                structureBuilder.Append(':');
            }

            var recordStart = true;

            while (reader.TryReadField())
            {
                if (!recordStart)
                {
                    structureBuilder.Append('.');
                }

                structureBuilder.Append(reader.GetString());
                recordStart = false;
            }

            streamStart = false;
        }

        Assert.AreEqual(structureScript, structureBuilder.ToString());
    }

    [DataTestMethod]
    [DataRow("ndqe", "vq")]
    [DataRow("ndqq", "vq")]
    [DataRow("rdqe", "vq")]
    [DataRow("rdqq", "vq")]
    [DataRow("rdqe", "rq")]
    [DataRow("rdqq", "rq")]
    [DataRow("rdqe", "nq")]
    [DataRow("rdqq", "nq")]
    [DataRow("ndqe", "eq")]
    [DataRow("rdqe", "eq")]
    [DataRow("ndqe", "qqv")]
    [DataRow("ndqq", "qqv")]
    [DataRow("rdqe", "qqv")]
    [DataRow("rdqq", "qqv")]
    [DataRow("rdqe", "qqn")]
    [DataRow("rdqq", "qqn")]
    [DataRow("ndqe", "qqq")]
    [DataRow("ndqq", "qqq")]
    [DataRow("rdqe", "qqq")]
    [DataRow("rdqq", "qqq")]
    [DataRow("ndqe", "qqe")]
    [DataRow("rdqe", "qqe")]
    [DataRow("ndqe", "qevq")]
    [DataRow("ndqq", "qqvq")]
    [DataRow("rdqe", "qevq")]
    [DataRow("rdqq", "qqvq")]
    [DataRow("ndqe", "qedq")]
    [DataRow("ndqq", "qqdq")]
    [DataRow("rdqe", "qedq")]
    [DataRow("rdqq", "qqdq")]
    [DataRow("ndqe", "qenq")]
    [DataRow("ndqq", "qqnq")]
    [DataRow("rdqe", "qerq")]
    [DataRow("rdqq", "qqrq")]
    [DataRow("rdqe", "qenq")]
    [DataRow("rdqq", "qqnq")]
    [DataRow("rdqe", "qqrv")]
    [DataRow("rdqq", "qqrv")]
    [DataRow("rdqe", "qqrd")]
    [DataRow("rdqq", "qqrd")]
    [DataRow("rdqe", "qqrr")]
    [DataRow("rdqq", "qqrr")]
    [DataRow("rdqe", "qqrq")]
    [DataRow("rdqq", "qqrq")]
    [DataRow("rdqe", "qqre")]
    [DataRow("ndqe", "q")]
    [DataRow("ndqq", "q")]
    [DataRow("rdqe", "q")]
    [DataRow("rdqq", "q")]
    [DataRow("ndqe", "qe")]
    [DataRow("rdqe", "qe")]
    [DataRow("rdqe", "qqr")]
    [DataRow("rdqq", "qqr")]
    [DataRow("ndqea", "qqa")]
    [DataRow("ndqqa", "qqa")]
    [DataRow("rdqea", "qqa")]
    [DataRow("rdqqa", "qqa")]
    [DataRow("ndqea", "qeaq")]
    [DataRow("ndqqa", "qqaq")]
    [DataRow("rdqea", "qeaq")]
    [DataRow("rdqqa", "qqaq")]
    [DataRow("rdqea", "qqra")]
    [DataRow("rdqqa", "qqra")]
    public void ConsumeNegative(string dialectScript, string content)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.ThrowsException<TabularContentException>(() => ReadStream(reader));

        static void ReadStream(TabularReader reader)
        {
            while (reader.TryPickRecord())
            {
                while (reader.TryReadField())
                {
                }
            }
        }
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", true)]
    [DataRow("ndqq", "v", true)]
    [DataRow("rdqe", "v", true)]
    [DataRow("rdqq", "v", true)]
    public void TryPickRecordIfStreamStart(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.AreEqual(expected, reader.TryPickRecord());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", false)]
    [DataRow("ndqq", "v", false)]
    [DataRow("rdqe", "v", false)]
    [DataRow("rdqq", "v", false)]
    public void TryPickRecordIfRecordStart(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryPickRecord();

        Assert.AreEqual(expected, reader.TryPickRecord());
    }

    [DataTestMethod]
    [DataRow("ndqe", "vdv", false)]
    [DataRow("ndqq", "vdv", false)]
    [DataRow("rdqe", "vdv", false)]
    [DataRow("rdqq", "vdv", false)]
    public void TryPickRecordIfDelimiter(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TryPickRecord());
    }

    [DataTestMethod]
    [DataRow("ndqe", "vnv", true)]
    [DataRow("ndqq", "vnv", true)]
    [DataRow("rdqe", "vrnv", true)]
    [DataRow("rdqq", "vrnv", true)]
    public void TryPickRecordIfRecordEnd(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TryPickRecord());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", false)]
    [DataRow("ndqq", "v", false)]
    [DataRow("rdqe", "v", false)]
    [DataRow("rdqq", "v", false)]
    public void TryPickRecordIfStreamEnd(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TryPickRecord());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", true)]
    [DataRow("ndqq", "v", true)]
    [DataRow("rdqe", "v", true)]
    [DataRow("rdqq", "v", true)]
    public void TryReadFieldIfStreamStart(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.AreEqual(expected, reader.TryReadField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", true)]
    [DataRow("ndqq", "v", true)]
    [DataRow("rdqe", "v", true)]
    [DataRow("rdqq", "v", true)]
    public void TryReadFieldIfRecordStart(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryPickRecord();

        Assert.AreEqual(expected, reader.TryReadField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "vdv", true)]
    [DataRow("ndqq", "vdv", true)]
    [DataRow("rdqe", "vdv", true)]
    [DataRow("rdqq", "vdv", true)]
    public void TryReadFieldIfDelimiter(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TryReadField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "vnv", false)]
    [DataRow("ndqq", "vnv", false)]
    [DataRow("rdqe", "vrnv", false)]
    [DataRow("rdqq", "vrnv", false)]
    public void TryReadFieldIfRecordEnd(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TryReadField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", false)]
    [DataRow("ndqq", "v", false)]
    [DataRow("rdqe", "v", false)]
    [DataRow("rdqq", "v", false)]
    public void TryReadFieldIfStreamEnd(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TryReadField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", true)]
    [DataRow("ndqq", "v", true)]
    [DataRow("rdqe", "v", true)]
    [DataRow("rdqq", "v", true)]
    public void TrySkipFieldIfStreamStart(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.AreEqual(expected, reader.TrySkipField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", true)]
    [DataRow("ndqq", "v", true)]
    [DataRow("rdqe", "v", true)]
    [DataRow("rdqq", "v", true)]
    public void TrySkipFieldIfRecordStart(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryPickRecord();

        Assert.AreEqual(expected, reader.TrySkipField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "vdv", true)]
    [DataRow("ndqq", "vdv", true)]
    [DataRow("rdqe", "vdv", true)]
    [DataRow("rdqq", "vdv", true)]
    public void TrySkipFieldIfDelimiter(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TrySkipField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "vnv", false)]
    [DataRow("ndqq", "vnv", false)]
    [DataRow("rdqe", "vrnv", false)]
    [DataRow("rdqq", "vrnv", false)]
    public void TrySkipFieldIfRecordEnd(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TrySkipField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "v", false)]
    [DataRow("ndqq", "v", false)]
    [DataRow("rdqe", "v", false)]
    [DataRow("rdqq", "v", false)]
    public void TrySkipFieldIfStreamEnd(string dialectScript, string content, bool expected)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.TrySkipField());
    }

    [DataTestMethod]
    [DataRow("ndqe", "h1dh2nv11dv12nv21dv22")]
    [DataRow("ndqq", "h1dh2nv11dv12nv21dv22")]
    [DataRow("rdqe", "h1dh2rnv11dv12rnv21dv22")]
    [DataRow("rdqq", "h1dh2rnv11dv12rnv21dv22")]
    [DataRow("ndqea", "h1dh2nv11dv12nv21dv22")]
    [DataRow("ndqqa", "h1dh2nv11dv12nv21dv22")]
    [DataRow("rdqea", "h1dh2rnv11dv12rnv21dv22")]
    [DataRow("rdqqa", "h1dh2rnv11dv12rnv21dv22")]
    public void TryReadRecord(string dialectScript, string content)
    {
        var dialect = CreateDialect(dialectScript);
        var options = new TabularOptions { Encoding = Encoding.ASCII, BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TrySkipRecord());
        Assert.IsTrue(reader.TryReadRecord());
        Assert.IsTrue(reader.TryReadRecord());

        Assert.IsNotNull(reader.CurrentRecord);
        Assert.AreEqual(2, reader.CurrentRecord.Length);
        Assert.AreEqual("v21", reader.CurrentRecord[0]);
        Assert.AreEqual("v22", reader.CurrentRecord[1]);
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
