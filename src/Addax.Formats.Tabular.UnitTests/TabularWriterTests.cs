using System.Text;
using Addax.Formats.Tabular.UnitTests.Streams;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularWriterTests
{
    [TestMethod]
    [DataRow("$", '\'', "", "$")]
    [DataRow("$", '\'', "/", "$$")]
    [DataRow("$", '\'', "v!", "v!$")]
    [DataRow("$", '\'', "v", "v$")]
    [DataRow("$", '\'', "v/", "v$$")]
    [DataRow("$", '\'', "v|", "v^$")]
    [DataRow("$", '\'', "|", "^$")]
    [DataRow("$", '\\', "", "$")]
    [DataRow("$", '\\', "/", "$$")]
    [DataRow("$", '\\', "\\!", "\\!$")]
    [DataRow("$", '\\', "\\", "\\$")]
    [DataRow("$", '\\', "\\/", "\\$$")]
    [DataRow("$", '\\', "\\|", "\\^$")]
    [DataRow("$", '\\', "v!", "v!$")]
    [DataRow("$", '\\', "v", "v$")]
    [DataRow("$", '\\', "v/", "v$$")]
    [DataRow("$", '\\', "v|", "v^$")]
    [DataRow("$", '\\', "v|v", "v^v$")]
    [DataRow("$", '\\', "|", "^$")]
    [DataRow("~$", '\'', "", "~$")]
    [DataRow("~$", '\'', "/", "~$~$")]
    [DataRow("~$", '\'', "v!", "v!~$")]
    [DataRow("~$", '\'', "v", "v~$")]
    [DataRow("~$", '\'', "v/", "v~$~$")]
    [DataRow("~$", '\'', "v|", "v^~$")]
    [DataRow("~$", '\'', "v|v", "v^v~$")]
    [DataRow("~$", '\'', "|", "^~$")]
    [DataRow("~$", '\\', "", "~$")]
    [DataRow("~$", '\\', "/", "~$~$")]
    [DataRow("~$", '\\', "\\!", "\\!~$")]
    [DataRow("~$", '\\', "\\", "\\~$")]
    [DataRow("~$", '\\', "\\/", "\\~$~$")]
    [DataRow("~$", '\\', "\\|", "\\^~$")]
    [DataRow("~$", '\\', "v!", "v!~$")]
    [DataRow("~$", '\\', "v", "v~$")]
    [DataRow("~$", '\\', "v/", "v~$~$")]
    [DataRow("~$", '\\', "v|", "v^~$")]
    [DataRow("~$", '\\', "|", "^~$")]
    public void WriteStringWritesPlainValueUnquoted(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "", "$")]
    [DataRow("$", '\'', "/", "$$")]
    [DataRow("$", '\'', "v!", "v!$")]
    [DataRow("$", '\'', "v", "v$")]
    [DataRow("$", '\'', "v/", "v$$")]
    [DataRow("$", '\'', "v|", "v^$")]
    [DataRow("$", '\'', "|", "^$")]
    [DataRow("$", '\\', "", "$")]
    [DataRow("$", '\\', "/", "$$")]
    [DataRow("$", '\\', "\\!", "\\!$")]
    [DataRow("$", '\\', "\\", "\\$")]
    [DataRow("$", '\\', "\\/", "\\$$")]
    [DataRow("$", '\\', "\\|", "\\^$")]
    [DataRow("$", '\\', "v!", "v!$")]
    [DataRow("$", '\\', "v", "v$")]
    [DataRow("$", '\\', "v/", "v$$")]
    [DataRow("$", '\\', "v|", "v^$")]
    [DataRow("$", '\\', "v|v", "v^v$")]
    [DataRow("$", '\\', "|", "^$")]
    [DataRow("~$", '\'', "", "~$")]
    [DataRow("~$", '\'', "/", "~$~$")]
    [DataRow("~$", '\'', "v!", "v!~$")]
    [DataRow("~$", '\'', "v", "v~$")]
    [DataRow("~$", '\'', "v/", "v~$~$")]
    [DataRow("~$", '\'', "v|", "v^~$")]
    [DataRow("~$", '\'', "v|v", "v^v~$")]
    [DataRow("~$", '\'', "|", "^~$")]
    [DataRow("~$", '\\', "", "~$")]
    [DataRow("~$", '\\', "/", "~$~$")]
    [DataRow("~$", '\\', "\\!", "\\!~$")]
    [DataRow("~$", '\\', "\\", "\\~$")]
    [DataRow("~$", '\\', "\\/", "\\~$~$")]
    [DataRow("~$", '\\', "\\|", "\\^~$")]
    [DataRow("~$", '\\', "v!", "v!~$")]
    [DataRow("~$", '\\', "v", "v~$")]
    [DataRow("~$", '\\', "v/", "v~$~$")]
    [DataRow("~$", '\\', "v|", "v^~$")]
    [DataRow("~$", '\\', "|", "^~$")]
    public async Task WriteStringWritesPlainValueUnquotedAsync(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "$", "'$'$")]
    [DataRow("$", '\'', "$/", "'$'$$")]
    [DataRow("$", '\'', "$|", "'$'^$")]
    [DataRow("$", '\'', "^!", "'^!'$")]
    [DataRow("$", '\'', "^", "'^'$")]
    [DataRow("$", '\'', "^/", "'^'$$")]
    [DataRow("$", '\'', "^|", "'^'^$")]
    [DataRow("$", '\'', "v$", "'v$'$")]
    [DataRow("$", '\\', "$", "'$'$")]
    [DataRow("$", '\\', "$/", "'$'$$")]
    [DataRow("$", '\\', "$|", "'$'^$")]
    [DataRow("$", '\\', "^!", "'^!'$")]
    [DataRow("$", '\\', "^", "'^'$")]
    [DataRow("$", '\\', "^/", "'^'$$")]
    [DataRow("$", '\\', "^|", "'^'^$")]
    [DataRow("$", '\\', "v$", "'v$'$")]
    [DataRow("$", '\\', "v^w", "'v^w'$")]
    [DataRow("~$", '\'', "^!", "'^!'~$")]
    [DataRow("~$", '\'', "^", "'^'~$")]
    [DataRow("~$", '\'', "^/", "'^'~$~$")]
    [DataRow("~$", '\'', "^|", "'^'^~$")]
    [DataRow("~$", '\'', "v^w", "'v^w'~$")]
    [DataRow("~$", '\'', "v~$", "'v~$'~$")]
    [DataRow("~$", '\'', "~$", "'~$'~$")]
    [DataRow("~$", '\'', "~$/", "'~$'~$~$")]
    [DataRow("~$", '\'', "~$|", "'~$'^~$")]
    [DataRow("~$", '\\', "^!", "'^!'~$")]
    [DataRow("~$", '\\', "^", "'^'~$")]
    [DataRow("~$", '\\', "^/", "'^'~$~$")]
    [DataRow("~$", '\\', "^|", "'^'^~$")]
    [DataRow("~$", '\\', "v~$", "'v~$'~$")]
    [DataRow("~$", '\\', "~$", "'~$'~$")]
    [DataRow("~$", '\\', "~$/", "'~$'~$~$")]
    [DataRow("~$", '\\', "~$|", "'~$'^~$")]
    public void WriteStringQuotesValueWithDelimiterOrLineTerminator(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "$", "'$'$")]
    [DataRow("$", '\'', "$/", "'$'$$")]
    [DataRow("$", '\'', "$|", "'$'^$")]
    [DataRow("$", '\'', "^!", "'^!'$")]
    [DataRow("$", '\'', "^", "'^'$")]
    [DataRow("$", '\'', "^/", "'^'$$")]
    [DataRow("$", '\'', "^|", "'^'^$")]
    [DataRow("$", '\'', "v$", "'v$'$")]
    [DataRow("$", '\\', "$", "'$'$")]
    [DataRow("$", '\\', "$/", "'$'$$")]
    [DataRow("$", '\\', "$|", "'$'^$")]
    [DataRow("$", '\\', "^!", "'^!'$")]
    [DataRow("$", '\\', "^", "'^'$")]
    [DataRow("$", '\\', "^/", "'^'$$")]
    [DataRow("$", '\\', "^|", "'^'^$")]
    [DataRow("$", '\\', "v$", "'v$'$")]
    [DataRow("$", '\\', "v^w", "'v^w'$")]
    [DataRow("~$", '\'', "^!", "'^!'~$")]
    [DataRow("~$", '\'', "^", "'^'~$")]
    [DataRow("~$", '\'', "^/", "'^'~$~$")]
    [DataRow("~$", '\'', "^|", "'^'^~$")]
    [DataRow("~$", '\'', "v^w", "'v^w'~$")]
    [DataRow("~$", '\'', "v~$", "'v~$'~$")]
    [DataRow("~$", '\'', "~$", "'~$'~$")]
    [DataRow("~$", '\'', "~$/", "'~$'~$~$")]
    [DataRow("~$", '\'', "~$|", "'~$'^~$")]
    [DataRow("~$", '\\', "^!", "'^!'~$")]
    [DataRow("~$", '\\', "^", "'^'~$")]
    [DataRow("~$", '\\', "^/", "'^'~$~$")]
    [DataRow("~$", '\\', "^|", "'^'^~$")]
    [DataRow("~$", '\\', "v~$", "'v~$'~$")]
    [DataRow("~$", '\\', "~$", "'~$'~$")]
    [DataRow("~$", '\\', "~$/", "'~$'~$~$")]
    [DataRow("~$", '\\', "~$|", "'~$'^~$")]
    public async Task WriteStringQuotesValueWithDelimiterOrLineTerminatorAsync(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "'!", "'''!'$")]
    [DataRow("$", '\'', "'", "''''$")]
    [DataRow("$", '\'', "'/", "''''$$")]
    [DataRow("$", '\'', "'|", "''''^$")]
    [DataRow("$", '\'', "v'w", "'v''w'$")]
    [DataRow("$", '\\', "'!", "'\\'!'$")]
    [DataRow("$", '\\', "'", "'\\''$")]
    [DataRow("$", '\\', "'/", "'\\''$$")]
    [DataRow("$", '\\', "'\\", "'\\'\\\\'$")]
    [DataRow("$", '\\', "'\\/", "'\\'\\\\'$$")]
    [DataRow("$", '\\', "'\\|", "'\\'\\\\'^$")]
    [DataRow("$", '\\', "'|", "'\\''^$")]
    [DataRow("$", '\\', "v'w", "'v\\'w'$")]
    [DataRow("~$", '\'', "'!", "'''!'~$")]
    [DataRow("~$", '\'', "'", "''''~$")]
    [DataRow("~$", '\'', "'/", "''''~$~$")]
    [DataRow("~$", '\'', "'|", "''''^~$")]
    [DataRow("~$", '\'', "v'w", "'v''w'~$")]
    [DataRow("~$", '\\', "'!", "'\\'!'~$")]
    [DataRow("~$", '\\', "'", "'\\''~$")]
    [DataRow("~$", '\\', "'/", "'\\''~$~$")]
    [DataRow("~$", '\\', "'\\", "'\\'\\\\'~$")]
    [DataRow("~$", '\\', "'\\/", "'\\'\\\\'~$~$")]
    [DataRow("~$", '\\', "'\\|", "'\\'\\\\'^~$")]
    [DataRow("~$", '\\', "'|", "'\\''^~$")]
    [DataRow("~$", '\\', "v'w", "'v\\'w'~$")]
    public void WriteStringEscapesQuoteAndEscapeChars(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "'!", "'''!'$")]
    [DataRow("$", '\'', "'", "''''$")]
    [DataRow("$", '\'', "'/", "''''$$")]
    [DataRow("$", '\'', "'|", "''''^$")]
    [DataRow("$", '\'', "v'w", "'v''w'$")]
    [DataRow("$", '\\', "'!", "'\\'!'$")]
    [DataRow("$", '\\', "'", "'\\''$")]
    [DataRow("$", '\\', "'/", "'\\''$$")]
    [DataRow("$", '\\', "'\\", "'\\'\\\\'$")]
    [DataRow("$", '\\', "'\\/", "'\\'\\\\'$$")]
    [DataRow("$", '\\', "'\\|", "'\\'\\\\'^$")]
    [DataRow("$", '\\', "'|", "'\\''^$")]
    [DataRow("$", '\\', "v'w", "'v\\'w'$")]
    [DataRow("~$", '\'', "'!", "'''!'~$")]
    [DataRow("~$", '\'', "'", "''''~$")]
    [DataRow("~$", '\'', "'/", "''''~$~$")]
    [DataRow("~$", '\'', "'|", "''''^~$")]
    [DataRow("~$", '\'', "v'w", "'v''w'~$")]
    [DataRow("~$", '\\', "'!", "'\\'!'~$")]
    [DataRow("~$", '\\', "'", "'\\''~$")]
    [DataRow("~$", '\\', "'/", "'\\''~$~$")]
    [DataRow("~$", '\\', "'\\", "'\\'\\\\'~$")]
    [DataRow("~$", '\\', "'\\/", "'\\'\\\\'~$~$")]
    [DataRow("~$", '\\', "'\\|", "'\\'\\\\'^~$")]
    [DataRow("~$", '\\', "'|", "'\\''^~$")]
    [DataRow("~$", '\\', "v'w", "'v\\'w'~$")]
    public async Task WriteStringEscapesQuoteAndEscapeCharsAsync(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "!", "'!'$")]
    [DataRow("$", '\'', "!'", "'!'''$")]
    [DataRow("$", '\'', "!/", "'!'$$")]
    [DataRow("$", '\'', "!^", "'!^'$")]
    [DataRow("$", '\'', "!v", "'!v'$")]
    [DataRow("$", '\'', "!|", "'!'^$")]
    [DataRow("$", '\\', "!", "'!'$")]
    [DataRow("$", '\\', "!'", "'!\\''$")]
    [DataRow("$", '\\', "!/", "'!'$$")]
    [DataRow("$", '\\', "!\\", "'!\\\\'$")]
    [DataRow("$", '\\', "!^", "'!^'$")]
    [DataRow("$", '\\', "!v", "'!v'$")]
    [DataRow("$", '\\', "!|", "'!'^$")]
    [DataRow("~$", '\'', "!", "'!'~$")]
    [DataRow("~$", '\'', "!$", "'!$'~$")]
    [DataRow("~$", '\'', "!'", "'!'''~$")]
    [DataRow("~$", '\'', "!/", "'!'~$~$")]
    [DataRow("~$", '\'', "!^", "'!^'~$")]
    [DataRow("~$", '\'', "!v", "'!v'~$")]
    [DataRow("~$", '\'', "!|", "'!'^~$")]
    [DataRow("~$", '\'', "!~", "'!~'~$")]
    [DataRow("~$", '\\', "!", "'!'~$")]
    [DataRow("~$", '\\', "!$", "'!$'~$")]
    [DataRow("~$", '\\', "!'", "'!\\''~$")]
    [DataRow("~$", '\\', "!/", "'!'~$~$")]
    [DataRow("~$", '\\', "!\\", "'!\\\\'~$")]
    [DataRow("~$", '\\', "!^", "'!^'~$")]
    [DataRow("~$", '\\', "!v", "'!v'~$")]
    [DataRow("~$", '\\', "!|", "'!'^~$")]
    [DataRow("~$", '\\', "!~", "'!~'~$")]
    public void WriteStringQuotesValueStartingWithCommentPrefix(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "!", "'!'$")]
    [DataRow("$", '\'', "!'", "'!'''$")]
    [DataRow("$", '\'', "!/", "'!'$$")]
    [DataRow("$", '\'', "!^", "'!^'$")]
    [DataRow("$", '\'', "!v", "'!v'$")]
    [DataRow("$", '\'', "!|", "'!'^$")]
    [DataRow("$", '\\', "!", "'!'$")]
    [DataRow("$", '\\', "!'", "'!\\''$")]
    [DataRow("$", '\\', "!/", "'!'$$")]
    [DataRow("$", '\\', "!\\", "'!\\\\'$")]
    [DataRow("$", '\\', "!^", "'!^'$")]
    [DataRow("$", '\\', "!v", "'!v'$")]
    [DataRow("$", '\\', "!|", "'!'^$")]
    [DataRow("~$", '\'', "!", "'!'~$")]
    [DataRow("~$", '\'', "!$", "'!$'~$")]
    [DataRow("~$", '\'', "!'", "'!'''~$")]
    [DataRow("~$", '\'', "!/", "'!'~$~$")]
    [DataRow("~$", '\'', "!^", "'!^'~$")]
    [DataRow("~$", '\'', "!v", "'!v'~$")]
    [DataRow("~$", '\'', "!|", "'!'^~$")]
    [DataRow("~$", '\'', "!~", "'!~'~$")]
    [DataRow("~$", '\\', "!", "'!'~$")]
    [DataRow("~$", '\\', "!$", "'!$'~$")]
    [DataRow("~$", '\\', "!'", "'!\\''~$")]
    [DataRow("~$", '\\', "!/", "'!'~$~$")]
    [DataRow("~$", '\\', "!\\", "'!\\\\'~$")]
    [DataRow("~$", '\\', "!^", "'!^'~$")]
    [DataRow("~$", '\\', "!v", "'!v'~$")]
    [DataRow("~$", '\\', "!|", "'!'^~$")]
    [DataRow("~$", '\\', "!~", "'!~'~$")]
    public async Task WriteStringQuotesValueStartingWithCommentPrefixAsync(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "!v", "!v$")]
    [DataRow("$", '\\', "!", "!$")]
    [DataRow("~$", '\'', "!", "!~$")]
    [DataRow("~$", '\\', "!v|!", "!v^!~$")]
    public void WriteStringWithoutCommentPrefixWritesLeadingCommentCharUnquoted(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "!v", "!v$")]
    [DataRow("$", '\\', "!", "!$")]
    [DataRow("~$", '\'', "!", "!~$")]
    [DataRow("~$", '\\', "!v|!", "!v^!~$")]
    public async Task WriteStringWithoutCommentPrefixWritesLeadingCommentCharUnquotedAsync(string lineTerminator, char escapeChar, string structure, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', " v | w ")]
    [DataRow("$", '\'', "!|v!|\\")]
    [DataRow("$", '\'', "")]
    [DataRow("$", '\'', "'\\'|''|\\\\")]
    [DataRow("$", '\'', "^'\\!|!")]
    [DataRow("$", '\'', "v|w/x")]
    [DataRow("$", '\'', "|/")]
    [DataRow("$", '\'', "~|$|~$")]
    [DataRow("$", '\\', " v | w ")]
    [DataRow("$", '\\', "!|v!|\\")]
    [DataRow("$", '\\', "")]
    [DataRow("$", '\\', "'\\'|''|\\\\")]
    [DataRow("$", '\\', "^'\\!|!")]
    [DataRow("$", '\\', "v|w/x")]
    [DataRow("$", '\\', "|/")]
    [DataRow("$", '\\', "~|$|~$")]
    [DataRow("~$", '\'', " v | w ")]
    [DataRow("~$", '\'', "!|v!|\\")]
    [DataRow("~$", '\'', "")]
    [DataRow("~$", '\'', "'\\'|''|\\\\")]
    [DataRow("~$", '\'', "^'\\!|!")]
    [DataRow("~$", '\'', "v|w/x")]
    [DataRow("~$", '\'', "|/")]
    [DataRow("~$", '\'', "~|$|~$")]
    [DataRow("~$", '\\', " v | w ")]
    [DataRow("~$", '\\', "!|v!|\\")]
    [DataRow("~$", '\\', "")]
    [DataRow("~$", '\\', "'\\'|''|\\\\")]
    [DataRow("~$", '\\', "^'\\!|!")]
    [DataRow("~$", '\\', "v|w/x")]
    [DataRow("~$", '\\', "|/")]
    [DataRow("~$", '\\', "~|$|~$")]
    public void WriteStringOutputReadsBackAsWrittenRecords(string lineTerminator, char escapeChar, string structure)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        writer.Flush();

        using var input = new MemoryStream(stream.ToArray());
        using var reader = new TabularReader(input, dialect, options);

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

        Assert.AreEqual(structure, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("$", '\'', " v | w ")]
    [DataRow("$", '\'', "!|v!|\\")]
    [DataRow("$", '\'', "")]
    [DataRow("$", '\'', "'\\'|''|\\\\")]
    [DataRow("$", '\'', "^'\\!|!")]
    [DataRow("$", '\'', "v|w/x")]
    [DataRow("$", '\'', "|/")]
    [DataRow("$", '\'', "~|$|~$")]
    [DataRow("$", '\\', " v | w ")]
    [DataRow("$", '\\', "!|v!|\\")]
    [DataRow("$", '\\', "")]
    [DataRow("$", '\\', "'\\'|''|\\\\")]
    [DataRow("$", '\\', "^'\\!|!")]
    [DataRow("$", '\\', "v|w/x")]
    [DataRow("$", '\\', "|/")]
    [DataRow("$", '\\', "~|$|~$")]
    [DataRow("~$", '\'', " v | w ")]
    [DataRow("~$", '\'', "!|v!|\\")]
    [DataRow("~$", '\'', "")]
    [DataRow("~$", '\'', "'\\'|''|\\\\")]
    [DataRow("~$", '\'', "^'\\!|!")]
    [DataRow("~$", '\'', "v|w/x")]
    [DataRow("~$", '\'', "|/")]
    [DataRow("~$", '\'', "~|$|~$")]
    [DataRow("~$", '\\', " v | w ")]
    [DataRow("~$", '\\', "!|v!|\\")]
    [DataRow("~$", '\\', "")]
    [DataRow("~$", '\\', "'\\'|''|\\\\")]
    [DataRow("~$", '\\', "^'\\!|!")]
    [DataRow("~$", '\\', "v|w/x")]
    [DataRow("~$", '\\', "|/")]
    [DataRow("~$", '\\', "~|$|~$")]
    public async Task WriteStringOutputReadsBackAsWrittenRecordsAsync(string lineTerminator, char escapeChar, string structure)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        await using var input = new MemoryStream(stream.ToArray());
        await using var reader = new TabularReader(input, dialect, options);

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

        Assert.AreEqual(structure, string.Join('/', records));
    }

    [TestMethod]
    [DataRow("", "\r\n")]
    [DataRow(" a | b ", " a , b \r\n")]
    [DataRow(",bcdefgh", "\",bcdefgh\"\r\n")]
    [DataRow("a\"b", "\"a\"\"b\"\r\n")]
    [DataRow("a'b", "a'b\r\n")]
    [DataRow("a,b", "\"a,b\"\r\n")]
    [DataRow("abcdefg,", "\"abcdefg,\"\r\n")]
    [DataRow("abcdefgh", "abcdefgh\r\n")]
    [DataRow("abcdefgh\nijklmnop", "\"abcdefgh\nijklmnop\"\r\n")]
    [DataRow("abcdefghijklmno\"p", "\"abcdefghijklmno\"\"p\"\r\n")]
    [DataRow("abcdefghijklmnop", "abcdefghijklmnop\r\n")]
    [DataRow("abcdefghijklmnop\r", "\"abcdefghijklmnop\r\"\r\n")]
    [DataRow("a|b", "a,b\r\n")]
    [DataRow("a|b/c|d", "a,b\r\nc,d\r\n")]
    public void WriteStringWithRfc4180Dialect(string structure, string expected)
    {
        var dialect = new TabularDialect("\r\n", ',', '"');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("", "\r\n")]
    [DataRow(" a | b ", " a , b \r\n")]
    [DataRow(",bcdefgh", "\",bcdefgh\"\r\n")]
    [DataRow("a\"b", "\"a\"\"b\"\r\n")]
    [DataRow("a'b", "a'b\r\n")]
    [DataRow("a,b", "\"a,b\"\r\n")]
    [DataRow("abcdefg,", "\"abcdefg,\"\r\n")]
    [DataRow("abcdefgh", "abcdefgh\r\n")]
    [DataRow("abcdefgh\nijklmnop", "\"abcdefgh\nijklmnop\"\r\n")]
    [DataRow("abcdefghijklmno\"p", "\"abcdefghijklmno\"\"p\"\r\n")]
    [DataRow("abcdefghijklmnop", "abcdefghijklmnop\r\n")]
    [DataRow("abcdefghijklmnop\r", "\"abcdefghijklmnop\r\"\r\n")]
    [DataRow("a|b", "a,b\r\n")]
    [DataRow("a|b/c|d", "a,b\r\nc,d\r\n")]
    public async Task WriteStringWithRfc4180DialectAsync(string structure, string expected)
    {
        var dialect = new TabularDialect("\r\n", ',', '"');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void WriteStringWithRfc4180DialectQuotesValueWithLineBreak()
    {
        var dialect = new TabularDialect("\r\n", ',', '"');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("a\r\nb");
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual("\"a\r\nb\"\r\n", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task WriteStringWithRfc4180DialectQuotesValueWithLineBreakAsync()
    {
        var dialect = new TabularDialect("\r\n", ',', '"');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("a\r\nb");
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual("\"a\r\nb\"\r\n", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', null, "^v$")]
    [DataRow("~$", '\'', null, "^v~$")]
    public void WriteStringWritesNullAsEmptyField(string lineTerminator, char escapeChar, string? value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString(value);
        writer.WriteString("v");
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', null, "^v$")]
    [DataRow("~$", '\'', null, "^v~$")]
    public async Task WriteStringWritesNullAsEmptyFieldAsync(string lineTerminator, char escapeChar, string? value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync(value);
        await writer.WriteStringAsync("v");
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "'", "''''$")]
    [DataRow("$", '\\', "v", "v$")]
    [DataRow("~$", '\\', "^", "'^'~$")]
    public void WriteStringWritesCharacterSequence(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString(value.AsSpan());
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "'", "''''$")]
    [DataRow("$", '\\', "v", "v$")]
    [DataRow("~$", '\\', "^", "'^'~$")]
    public async Task WriteStringWritesCharacterSequenceAsync(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync(value.AsMemory());
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', 1, "$")]
    [DataRow("$", '\\', 2, "^$")]
    [DataRow("~$", '\'', 1, "~$")]
    [DataRow("~$", '\'', 3, "^^~$")]
    public void WriteEmptyWritesEmptyField(string lineTerminator, char escapeChar, int count, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < count; i++)
        {
            writer.WriteEmpty();
        }

        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', 1, "$")]
    [DataRow("$", '\\', 2, "^$")]
    [DataRow("~$", '\'', 1, "~$")]
    [DataRow("~$", '\'', 3, "^^~$")]
    public async Task WriteEmptyWritesEmptyFieldAsync(string lineTerminator, char escapeChar, int count, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < count; i++)
        {
            await writer.WriteEmptyAsync();
        }

        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', 1, "$")]
    [DataRow("$", '\\', 2, "$$")]
    [DataRow("~$", '\'', 1, "~$")]
    [DataRow("~$", '\'', 3, "~$~$~$")]
    public void WriteEndOfRecordWithoutFieldsWritesEmptyRecord(string lineTerminator, char escapeChar, int count, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < count; i++)
        {
            writer.WriteEndOfRecord();
        }

        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', 1, "$")]
    [DataRow("$", '\\', 2, "$$")]
    [DataRow("~$", '\'', 1, "~$")]
    [DataRow("~$", '\'', 3, "~$~$~$")]
    public async Task WriteEndOfRecordWithoutFieldsWritesEmptyRecordAsync(string lineTerminator, char escapeChar, int count, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < count; i++)
        {
            await writer.WriteEndOfRecordAsync();
        }

        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$$w$")]
    [DataRow("~$", '\'', "v~$~$w~$")]
    public void WriteEndOfRecordWithoutFieldsBetweenRecordsWritesEmptyRecord(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("v");
        writer.WriteEndOfRecord();
        writer.WriteEndOfRecord();
        writer.WriteString("w");
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$$w$")]
    [DataRow("~$", '\'', "v~$~$w~$")]
    public async Task WriteEndOfRecordWithoutFieldsBetweenRecordsWritesEmptyRecordAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("v");
        await writer.WriteEndOfRecordAsync();
        await writer.WriteEndOfRecordAsync();
        await writer.WriteStringAsync("w");
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', " ' ", "''''$")]
    [DataRow("$", '\'', " ^ ", "'^'$")]
    [DataRow("$", '\\', "   ", "$")]
    [DataRow("$", '\\', "  v w  ", "v w$")]
    [DataRow("$", '\\', " v ", "v$")]
    public void WriteStringWithTrimWhitespaceTrimsValue(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString(value);
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', " ' ", "''''$")]
    [DataRow("$", '\'', " ^ ", "'^'$")]
    [DataRow("$", '\\', "   ", "$")]
    [DataRow("$", '\\', "  v w  ", "v w$")]
    [DataRow("$", '\\', " v ", "v$")]
    public async Task WriteStringWithTrimWhitespaceTrimsValueAsync(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync(value);
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "  ", "  $")]
    [DataRow("$", '\\', " v ", " v $")]
    [DataRow("~$", '\'', " ^ ", "' ^ '~$")]
    public void WriteStringWithoutTrimWhitespaceKeepsWhitespace(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString(value);
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "  ", "  $")]
    [DataRow("$", '\\', " v ", " v $")]
    [DataRow("~$", '\'', " ^ ", "' ^ '~$")]
    public async Task WriteStringWithoutTrimWhitespaceKeepsWhitespaceAsync(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync(value);
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "'", "!'$")]
    [DataRow("$", '\\', "!", "!!$")]
    [DataRow("$", '\\', "", "!$")]
    [DataRow("$", '\\', "\\", "!\\$")]
    [DataRow("$", '\\', "^", "!^$")]
    [DataRow("$", '\\', "x", "!x$")]
    [DataRow("$", '\\', "~", "!~$")]
    [DataRow("~$", '\'', "", "!~$")]
    [DataRow("~$", '\'', "$", "!$~$")]
    [DataRow("~$", '\'', "$~", "!$~~$")]
    [DataRow("~$", '\'', "x", "!x~$")]
    [DataRow("~$", '\\', "~", "!~~$")]
    public void WriteCommentWritesCommentRecord(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteComment(value);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "'", "!'$")]
    [DataRow("$", '\\', "!", "!!$")]
    [DataRow("$", '\\', "", "!$")]
    [DataRow("$", '\\', "\\", "!\\$")]
    [DataRow("$", '\\', "^", "!^$")]
    [DataRow("$", '\\', "x", "!x$")]
    [DataRow("$", '\\', "~", "!~$")]
    [DataRow("~$", '\'', "", "!~$")]
    [DataRow("~$", '\'', "$", "!$~$")]
    [DataRow("~$", '\'', "$~", "!$~~$")]
    [DataRow("~$", '\'', "x", "!x~$")]
    [DataRow("~$", '\\', "~", "!~~$")]
    public async Task WriteCommentWritesCommentRecordAsync(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteCommentAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "x", "!x$")]
    [DataRow("~$", '\'', "~", "!~~$")]
    public void WriteCommentWritesCharacterSequence(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteComment(value.AsSpan());
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "x", "!x$")]
    [DataRow("~$", '\'', "~", "!~~$")]
    public async Task WriteCommentWritesCharacterSequenceAsync(string lineTerminator, char escapeChar, string value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteCommentAsync(value.AsMemory());
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', null, "!$")]
    [DataRow("~$", '\'', null, "!~$")]
    public void WriteCommentWritesNullAsEmptyComment(string lineTerminator, char escapeChar, string? value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteComment(value);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', null, "!$")]
    [DataRow("~$", '\'', null, "!~$")]
    public async Task WriteCommentWritesNullAsEmptyCommentAsync(string lineTerminator, char escapeChar, string? value, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteCommentAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$!x$")]
    [DataRow("~$", '\'', "v~$!x~$")]
    public void WriteCommentAfterEndOfRecordWritesCommentRecord(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("v");
        writer.WriteEndOfRecord();
        writer.WriteComment("x");
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$!x$")]
    [DataRow("~$", '\'', "v~$!x~$")]
    public async Task WriteCommentAfterEndOfRecordWritesCommentRecordAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("v");
        await writer.WriteEndOfRecordAsync();
        await writer.WriteCommentAsync("x");
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "", "$!x$")]
    [DataRow("$", '\\', "v", "v$!x$")]
    [DataRow("~$", '\'', "v", "v~$!x~$")]
    [DataRow("~$", '\'', "v|w", "v^w~$!x~$")]
    public void WriteCommentEndsOpenRecord(string lineTerminator, char escapeChar, string fields, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var field in fields.Split('|'))
        {
            writer.WriteString(field);
        }

        writer.WriteComment("x");
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "", "$!x$")]
    [DataRow("$", '\\', "v", "v$!x$")]
    [DataRow("~$", '\'', "v", "v~$!x~$")]
    [DataRow("~$", '\'', "v|w", "v^w~$!x~$")]
    public async Task WriteCommentEndsOpenRecordAsync(string lineTerminator, char escapeChar, string fields, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var field in fields.Split('|'))
        {
            await writer.WriteStringAsync(field);
        }

        await writer.WriteCommentAsync("x");
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "", "!x$$")]
    [DataRow("$", '\\', "v", "!x$v$")]
    [DataRow("~$", '\'', "v|w", "!x~$v^w~$")]
    public void WriteStringAfterCommentStartsRecord(string lineTerminator, char escapeChar, string fields, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteComment("x");

        foreach (var field in fields.Split('|'))
        {
            writer.WriteString(field);
        }

        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "", "!x$$")]
    [DataRow("$", '\\', "v", "!x$v$")]
    [DataRow("~$", '\'', "v|w", "!x~$v^w~$")]
    public async Task WriteStringAfterCommentStartsRecordAsync(string lineTerminator, char escapeChar, string fields, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteCommentAsync("x");

        foreach (var field in fields.Split('|'))
        {
            await writer.WriteStringAsync(field);
        }

        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\'', "''")]
    [DataRow("$", '\\', "")]
    [DataRow("$", '\\', "^'\\!")]
    [DataRow("$", '\\', "x")]
    [DataRow("$", '\\', "~")]
    [DataRow("~$", '\'', "")]
    [DataRow("~$", '\'', "$")]
    [DataRow("~$", '\'', "$~")]
    [DataRow("~$", '\'', "x")]
    [DataRow("~$", '\\', "~")]
    public void WriteCommentOutputReadsBackAsComment(string lineTerminator, char escapeChar, string value)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteComment(value);
        writer.Flush();

        using var input = new MemoryStream(stream.ToArray());
        using var reader = new TabularReader(input, dialect, options);

        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.AreEqual(value, reader.GetString());
        Assert.IsFalse(reader.TryMoveToNextRecord());
    }

    [TestMethod]
    [DataRow("$", '\'', "''")]
    [DataRow("$", '\\', "")]
    [DataRow("$", '\\', "^'\\!")]
    [DataRow("$", '\\', "x")]
    [DataRow("$", '\\', "~")]
    [DataRow("~$", '\'', "")]
    [DataRow("~$", '\'', "$")]
    [DataRow("~$", '\'', "$~")]
    [DataRow("~$", '\'', "x")]
    [DataRow("~$", '\\', "~")]
    public async Task WriteCommentOutputReadsBackAsCommentAsync(string lineTerminator, char escapeChar, string value)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteCommentAsync(value);
        await writer.FlushAsync();

        await using var input = new MemoryStream(stream.ToArray());
        await using var reader = new TabularReader(input, dialect, options);

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.AreEqual(value, reader.GetString());
        Assert.IsFalse(await reader.TryMoveToNextRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\\', "$")]
    [DataRow("$", '\\', "x$y")]
    [DataRow("~$", '\'', "x~$y")]
    [DataRow("~$", '\'', "~$")]
    public void WriteCommentThrowsArgumentExceptionForLineTerminator(string lineTerminator, char escapeChar, string value)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        Assert.ThrowsExactly<ArgumentException>(() => writer.WriteComment(value));
    }

    [TestMethod]
    [DataRow("$", '\\', "$")]
    [DataRow("$", '\\', "x$y")]
    [DataRow("~$", '\'', "x~$y")]
    [DataRow("~$", '\'', "~$")]
    public async Task WriteCommentThrowsArgumentExceptionForLineTerminatorAsync(string lineTerminator, char escapeChar, string value)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<ArgumentException>(() => writer.WriteCommentAsync(value).AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriteCommentThrowsInvalidOperationExceptionForDialectWithoutCommentPrefix(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteComment("x"));
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriteCommentThrowsInvalidOperationExceptionForDialectWithoutCommentPrefixAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteCommentAsync("x").AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\', "v", 1L)]
    [DataRow("$", '\\', "v|w", 2L)]
    [DataRow("$", '\\', "v|w/", 1L)]
    [DataRow("~$", '\'', "v|w/x", 1L)]
    [DataRow("~$", '\'', "v|w|x", 3L)]
    public void FieldsWrittenCountsFieldsOfCurrentRecord(string lineTerminator, char escapeChar, string structure, long expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        var records = structure.Split('/');

        for (var i = 0; i < records.Length; i++)
        {
            foreach (var field in records[i].Split('|'))
            {
                writer.WriteString(field);
            }

            if (i < records.Length - 1)
            {
                writer.WriteEndOfRecord();
            }
        }

        Assert.AreEqual(expected, writer.FieldsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', "v", 1L)]
    [DataRow("$", '\\', "v|w", 2L)]
    [DataRow("$", '\\', "v|w/", 1L)]
    [DataRow("~$", '\'', "v|w/x", 1L)]
    [DataRow("~$", '\'', "v|w|x", 3L)]
    public async Task FieldsWrittenCountsFieldsOfCurrentRecordAsync(string lineTerminator, char escapeChar, string structure, long expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        var records = structure.Split('/');

        for (var i = 0; i < records.Length; i++)
        {
            foreach (var field in records[i].Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            if (i < records.Length - 1)
            {
                await writer.WriteEndOfRecordAsync();
            }
        }

        Assert.AreEqual(expected, writer.FieldsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', "v")]
    [DataRow("~$", '\'', "v|w")]
    public void FieldsWrittenIsZeroAfterEndOfRecord(string lineTerminator, char escapeChar, string fields)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var field in fields.Split('|'))
        {
            writer.WriteString(field);
        }

        writer.WriteEndOfRecord();

        Assert.AreEqual(0, writer.FieldsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', "v")]
    [DataRow("~$", '\'', "v|w")]
    public async Task FieldsWrittenIsZeroAfterEndOfRecordAsync(string lineTerminator, char escapeChar, string fields)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var field in fields.Split('|'))
        {
            await writer.WriteStringAsync(field);
        }

        await writer.WriteEndOfRecordAsync();

        Assert.AreEqual(0, writer.FieldsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', "v")]
    [DataRow("~$", '\'', "v|w")]
    public void FieldsWrittenIsZeroAfterComment(string lineTerminator, char escapeChar, string fields)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var field in fields.Split('|'))
        {
            writer.WriteString(field);
        }

        writer.WriteComment("x");

        Assert.AreEqual(0, writer.FieldsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', "v")]
    [DataRow("~$", '\'', "v|w")]
    public async Task FieldsWrittenIsZeroAfterCommentAsync(string lineTerminator, char escapeChar, string fields)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var field in fields.Split('|'))
        {
            await writer.WriteStringAsync(field);
        }

        await writer.WriteCommentAsync("x");

        Assert.AreEqual(0, writer.FieldsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', "v", 1L)]
    [DataRow("$", '\\', "v/w", 2L)]
    [DataRow("~$", '\'', "/", 2L)]
    [DataRow("~$", '\'', "v|w/x|y/z", 3L)]
    public void RecordsWrittenCountsRecords(string lineTerminator, char escapeChar, string structure, long expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                writer.WriteString(field);
            }

            writer.WriteEndOfRecord();
        }

        Assert.AreEqual(expected, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', "v", 1L)]
    [DataRow("$", '\\', "v/w", 2L)]
    [DataRow("~$", '\'', "/", 2L)]
    [DataRow("~$", '\'', "v|w/x|y/z", 3L)]
    public async Task RecordsWrittenCountsRecordsAsync(string lineTerminator, char escapeChar, string structure, long expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        foreach (var record in structure.Split('/'))
        {
            foreach (var field in record.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }

            await writer.WriteEndOfRecordAsync();
        }

        Assert.AreEqual(expected, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void RecordsWrittenExcludesComments(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteComment("x");
        writer.WriteString("v1");
        writer.WriteEndOfRecord();
        writer.WriteString("v2");
        writer.WriteComment("y");

        Assert.AreEqual(2, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task RecordsWrittenExcludesCommentsAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteCommentAsync("x");
        await writer.WriteStringAsync("v1");
        await writer.WriteEndOfRecordAsync();
        await writer.WriteStringAsync("v2");
        await writer.WriteCommentAsync("y");

        Assert.AreEqual(2, writer.RecordsWritten);
    }

    [TestMethod]
    [DataRow("$", '\\', false, 4L)]
    [DataRow("$", '\\', true, 7L)]
    [DataRow("~$", '\'', false, 5L)]
    [DataRow("~$", '\'', true, 8L)]
    public void BytesCommittedCountsBytesWrittenToStream(string lineTerminator, char escapeChar, bool writePreamble, long expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { Encoding = new UTF8Encoding(writePreamble) };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("a");
        writer.WriteString("b");
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, writer.BytesCommitted);
        Assert.AreEqual(stream.Length, writer.BytesCommitted);
    }

    [TestMethod]
    [DataRow("$", '\\', false, 4L)]
    [DataRow("$", '\\', true, 7L)]
    [DataRow("~$", '\'', false, 5L)]
    [DataRow("~$", '\'', true, 8L)]
    public async Task BytesCommittedCountsBytesWrittenToStreamAsync(string lineTerminator, char escapeChar, bool writePreamble, long expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { Encoding = new UTF8Encoding(writePreamble) };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("a");
        await writer.WriteStringAsync("b");
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, writer.BytesCommitted);
        Assert.AreEqual(stream.Length, writer.BytesCommitted);
    }

    [TestMethod]
    [DataRow("$", '\\', false)]
    [DataRow("~$", '\'', true)]
    public void BytesCommittedExcludesBufferedChars(string lineTerminator, char escapeChar, bool writePreamble)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { Encoding = new UTF8Encoding(writePreamble) };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("a");
        writer.WriteEndOfRecord();

        Assert.AreEqual(0, writer.BytesCommitted);
    }

    [TestMethod]
    [DataRow("$", '\\', false)]
    [DataRow("~$", '\'', true)]
    public async Task BytesCommittedExcludesBufferedCharsAsync(string lineTerminator, char escapeChar, bool writePreamble)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { Encoding = new UTF8Encoding(writePreamble) };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("a");
        await writer.WriteEndOfRecordAsync();

        Assert.AreEqual(0, writer.BytesCommitted);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void CurrentPositionTypeIsStartOfStreamBeforeWrites(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        Assert.AreEqual(TabularPositionType.StartOfStream, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void CurrentPositionTypeIsDelimiterAfterWriteString(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");

        Assert.AreEqual(TabularPositionType.Delimiter, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task CurrentPositionTypeIsDelimiterAfterWriteStringAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");

        Assert.AreEqual(TabularPositionType.Delimiter, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void CurrentPositionTypeIsDelimiterAfterWriteEmpty(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteEmpty();

        Assert.AreEqual(TabularPositionType.Delimiter, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task CurrentPositionTypeIsDelimiterAfterWriteEmptyAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteEmptyAsync();

        Assert.AreEqual(TabularPositionType.Delimiter, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void CurrentPositionTypeIsEndOfRecordAfterWriteEndOfRecord(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");
        writer.WriteEndOfRecord();

        Assert.AreEqual(TabularPositionType.EndOfRecord, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task CurrentPositionTypeIsEndOfRecordAfterWriteEndOfRecordAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");
        await writer.WriteEndOfRecordAsync();

        Assert.AreEqual(TabularPositionType.EndOfRecord, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void CurrentPositionTypeIsEndOfRecordAfterWriteComment(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");
        writer.WriteComment("x");

        Assert.AreEqual(TabularPositionType.EndOfRecord, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task CurrentPositionTypeIsEndOfRecordAfterWriteCommentAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");
        await writer.WriteCommentAsync("x");

        Assert.AreEqual(TabularPositionType.EndOfRecord, writer.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("utf-16", "\u20ac", "FFFEAC202400")]
    [DataRow("utf-16", "\ud83d\ude00", "FFFE3DD800DE2400")]
    [DataRow("utf-16", "^", "FFFE27005E0027002400")]
    [DataRow("utf-16BE", "\ud83d\ude00", "FEFFD83DDE000024")]
    [DataRow("utf-32", "\u20ac", "FFFE0000AC20000024000000")]
    [DataRow("utf-32", "\ud83d\ude00", "FFFE000000F6010024000000")]
    [DataRow("utf-32BE", "\ud83d\ude00", "0000FEFF0001F60000000024")]
    [DataRow("utf-8", "\u20ac", "EFBBBFE282AC24")]
    [DataRow("utf-8", "\ud83d\ude00", "EFBBBFF09F988024")]
    public void WriteStringEncodesWithEncoding(string encodingName, string value, string expected)
    {
        var dialect = CreateDialect("$", '\\');
        var options = new TabularOptions { BufferSize = 1, Encoding = Encoding.GetEncoding(encodingName) };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString(value);
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("utf-16", "\u20ac", "FFFEAC202400")]
    [DataRow("utf-16", "\ud83d\ude00", "FFFE3DD800DE2400")]
    [DataRow("utf-16", "^", "FFFE27005E0027002400")]
    [DataRow("utf-16BE", "\ud83d\ude00", "FEFFD83DDE000024")]
    [DataRow("utf-32", "\u20ac", "FFFE0000AC20000024000000")]
    [DataRow("utf-32", "\ud83d\ude00", "FFFE000000F6010024000000")]
    [DataRow("utf-32BE", "\ud83d\ude00", "0000FEFF0001F60000000024")]
    [DataRow("utf-8", "\u20ac", "EFBBBFE282AC24")]
    [DataRow("utf-8", "\ud83d\ude00", "EFBBBFF09F988024")]
    public async Task WriteStringEncodesWithEncodingAsync(string encodingName, string value, string expected)
    {
        var dialect = CreateDialect("$", '\\');
        var options = new TabularOptions { BufferSize = 1, Encoding = Encoding.GetEncoding(encodingName) };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync(value);
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("\u20ac", "E282AC24")]
    [DataRow("\ud83d\ude00", "F09F988024")]
    [DataRow("v", "7624")]
    public void WriteStringWithoutEncodingWritesUtf8WithoutPreamble(string value, string expected)
    {
        var dialect = CreateDialect("$", '\\');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString(value);
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("\u20ac", "E282AC24")]
    [DataRow("\ud83d\ude00", "F09F988024")]
    [DataRow("v", "7624")]
    public async Task WriteStringWithoutEncodingWritesUtf8WithoutPreambleAsync(string value, string expected)
    {
        var dialect = CreateDialect("$", '\\');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync(value);
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("", "EFBBBF7A24")]
    [DataRow("x^y$", "785E79247A24")]
    public void WriteStringWritesPreambleOnlyAtStartOfStream(string prefix, string expected)
    {
        var dialect = CreateDialect("$", '\\');
        var options = new TabularOptions { Encoding = new UTF8Encoding(true) };

        using var stream = new MemoryStream();

        stream.Write(Encoding.UTF8.GetBytes(prefix));

        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("z");
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(expected, Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("", "EFBBBF7A24")]
    [DataRow("x^y$", "785E79247A24")]
    public async Task WriteStringWritesPreambleOnlyAtStartOfStreamAsync(string prefix, string expected)
    {
        var dialect = CreateDialect("$", '\\');
        var options = new TabularOptions { Encoding = new UTF8Encoding(true) };

        await using var stream = new MemoryStream();

        await stream.WriteAsync(Encoding.UTF8.GetBytes(prefix));

        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("z");
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(expected, Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("utf-16")]
    [DataRow("utf-16BE")]
    [DataRow("utf-32")]
    [DataRow("utf-32BE")]
    [DataRow("utf-8")]
    public void WriteStringWithValueLargerThanBufferWritesWholeValue(string encodingName)
    {
        var dialect = CreateDialect("$", '\\');
        var encoding = Encoding.GetEncoding(encodingName);
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };
        var value = string.Concat(Enumerable.Repeat("v\u20ac\ud83d\ude00", 256));

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString(value);
        writer.WriteEndOfRecord();
        writer.Flush();

        Assert.AreEqual(Convert.ToHexString([.. encoding.GetPreamble(), .. encoding.GetBytes(value + "$")]), Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("utf-16")]
    [DataRow("utf-16BE")]
    [DataRow("utf-32")]
    [DataRow("utf-32BE")]
    [DataRow("utf-8")]
    public async Task WriteStringWithValueLargerThanBufferWritesWholeValueAsync(string encodingName)
    {
        var dialect = CreateDialect("$", '\\');
        var encoding = Encoding.GetEncoding(encodingName);
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };
        var value = string.Concat(Enumerable.Repeat("v\u20ac\ud83d\ude00", 256));

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync(value);
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual(Convert.ToHexString([.. encoding.GetPreamble(), .. encoding.GetBytes(value + "$")]), Convert.ToHexString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriteStringDrainsBufferWithoutFlushingStream(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var target = new MemoryStream();
        using var stream = new BufferedStream(target, 1024);
        using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            writer.WriteString("v");
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriteStringDrainsBufferWithoutFlushingStreamAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var target = new MemoryStream();
        await using var stream = new BufferedStream(target, 1024);
        await using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            await writer.WriteStringAsync("v");
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriteEmptyDrainsBufferWithoutFlushingStream(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var target = new MemoryStream();
        using var stream = new BufferedStream(target, 1024);
        using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            writer.WriteEmpty();
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriteEmptyDrainsBufferWithoutFlushingStreamAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var target = new MemoryStream();
        await using var stream = new BufferedStream(target, 1024);
        await using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            await writer.WriteEmptyAsync();
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriteCommentDrainsBufferWithoutFlushingStream(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var target = new MemoryStream();
        using var stream = new BufferedStream(target, 1024);
        using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            writer.WriteComment("x");
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriteCommentDrainsBufferWithoutFlushingStreamAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var target = new MemoryStream();
        await using var stream = new BufferedStream(target, 1024);
        await using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            await writer.WriteCommentAsync("x");
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriteEndOfRecordDrainsBufferWithoutFlushingStream(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var target = new MemoryStream();
        using var stream = new BufferedStream(target, 1024);
        using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            writer.WriteEndOfRecord();
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriteEndOfRecordDrainsBufferWithoutFlushingStreamAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var target = new MemoryStream();
        await using var stream = new BufferedStream(target, 1024);
        await using var writer = new TabularWriter(stream, dialect, options);

        for (var i = 0; i < 100; i++)
        {
            await writer.WriteEndOfRecordAsync();
        }

        Assert.AreNotEqual(0, writer.BytesCommitted);
        Assert.AreEqual(0, target.Length);
    }

    [TestMethod]
    [DataRow("$", '\\', "v")]
    [DataRow("~$", '\'', "v")]
    public void FlushFlushesStream(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var target = new MemoryStream();
        using var stream = new BufferedStream(target, 1024);
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v")]
    [DataRow("~$", '\'', "v")]
    public async Task FlushFlushesStreamAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var target = new MemoryStream();
        await using var stream = new BufferedStream(target, 1024);
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public void DisposeFlushesStream(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var target = new MemoryStream();
        using var stream = new BufferedStream(target, 1024);

        using (var writer = new TabularWriter(stream, dialect))
        {
            writer.WriteString("v");
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public async Task DisposeFlushesStreamAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var target = new MemoryStream();
        await using var stream = new BufferedStream(target, 1024);

        await using (var writer = new TabularWriter(stream, dialect))
        {
            await writer.WriteStringAsync("v");
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "", "$")]
    [DataRow("$", '\\', "v", "v$")]
    [DataRow("~$", '\'', "", "~$")]
    [DataRow("~$", '\'', "v|", "v^~$")]
    [DataRow("~$", '\'', "v|w", "v^w~$")]
    public void DisposeEndsOpenRecord(string lineTerminator, char escapeChar, string fields, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();

        using (var writer = new TabularWriter(stream, dialect, options))
        {
            foreach (var field in fields.Split('|'))
            {
                writer.WriteString(field);
            }
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "", "$")]
    [DataRow("$", '\\', "v", "v$")]
    [DataRow("~$", '\'', "", "~$")]
    [DataRow("~$", '\'', "v|", "v^~$")]
    [DataRow("~$", '\'', "v|w", "v^w~$")]
    public async Task DisposeEndsOpenRecordAsync(string lineTerminator, char escapeChar, string fields, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        await using (var writer = new TabularWriter(stream, dialect, options))
        {
            foreach (var field in fields.Split('|'))
            {
                await writer.WriteStringAsync(field);
            }
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public void DisposeAfterEndOfRecordWritesNothingMore(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();

        using (var writer = new TabularWriter(stream, dialect, options))
        {
            writer.WriteString("v");
            writer.WriteEndOfRecord();
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public async Task DisposeAfterEndOfRecordWritesNothingMoreAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        await using (var writer = new TabularWriter(stream, dialect, options))
        {
            await writer.WriteStringAsync("v");
            await writer.WriteEndOfRecordAsync();
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "!x$")]
    [DataRow("~$", '\'', "!x~$")]
    public void DisposeAfterCommentWritesNothingMore(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();

        using (var writer = new TabularWriter(stream, dialect, options))
        {
            writer.WriteComment("x");
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "!x$")]
    [DataRow("~$", '\'', "!x~$")]
    public async Task DisposeAfterCommentWritesNothingMoreAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        await using (var writer = new TabularWriter(stream, dialect, options))
        {
            await writer.WriteCommentAsync("x");
        }

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void DisposeWithoutWritesWritesNothing(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();

        using (var writer = new TabularWriter(stream, dialect, options))
        {
        }

        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task DisposeWithoutWritesWritesNothingAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        await using (var writer = new TabularWriter(stream, dialect, options))
        {
        }

        Assert.AreEqual(0, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public void DisposeTwiceEndsRecordOnce(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("v");
        writer.Dispose();
        writer.Dispose();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public async Task DisposeTwiceEndsRecordOnceAsync(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("v");
        await writer.DisposeAsync();
        await writer.DisposeAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public async Task DisposeAsyncAfterDisposeEndsRecordOnce(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect, options);

        writer.WriteString("v");
        writer.Dispose();

        await writer.DisposeAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public async Task DisposeAfterDisposeAsyncEndsRecordOnce(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("v");
        await writer.DisposeAsync();

        writer.Dispose();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void DisposeLeavesStreamOpen(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream();

        using (var writer = new TabularWriter(stream, dialect))
        {
            writer.WriteString("v");
        }

        Assert.IsTrue(stream.CanWrite);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task DisposeLeavesStreamOpenAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();

        await using (var writer = new TabularWriter(stream, dialect))
        {
            await writer.WriteStringAsync("v");
        }

        Assert.IsTrue(stream.CanWrite);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void DisposeWithFailingStreamThrowsAndDisposesWriter(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream([]);

        var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");

        Assert.ThrowsExactly<NotSupportedException>(() => writer.Dispose());
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteString("w"));
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task DisposeWithFailingStreamThrowsAndDisposesWriterAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream([]);

        var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => writer.DisposeAsync().AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteStringAsync("w").AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriterThrowsObjectDisposedExceptionAfterDispose(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');

        using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect);

        writer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteString("v"));
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteString("v".AsSpan()));
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteEmpty());
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteComment("x"));
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteComment("x".AsSpan()));
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteEndOfRecord());
        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Flush());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriterThrowsObjectDisposedExceptionAfterDisposeAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');

        await using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect);

        await writer.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteStringAsync("v").AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteStringAsync("v".AsMemory()).AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteEmptyAsync().AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteCommentAsync("x").AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteCommentAsync("x".AsMemory()).AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteEndOfRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.FlushAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriterIsFaultedAfterStreamException(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream([]);
        using var writer = new TabularWriter(stream, dialect, options);

        Assert.ThrowsExactly<NotSupportedException>(() => writer.WriteString("abc"));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteString("v".AsSpan()));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteEmpty());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteComment("x"));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteComment("x".AsSpan()));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteEndOfRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.Flush());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriterIsFaultedAfterStreamExceptionAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream([]);
        await using var writer = new TabularWriter(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => writer.WriteStringAsync("abc").AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteStringAsync("v".AsMemory()).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteEmptyAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteCommentAsync("x").AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteCommentAsync("x".AsMemory()).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteEndOfRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.FlushAsync().AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriterIsFaultedAfterStreamExceptionInWriteComment(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream([]);
        using var writer = new TabularWriter(stream, dialect, options);

        Assert.ThrowsExactly<NotSupportedException>(() => writer.WriteComment("xyz"));
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteString("v"));
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriterIsFaultedAfterStreamExceptionInWriteCommentAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream([]);
        await using var writer = new TabularWriter(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => writer.WriteCommentAsync("xyz").AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteStringAsync("v").AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void WriterIsFaultedAfterStreamExceptionInFlush(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream([]);
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");

        Assert.ThrowsExactly<NotSupportedException>(() => writer.Flush());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteString("v"));
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriterIsFaultedAfterStreamExceptionInFlushAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream([]);
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => writer.FlushAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteStringAsync("v").AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriterIsFaultedAfterCanceledWriteStringAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => writer.WriteStringAsync("abc", new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteStringAsync("v").AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriterIsFaultedAfterCanceledWriteCommentAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => writer.WriteCommentAsync("xyz", new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteStringAsync("v").AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task WriterIsFaultedAfterCanceledFlushAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");

        await Assert.ThrowsAsync<OperationCanceledException>(() => writer.FlushAsync(new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteStringAsync("v").AsTask());
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public void DisposeAfterFaultWritesNothing(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new FailingFlushStream();

        var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");

        Assert.ThrowsExactly<IOException>(() => writer.Flush());

        var length = stream.Length;

        writer.Dispose();

        Assert.AreEqual(length, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\\')]
    [DataRow("~$", '\'')]
    public async Task DisposeAfterFaultWritesNothingAsync(string lineTerminator, char escapeChar)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new FailingFlushStream();

        var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");

        await Assert.ThrowsExactlyAsync<IOException>(() => writer.FlushAsync().AsTask());

        var length = stream.Length;

        await writer.DisposeAsync();

        Assert.AreEqual(length, stream.Length);
    }

    [TestMethod]
    [DataRow("$", '\\', "'v^w'^x")]
    [DataRow("~$", '\'', "'v^w'^x")]
    public async Task WriteStringAsyncDoesNotUseSynchronousIO(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, Encoding = new UTF8Encoding(true) };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("v^w");
        await writer.WriteStringAsync("x".AsMemory());
        await writer.FlushAsync();

        Assert.AreEqual("\ufeff" + expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "^^")]
    [DataRow("~$", '\'', "^^")]
    public async Task WriteEmptyAsyncDoesNotUseSynchronousIO(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, Encoding = new UTF8Encoding(true) };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteEmptyAsync();
        await writer.WriteEmptyAsync();
        await writer.WriteEmptyAsync();
        await writer.FlushAsync();

        Assert.AreEqual("\ufeff" + expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "!x$!y$")]
    [DataRow("~$", '\'', "!x~$!y~$")]
    public async Task WriteCommentAsyncDoesNotUseSynchronousIO(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1, Encoding = new UTF8Encoding(true) };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteCommentAsync("x");
        await writer.WriteCommentAsync("y".AsMemory());
        await writer.FlushAsync();

        Assert.AreEqual("\ufeff" + expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "$$")]
    [DataRow("~$", '\'', "~$~$")]
    public async Task WriteEndOfRecordAsyncDoesNotUseSynchronousIO(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1, Encoding = new UTF8Encoding(true) };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteEndOfRecordAsync();
        await writer.WriteEndOfRecordAsync();
        await writer.FlushAsync();

        Assert.AreEqual("\ufeff" + expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v")]
    [DataRow("~$", '\'', "v")]
    public async Task FlushAsyncDoesNotUseSynchronousIO(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { Encoding = new UTF8Encoding(true) };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("v");
        await writer.FlushAsync();

        Assert.AreEqual("\ufeff" + expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    [DataRow("$", '\\', "v$")]
    [DataRow("~$", '\'', "v~$")]
    public async Task DisposeAsyncDoesNotUseSynchronousIO(string lineTerminator, char escapeChar, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { Encoding = new UTF8Encoding(true) };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);

        var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteStringAsync("v");
        await writer.DisposeAsync();

        Assert.AreEqual("\ufeff" + expected, Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullStream()
    {
        var dialect = CreateDialect("$", '\\');

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularWriter(null!, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullDialect()
    {
        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularWriter(stream, null!));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForUnwritableStream()
    {
        var dialect = CreateDialect("$", '\\');

        using var stream = new MemoryStream([], false);

        Assert.ThrowsExactly<ArgumentException>(() => new TabularWriter(stream, dialect));
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
