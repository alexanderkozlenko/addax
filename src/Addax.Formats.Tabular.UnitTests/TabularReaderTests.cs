using System.Text;
using Addax.Formats.Tabular.UnitTests.Streams;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularReaderTests
{
    [TestMethod]
    [DataRow("$", '\'', "!", "!")]
    [DataRow("$", '\'', "!v", "!v")]
    [DataRow("$", '\'', "' v '", " v ")]
    [DataRow("$", '\'', "'$'", "$")]
    [DataRow("$", '\'', "''", "")]
    [DataRow("$", '\'', "''$", "")]
    [DataRow("$", '\'', "''''", "'")]
    [DataRow("$", '\'', "''''$", "'")]
    [DataRow("$", '\'', "''''^", "'|")]
    [DataRow("$", '\'', "''^", "|")]
    [DataRow("$", '\'', "'\\'", "\\")]
    [DataRow("$", '\'', "'^'", "^")]
    [DataRow("$", '\'', "'v$w'^x$y", "v$w|x/y")]
    [DataRow("$", '\'', "'v'", "v")]
    [DataRow("$", '\'', "'v'$", "v")]
    [DataRow("$", '\'', "'v''w'", "v'w")]
    [DataRow("$", '\'', "'v'^", "v|")]
    [DataRow("$", '\'', "'v'^'w'", "v|w")]
    [DataRow("$", '\'', "'v'^w", "v|w")]
    [DataRow("$", '\'', "v^'w'", "v|w")]
    [DataRow("$", '\\', " v ", " v ")]
    [DataRow("$", '\\', "$", "")]
    [DataRow("$", '\\', "$$", "/")]
    [DataRow("$", '\\', "'$'", "$")]
    [DataRow("$", '\\', "''", "")]
    [DataRow("$", '\\', "''$", "")]
    [DataRow("$", '\\', "''^", "|")]
    [DataRow("$", '\\', "'\\''", "'")]
    [DataRow("$", '\\', "'\\''$", "'")]
    [DataRow("$", '\\', "'\\''^", "'|")]
    [DataRow("$", '\\', "'\\\\'", "\\")]
    [DataRow("$", '\\', "'\\\\'$", "\\")]
    [DataRow("$", '\\', "'\\\\'^", "\\|")]
    [DataRow("$", '\\', "'^'", "^")]
    [DataRow("$", '\\', "'v$w'^x$y", "v$w|x/y")]
    [DataRow("$", '\\', "'v'", "v")]
    [DataRow("$", '\\', "'v'$", "v")]
    [DataRow("$", '\\', "'v'^", "v|")]
    [DataRow("$", '\\', "'v'^'w'", "v|w")]
    [DataRow("$", '\\', "'v\\'w'", "v'w")]
    [DataRow("$", '\\', "\\", "\\")]
    [DataRow("$", '\\', "\\$", "\\")]
    [DataRow("$", '\\', "\\^", "\\|")]
    [DataRow("$", '\\', "^", "|")]
    [DataRow("$", '\\', "^$^", "|/|")]
    [DataRow("$", '\\', "^^", "||")]
    [DataRow("$", '\\', "^v", "|v")]
    [DataRow("$", '\\', "v", "v")]
    [DataRow("$", '\\', "v$", "v")]
    [DataRow("$", '\\', "v$$", "v/")]
    [DataRow("$", '\\', "v$$w", "v//w")]
    [DataRow("$", '\\', "v$w^x", "v/w|x")]
    [DataRow("$", '\\', "v\\w", "v\\w")]
    [DataRow("$", '\\', "v^", "v|")]
    [DataRow("$", '\\', "v^w$x^y", "v|w/x|y")]
    [DataRow("$", '\\', "~", "~")]
    [DataRow("~$", '\'', "'$'", "$")]
    [DataRow("~$", '\'', "'$'^", "$|")]
    [DataRow("~$", '\'', "'$'~$", "$")]
    [DataRow("~$", '\'', "''", "")]
    [DataRow("~$", '\'', "''''", "'")]
    [DataRow("~$", '\'', "''''^", "'|")]
    [DataRow("~$", '\'', "''''~$", "'")]
    [DataRow("~$", '\'', "''^", "|")]
    [DataRow("~$", '\'', "''~$", "")]
    [DataRow("~$", '\'', "'v'", "v")]
    [DataRow("~$", '\'', "'v'^", "v|")]
    [DataRow("~$", '\'', "'v'~$", "v")]
    [DataRow("~$", '\'', "'v~$w'~$x", "v~$w/x")]
    [DataRow("~$", '\'', "'~$'", "~$")]
    [DataRow("~$", '\'', "'~'", "~")]
    [DataRow("~$", '\'', "'~'^", "~|")]
    [DataRow("~$", '\'', "'~'~$", "~")]
    [DataRow("~$", '\\', "$", "$")]
    [DataRow("~$", '\\', "$^", "$|")]
    [DataRow("~$", '\\', "$~$", "$")]
    [DataRow("~$", '\\', "'$'", "$")]
    [DataRow("~$", '\\', "'$'^", "$|")]
    [DataRow("~$", '\\', "'$'~$", "$")]
    [DataRow("~$", '\\', "''", "")]
    [DataRow("~$", '\\', "''^", "|")]
    [DataRow("~$", '\\', "''~$", "")]
    [DataRow("~$", '\\', "'\\''", "'")]
    [DataRow("~$", '\\', "'\\''^", "'|")]
    [DataRow("~$", '\\', "'\\''~$", "'")]
    [DataRow("~$", '\\', "'\\\\'", "\\")]
    [DataRow("~$", '\\', "'\\\\'^", "\\|")]
    [DataRow("~$", '\\', "'\\\\'~$", "\\")]
    [DataRow("~$", '\\', "'v'", "v")]
    [DataRow("~$", '\\', "'v'^", "v|")]
    [DataRow("~$", '\\', "'v'~$", "v")]
    [DataRow("~$", '\\', "'v~$w'~$x", "v~$w/x")]
    [DataRow("~$", '\\', "'~$'", "~$")]
    [DataRow("~$", '\\', "'~'", "~")]
    [DataRow("~$", '\\', "'~'^", "~|")]
    [DataRow("~$", '\\', "'~'~$", "~")]
    [DataRow("~$", '\\', "\\", "\\")]
    [DataRow("~$", '\\', "\\^", "\\|")]
    [DataRow("~$", '\\', "\\~$", "\\")]
    [DataRow("~$", '\\', "^", "|")]
    [DataRow("~$", '\\', "v", "v")]
    [DataRow("~$", '\\', "v^", "v|")]
    [DataRow("~$", '\\', "v^w~$x^y", "v|w/x|y")]
    [DataRow("~$", '\\', "v~", "v~")]
    [DataRow("~$", '\\', "v~$", "v")]
    [DataRow("~$", '\\', "v~$~$", "v/")]
    [DataRow("~$", '\\', "v~$~$w", "v//w")]
    [DataRow("~$", '\\', "~", "~")]
    [DataRow("~$", '\\', "~$", "")]
    [DataRow("~$", '\\', "~$~$", "/")]
    [DataRow("~$", '\\', "~^", "~|")]
    [DataRow("~$", '\\', "~v", "~v")]
    [DataRow("~$", '\\', "~~$", "~")]
    public void TryReadFieldReturnsFieldsOfValidContent(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryMoveToNextRecord())
        {
            var fields = new List<string>();

            while (reader.TryReadField())
            {
                fields.Add(reader.CurrentField.ToString());
            }

            Assert.IsNotEmpty(fields);

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(expected.Split('/'), records);
    }

    [TestMethod]
    [DataRow("$", '\'', "!", "!")]
    [DataRow("$", '\'', "!v", "!v")]
    [DataRow("$", '\'', "' v '", " v ")]
    [DataRow("$", '\'', "'$'", "$")]
    [DataRow("$", '\'', "''", "")]
    [DataRow("$", '\'', "''$", "")]
    [DataRow("$", '\'', "''''", "'")]
    [DataRow("$", '\'', "''''$", "'")]
    [DataRow("$", '\'', "''''^", "'|")]
    [DataRow("$", '\'', "''^", "|")]
    [DataRow("$", '\'', "'\\'", "\\")]
    [DataRow("$", '\'', "'^'", "^")]
    [DataRow("$", '\'', "'v$w'^x$y", "v$w|x/y")]
    [DataRow("$", '\'', "'v'", "v")]
    [DataRow("$", '\'', "'v'$", "v")]
    [DataRow("$", '\'', "'v''w'", "v'w")]
    [DataRow("$", '\'', "'v'^", "v|")]
    [DataRow("$", '\'', "'v'^'w'", "v|w")]
    [DataRow("$", '\'', "'v'^w", "v|w")]
    [DataRow("$", '\'', "v^'w'", "v|w")]
    [DataRow("$", '\\', " v ", " v ")]
    [DataRow("$", '\\', "$", "")]
    [DataRow("$", '\\', "$$", "/")]
    [DataRow("$", '\\', "'$'", "$")]
    [DataRow("$", '\\', "''", "")]
    [DataRow("$", '\\', "''$", "")]
    [DataRow("$", '\\', "''^", "|")]
    [DataRow("$", '\\', "'\\''", "'")]
    [DataRow("$", '\\', "'\\''$", "'")]
    [DataRow("$", '\\', "'\\''^", "'|")]
    [DataRow("$", '\\', "'\\\\'", "\\")]
    [DataRow("$", '\\', "'\\\\'$", "\\")]
    [DataRow("$", '\\', "'\\\\'^", "\\|")]
    [DataRow("$", '\\', "'^'", "^")]
    [DataRow("$", '\\', "'v$w'^x$y", "v$w|x/y")]
    [DataRow("$", '\\', "'v'", "v")]
    [DataRow("$", '\\', "'v'$", "v")]
    [DataRow("$", '\\', "'v'^", "v|")]
    [DataRow("$", '\\', "'v'^'w'", "v|w")]
    [DataRow("$", '\\', "'v\\'w'", "v'w")]
    [DataRow("$", '\\', "\\", "\\")]
    [DataRow("$", '\\', "\\$", "\\")]
    [DataRow("$", '\\', "\\^", "\\|")]
    [DataRow("$", '\\', "^", "|")]
    [DataRow("$", '\\', "^$^", "|/|")]
    [DataRow("$", '\\', "^^", "||")]
    [DataRow("$", '\\', "^v", "|v")]
    [DataRow("$", '\\', "v", "v")]
    [DataRow("$", '\\', "v$", "v")]
    [DataRow("$", '\\', "v$$", "v/")]
    [DataRow("$", '\\', "v$$w", "v//w")]
    [DataRow("$", '\\', "v$w^x", "v/w|x")]
    [DataRow("$", '\\', "v\\w", "v\\w")]
    [DataRow("$", '\\', "v^", "v|")]
    [DataRow("$", '\\', "v^w$x^y", "v|w/x|y")]
    [DataRow("$", '\\', "~", "~")]
    [DataRow("~$", '\'', "'$'", "$")]
    [DataRow("~$", '\'', "'$'^", "$|")]
    [DataRow("~$", '\'', "'$'~$", "$")]
    [DataRow("~$", '\'', "''", "")]
    [DataRow("~$", '\'', "''''", "'")]
    [DataRow("~$", '\'', "''''^", "'|")]
    [DataRow("~$", '\'', "''''~$", "'")]
    [DataRow("~$", '\'', "''^", "|")]
    [DataRow("~$", '\'', "''~$", "")]
    [DataRow("~$", '\'', "'v'", "v")]
    [DataRow("~$", '\'', "'v'^", "v|")]
    [DataRow("~$", '\'', "'v'~$", "v")]
    [DataRow("~$", '\'', "'v~$w'~$x", "v~$w/x")]
    [DataRow("~$", '\'', "'~$'", "~$")]
    [DataRow("~$", '\'', "'~'", "~")]
    [DataRow("~$", '\'', "'~'^", "~|")]
    [DataRow("~$", '\'', "'~'~$", "~")]
    [DataRow("~$", '\\', "$", "$")]
    [DataRow("~$", '\\', "$^", "$|")]
    [DataRow("~$", '\\', "$~$", "$")]
    [DataRow("~$", '\\', "'$'", "$")]
    [DataRow("~$", '\\', "'$'^", "$|")]
    [DataRow("~$", '\\', "'$'~$", "$")]
    [DataRow("~$", '\\', "''", "")]
    [DataRow("~$", '\\', "''^", "|")]
    [DataRow("~$", '\\', "''~$", "")]
    [DataRow("~$", '\\', "'\\''", "'")]
    [DataRow("~$", '\\', "'\\''^", "'|")]
    [DataRow("~$", '\\', "'\\''~$", "'")]
    [DataRow("~$", '\\', "'\\\\'", "\\")]
    [DataRow("~$", '\\', "'\\\\'^", "\\|")]
    [DataRow("~$", '\\', "'\\\\'~$", "\\")]
    [DataRow("~$", '\\', "'v'", "v")]
    [DataRow("~$", '\\', "'v'^", "v|")]
    [DataRow("~$", '\\', "'v'~$", "v")]
    [DataRow("~$", '\\', "'v~$w'~$x", "v~$w/x")]
    [DataRow("~$", '\\', "'~$'", "~$")]
    [DataRow("~$", '\\', "'~'", "~")]
    [DataRow("~$", '\\', "'~'^", "~|")]
    [DataRow("~$", '\\', "'~'~$", "~")]
    [DataRow("~$", '\\', "\\", "\\")]
    [DataRow("~$", '\\', "\\^", "\\|")]
    [DataRow("~$", '\\', "\\~$", "\\")]
    [DataRow("~$", '\\', "^", "|")]
    [DataRow("~$", '\\', "v", "v")]
    [DataRow("~$", '\\', "v^", "v|")]
    [DataRow("~$", '\\', "v^w~$x^y", "v|w/x|y")]
    [DataRow("~$", '\\', "v~", "v~")]
    [DataRow("~$", '\\', "v~$", "v")]
    [DataRow("~$", '\\', "v~$~$", "v/")]
    [DataRow("~$", '\\', "v~$~$w", "v//w")]
    [DataRow("~$", '\\', "~", "~")]
    [DataRow("~$", '\\', "~$", "")]
    [DataRow("~$", '\\', "~$~$", "/")]
    [DataRow("~$", '\\', "~^", "~|")]
    [DataRow("~$", '\\', "~v", "~v")]
    [DataRow("~$", '\\', "~~$", "~")]
    public async Task TryReadFieldReturnsFieldsOfValidContentAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryMoveToNextRecordAsync())
        {
            var fields = new List<string>();

            while (await reader.TryReadFieldAsync())
            {
                fields.Add(reader.CurrentField.ToString());
            }

            Assert.IsNotEmpty(fields);

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(expected.Split('/'), records);
    }

    [TestMethod]
    [DataRow("$", '\'', "!'v", "!'v")]
    [DataRow("$", '\'', "'v$!w'$!x", "v$!w/!x")]
    [DataRow("$", '\\', "!!", "!!")]
    [DataRow("$", '\\', "!", "!")]
    [DataRow("$", '\\', "!$", "!")]
    [DataRow("$", '\\', "!'v", "!'v")]
    [DataRow("$", '\\', "!\\v", "!\\v")]
    [DataRow("$", '\\', "!^v", "!^v")]
    [DataRow("$", '\\', "!v", "!v")]
    [DataRow("$", '\\', "!v$!w", "!v/!w")]
    [DataRow("$", '\\', "!v$", "!v")]
    [DataRow("$", '\\', "!v$$", "!v/")]
    [DataRow("$", '\\', "!v$w", "!v/w")]
    [DataRow("$", '\\', "$!v", "/!v")]
    [DataRow("$", '\\', "'v'$!w", "v/!w")]
    [DataRow("$", '\\', "v$~!w", "v/~!w")]
    [DataRow("$", '\\', "w$!v", "w/!v")]
    [DataRow("$", '\\', "w$!v$", "w/!v")]
    [DataRow("~$", '\\', "!", "!")]
    [DataRow("~$", '\\', "!$", "!$")]
    [DataRow("~$", '\\', "!^v", "!^v")]
    [DataRow("~$", '\\', "!v", "!v")]
    [DataRow("~$", '\\', "!v~$", "!v")]
    [DataRow("~$", '\\', "!v~$w^x~$!y", "!v/w|x/!y")]
    [DataRow("~$", '\\', "!~", "!~")]
    [DataRow("~$", '\\', "!~$", "!")]
    [DataRow("~$", '\\', "!~'v~$", "!~'v")]
    [DataRow("~$", '\\', "!~^v~$", "!~^v")]
    [DataRow("~$", '\\', "!~^~$", "!~^")]
    [DataRow("~$", '\\', "!~~$", "!~")]
    [DataRow("~$", '\\', "~!v", "~!v")]
    public void TryReadFieldReturnsCommentsOfValidContent(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryMoveToNextRecord())
        {
            var fields = new List<string>();

            while (reader.TryReadField())
            {
                fields.Add(reader.CurrentFieldType == TabularFieldType.Comment ? "!" + reader.CurrentField.ToString() : reader.CurrentField.ToString());
            }

            Assert.IsNotEmpty(fields);

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(expected.Split('/'), records);
    }

    [TestMethod]
    [DataRow("$", '\'', "!'v", "!'v")]
    [DataRow("$", '\'', "'v$!w'$!x", "v$!w/!x")]
    [DataRow("$", '\\', "!!", "!!")]
    [DataRow("$", '\\', "!", "!")]
    [DataRow("$", '\\', "!$", "!")]
    [DataRow("$", '\\', "!'v", "!'v")]
    [DataRow("$", '\\', "!\\v", "!\\v")]
    [DataRow("$", '\\', "!^v", "!^v")]
    [DataRow("$", '\\', "!v", "!v")]
    [DataRow("$", '\\', "!v$!w", "!v/!w")]
    [DataRow("$", '\\', "!v$", "!v")]
    [DataRow("$", '\\', "!v$$", "!v/")]
    [DataRow("$", '\\', "!v$w", "!v/w")]
    [DataRow("$", '\\', "$!v", "/!v")]
    [DataRow("$", '\\', "'v'$!w", "v/!w")]
    [DataRow("$", '\\', "v$~!w", "v/~!w")]
    [DataRow("$", '\\', "w$!v", "w/!v")]
    [DataRow("$", '\\', "w$!v$", "w/!v")]
    [DataRow("~$", '\\', "!", "!")]
    [DataRow("~$", '\\', "!$", "!$")]
    [DataRow("~$", '\\', "!^v", "!^v")]
    [DataRow("~$", '\\', "!v", "!v")]
    [DataRow("~$", '\\', "!v~$", "!v")]
    [DataRow("~$", '\\', "!v~$w^x~$!y", "!v/w|x/!y")]
    [DataRow("~$", '\\', "!~", "!~")]
    [DataRow("~$", '\\', "!~$", "!")]
    [DataRow("~$", '\\', "!~'v~$", "!~'v")]
    [DataRow("~$", '\\', "!~^v~$", "!~^v")]
    [DataRow("~$", '\\', "!~^~$", "!~^")]
    [DataRow("~$", '\\', "!~~$", "!~")]
    [DataRow("~$", '\\', "~!v", "~!v")]
    public async Task TryReadFieldReturnsCommentsOfValidContentAsync(string lineTerminator, char escapeChar, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryMoveToNextRecordAsync())
        {
            var fields = new List<string>();

            while (await reader.TryReadFieldAsync())
            {
                fields.Add(reader.CurrentFieldType == TabularFieldType.Comment ? "!" + reader.CurrentField.ToString() : reader.CurrentField.ToString());
            }

            Assert.IsNotEmpty(fields);

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(expected.Split('/'), records);
    }

    [TestMethod]
    [DataRow("$", '\'', 30, "'*$*'^*", "*$*|*")]
    [DataRow("$", '\'', 30, "'*'$*$*", "*/*/*")]
    [DataRow("$", '\'', 30, "'*''*'^*", "*'*|*")]
    [DataRow("$", '\'', 30, "'*'^*^*", "*|*|*")]
    [DataRow("$", '\'', 30, "*^'*'^*", "*|*|*")]
    [DataRow("$", '\'', 31, "'*$*'$*", "*$*/*")]
    [DataRow("$", '\'', 31, "'*^*'^*", "*^*|*")]
    [DataRow("$", '\'', 31, "*$!*$*", "*/!*/*")]
    [DataRow("$", '\'', 31, "*$*$*", "*/*/*")]
    [DataRow("$", '\'', 31, "*^!*$*", "*|!*/*")]
    [DataRow("$", '\'', 31, "*^'*'^*", "*|*|*")]
    [DataRow("$", '\'', 31, "*^*^*", "*|*|*")]
    [DataRow("$", '\'', 32, "*$*$*", "*/*/*")]
    [DataRow("$", '\'', 32, "*^*^*", "*|*|*")]
    [DataRow("$", '\\', 30, "'*\\'*'^*", "*'*|*")]
    [DataRow("$", '\\', 30, "'*\\\\*'^*", "*\\*|*")]
    [DataRow("~$", '\'', 30, "'*'~$*~$*", "*/*/*")]
    [DataRow("~$", '\'', 30, "'*~$*'~$*", "*~$*/*")]
    [DataRow("~$", '\'', 31, "*~$*~$*", "*/*/*")]
    [DataRow("~$", '\'', 31, "*~*~$*", "*~*/*")]
    public void TryReadFieldReturnsFieldsAcrossBlocks(string lineTerminator, char escapeChar, int length, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var value = new string('v', length);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content.Replace("*", value)));
        using var reader = new TabularReader(stream, dialect);

        var records = new List<string>();

        while (reader.TryMoveToNextRecord())
        {
            var fields = new List<string>();

            while (reader.TryReadField())
            {
                fields.Add(reader.CurrentFieldType == TabularFieldType.Comment ? "!" + reader.CurrentField.ToString() : reader.CurrentField.ToString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(expected.Replace("*", value).Split('/'), records);
    }

    [TestMethod]
    [DataRow("$", '\'', 30, "'*$*'^*", "*$*|*")]
    [DataRow("$", '\'', 30, "'*'$*$*", "*/*/*")]
    [DataRow("$", '\'', 30, "'*''*'^*", "*'*|*")]
    [DataRow("$", '\'', 30, "'*'^*^*", "*|*|*")]
    [DataRow("$", '\'', 30, "*^'*'^*", "*|*|*")]
    [DataRow("$", '\'', 31, "'*$*'$*", "*$*/*")]
    [DataRow("$", '\'', 31, "'*^*'^*", "*^*|*")]
    [DataRow("$", '\'', 31, "*$!*$*", "*/!*/*")]
    [DataRow("$", '\'', 31, "*$*$*", "*/*/*")]
    [DataRow("$", '\'', 31, "*^!*$*", "*|!*/*")]
    [DataRow("$", '\'', 31, "*^'*'^*", "*|*|*")]
    [DataRow("$", '\'', 31, "*^*^*", "*|*|*")]
    [DataRow("$", '\'', 32, "*$*$*", "*/*/*")]
    [DataRow("$", '\'', 32, "*^*^*", "*|*|*")]
    [DataRow("$", '\\', 30, "'*\\'*'^*", "*'*|*")]
    [DataRow("$", '\\', 30, "'*\\\\*'^*", "*\\*|*")]
    [DataRow("~$", '\'', 30, "'*'~$*~$*", "*/*/*")]
    [DataRow("~$", '\'', 30, "'*~$*'~$*", "*~$*/*")]
    [DataRow("~$", '\'', 31, "*~$*~$*", "*/*/*")]
    [DataRow("~$", '\'', 31, "*~*~$*", "*~*/*")]
    public async Task TryReadFieldReturnsFieldsAcrossBlocksAsync(string lineTerminator, char escapeChar, int length, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var value = new string('v', length);

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content.Replace("*", value)));
        await using var reader = new TabularReader(stream, dialect);

        var records = new List<string>();

        while (await reader.TryMoveToNextRecordAsync())
        {
            var fields = new List<string>();

            while (await reader.TryReadFieldAsync())
            {
                fields.Add(reader.CurrentFieldType == TabularFieldType.Comment ? "!" + reader.CurrentField.ToString() : reader.CurrentField.ToString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(expected.Replace("*", value).Split('/'), records);
    }

    [TestMethod]
    [DataRow(4097)]
    [DataRow(10000)]
    public void TryReadFieldReturnsAllFieldsOfRecordWithManyFields(int count)
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(new string('^', count - 1)));
        using var reader = new TabularReader(stream, dialect);

        var fields = new List<string>();

        Assert.IsTrue(reader.TryMoveToNextRecord());

        while (reader.TryReadField())
        {
            fields.Add(reader.CurrentField.ToString());
        }

        Assert.HasCount(count, fields);
        Assert.IsTrue(fields.TrueForAll(static x => x.Length == 0));
        Assert.IsFalse(reader.TryMoveToNextRecord());
    }

    [TestMethod]
    [DataRow(4097)]
    [DataRow(10000)]
    public async Task TryReadFieldReturnsAllFieldsOfRecordWithManyFieldsAsync(int count)
    {
        var dialect = CreateDialect("$", '\'');

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(new string('^', count - 1)));
        await using var reader = new TabularReader(stream, dialect);

        var fields = new List<string>();

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());

        while (await reader.TryReadFieldAsync())
        {
            fields.Add(reader.CurrentField.ToString());
        }

        Assert.HasCount(count, fields);
        Assert.IsTrue(fields.TrueForAll(static x => x.Length == 0));
        Assert.IsFalse(await reader.TryMoveToNextRecordAsync());
    }

    [TestMethod]
    [DataRow("$", '\'', "!v$w'", 4)]
    [DataRow("$", '\'', "'", 1)]
    [DataRow("$", '\'', "''!", 2)]
    [DataRow("$", '\'', "''!'", 2)]
    [DataRow("$", '\'', "''$'", 4)]
    [DataRow("$", '\'', "'''", 3)]
    [DataRow("$", '\'', "''^'", 4)]
    [DataRow("$", '\'', "''v", 2)]
    [DataRow("$", '\'', "''v'", 2)]
    [DataRow("$", '\'', "'v$w'x", 5)]
    [DataRow("$", '\'', "'v' ", 3)]
    [DataRow("$", '\'', "'v''", 4)]
    [DataRow("$", '\'', "v^'w'x^y", 5)]
    [DataRow("$", '\'', "v^w$x'", 5)]
    [DataRow("$", '\\', " 'v'", 1)]
    [DataRow("$", '\\', "'", 1)]
    [DataRow("$", '\\', "''!", 2)]
    [DataRow("$", '\\', "'''", 2)]
    [DataRow("$", '\\', "''\\", 2)]
    [DataRow("$", '\\', "''v", 2)]
    [DataRow("$", '\\', "'\\!'", 2)]
    [DataRow("$", '\\', "'\\", 2)]
    [DataRow("$", '\\', "'\\$'", 2)]
    [DataRow("$", '\\', "'\\^'", 2)]
    [DataRow("$", '\\', "'\\v'", 2)]
    [DataRow("$", '\\', "'v' ", 3)]
    [DataRow("$", '\\', "'v\\'", 4)]
    [DataRow("$", '\\', "'v\\w'", 3)]
    [DataRow("$", '\\', "'vwx", 4)]
    [DataRow("$", '\\', "\\'", 1)]
    [DataRow("$", '\\', "v$'w", 4)]
    [DataRow("$", '\\', "v'", 1)]
    [DataRow("$", '\\', "v^'wx", 5)]
    [DataRow("$", '\\', "vw'x", 2)]
    [DataRow("~$", '\'', "!v~$w'", 5)]
    [DataRow("~$", '\'', "$'", 1)]
    [DataRow("~$", '\'', "'", 1)]
    [DataRow("~$", '\'', "''!", 2)]
    [DataRow("~$", '\'', "''!'", 2)]
    [DataRow("~$", '\'', "''$", 2)]
    [DataRow("~$", '\'', "''$'", 2)]
    [DataRow("~$", '\'', "'''", 3)]
    [DataRow("~$", '\'', "''^'", 4)]
    [DataRow("~$", '\'', "''v", 2)]
    [DataRow("~$", '\'', "''v'", 2)]
    [DataRow("~$", '\'', "''~!", 3)]
    [DataRow("~$", '\'', "''~", 3)]
    [DataRow("~$", '\'', "''~'", 3)]
    [DataRow("~$", '\'', "''~^", 3)]
    [DataRow("~$", '\'', "''~v", 3)]
    [DataRow("~$", '\'', "''~~", 3)]
    [DataRow("~$", '\'', "v'", 1)]
    [DataRow("~$", '\'', "v^w~$x'", 6)]
    [DataRow("~$", '\\', "'", 1)]
    [DataRow("~$", '\\', "''!", 2)]
    [DataRow("~$", '\\', "''$", 2)]
    [DataRow("~$", '\\', "'''", 2)]
    [DataRow("~$", '\\', "''\\", 2)]
    [DataRow("~$", '\\', "''v", 2)]
    [DataRow("~$", '\\', "''~!", 3)]
    [DataRow("~$", '\\', "''~", 3)]
    [DataRow("~$", '\\', "''~'", 3)]
    [DataRow("~$", '\\', "''~\\", 3)]
    [DataRow("~$", '\\', "''~^", 3)]
    [DataRow("~$", '\\', "''~v", 3)]
    [DataRow("~$", '\\', "''~~", 3)]
    [DataRow("~$", '\\', "'\\!'", 2)]
    [DataRow("~$", '\\', "'\\", 2)]
    [DataRow("~$", '\\', "'\\$'", 2)]
    [DataRow("~$", '\\', "'\\^'", 2)]
    [DataRow("~$", '\\', "'\\v'", 2)]
    [DataRow("~$", '\\', "'\\~'", 2)]
    [DataRow("~$", '\\', "'v'~~$", 4)]
    [DataRow("~$", '\\', "\\'", 1)]
    [DataRow("~$", '\\', "~'", 1)]
    public void TryReadFieldThrowsTabularContentExceptionForInvalidContent(string lineTerminator, char escapeChar, string content, int position)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var exception = Assert.ThrowsExactly<TabularContentException>(() => ReadStream(reader));

        Assert.AreEqual(position, exception.Position);

        static void ReadStream(TabularReader reader)
        {
            while (reader.TryMoveToNextRecord())
            {
                while (reader.TryReadField())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow("$", '\'', "!v$w'", 4)]
    [DataRow("$", '\'', "'", 1)]
    [DataRow("$", '\'', "''!", 2)]
    [DataRow("$", '\'', "''!'", 2)]
    [DataRow("$", '\'', "''$'", 4)]
    [DataRow("$", '\'', "'''", 3)]
    [DataRow("$", '\'', "''^'", 4)]
    [DataRow("$", '\'', "''v", 2)]
    [DataRow("$", '\'', "''v'", 2)]
    [DataRow("$", '\'', "'v$w'x", 5)]
    [DataRow("$", '\'', "'v' ", 3)]
    [DataRow("$", '\'', "'v''", 4)]
    [DataRow("$", '\'', "v^'w'x^y", 5)]
    [DataRow("$", '\'', "v^w$x'", 5)]
    [DataRow("$", '\\', " 'v'", 1)]
    [DataRow("$", '\\', "'", 1)]
    [DataRow("$", '\\', "''!", 2)]
    [DataRow("$", '\\', "'''", 2)]
    [DataRow("$", '\\', "''\\", 2)]
    [DataRow("$", '\\', "''v", 2)]
    [DataRow("$", '\\', "'\\!'", 2)]
    [DataRow("$", '\\', "'\\", 2)]
    [DataRow("$", '\\', "'\\$'", 2)]
    [DataRow("$", '\\', "'\\^'", 2)]
    [DataRow("$", '\\', "'\\v'", 2)]
    [DataRow("$", '\\', "'v' ", 3)]
    [DataRow("$", '\\', "'v\\'", 4)]
    [DataRow("$", '\\', "'v\\w'", 3)]
    [DataRow("$", '\\', "'vwx", 4)]
    [DataRow("$", '\\', "\\'", 1)]
    [DataRow("$", '\\', "v$'w", 4)]
    [DataRow("$", '\\', "v'", 1)]
    [DataRow("$", '\\', "v^'wx", 5)]
    [DataRow("$", '\\', "vw'x", 2)]
    [DataRow("~$", '\'', "!v~$w'", 5)]
    [DataRow("~$", '\'', "$'", 1)]
    [DataRow("~$", '\'', "'", 1)]
    [DataRow("~$", '\'', "''!", 2)]
    [DataRow("~$", '\'', "''!'", 2)]
    [DataRow("~$", '\'', "''$", 2)]
    [DataRow("~$", '\'', "''$'", 2)]
    [DataRow("~$", '\'', "'''", 3)]
    [DataRow("~$", '\'', "''^'", 4)]
    [DataRow("~$", '\'', "''v", 2)]
    [DataRow("~$", '\'', "''v'", 2)]
    [DataRow("~$", '\'', "''~!", 3)]
    [DataRow("~$", '\'', "''~", 3)]
    [DataRow("~$", '\'', "''~'", 3)]
    [DataRow("~$", '\'', "''~^", 3)]
    [DataRow("~$", '\'', "''~v", 3)]
    [DataRow("~$", '\'', "''~~", 3)]
    [DataRow("~$", '\'', "v'", 1)]
    [DataRow("~$", '\'', "v^w~$x'", 6)]
    [DataRow("~$", '\\', "'", 1)]
    [DataRow("~$", '\\', "''!", 2)]
    [DataRow("~$", '\\', "''$", 2)]
    [DataRow("~$", '\\', "'''", 2)]
    [DataRow("~$", '\\', "''\\", 2)]
    [DataRow("~$", '\\', "''v", 2)]
    [DataRow("~$", '\\', "''~!", 3)]
    [DataRow("~$", '\\', "''~", 3)]
    [DataRow("~$", '\\', "''~'", 3)]
    [DataRow("~$", '\\', "''~\\", 3)]
    [DataRow("~$", '\\', "''~^", 3)]
    [DataRow("~$", '\\', "''~v", 3)]
    [DataRow("~$", '\\', "''~~", 3)]
    [DataRow("~$", '\\', "'\\!'", 2)]
    [DataRow("~$", '\\', "'\\", 2)]
    [DataRow("~$", '\\', "'\\$'", 2)]
    [DataRow("~$", '\\', "'\\^'", 2)]
    [DataRow("~$", '\\', "'\\v'", 2)]
    [DataRow("~$", '\\', "'\\~'", 2)]
    [DataRow("~$", '\\', "'v'~~$", 4)]
    [DataRow("~$", '\\', "\\'", 1)]
    [DataRow("~$", '\\', "~'", 1)]
    public async Task TryReadFieldThrowsTabularContentExceptionForInvalidContentAsync(string lineTerminator, char escapeChar, string content, int position)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar, '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        var exception = await Assert.ThrowsExactlyAsync<TabularContentException>(() => ReadStreamAsync(reader));

        Assert.AreEqual(position, exception.Position);

        static async Task ReadStreamAsync(TabularReader reader)
        {
            while (await reader.TryMoveToNextRecordAsync())
            {
                while (await reader.TryReadFieldAsync())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow(1, "1^'abc", 6)]
    [DataRow(1, "1^'y'z^2$3^4", 5)]
    [DataRow(1, "v^w$x^y'", 7)]
    [DataRow(16384, "1^'abc", 6)]
    [DataRow(16384, "1^'y'z^2$3^4", 5)]
    [DataRow(16384, "v^w$x^y'", 7)]
    public void ContentExceptionPositionIsIndependentOfBufferSize(int bufferSize, string content, int position)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var exception = Assert.ThrowsExactly<TabularContentException>(() => ReadStream(reader));

        Assert.AreEqual(position, exception.Position);

        static void ReadStream(TabularReader reader)
        {
            while (reader.TryMoveToNextRecord())
            {
                while (reader.TryReadField())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow(1, "1^'abc", 6)]
    [DataRow(1, "1^'y'z^2$3^4", 5)]
    [DataRow(1, "v^w$x^y'", 7)]
    [DataRow(16384, "1^'abc", 6)]
    [DataRow(16384, "1^'y'z^2$3^4", 5)]
    [DataRow(16384, "v^w$x^y'", 7)]
    public async Task ContentExceptionPositionIsIndependentOfBufferSizeAsync(int bufferSize, string content, int position)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        var exception = await Assert.ThrowsExactlyAsync<TabularContentException>(() => ReadStreamAsync(reader));

        Assert.AreEqual(position, exception.Position);

        static async Task ReadStreamAsync(TabularReader reader)
        {
            while (await reader.TryMoveToNextRecordAsync())
            {
                while (await reader.TryReadFieldAsync())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow("$", '\'', 30, "'*'v*", 32)]
    [DataRow("$", '\'', 31, "'*^*", 64)]
    [DataRow("$", '\'', 31, "*'*", 31)]
    [DataRow("$", '\'', 31, "*'v'^*", 31)]
    [DataRow("$", '\'', 32, "*^*'*", 65)]
    [DataRow("$", '\\', 30, "'*\\v*'", 32)]
    [DataRow("~$", '\'', 30, "'*'~v*", 33)]
    public void ContentExceptionPositionIsIndependentOfBlocks(string lineTerminator, char escapeChar, int length, string content, int position)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content.Replace("*", new string('v', length))));
        using var reader = new TabularReader(stream, dialect);

        var exception = Assert.ThrowsExactly<TabularContentException>(() => ReadStream(reader));

        Assert.AreEqual(position, exception.Position);

        static void ReadStream(TabularReader reader)
        {
            while (reader.TryMoveToNextRecord())
            {
                while (reader.TryReadField())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow("$", '\'', 30, "'*'v*", 32)]
    [DataRow("$", '\'', 31, "'*^*", 64)]
    [DataRow("$", '\'', 31, "*'*", 31)]
    [DataRow("$", '\'', 31, "*'v'^*", 31)]
    [DataRow("$", '\'', 32, "*^*'*", 65)]
    [DataRow("$", '\\', 30, "'*\\v*'", 32)]
    [DataRow("~$", '\'', 30, "'*'~v*", 33)]
    public async Task ContentExceptionPositionIsIndependentOfBlocksAsync(string lineTerminator, char escapeChar, int length, string content, int position)
    {
        var dialect = CreateDialect(lineTerminator, escapeChar);

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content.Replace("*", new string('v', length))));
        await using var reader = new TabularReader(stream, dialect);

        var exception = await Assert.ThrowsExactlyAsync<TabularContentException>(() => ReadStreamAsync(reader));

        Assert.AreEqual(position, exception.Position);

        static async Task ReadStreamAsync(TabularReader reader)
        {
            while (await reader.TryMoveToNextRecordAsync())
            {
                while (await reader.TryReadFieldAsync())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow(4097)]
    [DataRow(10000)]
    public void ContentExceptionPositionIsIndependentOfFieldCount(int count)
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(new string('^', count) + "v'"));
        using var reader = new TabularReader(stream, dialect);

        var exception = Assert.ThrowsExactly<TabularContentException>(() => ReadStream(reader));

        Assert.AreEqual(count + 1, exception.Position);

        static void ReadStream(TabularReader reader)
        {
            while (reader.TryMoveToNextRecord())
            {
                while (reader.TryReadField())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow(4097)]
    [DataRow(10000)]
    public async Task ContentExceptionPositionIsIndependentOfFieldCountAsync(int count)
    {
        var dialect = CreateDialect("$", '\'');

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(new string('^', count) + "v'"));
        await using var reader = new TabularReader(stream, dialect);

        var exception = await Assert.ThrowsExactlyAsync<TabularContentException>(() => ReadStreamAsync(reader));

        Assert.AreEqual(count + 1, exception.Position);

        static async Task ReadStreamAsync(TabularReader reader)
        {
            while (await reader.TryMoveToNextRecordAsync())
            {
                while (await reader.TryReadFieldAsync())
                {
                }
            }
        }
    }

    [TestMethod]
    public void ContentExceptionPositionExcludesByteOrderMark()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v'")]);
        using var reader = new TabularReader(stream, dialect, options);

        var exception = Assert.ThrowsExactly<TabularContentException>(() => reader.TryReadField());

        Assert.AreEqual(1, exception.Position);
    }

    [TestMethod]
    public async Task ContentExceptionPositionExcludesByteOrderMarkAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v'")]);
        await using var reader = new TabularReader(stream, dialect, options);

        var exception = await Assert.ThrowsExactlyAsync<TabularContentException>(() => reader.TryReadFieldAsync().AsTask());

        Assert.AreEqual(1, exception.Position);
    }

    [TestMethod]
    [DataRow("utf-16", "\u00e9'", 1)]
    [DataRow("utf-32", "v^w'", 3)]
    [DataRow("utf-8", "\u00e9'", 1)]
    [DataRow("utf-8", "\u00e9^w\u20ac'", 4)]
    [DataRow("utf-8", "\u20ac\u00e9'", 2)]
    public void ContentExceptionPositionCountsCharacters(string encodingName, string content, int position)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        using var stream = new MemoryStream(encoding.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var exception = Assert.ThrowsExactly<TabularContentException>(() => ReadStream(reader));

        Assert.AreEqual(position, exception.Position);

        static void ReadStream(TabularReader reader)
        {
            while (reader.TryReadField())
            {
            }
        }
    }

    [TestMethod]
    [DataRow("utf-16", "\u00e9'", 1)]
    [DataRow("utf-32", "v^w'", 3)]
    [DataRow("utf-8", "\u00e9'", 1)]
    [DataRow("utf-8", "\u00e9^w\u20ac'", 4)]
    [DataRow("utf-8", "\u20ac\u00e9'", 2)]
    public async Task ContentExceptionPositionCountsCharactersAsync(string encodingName, string content, int position)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        await using var stream = new MemoryStream(encoding.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        var exception = await Assert.ThrowsExactlyAsync<TabularContentException>(() => ReadStreamAsync(reader));

        Assert.AreEqual(position, exception.Position);

        static async Task ReadStreamAsync(TabularReader reader)
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }
    }

    [TestMethod]
    public void TryReadFieldReadsRfc4180Dialect()
    {
        var dialect = new TabularDialect("\r\n", ',', '"');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a,b\r\n\"c\"\"d\",\"e\r\nf,g\"\r\n,\r\n\r\nh\r\n"));
        using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryMoveToNextRecord())
        {
            var fields = new List<string>();

            while (reader.TryReadField())
            {
                fields.Add(reader.CurrentField.ToString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(["a|b", "c\"d|e\r\nf,g", "|", "", "h"], records);
    }

    [TestMethod]
    public async Task TryReadFieldReadsRfc4180DialectAsync()
    {
        var dialect = new TabularDialect("\r\n", ',', '"');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("a,b\r\n\"c\"\"d\",\"e\r\nf,g\"\r\n,\r\n\r\nh\r\n"));
        await using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryMoveToNextRecordAsync())
        {
            var fields = new List<string>();

            while (await reader.TryReadFieldAsync())
            {
                fields.Add(reader.CurrentField.ToString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual(["a|b", "c\"d|e\r\nf,g", "|", "", "h"], records);
    }

    [TestMethod]
    public void CurrentPositionTypeIsStartOfStreamBeforeRead()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        using var reader = new TabularReader(stream, dialect);

        Assert.AreEqual(TabularPositionType.StartOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    public void CurrentFieldTypeIsNoneBeforeRead()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        using var reader = new TabularReader(stream, dialect);

        Assert.AreEqual(TabularFieldType.None, reader.CurrentFieldType);
    }

    [TestMethod]
    [DataRow("$", "$")]
    [DataRow("$", "'v'")]
    [DataRow("$", "v")]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v")]
    [DataRow("~$", "~$")]
    public void TryMoveToNextRecordAtStartOfStreamMovesToFirstRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$")]
    [DataRow("$", "'v'")]
    [DataRow("$", "v")]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v")]
    [DataRow("~$", "~$")]
    public async Task TryMoveToNextRecordAtStartOfStreamMovesToFirstRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "'v$w'$x")]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v~$w")]
    public void TryMoveToNextRecordAtStartOfRecordMovesToNextRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();

        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "'v$w'$x")]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v~$w")]
    public async Task TryMoveToNextRecordAtStartOfRecordMovesToNextRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "'v$w'$")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("$", "v^w")]
    [DataRow("~$", "v~$")]
    public void TryMoveToNextRecordAtStartOfRecordReturnsFalseForLastRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();

        Assert.IsFalse(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "'v$w'$")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("$", "v^w")]
    [DataRow("~$", "v~$")]
    public async Task TryMoveToNextRecordAtStartOfRecordReturnsFalseForLastRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();

        Assert.IsFalse(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^'w$x'$y")]
    [DataRow("$", "v^w$x")]
    [DataRow("$", "v^w^x$y")]
    [DataRow("~$", "v^w~$x")]
    public void TryMoveToNextRecordAtDelimiterMovesToNextRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^'w$x'$y")]
    [DataRow("$", "v^w$x")]
    [DataRow("$", "v^w^x$y")]
    [DataRow("~$", "v^w~$x")]
    public async Task TryMoveToNextRecordAtDelimiterMovesToNextRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^")]
    [DataRow("$", "v^w")]
    [DataRow("$", "v^w$")]
    [DataRow("~$", "v^w~$")]
    public void TryMoveToNextRecordAtDelimiterReturnsFalseForLastRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsFalse(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^")]
    [DataRow("$", "v^w")]
    [DataRow("$", "v^w$")]
    [DataRow("~$", "v^w~$")]
    public async Task TryMoveToNextRecordAtDelimiterReturnsFalseForLastRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsFalse(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("~$", "v~$w")]
    [DataRow("~$", "v~$~$")]
    public void TryMoveToNextRecordAtEndOfRecordMovesToNextRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("~$", "v~$w")]
    [DataRow("~$", "v~$~$")]
    public async Task TryMoveToNextRecordAtEndOfRecordMovesToNextRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.StartOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$")]
    public void TryMoveToNextRecordAtEndOfRecordReturnsFalseForLastRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsFalse(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$")]
    public async Task TryMoveToNextRecordAtEndOfRecordReturnsFalseForLastRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsFalse(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$w")]
    public void TryMoveToNextRecordAtEndOfStreamReturnsFalse(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
        }

        Assert.IsFalse(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$w")]
    public async Task TryMoveToNextRecordAtEndOfStreamReturnsFalseAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
        }

        Assert.IsFalse(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "u$!c")]
    [DataRow("$", "u$v$")]
    [DataRow("$", "u^v")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "u~$!c~$")]
    [DataRow("~$", "v~$")]
    public void TryMoveToNextRecordAtEndOfStreamClearsCurrentField(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }

        Assert.AreEqual(TabularFieldType.None, reader.CurrentFieldType);
        Assert.IsTrue(reader.CurrentField.IsEmpty);
    }

    [TestMethod]
    [DataRow("$", "u$!c")]
    [DataRow("$", "u$v$")]
    [DataRow("$", "u^v")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "u~$!c~$")]
    [DataRow("~$", "v~$")]
    public async Task TryMoveToNextRecordAtEndOfStreamClearsCurrentFieldAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }

        Assert.AreEqual(TabularFieldType.None, reader.CurrentFieldType);
        Assert.IsTrue(reader.CurrentField.IsEmpty);
    }

    [TestMethod]
    [DataRow("$", "v1^'v2$v2'$v3^v4", "v1/v3")]
    [DataRow("$", "v1^v2$v3^v4", "v1/v3")]
    [DataRow("$", "v1^v2^v3$$v4", "v1//v4")]
    [DataRow("~$", "v1^v2~$v3^v4", "v1/v3")]
    public void TryMoveToNextRecordSkipsUnreadFields(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var fields = new List<string>();

        while (reader.TryMoveToNextRecord())
        {
            reader.TryReadField();
            fields.Add(reader.CurrentField.ToString());
        }

        Assert.AreSequenceEqual(expected.Split('/'), fields);
    }

    [TestMethod]
    [DataRow("$", "v1^'v2$v2'$v3^v4", "v1/v3")]
    [DataRow("$", "v1^v2$v3^v4", "v1/v3")]
    [DataRow("$", "v1^v2^v3$$v4", "v1//v4")]
    [DataRow("~$", "v1^v2~$v3^v4", "v1/v3")]
    public async Task TryMoveToNextRecordSkipsUnreadFieldsAsync(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        var fields = new List<string>();

        while (await reader.TryMoveToNextRecordAsync())
        {
            await reader.TryReadFieldAsync();

            fields.Add(reader.CurrentField.ToString());
        }

        Assert.AreSequenceEqual(expected.Split('/'), fields);
    }

    [TestMethod]
    [DataRow("$", "!x$v^w")]
    [DataRow("~$", "!x~$v^w")]
    public void TryMoveToNextRecordSetsFieldTypeOfFirstField(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
    }

    [TestMethod]
    [DataRow("$", "!x$v^w")]
    [DataRow("~$", "!x~$v^w")]
    public async Task TryMoveToNextRecordSetsFieldTypeOfFirstFieldAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
    }

    [TestMethod]
    [DataRow("$", "$", "")]
    [DataRow("$", "'v^w'", "v^w")]
    [DataRow("$", "v", "v")]
    [DataRow("$", "v$w", "v")]
    [DataRow("$", "v^w", "v")]
    [DataRow("~$", "v~$w", "v")]
    public void TryReadFieldAtStartOfStreamReadsFirstField(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "$", "")]
    [DataRow("$", "'v^w'", "v^w")]
    [DataRow("$", "v", "v")]
    [DataRow("$", "v$w", "v")]
    [DataRow("$", "v^w", "v")]
    [DataRow("~$", "v~$w", "v")]
    public async Task TryReadFieldAtStartOfStreamReadsFirstFieldAsync(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public void TryReadFieldAtStartOfStreamSetsPositionAfterFirstField(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public async Task TryReadFieldAtStartOfStreamSetsPositionAfterFirstFieldAsync(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$", "")]
    [DataRow("$", "'v^w'", "v^w")]
    [DataRow("$", "v", "v")]
    [DataRow("$", "v$w", "v")]
    [DataRow("$", "v^w", "v")]
    [DataRow("~$", "v~$w", "v")]
    public void TryReadFieldAtStartOfRecordReadsFirstField(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "$", "")]
    [DataRow("$", "'v^w'", "v^w")]
    [DataRow("$", "v", "v")]
    [DataRow("$", "v$w", "v")]
    [DataRow("$", "v^w", "v")]
    [DataRow("~$", "v~$w", "v")]
    public async Task TryReadFieldAtStartOfRecordReadsFirstFieldAsync(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public void TryReadFieldAtStartOfRecordSetsPositionAfterFirstField(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();
        reader.TryReadField();

        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public async Task TryReadFieldAtStartOfRecordSetsPositionAfterFirstFieldAsync(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();
        await reader.TryReadFieldAsync();

        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^", "")]
    [DataRow("$", "v^'w$x'", "w$x")]
    [DataRow("$", "v^w", "w")]
    [DataRow("$", "v^w$x", "w")]
    [DataRow("$", "v^w^x", "w")]
    [DataRow("~$", "v^w~$x", "w")]
    public void TryReadFieldAtDelimiterReadsNextField(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "v^", "")]
    [DataRow("$", "v^'w$x'", "w$x")]
    [DataRow("$", "v^w", "w")]
    [DataRow("$", "v^w$x", "w")]
    [DataRow("$", "v^w^x", "w")]
    [DataRow("~$", "v^w~$x", "w")]
    public async Task TryReadFieldAtDelimiterReadsNextFieldAsync(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "v^", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^'w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w$x", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w^x", TabularPositionType.Delimiter)]
    [DataRow("~$", "v^w~$x", TabularPositionType.EndOfRecord)]
    public void TryReadFieldAtDelimiterSetsPositionAfterNextField(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();
        reader.TryReadField();

        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^'w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w$x", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w^x", TabularPositionType.Delimiter)]
    [DataRow("~$", "v^w~$x", TabularPositionType.EndOfRecord)]
    public async Task TryReadFieldAtDelimiterSetsPositionAfterNextFieldAsync(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();
        await reader.TryReadFieldAsync();

        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$")]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("~$", "v~$w")]
    public void TryReadFieldAtEndOfRecordReturnsFalse(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsFalse(reader.TryReadField());
        Assert.AreEqual(TabularPositionType.EndOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$")]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("~$", "v~$w")]
    public async Task TryReadFieldAtEndOfRecordReturnsFalseAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsFalse(await reader.TryReadFieldAsync());
        Assert.AreEqual(TabularPositionType.EndOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^'w$x'$y", "w$x")]
    [DataRow("$", "v^w$x", "w")]
    [DataRow("~$", "v^w~$x", "w")]
    public void CurrentFieldKeepsLastReadFieldAtEndOfRecord(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();
        reader.TryReadField();
        reader.TryReadField();

        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "v^'w$x'$y", "w$x")]
    [DataRow("$", "v^w$x", "w")]
    [DataRow("~$", "v^w~$x", "w")]
    public async Task CurrentFieldKeepsLastReadFieldAtEndOfRecordAsync(string lineTerminator, string content, string expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();
        await reader.TryReadFieldAsync();
        await reader.TryReadFieldAsync();

        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "!x$v")]
    [DataRow("~$", "!x~$v")]
    public void TryReadFieldSetsFieldTypeOfReadField(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
    }

    [TestMethod]
    [DataRow("$", "!x$v")]
    [DataRow("~$", "!x~$v")]
    public async Task TryReadFieldSetsFieldTypeOfReadFieldAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
    }

    [TestMethod]
    [DataRow("$", "")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$w")]
    public void TryReadFieldAtEndOfStreamReturnsFalse(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
        }

        Assert.IsFalse(reader.TryReadField());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$w")]
    public async Task TryReadFieldAtEndOfStreamReturnsFalseAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
        }

        Assert.IsFalse(await reader.TryReadFieldAsync());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public void TrySkipFieldAtStartOfStreamSkipsFirstField(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TrySkipField());
        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public async Task TrySkipFieldAtStartOfStreamSkipsFirstFieldAsync(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TrySkipFieldAsync());
        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public void TrySkipFieldAtStartOfRecordSkipsFirstField(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();

        Assert.IsTrue(reader.TrySkipField());
        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "$", TabularPositionType.EndOfRecord)]
    [DataRow("$", "'v^w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v", TabularPositionType.EndOfStream)]
    [DataRow("$", "v$w", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w", TabularPositionType.Delimiter)]
    [DataRow("~$", "v~$w", TabularPositionType.EndOfRecord)]
    public async Task TrySkipFieldAtStartOfRecordSkipsFirstFieldAsync(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();

        Assert.IsTrue(await reader.TrySkipFieldAsync());
        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^'w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w$x", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w^x", TabularPositionType.Delimiter)]
    [DataRow("~$", "v^w~$x", TabularPositionType.EndOfRecord)]
    public void TrySkipFieldAtDelimiterSkipsNextField(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TrySkipField());
        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v^", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^'w$x'", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w", TabularPositionType.EndOfStream)]
    [DataRow("$", "v^w$x", TabularPositionType.EndOfRecord)]
    [DataRow("$", "v^w^x", TabularPositionType.Delimiter)]
    [DataRow("~$", "v^w~$x", TabularPositionType.EndOfRecord)]
    public async Task TrySkipFieldAtDelimiterSkipsNextFieldAsync(string lineTerminator, string content, TabularPositionType expectedPositionType)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsTrue(await reader.TrySkipFieldAsync());
        Assert.AreEqual(expectedPositionType, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$")]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("~$", "v~$w")]
    public void TrySkipFieldAtEndOfRecordReturnsFalse(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsFalse(reader.TrySkipField());
        Assert.AreEqual(TabularPositionType.EndOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "v$")]
    [DataRow("$", "v$$")]
    [DataRow("$", "v$w")]
    [DataRow("~$", "v~$w")]
    public async Task TrySkipFieldAtEndOfRecordReturnsFalseAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsFalse(await reader.TrySkipFieldAsync());
        Assert.AreEqual(TabularPositionType.EndOfRecord, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$w")]
    public void TrySkipFieldAtEndOfStreamReturnsFalse(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
        }

        Assert.IsFalse(reader.TrySkipField());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "")]
    [DataRow("$", "v")]
    [DataRow("$", "v$")]
    [DataRow("~$", "v~$w")]
    public async Task TrySkipFieldAtEndOfStreamReturnsFalseAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
        }

        Assert.IsFalse(await reader.TrySkipFieldAsync());
        Assert.AreEqual(TabularPositionType.EndOfStream, reader.CurrentPositionType);
    }

    [TestMethod]
    [DataRow("$", "!x$v")]
    [DataRow("~$", "!x~$v")]
    public void TrySkipFieldSetsFieldTypeOfSkippedField(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TrySkipField());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.IsTrue(reader.TrySkipField());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
    }

    [TestMethod]
    [DataRow("$", "!x$v")]
    [DataRow("~$", "!x~$v")]
    public async Task TrySkipFieldSetsFieldTypeOfSkippedFieldAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TrySkipFieldAsync());
        Assert.AreEqual(TabularFieldType.Comment, reader.CurrentFieldType);
        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.IsTrue(await reader.TrySkipFieldAsync());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
    }

    [TestMethod]
    [DataRow("$", "v^!w")]
    [DataRow("~$", "v^!w~$")]
    public void TryReadFieldReadsCommentPrefixAfterDelimiterAsValue(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
        Assert.AreEqual("!w", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "v^!w")]
    [DataRow("~$", "v^!w~$")]
    public async Task TryReadFieldReadsCommentPrefixAfterDelimiterAsValueAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
        Assert.AreEqual("!w", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "!x$'!w'^v")]
    [DataRow("$", "v$'!w'")]
    [DataRow("~$", "v~$'!w'~$")]
    public void TryReadFieldReadsQuotedCommentPrefixAsValue(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();
        reader.TryMoveToNextRecord();

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
        Assert.AreEqual("!w", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "!x$'!w'^v")]
    [DataRow("$", "v$'!w'")]
    [DataRow("~$", "v~$'!w'~$")]
    public async Task TryReadFieldReadsQuotedCommentPrefixAsValueAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();
        await reader.TryMoveToNextRecordAsync();

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(TabularFieldType.Value, reader.CurrentFieldType);
        Assert.AreEqual("!w", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("$", "$", 1)]
    [DataRow("$", "v", 1)]
    [DataRow("$", "v^'w$x'^y$z", 3)]
    [DataRow("$", "v^w^x", 3)]
    [DataRow("$", "v^w^x$", 3)]
    [DataRow("~$", "v^w^x~$", 3)]
    public void FieldsReadCountsReadFieldsOfRecord(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();

        while (reader.TryReadField())
        {
        }

        Assert.AreEqual(expected, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "$", 1)]
    [DataRow("$", "v", 1)]
    [DataRow("$", "v^'w$x'^y$z", 3)]
    [DataRow("$", "v^w^x", 3)]
    [DataRow("$", "v^w^x$", 3)]
    [DataRow("~$", "v^w^x~$", 3)]
    public async Task FieldsReadCountsReadFieldsOfRecordAsync(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();

        while (await reader.TryReadFieldAsync())
        {
        }

        Assert.AreEqual(expected, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "v^w^x")]
    [DataRow("~$", "v^w^x~$")]
    public void FieldsReadCountsSkippedFields(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TrySkipField();
        reader.TrySkipField();

        Assert.AreEqual(2, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "v^w^x")]
    [DataRow("~$", "v^w^x~$")]
    public async Task FieldsReadCountsSkippedFieldsAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TrySkipFieldAsync();
        await reader.TrySkipFieldAsync();

        Assert.AreEqual(2, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v^w~$x^y")]
    public void FieldsReadIsZeroAtStartOfRecord(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();
        reader.TryReadField();
        reader.TryMoveToNextRecord();

        Assert.AreEqual(0, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v^w~$x^y")]
    public async Task FieldsReadIsZeroAtStartOfRecordAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();
        await reader.TryReadFieldAsync();
        await reader.TryMoveToNextRecordAsync();

        Assert.AreEqual(0, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "!x^y")]
    [DataRow("~$", "!x^y~$v")]
    public void FieldsReadCountsCommentAsOneField(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryReadField())
        {
        }

        Assert.AreEqual(1, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "!x^y")]
    [DataRow("~$", "!x^y~$v")]
    public async Task FieldsReadCountsCommentAsOneFieldAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryReadFieldAsync())
        {
        }

        Assert.AreEqual(1, reader.FieldsRead);
    }

    [TestMethod]
    [DataRow("$", "", 0)]
    [DataRow("$", "$$", 2)]
    [DataRow("$", "'v$w'$x", 2)]
    [DataRow("$", "h1^h2$v1^v2", 2)]
    [DataRow("$", "h1^h2$v1^v2$", 2)]
    [DataRow("$", "v", 1)]
    [DataRow("~$", "h1^h2~$v1^v2~$", 2)]
    public void RecordsReadCountsRecords(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }

        Assert.AreEqual(expected, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", "", 0)]
    [DataRow("$", "$$", 2)]
    [DataRow("$", "'v$w'$x", 2)]
    [DataRow("$", "h1^h2$v1^v2", 2)]
    [DataRow("$", "h1^h2$v1^v2$", 2)]
    [DataRow("$", "v", 1)]
    [DataRow("~$", "h1^h2~$v1^v2~$", 2)]
    public async Task RecordsReadCountsRecordsAsync(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }

        Assert.AreEqual(expected, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", "v1^v2$v3^v4$v5", 3)]
    [DataRow("~$", "v1^v2~$v3^v4~$", 2)]
    public void RecordsReadCountsRecordsSkippedByTryMoveToNextRecord(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
        }

        Assert.AreEqual(expected, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", "v1^v2$v3^v4$v5", 3)]
    [DataRow("~$", "v1^v2~$v3^v4~$", 2)]
    public async Task RecordsReadCountsRecordsSkippedByTryMoveToNextRecordAsync(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
        }

        Assert.AreEqual(expected, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", "!x$!y", 0)]
    [DataRow("$", "!x$v1^v2$!y$v3$", 2)]
    [DataRow("$", "v$!x", 1)]
    [DataRow("~$", "!x~$v1^v2~$!y~$v3~$", 2)]
    public void RecordsReadExcludesComments(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }

        Assert.AreEqual(expected, reader.RecordsRead);
    }

    [TestMethod]
    [DataRow("$", "!x$!y", 0)]
    [DataRow("$", "!x$v1^v2$!y$v3$", 2)]
    [DataRow("$", "v$!x", 1)]
    [DataRow("~$", "!x~$v1^v2~$!y~$v3~$", 2)]
    public async Task RecordsReadExcludesCommentsAsync(string lineTerminator, string content, int expected)
    {
        var dialect = CreateDialect(lineTerminator, '\'', '!');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }

        Assert.AreEqual(expected, reader.RecordsRead);
    }

    [TestMethod]
    public void BytesConsumedIsZeroBeforeRead()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        using var reader = new TabularReader(stream, dialect);

        Assert.AreEqual(0, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow("$", "\u00e9^\ud83d\ude00")]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v^w~$x~$")]
    public void BytesConsumedCountsStreamBytesAfterReading(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow("$", "\u00e9^\ud83d\ude00")]
    [DataRow("$", "v^w$x")]
    [DataRow("~$", "v^w~$x~$")]
    public async Task BytesConsumedCountsStreamBytesAfterReadingAsync(string lineTerminator, string content)
    {
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow("utf-16")]
    [DataRow("utf-16BE")]
    [DataRow("utf-32")]
    [DataRow("utf-32BE")]
    public void BytesConsumedCountsEncodedBytes(string encodingName)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        using var stream = new MemoryStream(encoding.GetBytes("v^\u00e9$\ud83d\ude00"));
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow("utf-16")]
    [DataRow("utf-16BE")]
    [DataRow("utf-32")]
    [DataRow("utf-32BE")]
    public async Task BytesConsumedCountsEncodedBytesAsync(string encodingName)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        await using var stream = new MemoryStream(encoding.GetBytes("v^\u00e9$\ud83d\ude00"));
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(16384)]
    public void BytesConsumedIncludesPreamble(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v^w$x")]);
        using var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(16384)]
    public async Task BytesConsumedIncludesPreambleAsync(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v^w$x")]);
        await using var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }

        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(16384)]
    public void TryReadFieldSkipsPreambleWithDefaultEncoding(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v^w")]);
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("v", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(16384)]
    public async Task TryReadFieldSkipsPreambleWithDefaultEncodingAsync(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v^w")]);
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("v", reader.CurrentField.ToString());
    }

    [TestMethod]
    public void TryReadFieldKeepsPreambleAfterStartOfStream()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream([.. Encoding.UTF8.GetBytes("v$"), 0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("w")]);
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryMoveToNextRecord();
        reader.TryMoveToNextRecord();

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("\ufeffw", reader.CurrentField.ToString());
    }

    [TestMethod]
    public async Task TryReadFieldKeepsPreambleAfterStartOfStreamAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream([.. Encoding.UTF8.GetBytes("v$"), 0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("w")]);
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryMoveToNextRecordAsync();
        await reader.TryMoveToNextRecordAsync();

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("\ufeffw", reader.CurrentField.ToString());
    }

    [TestMethod]
    public void TryReadFieldKeepsPreambleWhenStreamIsNotAtStart()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream([.. Encoding.UTF8.GetBytes("v$"), 0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("w$")]);

        stream.Position = 2;

        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryMoveToNextRecord());
        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("\ufeffw", reader.CurrentField.ToString());
    }

    [TestMethod]
    public async Task TryReadFieldKeepsPreambleWhenStreamIsNotAtStartAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream([.. Encoding.UTF8.GetBytes("v$"), 0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("w$")]);

        stream.Position = 2;

        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryMoveToNextRecordAsync());
        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("\ufeffw", reader.CurrentField.ToString());
    }

    [TestMethod]
    public void TryReadFieldKeepsByteOrderMarkWhenEncodingHasNoPreamble()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = new UTF8Encoding(false) };

        using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v^w")]);
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("\ufeffv", reader.CurrentField.ToString());
    }

    [TestMethod]
    public async Task TryReadFieldKeepsByteOrderMarkWhenEncodingHasNoPreambleAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = new UTF8Encoding(false) };

        await using var stream = new MemoryStream([0xEF, 0xBB, 0xBF, .. Encoding.UTF8.GetBytes("v^w")]);
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("\ufeffv", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("utf-16")]
    [DataRow("utf-16BE")]
    [DataRow("utf-32")]
    [DataRow("utf-32BE")]
    [DataRow("utf-8")]
    public void TryReadFieldSkipsPreambleOfEncoding(string encodingName)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        using var stream = new MemoryStream([.. encoding.GetPreamble(), .. encoding.GetBytes("v^w")]);
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("v", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("utf-16")]
    [DataRow("utf-16BE")]
    [DataRow("utf-32")]
    [DataRow("utf-32BE")]
    [DataRow("utf-8")]
    public async Task TryReadFieldSkipsPreambleOfEncodingAsync(string encodingName)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        await using var stream = new MemoryStream([.. encoding.GetPreamble(), .. encoding.GetBytes("v^w")]);
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("v", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("utf-16", "$")]
    [DataRow("utf-16", "~$")]
    [DataRow("utf-16BE", "$")]
    [DataRow("utf-16BE", "~$")]
    [DataRow("utf-32", "$")]
    [DataRow("utf-32", "~$")]
    [DataRow("utf-32BE", "$")]
    [DataRow("utf-32BE", "~$")]
    [DataRow("utf-8", "$")]
    [DataRow("utf-8", "~$")]
    public void TryReadFieldDecodesCharactersSplitAcrossReads(string encodingName, string lineTerminator)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        using var stream = new MemoryStream(encoding.GetBytes($"\u00e9^'\ud83d\ude00{lineTerminator}\u20ac'{lineTerminator}\ud83d\ude00"));
        using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (reader.TryMoveToNextRecord())
        {
            var fields = new List<string>();

            while (reader.TryReadField())
            {
                fields.Add(reader.CurrentField.ToString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual([$"\u00e9|\ud83d\ude00{lineTerminator}\u20ac", "\ud83d\ude00"], records);
    }

    [TestMethod]
    [DataRow("utf-16", "$")]
    [DataRow("utf-16", "~$")]
    [DataRow("utf-16BE", "$")]
    [DataRow("utf-16BE", "~$")]
    [DataRow("utf-32", "$")]
    [DataRow("utf-32", "~$")]
    [DataRow("utf-32BE", "$")]
    [DataRow("utf-32BE", "~$")]
    [DataRow("utf-8", "$")]
    [DataRow("utf-8", "~$")]
    public async Task TryReadFieldDecodesCharactersSplitAcrossReadsAsync(string encodingName, string lineTerminator)
    {
        var encoding = Encoding.GetEncoding(encodingName);
        var dialect = CreateDialect(lineTerminator, '\'');
        var options = new TabularOptions { BufferSize = 1, Encoding = encoding };

        await using var stream = new MemoryStream(encoding.GetBytes($"\u00e9^'\ud83d\ude00{lineTerminator}\u20ac'{lineTerminator}\ud83d\ude00"));
        await using var reader = new TabularReader(stream, dialect, options);

        var records = new List<string>();

        while (await reader.TryMoveToNextRecordAsync())
        {
            var fields = new List<string>();

            while (await reader.TryReadFieldAsync())
            {
                fields.Add(reader.CurrentField.ToString());
            }

            records.Add(string.Join('|', fields));
        }

        Assert.AreSequenceEqual([$"\u00e9|\ud83d\ude00{lineTerminator}\u20ac", "\ud83d\ude00"], records);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public void TryReadFieldReplacesIncompleteCharacterAtEndOfStream(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream([0x76, 0x5E, 0x77, 0xC3]);
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("v", reader.CurrentField.ToString());
        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("w\ufffd", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    [DataRow(5)]
    public async Task TryReadFieldReplacesIncompleteCharacterAtEndOfStreamAsync(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream([0x76, 0x5E, 0x77, 0xC3]);
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("v", reader.CurrentField.ToString());
        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("w\ufffd", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void TryReadFieldReadsWithBufferSizeLessThanOne(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^'w$x'"));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("v", reader.CurrentField.ToString());
        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual("w$x", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public async Task TryReadFieldReadsWithBufferSizeLessThanOneAsync(int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^'w$x'"));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("v", reader.CurrentField.ToString());
        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual("w$x", reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("   ", "")]
    [DataRow("  v  ", "v")]
    [DataRow(" v w ", "v w")]
    [DataRow("' v '", "v")]
    [DataRow("\u3000v\u3000", "v")]
    public void CurrentFieldIsTrimmedWithTrimWhitespace(string content, string expected)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("   ", "")]
    [DataRow("  v  ", "v")]
    [DataRow(" v w ", "v w")]
    [DataRow("' v '", "v")]
    [DataRow("\u3000v\u3000", "v")]
    public async Task CurrentFieldIsTrimmedWithTrimWhitespaceAsync(string content, string expected)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1, TrimWhitespace = true };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("   ", "   ")]
    [DataRow("  v  ", "  v  ")]
    [DataRow("' v '", " v ")]
    [DataRow("\u3000v\u3000", "\u3000v\u3000")]
    public void CurrentFieldIsNotTrimmedWithoutTrimWhitespace(string content, string expected)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("   ", "   ")]
    [DataRow("  v  ", "  v  ")]
    [DataRow("' v '", " v ")]
    [DataRow("\u3000v\u3000", "\u3000v\u3000")]
    public async Task CurrentFieldIsNotTrimmedWithoutTrimWhitespaceAsync(string content, string expected)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();

        Assert.AreEqual(expected, reader.CurrentField.ToString());
    }

    [TestMethod]
    [DataRow("1^x$2^'y'z$3^z$", 1)]
    [DataRow("1^x$2^'y'z$3^z$", 16384)]
    [DataRow("1^x$2^'yz$", 1)]
    [DataRow("1^x$2^'yz$", 16384)]
    public void ReaderIsFaultedAfterContentException(string content, int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.ThrowsExactly<TabularContentException>(() => ReadStream(reader));

        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryMoveToNextRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryReadField());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TrySkipField());
        Assert.ThrowsExactly<InvalidOperationException>(() => _ = reader.CurrentField);
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryGetString(out _));
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryGetInt32(out _));

        static void ReadStream(TabularReader reader)
        {
            while (reader.TryMoveToNextRecord())
            {
                while (reader.TryReadField())
                {
                }
            }
        }
    }

    [TestMethod]
    [DataRow("1^x$2^'y'z$3^z$", 1)]
    [DataRow("1^x$2^'y'z$3^z$", 16384)]
    [DataRow("1^x$2^'yz$", 1)]
    [DataRow("1^x$2^'yz$", 16384)]
    public async Task ReaderIsFaultedAfterContentExceptionAsync(string content, int bufferSize)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = bufferSize };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        await Assert.ThrowsExactlyAsync<TabularContentException>(() => ReadStreamAsync(reader));
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryMoveToNextRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryReadFieldAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TrySkipFieldAsync().AsTask());

        Assert.ThrowsExactly<InvalidOperationException>(() => _ = reader.CurrentField);
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryGetString(out _));
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryGetInt32(out _));

        static async Task ReadStreamAsync(TabularReader reader)
        {
            while (await reader.TryMoveToNextRecordAsync())
            {
                while (await reader.TryReadFieldAsync())
                {
                }
            }
        }
    }

    [TestMethod]
    public void ReaderIsFaultedAfterStreamException()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new FailingReadStream();
        using var reader = new TabularReader(stream, dialect);

        Assert.ThrowsExactly<IOException>(() => reader.TryReadField());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryMoveToNextRecord());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TryReadField());
        Assert.ThrowsExactly<InvalidOperationException>(() => reader.TrySkipField());
    }

    [TestMethod]
    public async Task ReaderIsFaultedAfterStreamExceptionAsync()
    {
        var dialect = CreateDialect("$", '\'');

        await using var stream = new FailingReadStream();
        await using var reader = new TabularReader(stream, dialect);

        await Assert.ThrowsExactlyAsync<IOException>(() => reader.TryReadFieldAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryMoveToNextRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryReadFieldAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TrySkipFieldAsync().AsTask());
    }

    [TestMethod]
    public async Task TryMoveToNextRecordThrowsOperationCanceledExceptionForCanceledTokenAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        await using var reader = new TabularReader(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => reader.TryMoveToNextRecordAsync(new CancellationToken(true)).AsTask());
    }

    [TestMethod]
    public async Task TryReadFieldThrowsOperationCanceledExceptionForCanceledTokenAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        await using var reader = new TabularReader(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => reader.TryReadFieldAsync(new CancellationToken(true)).AsTask());
    }

    [TestMethod]
    public async Task TrySkipFieldThrowsOperationCanceledExceptionForCanceledTokenAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        await using var reader = new TabularReader(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => reader.TrySkipFieldAsync(new CancellationToken(true)).AsTask());
    }

    [TestMethod]
    public async Task ReaderIsFaultedAfterCanceledReadAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        await using var reader = new TabularReader(stream, dialect, options);

        await Assert.ThrowsAsync<OperationCanceledException>(() => reader.TryReadFieldAsync(new CancellationToken(true)).AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryMoveToNextRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TryReadFieldAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => reader.TrySkipFieldAsync().AsTask());
    }

    [TestMethod]
    public void ReaderThrowsObjectDisposedExceptionAfterDispose()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^w"));

        var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();
        reader.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryMoveToNextRecord());
        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryReadField());
        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TrySkipField());
        Assert.ThrowsExactly<ObjectDisposedException>(() => _ = reader.CurrentField);
        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryGetString(out _));
        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryGetInt32(out _));
    }

    [TestMethod]
    public async Task ReaderThrowsObjectDisposedExceptionAfterDisposeAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^w"));

        var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();
        await reader.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => reader.TryMoveToNextRecordAsync().AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => reader.TryReadFieldAsync().AsTask());
        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => reader.TrySkipFieldAsync().AsTask());
        Assert.ThrowsExactly<ObjectDisposedException>(() => _ = reader.CurrentField);
        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryGetString(out _));
        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryGetInt32(out _));
    }

    [TestMethod]
    public void DisposeCanBeCalledTwice()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));

        var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();
        reader.Dispose();
        reader.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => reader.TryReadField());
    }

    [TestMethod]
    public async Task DisposeCanBeCalledTwiceAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));

        var reader = new TabularReader(stream, dialect, options);

        await reader.TryReadFieldAsync();
        await reader.DisposeAsync();
        await reader.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => reader.TryReadFieldAsync().AsTask());
    }

    [TestMethod]
    public void DisposeLeavesStreamOpen()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));

        var reader = new TabularReader(stream, dialect);

        reader.TryReadField();
        reader.Dispose();

        Assert.IsTrue(stream.CanRead);
    }

    [TestMethod]
    public async Task DisposeLeavesStreamOpenAsync()
    {
        var dialect = CreateDialect("$", '\'');

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));

        var reader = new TabularReader(stream, dialect);

        await reader.TryReadFieldAsync();
        await reader.DisposeAsync();

        Assert.IsTrue(stream.CanRead);
    }

    [TestMethod]
    public void CountersAreReadableAfterDispose()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^w$x"));

        var reader = new TabularReader(stream, dialect, options);

        while (reader.TryMoveToNextRecord())
        {
            while (reader.TryReadField())
            {
            }
        }

        reader.Dispose();

        Assert.AreEqual(2, reader.RecordsRead);
        Assert.AreEqual(1, reader.FieldsRead);
        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    public async Task CountersAreReadableAfterDisposeAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { BufferSize = 1 };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^w$x"));

        var reader = new TabularReader(stream, dialect, options);

        while (await reader.TryMoveToNextRecordAsync())
        {
            while (await reader.TryReadFieldAsync())
            {
            }
        }

        await reader.DisposeAsync();

        Assert.AreEqual(2, reader.RecordsRead);
        Assert.AreEqual(1, reader.FieldsRead);
        Assert.AreEqual(stream.Length, reader.BytesConsumed);
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullStream()
    {
        var dialect = CreateDialect("$", '\'');

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularReader(null!, dialect));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentNullExceptionForNullDialect()
    {
        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentNullException>(() => new TabularReader(stream, null!));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForUnreadableStream()
    {
        var dialect = CreateDialect("$", '\'');

        using var stream = new MemoryStream();

        stream.Dispose();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularReader(stream, dialect));
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
