using System.Text;
using Addax.Formats.Tabular.Converters;
using Addax.Formats.Tabular.UnitTests.Handlers;
using Addax.Formats.Tabular.UnitTests.Records;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularRecordReaderTests
{
    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$", "a|b/c|d")]
    [DataRow("$", '\\', "a^b$c^d", "a|b/c|d")]
    [DataRow("~$", '\'', "a~$", "a")]
    [DataRow("~$", '\\', "a^b~$c^d~$", "a|b/c|d")]
    public void TryReadRecordReadsEveryRecord(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryReadRecord())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$", "a|b/c|d")]
    [DataRow("$", '\\', "a^b$c^d", "a|b/c|d")]
    [DataRow("~$", '\'', "a~$", "a")]
    [DataRow("~$", '\\', "a^b~$c^d~$", "a|b/c|d")]
    public async Task TryReadRecordReadsEveryRecordAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryReadRecordAsync())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b^c$d$e^f$", "a|b|c/d/e|f")]
    [DataRow("~$", '\\', "a~$b^c^d^e~$f^g", "a/b|c|d|e/f|g")]
    public void TryReadRecordReadsRecordsWithDifferentFieldCounts(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryReadRecord())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b^c$d$e^f$", "a|b|c/d/e|f")]
    [DataRow("~$", '\\', "a~$b^c^d^e~$f^g", "a/b|c|d|e/f|g")]
    public async Task TryReadRecordReadsRecordsWithDifferentFieldCountsAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryReadRecordAsync())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$", "c|d")]
    [DataRow("~$", '\\', "a^b~$c^d", "c|d")]
    public void TrySkipRecordSkipsOneRecord(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TrySkipRecord());
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$", "c|d")]
    [DataRow("~$", '\\', "a^b~$c^d", "c|d")]
    public async Task TrySkipRecordSkipsOneRecordAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TrySkipRecordAsync());
        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "", 0)]
    [DataRow("$", '\\', "a^b$c", 2)]
    [DataRow("~$", '\\', "a^b~$", 1)]
    public void TrySkipRecordSkipsEveryRecord(string lineTerminator, char escapeChar, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var count = 0;

        while (reader.TrySkipRecord())
        {
            count++;
        }

        Assert.AreEqual(expected, count);
    }

    [TestMethod]
    [DataRow("$", '\'', "", 0)]
    [DataRow("$", '\\', "a^b$c", 2)]
    [DataRow("~$", '\\', "a^b~$", 1)]
    public async Task TrySkipRecordSkipsEveryRecordAsync(string lineTerminator, char escapeChar, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var count = 0;

        while (await reader.TrySkipRecordAsync())
        {
            count++;
        }

        Assert.AreEqual(expected, count);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public void CurrentRecordThrowsInvalidOperationExceptionBeforeTryReadRecord(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect);

        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public void CurrentRecordThrowsInvalidOperationExceptionAfterLastRecord(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.IsFalse(reader.TryReadRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public async Task CurrentRecordThrowsInvalidOperationExceptionAfterLastRecordAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.IsFalse(await reader.TryReadRecordAsync());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$b$")]
    [DataRow("~$", '\\', "a~$b~$")]
    public void CurrentRecordThrowsInvalidOperationExceptionAfterTrySkipRecord(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.IsTrue(reader.TrySkipRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$b$")]
    [DataRow("~$", '\\', "a~$b~$")]
    public async Task CurrentRecordThrowsInvalidOperationExceptionAfterTrySkipRecordAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.IsTrue(await reader.TrySkipRecordAsync());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$")]
    [DataRow("~$", '\\', "a^b~$")]
    public void CurrentRecordRefReturnsCurrentRecord(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSame(reader.CurrentRecord, reader.CurrentRecordRef);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$")]
    [DataRow("~$", '\\', "a^b~$")]
    public async Task CurrentRecordRefReturnsCurrentRecordAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSame(reader.CurrentRecord, reader.CurrentRecordRef);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public void CurrentRecordRefThrowsInvalidOperationExceptionBeforeTryReadRecord(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect);

        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecordRef);
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$v3^v4$", "v1|v2/v3|v4")]
    [DataRow("~$", '\\', "h1^h2~$v1^v2~$v3^v4", "v1|v2/v3|v4")]
    public void TryReadRecordSkipsHeader(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryReadRecord())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$v3^v4$", "v1|v2/v3|v4")]
    [DataRow("~$", '\\', "h1^h2~$v1^v2~$v3^v4", "v1|v2/v3|v4")]
    public async Task TryReadRecordSkipsHeaderAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryReadRecordAsync())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$h1^h2$v1^v2$", "v1|v2")]
    [DataRow("~$", '\\', "!x~$!y~$h1^h2~$!z~$v1^v2~$", "v1|v2")]
    public void TryReadRecordSkipsHeaderAfterComments(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryReadRecord())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$h1^h2$v1^v2$", "v1|v2")]
    [DataRow("~$", '\\', "!x~$!y~$h1^h2~$!z~$v1^v2~$", "v1|v2")]
    public async Task TryReadRecordSkipsHeaderAfterCommentsAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryReadRecordAsync())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "")]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "!x~$h1^h2~$!y~$")]
    [DataRow("~$", '\\', "h1^h2")]
    public void TryReadRecordReturnsFalseWithoutRecordsAfterHeader(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsFalse(reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "")]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "!x~$h1^h2~$!y~$")]
    [DataRow("~$", '\\', "h1^h2")]
    public async Task TryReadRecordReturnsFalseWithoutRecordsAfterHeaderAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsFalse(await reader.TryReadRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$v3^v4$", "v3|v4")]
    [DataRow("~$", '\\', "h1^h2~$v1^v2~$v3^v4", "v3|v4")]
    public void TrySkipRecordSkipsHeader(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsTrue(reader.TrySkipRecord());
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$v3^v4$", "v3|v4")]
    [DataRow("~$", '\\', "h1^h2~$v1^v2~$v3^v4", "v3|v4")]
    public async Task TrySkipRecordSkipsHeaderAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TrySkipRecordAsync());
        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "!x~$h1^h2")]
    public void TrySkipRecordReturnsFalseWithoutRecordsAfterHeader(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsFalse(reader.TrySkipRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$")]
    [DataRow("~$", '\\', "!x~$h1^h2")]
    public async Task TrySkipRecordReturnsFalseWithoutRecordsAfterHeaderAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsFalse(await reader.TrySkipRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$a^b$!y$c^d$!z", "a|b/c|d")]
    [DataRow("~$", '\\', "!x~$!y~$a^b~$!z~$c^d~$", "a|b/c|d")]
    public void TryReadRecordSkipsComments(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryReadRecord())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$a^b$!y$c^d$!z", "a|b/c|d")]
    [DataRow("~$", '\\', "!x~$!y~$a^b~$!z~$c^d~$", "a|b/c|d")]
    public async Task TryReadRecordSkipsCommentsAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryReadRecordAsync())
        {
            records.Add(string.Join('|', reader.CurrentRecord));
        }

        Assert.AreEqual(expected, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$a^b$!y$c^d$!z", "c|d")]
    [DataRow("~$", '\\', "!x~$!y~$a^b~$!z~$c^d~$", "c|d")]
    public void TrySkipRecordSkipsComments(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TrySkipRecord());
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$a^b$!y$c^d$!z", "c|d")]
    [DataRow("~$", '\\', "!x~$!y~$a^b~$!z~$c^d~$", "c|d")]
    public async Task TrySkipRecordSkipsCommentsAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TrySkipRecordAsync());
        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$!y")]
    [DataRow("~$", '\\', "!x~$!y~$")]
    public void TrySkipRecordReturnsFalseWhenOnlyCommentsRemain(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsFalse(reader.TrySkipRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$!y")]
    [DataRow("~$", '\\', "!x~$!y~$")]
    public async Task TrySkipRecordReturnsFalseWhenOnlyCommentsRemainAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsFalse(await reader.TrySkipRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$a^b$!y$c^d$!z")]
    [DataRow("~$", '\\', "!x~$!y~$a^b~$!z~$c^d~$!w~$")]
    public void RecordsReadExcludesComments(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(1, reader.RecordsRead);
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(2, reader.RecordsRead);
        Assert.IsFalse(reader.TryReadRecord());
        Assert.AreEqual(2, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "!x$a^b$!y$c^d$!z")]
    [DataRow("~$", '\\', "!x~$!y~$a^b~$!z~$c^d~$!w~$")]
    public async Task RecordsReadExcludesCommentsAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(1, reader.RecordsRead);
        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(2, reader.RecordsRead);
        Assert.IsFalse(await reader.TryReadRecordAsync());
        Assert.AreEqual(2, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$v3^v4$")]
    [DataRow("~$", '\\', "!x~$h1^h2~$v1^v2~$!y~$v3^v4")]
    public void RecordsReadIncludesHeader(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(2, reader.RecordsRead);
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(3, reader.RecordsRead);
        Assert.IsFalse(reader.TryReadRecord());
        Assert.AreEqual(3, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$v3^v4$")]
    [DataRow("~$", '\\', "!x~$h1^h2~$v1^v2~$!y~$v3^v4")]
    public async Task RecordsReadIncludesHeaderAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(2, reader.RecordsRead);
        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(3, reader.RecordsRead);
        Assert.IsFalse(await reader.TryReadRecordAsync());
        Assert.AreEqual(3, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$")]
    [DataRow("~$", '\\', "!x~$h1^h2~$!y~$v1^v2")]
    public void RecordsReadIncludesSkippedHeader(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsTrue(reader.TrySkipRecord());
        Assert.AreEqual(2, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "h1^h2$v1^v2$")]
    [DataRow("~$", '\\', "!x~$h1^h2~$!y~$v1^v2")]
    public async Task RecordsReadIncludesSkippedHeaderAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new HeaderHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TrySkipRecordAsync());
        Assert.AreEqual(2, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$")]
    [DataRow("~$", '\\', "a^b~$c^d")]
    public void TryReadRecordWithFieldlessHandlerAdvancesOneRecord(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string)] = new FieldlessHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(1, reader.RecordsRead);
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(2, reader.RecordsRead);
        Assert.IsFalse(reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$")]
    [DataRow("~$", '\\', "a^b~$c^d")]
    public async Task TryReadRecordWithFieldlessHandlerAdvancesOneRecordAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string)] = new FieldlessHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(1, reader.RecordsRead);
        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(2, reader.RecordsRead);
        Assert.IsFalse(await reader.TryReadRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a$")]
    [DataRow("~$", '\\', "a^?~$")]
    public void TryReadRecordThrowsHandlerException(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a$")]
    [DataRow("~$", '\\', "a^?~$")]
    public async Task TryReadRecordThrowsHandlerExceptionAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a^b$c^d$", "c|d")]
    [DataRow("$", '\\', "a^b^?$c^d", "c|d")]
    [DataRow("~$", '\\', "a^?^b~$c^d~$", "c|d")]
    public void TryReadRecordAfterHandlerExceptionReadsNextRecord(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => reader.TryReadRecord());
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a^b$c^d$", "c|d")]
    [DataRow("$", '\\', "a^b^?$c^d", "c|d")]
    [DataRow("~$", '\\', "a^?^b~$c^d~$", "c|d")]
    public async Task TryReadRecordAfterHandlerExceptionReadsNextRecordAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => reader.TryReadRecordAsync().AsTask());

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(expected, string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a$")]
    [DataRow("~$", '\\', "a^?~$")]
    public void CurrentRecordThrowsInvalidOperationExceptionAfterHandlerException(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => reader.TryReadRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a$")]
    [DataRow("~$", '\\', "a^?~$")]
    public async Task CurrentRecordThrowsInvalidOperationExceptionAfterHandlerExceptionAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => reader.TryReadRecordAsync().AsTask());

        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a^b$c^d$")]
    [DataRow("$", '\\', "a^b^?$c^d")]
    [DataRow("~$", '\\', "a^?^b~$c^d~$")]
    public void RecordsReadCountsRecordWithHandlerException(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<FormatException>(() => reader.TryReadRecord());
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual(2, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "?^a^b$c^d$")]
    [DataRow("$", '\\', "a^b^?$c^d")]
    [DataRow("~$", '\\', "a^?^b~$c^d~$")]
    public async Task RecordsReadCountsRecordWithHandlerExceptionAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        options.Handlers[typeof(string[])] = new FailingHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<FormatException>(() => reader.TryReadRecordAsync().AsTask());

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual(2, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$'c'd^e$f$")]
    [DataRow("~$", '\\', "a^b~$'c'd^e~$f~$")]
    public void TryReadRecordThrowsInvalidOperationExceptionAfterContentException(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.ThrowsExactly<TabularContentException>(() => reader.TryReadRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$'c'd^e$f$")]
    [DataRow("~$", '\\', "a^b~$'c'd^e~$f~$")]
    public async Task TryReadRecordThrowsInvalidOperationExceptionAfterContentExceptionAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());

        await Assert.ThrowsExactlyAsync<TabularContentException>(() => reader.TryReadRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$'c'd^e$f$")]
    [DataRow("~$", '\\', "a^b~$'c'd^e~$f~$")]
    public void TrySkipRecordThrowsInvalidOperationExceptionAfterContentException(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.ThrowsExactly<TabularContentException>(() => reader.TryReadRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TrySkipRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$'c'd^e$f$")]
    [DataRow("~$", '\\', "a^b~$'c'd^e~$f~$")]
    public async Task TrySkipRecordThrowsInvalidOperationExceptionAfterContentExceptionAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());

        await Assert.ThrowsExactlyAsync<TabularContentException>(() => reader.TryReadRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TrySkipRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$'c'd^e$f$")]
    [DataRow("~$", '\\', "a^b~$'c'd^e~$f~$")]
    public void CurrentRecordThrowsInvalidOperationExceptionAfterContentException(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.ThrowsExactly<TabularContentException>(() => reader.TryReadRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$'c'd^e$f$")]
    [DataRow("~$", '\\', "a^b~$'c'd^e~$f~$")]
    public async Task CurrentRecordThrowsInvalidOperationExceptionAfterContentExceptionAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());

        await Assert.ThrowsExactlyAsync<TabularContentException>(() => reader.TryReadRecordAsync().AsTask());

        Assert.ThrowsExactly<InvalidOperationException>(() => reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$")]
    [DataRow("~$", '\\', "a^b~$c^d~$")]
    public async Task TryReadRecordThrowsOperationCanceledExceptionForCanceledTokenAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => reader.TryReadRecordAsync(new CancellationToken(true)).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$")]
    [DataRow("~$", '\\', "a^b~$c^d~$")]
    public async Task TrySkipRecordThrowsOperationCanceledExceptionForCanceledTokenAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => reader.TrySkipRecordAsync(new CancellationToken(true)).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$")]
    [DataRow("~$", '\\', "a^b~$c^d~$")]
    public async Task TryReadRecordThrowsInvalidOperationExceptionAfterCanceledReadAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => reader.TryReadRecordAsync(new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$", 1)]
    [DataRow("$", '\'', "a^b$c^d$", 16384)]
    [DataRow("~$", '\\', "a^b~$c^d", 1)]
    [DataRow("~$", '\\', "a^b~$c^d", 16384)]
    public void BytesConsumedCountsStreamBytesAfterReading(string lineTerminator, char escapeChar, string content, int bufferSize)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        while (reader.TryReadRecord())
        {
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^b$c^d$", 1)]
    [DataRow("$", '\'', "a^b$c^d$", 16384)]
    [DataRow("~$", '\\', "a^b~$c^d", 1)]
    [DataRow("~$", '\\', "a^b~$c^d", 16384)]
    public async Task BytesConsumedCountsStreamBytesAfterReadingAsync(string lineTerminator, char escapeChar, string content, int bufferSize)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        while (await reader.TryReadRecordAsync())
        {
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public void TryReadRecordThrowsObjectDisposedExceptionAfterDispose(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var reader = new TabularReader<string?[]>(stream, dialect, options);

        reader.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public async Task TryReadRecordThrowsObjectDisposedExceptionAfterDisposeAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var reader = new TabularReader<string?[]>(stream, dialect, options);

        await reader.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public void TrySkipRecordThrowsObjectDisposedExceptionAfterDispose(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var reader = new TabularReader<string?[]>(stream, dialect, options);

        reader.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TrySkipRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public async Task TrySkipRecordThrowsObjectDisposedExceptionAfterDisposeAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var reader = new TabularReader<string?[]>(stream, dialect, options);

        await reader.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => reader.TrySkipRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "a$b$")]
    [DataRow("~$", '\\', "a~$b~$")]
    public void DisposeLeavesStreamOpen(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var reader = new TabularReader<string?[]>(stream, dialect, options);

        reader.TryReadRecord();
        reader.Dispose();

        Assert.IsTrue(stream.CanRead);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$b$")]
    [DataRow("~$", '\\', "a~$b~$")]
    public async Task DisposeLeavesStreamOpenAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var reader = new TabularReader<string?[]>(stream, dialect, options);

        await reader.TryReadRecordAsync();
        await reader.DisposeAsync();

        Assert.IsTrue(stream.CanRead);
    }

    [TestMethod]
    public void ConstructorThrowsInvalidOperationExceptionForTypeWithoutHandler()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<InvalidOperationException>(() => new TabularReader<UnregisteredRecord>(stream, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullStream()
    {
        var dialect = CreateDialect("$", '\'');

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularReader<string?[]>(null!, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullDialect()
    {
        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularReader<string?[]>(stream, null!));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForUnreadableStream()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream();

        stream.Dispose();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularReader<string?[]>(stream, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForHandlerOfOtherType()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = TabularRegistry.Handlers[typeof(int[])];

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularReader<string[]>(stream, dialect, options));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForNullHandler()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = null!;

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularReader<string[]>(stream, dialect, options));
    }

    [TestMethod]
    [DataRow("$", '\'', "a^^b$")]
    [DataRow("~$", '\\', "a^^b")]
    public void TryReadRecordReadsEmptyStringFieldAsNull(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual(["a", null, "b"], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^^b$")]
    [DataRow("~$", '\\', "a^^b")]
    public async Task TryReadRecordReadsEmptyStringFieldAsNullAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual(["a", null, "b"], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^  ^b$")]
    [DataRow("~$", '\\', "a^  ^b~$")]
    public void TryReadRecordWithTrimWhitespaceReadsWhitespaceStringFieldAsNull(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual(["a", null, "b"], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^  ^b$")]
    [DataRow("~$", '\\', "a^  ^b~$")]
    public async Task TryReadRecordWithTrimWhitespaceReadsWhitespaceStringFieldAsNullAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual(["a", null, "b"], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^  ^b$")]
    [DataRow("~$", '\\', "a^  ^b~$")]
    public void TryReadRecordWithoutTrimWhitespaceReadsWhitespaceStringField(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual(["a", "  ", "b"], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a^  ^b$")]
    [DataRow("~$", '\\', "a^  ^b~$")]
    public async Task TryReadRecordWithoutTrimWhitespaceReadsWhitespaceStringFieldAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual(["a", "  ", "b"], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "1^-2^3$")]
    [DataRow("~$", '\\', "1^-2^3")]
    public void TryReadRecordReadsInt32Array(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<int[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual([1, -2, 3], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "1^-2^3$")]
    [DataRow("~$", '\\', "1^-2^3")]
    public async Task TryReadRecordReadsInt32ArrayAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<int[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual([1, -2, 3], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "1^^3$")]
    [DataRow("~$", '\\', "1^^3")]
    public void TryReadRecordReadsEmptyNullableInt32FieldAsNull(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual([1, null, 3], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "1^^3$")]
    [DataRow("~$", '\\', "1^^3")]
    public async Task TryReadRecordReadsEmptyNullableInt32FieldAsNullAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual([1, null, 3], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02^2025-12-31$")]
    [DataRow("~$", '\\', "2024-01-02^2025-12-31")]
    public void TryReadRecordReadsDateOnlyArray(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual([new(2024, 1, 2), new(2025, 12, 31)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02^2025-12-31$")]
    [DataRow("~$", '\\', "2024-01-02^2025-12-31")]
    public async Task TryReadRecordReadsDateOnlyArrayAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual([new(2024, 1, 2), new(2025, 12, 31)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "03.02.0001^04.05.0006$")]
    [DataRow("~$", '\\', "03.02.0001^04.05.0006")]
    public void TryReadRecordReadsDateOnlyArrayWithConverterFromOptions(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual([new(1, 2, 3), new(6, 5, 4)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "03.02.0001^04.05.0006$")]
    [DataRow("~$", '\\', "03.02.0001^04.05.0006")]
    public async Task TryReadRecordReadsDateOnlyArrayWithConverterFromOptionsAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual([new(1, 2, 3), new(6, 5, 4)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "^2024-01-02$")]
    [DataRow("~$", '\\', "^2024-01-02")]
    public void TryReadRecordReadsEmptyNullableDateOnlyFieldAsNull(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<DateOnly?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual([null, new DateOnly(2024, 1, 2)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "^2024-01-02$")]
    [DataRow("~$", '\\', "^2024-01-02")]
    public async Task TryReadRecordReadsEmptyNullableDateOnlyFieldAsNullAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<DateOnly?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual([null, new DateOnly(2024, 1, 2)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "^urn:a^../b$")]
    [DataRow("~$", '\\', "^urn:a^../b")]
    public void TryReadRecordReadsUriArray(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<Uri?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual([null, new Uri("urn:a"), new Uri("../b", UriKind.Relative)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "^urn:a^../b$")]
    [DataRow("~$", '\\', "^urn:a^../b")]
    public async Task TryReadRecordReadsUriArrayAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<Uri?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual([null, new Uri("urn:a"), new Uri("../b", UriKind.Relative)], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "1^2$x^4$")]
    [DataRow("$", '\\', "1^2$3^^5$")]
    [DataRow("~$", '\'', "1^2~$3^ 4~$")]
    [DataRow("~$", '\\', "1^2~$3^x~$")]
    public void TryReadRecordThrowsTabularExceptionForInvalidInt32Value(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<int[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.ThrowsExactly<TabularException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "1^2$x^4$")]
    [DataRow("$", '\\', "1^2$3^^5$")]
    [DataRow("~$", '\'', "1^2~$3^ 4~$")]
    [DataRow("~$", '\\', "1^2~$3^x~$")]
    public async Task TryReadRecordThrowsTabularExceptionForInvalidInt32ValueAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<int[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());

        await Assert.ThrowsExactlyAsync<TabularException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "1^2$^x$")]
    [DataRow("$", '\\', "1^2$3^1.5$")]
    [DataRow("~$", '\\', "1^2~$x^~$")]
    public void TryReadRecordThrowsTabularExceptionForInvalidNullableInt32Value(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.ThrowsExactly<TabularException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "1^2$^x$")]
    [DataRow("$", '\\', "1^2$3^1.5$")]
    [DataRow("~$", '\\', "1^2~$x^~$")]
    public async Task TryReadRecordThrowsTabularExceptionForInvalidNullableInt32ValueAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());

        await Assert.ThrowsExactlyAsync<TabularException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02$2024-13-01$")]
    [DataRow("~$", '\\', "2024-01-02~$2024-01-03^~$")]
    public void TryReadRecordThrowsTabularExceptionForInvalidDateOnlyValue(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.ThrowsExactly<TabularException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02$2024-13-01$")]
    [DataRow("~$", '\\', "2024-01-02~$2024-01-03^~$")]
    public async Task TryReadRecordThrowsTabularExceptionForInvalidDateOnlyValueAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<DateOnly[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());

        await Assert.ThrowsExactlyAsync<TabularException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02$^02.01.2024$")]
    [DataRow("~$", '\\', "2024-01-02~$2024-01-02T00:00:00^~$")]
    public void TryReadRecordThrowsTabularExceptionForInvalidNullableDateOnlyValue(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<DateOnly?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.ThrowsExactly<TabularException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "2024-01-02$^02.01.2024$")]
    [DataRow("~$", '\\', "2024-01-02~$2024-01-02T00:00:00^~$")]
    public async Task TryReadRecordThrowsTabularExceptionForInvalidNullableDateOnlyValueAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<DateOnly?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());

        await Assert.ThrowsExactlyAsync<TabularException>(() => reader.TryReadRecordAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\'', "1^x$3^4$")]
    [DataRow("~$", '\\', "x^2^3~$3^4")]
    public void TryReadRecordAfterInvalidValueReadsNextRecord(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<int[]>(stream, dialect, options);

        Assert.ThrowsExactly<TabularException>(() => reader.TryReadRecord());
        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual("3|4", string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', "1^x$3^4$")]
    [DataRow("~$", '\\', "x^2^3~$3^4")]
    public async Task TryReadRecordAfterInvalidValueReadsNextRecordAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<int[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<TabularException>(() => reader.TryReadRecordAsync().AsTask());

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual("3|4", string.Join('|', reader.CurrentRecord));
    }

    [TestMethod]
    [DataRow("$", '\'', " 1 ^  ^3$")]
    [DataRow("~$", '\\', "1^  ^ 3~$")]
    public void TryReadRecordWithTrimWhitespaceReadsNullableInt32Array(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreSequenceEqual([1, null, 3], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', " 1 ^  ^3$")]
    [DataRow("~$", '\\', "1^  ^ 3~$")]
    public async Task TryReadRecordWithTrimWhitespaceReadsNullableInt32ArrayAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<int?[]>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreSequenceEqual([1, null, 3], reader.CurrentRecord);
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public void TryReadRecordThrowsNotSupportedExceptionForHandlerWithoutOverride(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new AsyncOnlyHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader<string[]>(stream, dialect, options);

        Assert.ThrowsExactly<NotSupportedException>(() => reader.TryReadRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "a$")]
    [DataRow("~$", '\\', "a~$")]
    public async Task TryReadRecordThrowsNotSupportedExceptionForHandlerWithoutOverrideAsync(string lineTerminator, char escapeChar, string content)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions();

        options.Handlers[typeof(string[])] = new SyncOnlyHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader<string[]>(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => reader.TryReadRecordAsync().AsTask());
    }

    private static TabularDialect CreateDialect(string lineTerminator, char escapeChar)
    {
        return new(lineTerminator, '^', '\'', escapeChar);
    }

    private static TabularDialect CreateDialect(string lineTerminator, char escapeChar, char commentPrefix)
    {
        return new(lineTerminator, '^', '\'', escapeChar, commentPrefix);
    }
}
