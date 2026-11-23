using System.Text;
using Addax.Formats.Tabular.Converters;
using Addax.Formats.Tabular.UnitTests.Handlers;
using Addax.Formats.Tabular.UnitTests.Records;
using Addax.Formats.Tabular.UnitTests.Streams;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularRecordWriterTests
{
    [TestMethod]
    [DataRow("$", '\'', "a|b/c|d", "a^b$c^d$")]
    [DataRow("$", '\\', "a|b|c/d/e|f", "a^b^c$d$e^f$")]
    [DataRow("~$", '\\', "a|b/c|d", "a^b~$c^d~$")]
    public void WriteRecordWritesEveryRecord(string lineTerminator, char escapeChar, string records, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        foreach (var record in records.Split('/'))
        {
            writer.WriteRecord(record.Split('|'));
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "a|b/c|d", "a^b$c^d$")]
    [DataRow("$", '\\', "a|b|c/d/e|f", "a^b^c$d$e^f$")]
    [DataRow("~$", '\\', "a|b/c|d", "a^b~$c^d~$")]
    public async Task WriteRecordWritesEveryRecordAsync(string lineTerminator, char escapeChar, string records, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        foreach (var record in records.Split('/'))
        {
            await writer.WriteRecordAsync(record.Split('|'));
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "v1|v2/v3|v4", "h1^h2$v1^v2$v3^v4$")]
    [DataRow("~$", '\\', "v1|v2", "h1^h2~$v1^v2~$")]
    public void WriteRecordWritesHeaderBeforeFirstRecord(string lineTerminator, char escapeChar, string records, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        foreach (var record in records.Split('/'))
        {
            writer.WriteRecord(record.Split('|'));
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "v1|v2/v3|v4", "h1^h2$v1^v2$v3^v4$")]
    [DataRow("~$", '\\', "v1|v2", "h1^h2~$v1^v2~$")]
    public async Task WriteRecordWritesHeaderBeforeFirstRecordAsync(string lineTerminator, char escapeChar, string records, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        foreach (var record in records.Split('/'))
        {
            await writer.WriteRecordAsync(record.Split('|'));
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "h1^h2~$")]
    public void DisposeWritesHeaderWithoutRecords(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        writer.Dispose();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "h1^h2~$")]
    public async Task DisposeWritesHeaderWithoutRecordsAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await writer.DisposeAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void DisposeWithoutHeaderAndRecordsWritesNothing(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect, options);

        writer.Dispose();

        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task DisposeWithoutHeaderAndRecordsWritesNothingAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect, options);

        await writer.DisposeAsync();

        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void DisposeAfterFailedFlushDoesNotWriteHeader(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new FailingFlushStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<IOException>(() => writer.Flush());

        writer.Dispose();

        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task DisposeAfterFailedFlushDoesNotWriteHeaderAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new FailingFlushStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<IOException>(() => writer.FlushAsync().AsTask());

        await writer.DisposeAsync();

        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void RecordsWrittenIncludesHeader(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        writer.WriteRecord(["v1", "v2"]);

        Assert.AreEqual(2, writer.RecordsWritten);

        writer.WriteRecord(["v3", "v4"]);

        Assert.AreEqual(3, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task RecordsWrittenIncludesHeaderAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        await writer.WriteRecordAsync(["v1", "v2"]);

        Assert.AreEqual(2, writer.RecordsWritten);

        await writer.WriteRecordAsync(["v3", "v4"]);

        Assert.AreEqual(3, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void RecordsWrittenCountsRecords(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        writer.WriteRecord(["v1", "v2"]);
        writer.WriteRecord(["v3"]);

        Assert.AreEqual(2, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task RecordsWrittenCountsRecordsAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        await writer.WriteRecordAsync(["v1", "v2"]);
        await writer.WriteRecordAsync(["v3"]);

        Assert.AreEqual(2, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordThrowsInvalidOperationExceptionAfterFailedRecord(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        writer.WriteRecord(["a", "b"]);

        Assert.ThrowsExactly<FormatException>(() => writer.WriteRecord(["c", "?"]));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteRecord(["d", "e"]));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordThrowsInvalidOperationExceptionAfterFailedRecordAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        await writer.WriteRecordAsync(["a", "b"]);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.WriteRecordAsync(["c", "?"]).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteRecordAsync(["d", "e"]).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void FlushThrowsInvalidOperationExceptionAfterFailedRecord(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        writer.WriteRecord(["a", "b"]);

        Assert.ThrowsExactly<FormatException>(() => writer.WriteRecord(["c", "?"]));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.Flush());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task FlushThrowsInvalidOperationExceptionAfterFailedRecordAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        await writer.WriteRecordAsync(["a", "b"]);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.WriteRecordAsync(["c", "?"]).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.FlushAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c$")]
    [DataRow("~$", '\\', "a^b~$c~$")]
    public void DisposeAfterFailedRecordEndsPartialRecord(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        writer.WriteRecord(["a", "b"]);

        Assert.ThrowsExactly<FormatException>(() => writer.WriteRecord(["c", "?"]));

        writer.Dispose();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c$")]
    [DataRow("~$", '\\', "a^b~$c~$")]
    public async Task DisposeAfterFailedRecordEndsPartialRecordAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await writer.WriteRecordAsync(["a", "b"]);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.WriteRecordAsync(["c", "?"]).AsTask());
        await writer.DisposeAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordThrowsInvalidOperationExceptionAfterFailedHeader(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => writer.WriteRecord(["a"]));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteRecord(["b"]));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordThrowsInvalidOperationExceptionAfterFailedHeaderAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.WriteRecordAsync(["a"]).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteRecordAsync(["b"]).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void FlushThrowsInvalidOperationExceptionAfterFailedHeader(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => writer.WriteRecord(["a"]));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.Flush());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task FlushThrowsInvalidOperationExceptionAfterFailedHeaderAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.WriteRecordAsync(["a"]).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.FlushAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "h1$")]
    [DataRow("~$", '\\', "h1~$")]
    public void DisposeAfterFailedHeaderEndsPartialHeader(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => writer.WriteRecord(["a"]));

        writer.Dispose();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1$")]
    [DataRow("~$", '\\', "h1~$")]
    public async Task DisposeAfterFailedHeaderEndsPartialHeaderAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.WriteRecordAsync(["a"]).AsTask());
        await writer.DisposeAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void DisposeThrowsHandlerExceptionForFailedHeader(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => writer.Dispose());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task DisposeThrowsHandlerExceptionForFailedHeaderAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.DisposeAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordThrowsObjectDisposedExceptionAfterDisposeWithFailedHeader(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => writer.Dispose());
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteRecord(["a"]));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordThrowsObjectDisposedExceptionAfterDisposeWithFailedHeaderAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.DisposeAsync().AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteRecordAsync(["a"]).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "h1$")]
    [DataRow("~$", '\\', "h1~$")]
    public void DisposeWithFailedHeaderEndsPartialHeader(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => writer.Dispose());
        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1$")]
    [DataRow("~$", '\\', "h1~$")]
    public async Task DisposeWithFailedHeaderEndsPartialHeaderAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new FailingHeaderHandler();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => writer.DisposeAsync().AsTask());

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordThrowsInvalidOperationExceptionAfterCanceledWriteRecordAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => writer.WriteRecordAsync(["abc"], new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteRecordAsync(["d"]).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordThrowsInvalidOperationExceptionAfterCanceledFlushAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect);

        await writer.WriteRecordAsync(["a"]);

        await Assert.ThrowsAsync<OperationCanceledException>(() => writer.FlushAsync(new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteRecordAsync(["b"]).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task FlushThrowsInvalidOperationExceptionAfterCanceledFlushAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect);

        await writer.WriteRecordAsync(["a"]);

        await Assert.ThrowsAsync<OperationCanceledException>(() => writer.FlushAsync(new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.FlushAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "1^-2^3$4$")]
    [DataRow("~$", '\\', "1^-2^3~$4~$")]
    public async Task WriteRecordAsyncDoesNotUseSynchronousIO(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, Encoding = new UTF8Encoding(true) };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter<int[]>(stream, dialect, options);

        await writer.WriteRecordAsync([1, -2, 3]);
        await writer.WriteRecordAsync([4]);
        await writer.FlushAsync();

        Assert.AreEqual("\ufeff" + expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordThrowsNotSupportedExceptionForHandlerWithoutOverride(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new AsyncOnlyHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<NotSupportedException>(() => writer.WriteRecord(["a"]));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordThrowsNotSupportedExceptionForHandlerWithoutOverrideAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new SyncOnlyHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => writer.WriteRecordAsync(["a"]).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$")]
    [DataRow("~$", '\\', "a^b~$")]
    public void FlushWritesRecordsToStream(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var target = new MemoryStream();
        using var stream = new BufferedStream(target, 1024);
        using var writer = new TabularWriter<string?[]>(stream, dialect);

        writer.WriteRecord(["a", "b"]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$")]
    [DataRow("~$", '\\', "a^b~$")]
    public async Task FlushWritesRecordsToStreamAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var target = new MemoryStream();
        await using var stream = new BufferedStream(target, 1024);
        await using var writer = new TabularWriter<string?[]>(stream, dialect);

        await writer.WriteRecordAsync(["a", "b"]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c$")]
    [DataRow("~$", '\\', "a^b~$c~$")]
    public void DisposeWritesBufferedRecords(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        writer.WriteRecord(["a", "b"]);
        writer.WriteRecord(["c"]);
        writer.Dispose();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c$")]
    [DataRow("~$", '\\', "a^b~$c~$")]
    public async Task DisposeWritesBufferedRecordsAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        await writer.WriteRecordAsync(["a", "b"]);
        await writer.WriteRecordAsync(["c"]);
        await writer.DisposeAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void BytesCommittedCountsBytesWrittenToStream(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string[]>(stream, dialect, options);

        writer.WriteRecord(["v1", "v2"]);
        writer.Flush();

        Assert.AreEqual(stream.Length, writer.BytesCommitted);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task BytesCommittedCountsBytesWrittenToStreamAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string[]>(stream, dialect, options);

        await writer.WriteRecordAsync(["v1", "v2"]);
        await writer.FlushAsync();

        Assert.AreEqual(stream.Length, writer.BytesCommitted);
    }

    [TestMethod]
    public void WriteRecordThrowsArgumentNullExceptionForNullRecord()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string?[]>(stream, dialect);

        Assert.ThrowsExactly<ArgumentNullException>(() => writer.WriteRecord(null!));
    }

    [TestMethod]
    public async Task WriteRecordThrowsArgumentNullExceptionForNullRecordAsync()
    {
        var dialect = CreateDialect("$", '\'');

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect);

        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => writer.WriteRecordAsync(null!).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "h1^h2~$")]
    public void DisposeTwiceWritesHeaderOnce(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        writer.Dispose();
        writer.Dispose();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "h1^h2~$")]
    public async Task DisposeTwiceWritesHeaderOnceAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string[]>(stream, dialect, options);

        await writer.DisposeAsync();
        await writer.DisposeAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordThrowsObjectDisposedExceptionAfterDispose(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        writer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteRecord(["a"]));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordThrowsObjectDisposedExceptionAfterDisposeAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        await writer.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteRecordAsync(["a"]).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void FlushThrowsObjectDisposedExceptionAfterDispose(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        writer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Flush());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task FlushThrowsObjectDisposedExceptionAfterDisposeAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        await writer.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.FlushAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void DisposeLeavesStreamOpen(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        writer.WriteRecord(["a"]);
        writer.Dispose();

        Assert.IsTrue(stream.CanWrite);
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task DisposeLeavesStreamOpenAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();

        var writer = new TabularWriter<string?[]>(stream, dialect);

        await writer.WriteRecordAsync(["a"]);
        await writer.DisposeAsync();

        Assert.IsTrue(stream.CanWrite);
    }

    [TestMethod]
    public void ConstructorThrowsInvalidOperationExceptionForTypeWithoutHandler()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<InvalidOperationException>(() => new TabularWriter<UnregisteredRecord>(stream, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullStream()
    {
        var dialect = CreateDialect("$", '\'');

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularWriter<string?[]>(null!, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullDialect()
    {
        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularWriter<string?[]>(stream, null!));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForUnwritableStream()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream([], false);

        Assert.ThrowsExactly<ArgumentException>(() => new TabularWriter<string?[]>(stream, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForHandlerOfOtherType()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = TabularRegistry.Handlers[typeof(int[])];

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularWriter<string[]>(stream, dialect, options));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForNullHandler()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = null!;

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularWriter<string[]>(stream, dialect, options));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^^b$")]
    [DataRow("~$", '\\', "a^^b~$")]
    public void WriteRecordWritesNullStringAsEmptyField(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        writer.WriteRecord(["a", null, "b"]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^^b$")]
    [DataRow("~$", '\\', "a^^b~$")]
    public async Task WriteRecordWritesNullStringAsEmptyFieldAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        await writer.WriteRecordAsync(["a", null, "b"]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "1^-2^3$")]
    [DataRow("~$", '\\', "1^-2^3~$")]
    public void WriteRecordWritesInt32Array(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<int[]>(stream, dialect, options);

        writer.WriteRecord([1, -2, 3]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "1^-2^3$")]
    [DataRow("~$", '\\', "1^-2^3~$")]
    public async Task WriteRecordWritesInt32ArrayAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<int[]>(stream, dialect, options);

        await writer.WriteRecordAsync([1, -2, 3]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "1^^3$")]
    [DataRow("~$", '\\', "1^^3~$")]
    public void WriteRecordWritesNullNullableInt32AsEmptyField(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<int?[]>(stream, dialect, options);

        writer.WriteRecord([1, null, 3]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "1^^3$")]
    [DataRow("~$", '\\', "1^^3~$")]
    public async Task WriteRecordWritesNullNullableInt32AsEmptyFieldAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<int?[]>(stream, dialect, options);

        await writer.WriteRecordAsync([1, null, 3]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02^^2025-12-31$")]
    [DataRow("~$", '\\', "2024-01-02^^2025-12-31~$")]
    public void WriteRecordWritesNullableDateOnlyArray(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<DateOnly?[]>(stream, dialect, options);

        writer.WriteRecord([new DateOnly(2024, 1, 2), null, new DateOnly(2025, 12, 31)]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02^^2025-12-31$")]
    [DataRow("~$", '\\', "2024-01-02^^2025-12-31~$")]
    public async Task WriteRecordWritesNullableDateOnlyArrayAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<DateOnly?[]>(stream, dialect, options);

        await writer.WriteRecordAsync([new DateOnly(2024, 1, 2), null, new DateOnly(2025, 12, 31)]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "03.02.0001^^04.05.0006$")]
    [DataRow("~$", '\\', "03.02.0001^^04.05.0006~$")]
    public void WriteRecordWritesNullableDateOnlyArrayWithConverterFromOptions(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<DateOnly?[]>(stream, dialect, options);

        writer.WriteRecord([new DateOnly(1, 2, 3), null, new DateOnly(6, 5, 4)]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "03.02.0001^^04.05.0006$")]
    [DataRow("~$", '\\', "03.02.0001^^04.05.0006~$")]
    public async Task WriteRecordWritesNullableDateOnlyArrayWithConverterFromOptionsAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<DateOnly?[]>(stream, dialect, options);

        await writer.WriteRecordAsync([new DateOnly(1, 2, 3), null, new DateOnly(6, 5, 4)]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "urn:a^^../b$")]
    [DataRow("~$", '\\', "urn:a^^../b~$")]
    public void WriteRecordWritesUriArray(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<Uri?[]>(stream, dialect, options);

        writer.WriteRecord([new Uri("urn:a"), null, new Uri("../b", UriKind.Relative)]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "urn:a^^../b$")]
    [DataRow("~$", '\\', "urn:a^^../b~$")]
    public async Task WriteRecordWritesUriArrayAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<Uri?[]>(stream, dialect, options);

        await writer.WriteRecordAsync([new Uri("urn:a"), null, new Uri("../b", UriKind.Relative)]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordRoundTripsStringArray(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };
        var record = new string?[] { "a", null, "b^c", "d'e", "f$g", "h~$i", "j~k" };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        writer.WriteRecord(record);
        writer.Flush();
        stream.Position = 0;

        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual(record, reader.CurrentRecord);
        Assert.IsFalse(reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordRoundTripsStringArrayAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };
        var record = new string?[] { "a", null, "b^c", "d'e", "f$g", "h~$i", "j~k" };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<string?[]>(stream, dialect, options);

        await writer.WriteRecordAsync(record);
        await writer.FlushAsync();

        stream.Position = 0;

        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual(record, reader.CurrentRecord);
        Assert.IsFalse(await reader.TryReadRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordRoundTripsNullableInt32Array(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };
        var record = new int?[] { int.MinValue, null, 0, int.MaxValue };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<int?[]>(stream, dialect, options);

        writer.WriteRecord(record);
        writer.Flush();
        stream.Position = 0;

        using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual(record, reader.CurrentRecord);
        Assert.IsFalse(reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordRoundTripsNullableInt32ArrayAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };
        var record = new int?[] { int.MinValue, null, 0, int.MaxValue };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<int?[]>(stream, dialect, options);

        await writer.WriteRecordAsync(record);
        await writer.FlushAsync();

        stream.Position = 0;

        await using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual(record, reader.CurrentRecord);
        Assert.IsFalse(await reader.TryReadRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public void WriteRecordRoundTripsDateOnlyArray(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };
        var record = new DateOnly[] { DateOnly.MinValue, new(2024, 2, 29), DateOnly.MaxValue };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<DateOnly[]>(stream, dialect, options);

        writer.WriteRecord(record);
        writer.Flush();
        stream.Position = 0;

        using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual(record, reader.CurrentRecord);
        Assert.IsFalse(reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'')]
    [DataRow("~$", '\\')]
    public async Task WriteRecordRoundTripsDateOnlyArrayAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };
        var record = new DateOnly[] { DateOnly.MinValue, new(2024, 2, 29), DateOnly.MaxValue };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<DateOnly[]>(stream, dialect, options);

        await writer.WriteRecordAsync(record);
        await writer.FlushAsync();

        stream.Position = 0;

        await using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual(record, reader.CurrentRecord);
        Assert.IsFalse(await reader.TryReadRecordAsync());
    }

    private static TabularDialect CreateDialect(string lineTerminator, char escapeChar)
    {
        return new(lineTerminator, '^', '\'', escapeChar);
    }
}
