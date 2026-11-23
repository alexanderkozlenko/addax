namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularDialectTests
{
    [TestMethod]
    [DataRow("$", '^', '\'')]
    [DataRow("$~", ';', '*')]
    [DataRow("@", '|', '\'')]
    [DataRow("~$", '^', '\'')]
    public void ConstructorWithQuoteCharSetsProperties(string lineTerminator, char delimiter, char quoteChar)
    {
        var dialect = new TabularDialect(lineTerminator, delimiter, quoteChar);

        Assert.AreEqual(lineTerminator, dialect.LineTerminator);
        Assert.AreEqual(delimiter, dialect.Delimiter);
        Assert.AreEqual(quoteChar, dialect.QuoteChar);
        Assert.AreEqual(quoteChar, dialect.EscapeChar);
        Assert.IsNull(dialect.CommentPrefix);
    }

    [TestMethod]
    [DataRow("$", '^', '\'', '\\')]
    [DataRow("$~", ';', '*', '\\')]
    [DataRow("@", '|', '\'', '\'')]
    [DataRow("~$", '^', '\'', '\'')]
    public void ConstructorWithEscapeCharSetsProperties(string lineTerminator, char delimiter, char quoteChar, char escapeChar)
    {
        var dialect = new TabularDialect(lineTerminator, delimiter, quoteChar, escapeChar);

        Assert.AreEqual(lineTerminator, dialect.LineTerminator);
        Assert.AreEqual(delimiter, dialect.Delimiter);
        Assert.AreEqual(quoteChar, dialect.QuoteChar);
        Assert.AreEqual(escapeChar, dialect.EscapeChar);
        Assert.IsNull(dialect.CommentPrefix);
    }

    [TestMethod]
    [DataRow("$", '^', '\'', '\\', '!')]
    [DataRow("$~", ';', '*', '\\', '@')]
    [DataRow("@", '|', '\'', '\'', '!')]
    [DataRow("~$", '^', '\'', '\'', '!')]
    public void ConstructorWithCommentPrefixSetsProperties(string lineTerminator, char delimiter, char quoteChar, char escapeChar, char commentPrefix)
    {
        var dialect = new TabularDialect(lineTerminator, delimiter, quoteChar, escapeChar, commentPrefix);

        Assert.AreEqual(lineTerminator, dialect.LineTerminator);
        Assert.AreEqual(delimiter, dialect.Delimiter);
        Assert.AreEqual(quoteChar, dialect.QuoteChar);
        Assert.AreEqual(escapeChar, dialect.EscapeChar);
        Assert.AreEqual(commentPrefix, dialect.CommentPrefix);
    }

    [TestMethod]
    public void ConstructorWithQuoteCharThrowsForNullLineTerminator()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(() => new TabularDialect(null!, '^', '\''));

        Assert.AreEqual("lineTerminator", exception.ParamName);
    }

    [TestMethod]
    public void ConstructorWithEscapeCharThrowsForNullLineTerminator()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(() => new TabularDialect(null!, '^', '\'', '\\'));

        Assert.AreEqual("lineTerminator", exception.ParamName);
    }

    [TestMethod]
    public void ConstructorWithCommentPrefixThrowsForNullLineTerminator()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(() => new TabularDialect(null!, '^', '\'', '\\', '!'));

        Assert.AreEqual("lineTerminator", exception.ParamName);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("$$")]
    [DataRow("~$$")]
    [DataRow("~$~")]
    [DataRow("~~")]
    public void ConstructorWithQuoteCharThrowsForInvalidLineTerminator(string lineTerminator)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', '\''));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("$$")]
    [DataRow("~$$")]
    [DataRow("~$~")]
    [DataRow("~~")]
    public void ConstructorWithEscapeCharThrowsForInvalidLineTerminator(string lineTerminator)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', '\'', '\\'));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("$$")]
    [DataRow("~$$")]
    [DataRow("~$~")]
    [DataRow("~~")]
    public void ConstructorWithCommentPrefixThrowsForInvalidLineTerminator(string lineTerminator)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', '\'', '\\', '!'));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithQuoteCharThrowsForDelimiterInLineTerminator(string lineTerminator, char delimiter)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, delimiter, '\''));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithEscapeCharThrowsForDelimiterInLineTerminator(string lineTerminator, char delimiter)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, delimiter, '\'', '\\'));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithCommentPrefixThrowsForDelimiterInLineTerminator(string lineTerminator, char delimiter)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, delimiter, '\'', '\\', '!'));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("$", '^')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithQuoteCharThrowsForQuoteCharEqualToPrecedingToken(string lineTerminator, char quoteChar)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', quoteChar));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("$", '^')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithEscapeCharThrowsForQuoteCharEqualToPrecedingToken(string lineTerminator, char quoteChar)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', quoteChar, '\\'));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("$", '^')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithCommentPrefixThrowsForQuoteCharEqualToPrecedingToken(string lineTerminator, char quoteChar)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', quoteChar, '\\', '!'));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("$", '^')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithEscapeCharThrowsForEscapeCharEqualToPrecedingToken(string lineTerminator, char escapeChar)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', '\'', escapeChar));
    }

    [TestMethod]
    [DataRow("$", '$')]
    [DataRow("$", '^')]
    [DataRow("~$", '$')]
    [DataRow("~$", '~')]
    public void ConstructorWithCommentPrefixThrowsForEscapeCharEqualToPrecedingToken(string lineTerminator, char escapeChar)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', '\'', escapeChar, '!'));
    }

    [TestMethod]
    [DataRow("$", '\'', '\'')]
    [DataRow("$", '\\', '$')]
    [DataRow("$", '\\', '\'')]
    [DataRow("$", '\\', '\\')]
    [DataRow("$", '\\', '^')]
    [DataRow("~$", '\\', '$')]
    [DataRow("~$", '\\', '~')]
    public void ConstructorWithCommentPrefixThrowsForCommentPrefixEqualToPrecedingToken(string lineTerminator, char escapeChar, char commentPrefix)
    {
        Assert.ThrowsExactly<ArgumentException>(() => new TabularDialect(lineTerminator, '^', '\'', escapeChar, commentPrefix));
    }
}
