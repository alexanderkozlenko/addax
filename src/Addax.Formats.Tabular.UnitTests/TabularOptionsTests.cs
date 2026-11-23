using System.Text;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularOptionsTests
{
    [TestMethod]
    public void BufferSizeIsSixteenKilobytesByDefault()
    {
        var options = new TabularOptions();

        Assert.AreEqual(16384, options.BufferSize);
    }

    [TestMethod]
    public void EncodingIsNullByDefault()
    {
        var options = new TabularOptions();

        Assert.IsNull(options.Encoding);
    }

    [TestMethod]
    public void StringFactoryIsNullByDefault()
    {
        var options = new TabularOptions();

        Assert.IsNull(options.StringFactory);
    }

    [TestMethod]
    public void TrimWhitespaceIsFalseByDefault()
    {
        var options = new TabularOptions();

        Assert.IsFalse(options.TrimWhitespace);
    }

    [TestMethod]
    public void FormatProviderIsNullByDefault()
    {
        var options = new TabularOptions();

        Assert.IsNull(options.FormatProvider);
    }

    [TestMethod]
    public void ConvertersIsEmptyByDefault()
    {
        var options = new TabularOptions();

        Assert.IsEmpty(options.Converters);
    }

    [TestMethod]
    public void HandlersIsEmptyByDefault()
    {
        var options = new TabularOptions();

        Assert.IsEmpty(options.Handlers);
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void ReaderAcceptsBufferSizeOutOfRange(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a^'b$c'$d"));
        using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryMoveToNextRecord())
        {
            var fields = new List<string>();

            while (reader.TryReadField())
            {
                fields.Add(reader.GetString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreEqual("a|b$c/d", string.Join('/', records));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public async Task ReaderAcceptsBufferSizeOutOfRangeAsync(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a^'b$c'$d"));
        await using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryMoveToNextRecordAsync())
        {
            var fields = new List<string>();

            while (await reader.TryReadFieldAsync())
            {
                fields.Add(reader.GetString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreEqual("a|b$c/d", string.Join('/', records));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public void WriterAcceptsBufferSizeOutOfRange(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("a");
        writer.WriteString("b$c");
        writer.WriteEndOfRecord();
        writer.WriteString("d");
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual("a^'b$c'$d$", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow(int.MinValue)]
    [DataRow(int.MaxValue)]
    public async Task WriterAcceptsBufferSizeOutOfRangeAsync(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("a");
        await writer.WriteStringAsync("b$c");
        await writer.WriteEndOfRecordAsync();
        await writer.WriteStringAsync("d");
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual("a^'b$c'$d$", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("'a'^'a'")]
    [DataRow("a^a")]
    public void ReaderWithDefaultStringFactoryCreatesNewInstances(string content)
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        Assert.IsTrue(reader.TryReadField());

        var value1 = reader.GetString();

        Assert.IsTrue(reader.TryReadField());

        var value2 = reader.GetString();

        Assert.AreEqual(value1, value2);
        Assert.AreNotSame(value1, value2);
    }

    [TestMethod]
    [DataRow("'a'^'a'")]
    [DataRow("a^a")]
    public async Task ReaderWithDefaultStringFactoryCreatesNewInstancesAsync(string content)
    {
        var dialect = CreateDialect("$", '\'');

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect);

        Assert.IsTrue(await reader.TryReadFieldAsync());

        var value1 = reader.GetString();

        Assert.IsTrue(await reader.TryReadFieldAsync());

        var value2 = reader.GetString();

        Assert.AreEqual(value1, value2);
        Assert.AreNotSame(value1, value2);
    }

    [TestMethod]
    [DataRow("$")]
    [DataRow("''")]
    [DataRow("^")]
    public void ReaderWithDefaultStringFactoryReturnsEmptyString(string content)
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreSame(string.Empty, reader.GetString());
    }

    [TestMethod]
    [DataRow("$")]
    [DataRow("''")]
    [DataRow("^")]
    public async Task ReaderWithDefaultStringFactoryReturnsEmptyStringAsync(string content)
    {
        var dialect = CreateDialect("$", '\'');

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreSame(string.Empty, reader.GetString());
    }

    private static TabularDialect CreateDialect(string lineTerminator, char escapeChar)
    {
        return new(lineTerminator, '^', '\'', escapeChar);
    }
}
