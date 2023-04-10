using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularDataDialectTests
{
    [DataTestMethod]
    [DataRow("", 'd', 'q', 'e', 'c')]
    [DataRow("\u0000", 'd', 'q', 'e', 'c')]
    [DataRow("\u000a\u000a", 'd', 'q', 'e', 'c')]
    [DataRow("\u000a\u000d", 'd', 'q', 'e', 'c')]
    [DataRow("\u000d\u000d", 'd', 'q', 'e', 'c')]
    [DataRow("\u0030", 'd', 'q', 'e', 'c')]
    [DataRow("\u0031", 'd', 'q', 'e', 'c')]
    [DataRow("\u000a", '\u000a', 'q', 'e', 'c')]
    [DataRow("\u000a", '\u000b', 'q', 'e', 'c')]
    [DataRow("\u000a", '\u000c', 'q', 'e', 'c')]
    [DataRow("\u000a", '\u000d', 'q', 'e', 'c')]
    [DataRow("\u000a", '\u0085', 'q', 'e', 'c')]
    [DataRow("\u000a", '\u2028', 'q', 'e', 'c')]
    [DataRow("\u000a", '\u2029', 'q', 'e', 'c')]
    [DataRow("\u000a", 'd', '\u000a', 'e', 'c')]
    [DataRow("\u000a", 'd', '\u000b', 'e', 'c')]
    [DataRow("\u000a", 'd', '\u000c', 'e', 'c')]
    [DataRow("\u000a", 'd', '\u000d', 'e', 'c')]
    [DataRow("\u000a", 'd', '\u0085', 'e', 'c')]
    [DataRow("\u000a", 'd', '\u2028', 'e', 'c')]
    [DataRow("\u000a", 'd', '\u2029', 'e', 'c')]
    [DataRow("\u000a", 'd', 'q', '\u000a', 'c')]
    [DataRow("\u000a", 'd', 'q', '\u000b', 'c')]
    [DataRow("\u000a", 'd', 'q', '\u000c', 'c')]
    [DataRow("\u000a", 'd', 'q', '\u000d', 'c')]
    [DataRow("\u000a", 'd', 'q', '\u0085', 'c')]
    [DataRow("\u000a", 'd', 'q', '\u2028', 'c')]
    [DataRow("\u000a", 'd', 'q', '\u2029', 'c')]
    [DataRow("\u000a", 'd', 'd', 'e', 'c')]
    [DataRow("\u000a", 'd', 'q', 'd', 'c')]
    [DataRow("\u000d\u000a", 'd', 'q', 'e', 'd')]
    [DataRow("\u000d\u000a", 'd', 'q', 'e', 'q')]
    [DataRow("\u000d\u000a", 'd', 'q', 'e', 'e')]
    public void TokenSetNotSupported5(string lineTerminator, char delimiter, char quoteChar, char escapeChar, char commentPrefix)
    {
        Assert.ThrowsException<ArgumentException>(
            () => new TabularDataDialect(lineTerminator, delimiter, quoteChar, escapeChar, commentPrefix));
    }

    [DataTestMethod]
    [DataRow("", 'd', 'q', 'e')]
    [DataRow("\u0000", 'd', 'q', 'e')]
    [DataRow("\u000a\u000a", 'd', 'q', 'e')]
    [DataRow("\u000a\u000d", 'd', 'q', 'e')]
    [DataRow("\u000d\u000d", 'd', 'q', 'e')]
    [DataRow("\u0030", 'd', 'q', 'e')]
    [DataRow("\u0031", 'd', 'q', 'e')]
    [DataRow("\u000a", '\u000a', 'q', 'e')]
    [DataRow("\u000a", '\u000b', 'q', 'e')]
    [DataRow("\u000a", '\u000c', 'q', 'e')]
    [DataRow("\u000a", '\u000d', 'q', 'e')]
    [DataRow("\u000a", '\u0085', 'q', 'e')]
    [DataRow("\u000a", '\u2028', 'q', 'e')]
    [DataRow("\u000a", '\u2029', 'q', 'e')]
    [DataRow("\u000a", 'd', '\u000a', 'e')]
    [DataRow("\u000a", 'd', '\u000b', 'e')]
    [DataRow("\u000a", 'd', '\u000c', 'e')]
    [DataRow("\u000a", 'd', '\u000d', 'e')]
    [DataRow("\u000a", 'd', '\u0085', 'e')]
    [DataRow("\u000a", 'd', '\u2028', 'e')]
    [DataRow("\u000a", 'd', '\u2029', 'e')]
    [DataRow("\u000a", 'd', 'q', '\u000a')]
    [DataRow("\u000a", 'd', 'q', '\u000b')]
    [DataRow("\u000a", 'd', 'q', '\u000c')]
    [DataRow("\u000a", 'd', 'q', '\u000d')]
    [DataRow("\u000a", 'd', 'q', '\u0085')]
    [DataRow("\u000a", 'd', 'q', '\u2028')]
    [DataRow("\u000a", 'd', 'q', '\u2029')]
    [DataRow("\u000a", 'd', 'd', 'e')]
    [DataRow("\u000a", 'd', 'q', 'd')]
    public void TokenSetNotSupported4(string lineTerminator, char delimiter, char quoteChar, char escapeChar)
    {
        Assert.ThrowsException<ArgumentException>(
            () => new TabularDataDialect(lineTerminator, delimiter, quoteChar, escapeChar));
    }

    [DataTestMethod]
    [DataRow("\u000a", 'd', 'q', 'e', 'c')]
    [DataRow("\u000b", 'd', 'q', 'e', 'c')]
    [DataRow("\u000c", 'd', 'q', 'e', 'c')]
    [DataRow("\u000d", 'd', 'q', 'e', 'c')]
    [DataRow("\u0085", 'd', 'q', 'e', 'c')]
    [DataRow("\u2028", 'd', 'q', 'e', 'c')]
    [DataRow("\u2029", 'd', 'q', 'e', 'c')]
    [DataRow("\u000d\u000a", 'd', 'q', 'e', 'c')]
    public void TokenSetSupported5(string lineTerminator, char delimiter, char quoteChar, char escapeChar, char commentPrefix)
    {
        var dialect = new TabularDataDialect(lineTerminator, delimiter, quoteChar, escapeChar, commentPrefix);

        Assert.AreEqual(lineTerminator, dialect.LineTerminator);
        Assert.AreEqual(delimiter, dialect.Delimiter);
        Assert.AreEqual(quoteChar, dialect.QuoteChar);
        Assert.AreEqual(escapeChar, dialect.EscapeChar);
        Assert.AreEqual(commentPrefix, dialect.CommentPrefix);
    }

    [DataTestMethod]
    [DataRow("\u000a", 'd', 'q', 'e')]
    [DataRow("\u000b", 'd', 'q', 'e')]
    [DataRow("\u000c", 'd', 'q', 'e')]
    [DataRow("\u000d", 'd', 'q', 'e')]
    [DataRow("\u0085", 'd', 'q', 'e')]
    [DataRow("\u2028", 'd', 'q', 'e')]
    [DataRow("\u2029", 'd', 'q', 'e')]
    [DataRow("\u000d\u000a", 'd', 'q', 'e')]
    public void TokenSetSupported4(string lineTerminator, char delimiter, char quoteChar, char escapeChar)
    {
        var dialect = new TabularDataDialect(lineTerminator, delimiter, quoteChar, escapeChar);

        Assert.AreEqual(lineTerminator, dialect.LineTerminator);
        Assert.AreEqual(delimiter, dialect.Delimiter);
        Assert.AreEqual(quoteChar, dialect.QuoteChar);
        Assert.AreEqual(escapeChar, dialect.EscapeChar);
    }

    [DataTestMethod]
    [DataRow("\u000a", 'd', 'q')]
    [DataRow("\u000b", 'd', 'q')]
    [DataRow("\u000c", 'd', 'q')]
    [DataRow("\u000d", 'd', 'q')]
    [DataRow("\u0085", 'd', 'q')]
    [DataRow("\u2028", 'd', 'q')]
    [DataRow("\u2029", 'd', 'q')]
    [DataRow("\u000d\u000a", 'd', 'q')]
    public void TokenSetSupported3(string lineTerminator, char delimiter, char quoteChar)
    {
        var dialect = new TabularDataDialect(lineTerminator, delimiter, quoteChar);

        Assert.AreEqual(lineTerminator, dialect.LineTerminator);
        Assert.AreEqual(delimiter, dialect.Delimiter);
        Assert.AreEqual(quoteChar, dialect.QuoteChar);
    }
}
