using System.Globalization;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular.UnitTests;

public sealed partial class TabularReaderTests
{
    [TestMethod]
    [DataRow("''", "")]
    [DataRow("v", "v")]
    public void TryGetStringReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetString(out var result));
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow(" v ", "v")]
    public void TryGetStringWithTrimWhitespaceReturnsTrimmedValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { TrimWhitespace = true };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetString(out var result));
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("v", "v")]
    public void GetStringReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.GetString());
    }

    [TestMethod]
    [DataRow("0", false)]
    [DataRow("1", true)]
    [DataRow("false", false)]
    [DataRow("true", true)]
    public void TryGetBooleanReturnsValue(string content, bool expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBoolean(out var result));
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow(" true")]
    [DataRow("''")]
    [DataRow("+1")]
    [DataRow("-0")]
    [DataRow("01")]
    [DataRow("2")]
    [DataRow("FALSE")]
    [DataRow("TRUE")]
    [DataRow("True")]
    [DataRow("f")]
    [DataRow("no")]
    [DataRow("t")]
    [DataRow("true ")]
    [DataRow("yes")]
    public void TryGetBooleanReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBoolean(out _));
    }

    [TestMethod]
    [DataRow("true", true)]
    public void GetBooleanReturnsValue(string content, bool expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.GetBoolean());
    }

    [TestMethod]
    [DataRow("True")]
    public void GetBooleanThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBoolean());
    }

    [TestMethod]
    [DataRow(" ", ' ')]
    [DataRow("\u00e9", '\u00e9')]
    [DataRow("v", 'v')]
    public void TryGetCharReturnsValue(string content, char expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetChar(out var result));
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("''")]
    [DataRow("\ud83d\udd2e")]
    [DataRow("ab")]
    public void TryGetCharReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetChar(out _));
    }

    [TestMethod]
    [DataRow(" v ", 'v')]
    public void TryGetCharWithTrimWhitespaceReturnsValue(string content, char expected)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { TrimWhitespace = true };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetChar(out var result));
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    [DataRow("   ")]
    [DataRow(" ")]
    public void TryGetCharWithTrimWhitespaceReturnsFalseForBlankField(string content)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { TrimWhitespace = true };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetChar(out _));
    }

    [TestMethod]
    [DataRow("v", 'v')]
    public void GetCharReturnsValue(string content, char expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(expected, reader.GetChar());
    }

    [TestMethod]
    [DataRow("ab")]
    public void GetCharThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetChar());
    }

    [TestMethod]
    [DataRow("\u00e9", "\u00e9")]
    [DataRow("\ud83d\udd2e", "\ud83d\udd2e")]
    [DataRow("v", "v")]
    public void TryGetRuneReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetRune(out var result));
        Assert.AreEqual(Rune.GetRuneAt(expected, 0), result);
    }

    [TestMethod]
    [DataRow("''")]
    [DataRow("\ud83d\udd2e\ud83d\udd2e")]
    [DataRow("ab")]
    [DataRow("v\ud83d\udd2e")]
    public void TryGetRuneReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetRune(out _));
    }

    [TestMethod]
    [DataRow("\ud83d\udd2e", "\ud83d\udd2e")]
    public void GetRuneReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(Rune.GetRuneAt(expected, 0), reader.GetRune());
    }

    [TestMethod]
    [DataRow("ab")]
    public void GetRuneThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetRune());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+127", "127")]
    [DataRow("-0", "0")]
    [DataRow("-128", "-128")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetSByteReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetSByte(out var result));
        Assert.AreEqual(sbyte.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-129")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,23")]
    [DataRow("1.0")]
    [DataRow("128")]
    [DataRow("12a")]
    [DataRow("1e1")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetSByteReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetSByte(out _));
    }

    [TestMethod]
    [DataRow("-128", "-128")]
    public void GetSByteReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(sbyte.Parse(expected, CultureInfo.InvariantCulture), reader.GetSByte());
    }

    [TestMethod]
    [DataRow("128")]
    public void GetSByteThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetSByte());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+255", "255")]
    [DataRow("-0", "0")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetByteReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetByte(out var result));
        Assert.AreEqual(byte.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-1")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,23")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e1")]
    [DataRow("256")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetByteReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetByte(out _));
    }

    [TestMethod]
    [DataRow("255", "255")]
    public void GetByteReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(byte.Parse(expected, CultureInfo.InvariantCulture), reader.GetByte());
    }

    [TestMethod]
    [DataRow("256")]
    public void GetByteThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetByte());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+32767", "32767")]
    [DataRow("-0", "0")]
    [DataRow("-32768", "-32768")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetInt16ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetInt16(out var result));
        Assert.AreEqual(short.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-32769")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e3")]
    [DataRow("32768")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetInt16ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetInt16(out _));
    }

    [TestMethod]
    [DataRow("-32768", "-32768")]
    public void GetInt16ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(short.Parse(expected, CultureInfo.InvariantCulture), reader.GetInt16());
    }

    [TestMethod]
    [DataRow("32768")]
    public void GetInt16ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetInt16());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+65535", "65535")]
    [DataRow("-0", "0")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetUInt16ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetUInt16(out var result));
        Assert.AreEqual(ushort.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-1")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e3")]
    [DataRow("50%")]
    [DataRow("65536")]
    [DataRow("\u0661\u0662")]
    public void TryGetUInt16ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetUInt16(out _));
    }

    [TestMethod]
    [DataRow("65535", "65535")]
    public void GetUInt16ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(ushort.Parse(expected, CultureInfo.InvariantCulture), reader.GetUInt16());
    }

    [TestMethod]
    [DataRow("-1")]
    public void GetUInt16ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetUInt16());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+2147483647", "2147483647")]
    [DataRow("-0", "0")]
    [DataRow("-2147483648", "-2147483648")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    [DataRow("12", "12")]
    public void TryGetInt32ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetInt32(out var result));
        Assert.AreEqual(int.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-2147483649")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e3")]
    [DataRow("200%")]
    [DataRow("2000\u2030")]
    [DataRow("2147483648")]
    [DataRow("\u0661\u0662")]
    public void TryGetInt32ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetInt32(out _));
    }

    [TestMethod]
    [DataRow(" 12 ", "12")]
    public void TryGetInt32WithTrimWhitespaceReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { TrimWhitespace = true };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetInt32(out var result));
        Assert.AreEqual(int.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow("-2147483648", "-2147483648")]
    public void GetInt32ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(int.Parse(expected, CultureInfo.InvariantCulture), reader.GetInt32());
    }

    [TestMethod]
    [DataRow(" 12 ")]
    public void GetInt32ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetInt32());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+4294967295", "4294967295")]
    [DataRow("-0", "0")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetUInt32ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetUInt32(out var result));
        Assert.AreEqual(uint.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-1")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e3")]
    [DataRow("4294967296")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetUInt32ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetUInt32(out _));
    }

    [TestMethod]
    [DataRow("4294967295", "4294967295")]
    public void GetUInt32ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(uint.Parse(expected, CultureInfo.InvariantCulture), reader.GetUInt32());
    }

    [TestMethod]
    [DataRow("-1")]
    public void GetUInt32ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetUInt32());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+9223372036854775807", "9223372036854775807")]
    [DataRow("-0", "0")]
    [DataRow("-9223372036854775808", "-9223372036854775808")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetInt64ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetInt64(out var result));
        Assert.AreEqual(long.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-9223372036854775809")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e3")]
    [DataRow("50%")]
    [DataRow("9223372036854775808")]
    [DataRow("\u0661\u0662")]
    public void TryGetInt64ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetInt64(out _));
    }

    [TestMethod]
    [DataRow("-9223372036854775808", "-9223372036854775808")]
    public void GetInt64ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(long.Parse(expected, CultureInfo.InvariantCulture), reader.GetInt64());
    }

    [TestMethod]
    [DataRow("9223372036854775808")]
    public void GetInt64ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetInt64());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+18446744073709551615", "18446744073709551615")]
    [DataRow("-0", "0")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetUInt64ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetUInt64(out var result));
        Assert.AreEqual(ulong.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-1")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("18446744073709551616")]
    [DataRow("1e3")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetUInt64ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetUInt64(out _));
    }

    [TestMethod]
    [DataRow("18446744073709551615", "18446744073709551615")]
    public void GetUInt64ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(ulong.Parse(expected, CultureInfo.InvariantCulture), reader.GetUInt64());
    }

    [TestMethod]
    [DataRow("-1")]
    public void GetUInt64ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetUInt64());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+170141183460469231731687303715884105727", "170141183460469231731687303715884105727")]
    [DataRow("-0", "0")]
    [DataRow("-170141183460469231731687303715884105728", "-170141183460469231731687303715884105728")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetInt128ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetInt128(out var result));
        Assert.AreEqual(Int128.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-170141183460469231731687303715884105729")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("170141183460469231731687303715884105728")]
    [DataRow("1e3")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetInt128ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetInt128(out _));
    }

    [TestMethod]
    [DataRow("-170141183460469231731687303715884105728", "-170141183460469231731687303715884105728")]
    public void GetInt128ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(Int128.Parse(expected, CultureInfo.InvariantCulture), reader.GetInt128());
    }

    [TestMethod]
    [DataRow("170141183460469231731687303715884105728")]
    public void GetInt128ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetInt128());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+340282366920938463463374607431768211455", "340282366920938463463374607431768211455")]
    [DataRow("-0", "0")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetUInt128ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetUInt128(out var result));
        Assert.AreEqual(UInt128.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("-1")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e3")]
    [DataRow("340282366920938463463374607431768211456")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetUInt128ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetUInt128(out _));
    }

    [TestMethod]
    [DataRow("340282366920938463463374607431768211455", "340282366920938463463374607431768211455")]
    public void GetUInt128ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(UInt128.Parse(expected, CultureInfo.InvariantCulture), reader.GetUInt128());
    }

    [TestMethod]
    [DataRow("-1")]
    public void GetUInt128ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetUInt128());
    }

    [TestMethod]
    [DataRow("+0", "0")]
    [DataRow("+999999999999999999999999999999999999999", "999999999999999999999999999999999999999")]
    [DataRow("-0", "0")]
    [DataRow("-999999999999999999999999999999999999999", "-999999999999999999999999999999999999999")]
    [DataRow("0", "0")]
    [DataRow("007", "7")]
    public void TryGetBigIntegerReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBigInteger(out var result));
        Assert.AreEqual(BigInteger.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 12 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("+-1")]
    [DataRow("-")]
    [DataRow("--1")]
    [DataRow("0x1F")]
    [DataRow("1 234")]
    [DataRow("1,234")]
    [DataRow("1.0")]
    [DataRow("12a")]
    [DataRow("1e3")]
    [DataRow("50%")]
    [DataRow("\u0661\u0662")]
    public void TryGetBigIntegerReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBigInteger(out _));
    }

    [TestMethod]
    [DataRow("-999999999999999999999999999999999999999", "-999999999999999999999999999999999999999")]
    public void GetBigIntegerReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(BigInteger.Parse(expected, CultureInfo.InvariantCulture), reader.GetBigInteger());
    }

    [TestMethod]
    [DataRow("1.0")]
    public void GetBigIntegerThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBigInteger());
    }

    [TestMethod]
    [DataRow("+1.5", "1.5")]
    [DataRow("+65500", "65500")]
    [DataRow("+INF", "Infinity")]
    [DataRow("-0", "-0")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-65500", "-65500")]
    [DataRow("-INF", "-Infinity")]
    [DataRow(".5", "0.5")]
    [DataRow("0", "0")]
    [DataRow("1.5e-3", "0.0015")]
    [DataRow("1E3", "1000")]
    [DataRow("1e+3", "1000")]
    [DataRow("1e3", "1000")]
    [DataRow("5.", "5")]
    [DataRow("INF", "Infinity")]
    [DataRow("NaN", "NaN")]
    public void TryGetHalfReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetHalf(out var result));
        Assert.AreEqual(Half.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("-Infinity")]
    [DataRow(".")]
    [DataRow("0x1p3")]
    [DataRow("1 000")]
    [DataRow("1,234.5")]
    [DataRow("1.2.3")]
    [DataRow("1d")]
    [DataRow("1e")]
    [DataRow("1e3.5")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("Infinity")]
    [DataRow("\u221e")]
    [DataRow("e3")]
    public void TryGetHalfReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetHalf(out _));
    }

    [TestMethod]
    [DataRow("-INF", "-Infinity")]
    public void GetHalfReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(Half.Parse(expected, CultureInfo.InvariantCulture), reader.GetHalf());
    }

    [TestMethod]
    [DataRow("50%")]
    public void GetHalfThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetHalf());
    }

    [TestMethod]
    [DataRow("+1234.5", "1234.5")]
    [DataRow("+3.39e+38", "3.39E+38")]
    [DataRow("+INF", "Infinity")]
    [DataRow("-0", "-0")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-3.39e+38", "-3.39E+38")]
    [DataRow("-INF", "-Infinity")]
    [DataRow(".5", "0.5")]
    [DataRow("0", "0")]
    [DataRow("1E3", "1000")]
    [DataRow("1e3", "1000")]
    [DataRow("5.", "5")]
    [DataRow("INF", "Infinity")]
    [DataRow("NaN", "NaN")]
    public void TryGetBFloat16ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBFloat16(out var result));
        Assert.AreEqual(BFloat16.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("-Infinity")]
    [DataRow(".")]
    [DataRow("0x1p3")]
    [DataRow("1 000")]
    [DataRow("1,234.5")]
    [DataRow("1.2.3")]
    [DataRow("1d")]
    [DataRow("1e")]
    [DataRow("1e3.5")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("Infinity")]
    [DataRow("\u221e")]
    [DataRow("e3")]
    public void TryGetBFloat16ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBFloat16(out _));
    }

    [TestMethod]
    [DataRow("-INF", "-Infinity")]
    public void GetBFloat16ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(BFloat16.Parse(expected, CultureInfo.InvariantCulture), reader.GetBFloat16());
    }

    [TestMethod]
    [DataRow("50%")]
    public void GetBFloat16ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBFloat16());
    }

    [TestMethod]
    [DataRow("+1.5", "1.5")]
    [DataRow("+3.402823e+38", "3.402823E+38")]
    [DataRow("+INF", "Infinity")]
    [DataRow("-0", "-0")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-3.402823e+38", "-3.402823E+38")]
    [DataRow("-INF", "-Infinity")]
    [DataRow(".5", "0.5")]
    [DataRow("0", "0")]
    [DataRow("1.5e-3", "0.0015")]
    [DataRow("1E3", "1000")]
    [DataRow("1e3", "1000")]
    [DataRow("5.", "5")]
    [DataRow("INF", "Infinity")]
    [DataRow("NaN", "NaN")]
    public void TryGetSingleReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetSingle(out var result));
        Assert.AreEqual(float.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("-Infinity")]
    [DataRow(".")]
    [DataRow("0x1p3")]
    [DataRow("1 000")]
    [DataRow("1,234.5")]
    [DataRow("1.2.3")]
    [DataRow("1d")]
    [DataRow("1e")]
    [DataRow("1e3.5")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("Infinity")]
    [DataRow("\u221e")]
    [DataRow("e3")]
    public void TryGetSingleReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetSingle(out _));
    }

    [TestMethod]
    [DataRow("-INF", "-Infinity")]
    public void GetSingleReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(float.Parse(expected, CultureInfo.InvariantCulture), reader.GetSingle());
    }

    [TestMethod]
    [DataRow("50%")]
    public void GetSingleThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetSingle());
    }

    [TestMethod]
    [DataRow("+1.5", "1.5")]
    [DataRow("+1.7976931348623157e+308", "1.7976931348623157E+308")]
    [DataRow("+INF", "Infinity")]
    [DataRow("-0", "-0")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-1.7976931348623157e+308", "-1.7976931348623157E+308")]
    [DataRow("-INF", "-Infinity")]
    [DataRow(".5", "0.5")]
    [DataRow("0", "0")]
    [DataRow("007.50", "7.5")]
    [DataRow("1.5e-3", "0.0015")]
    [DataRow("1E3", "1000")]
    [DataRow("1e+3", "1000")]
    [DataRow("1e3", "1000")]
    [DataRow("5.", "5")]
    [DataRow("INF", "Infinity")]
    [DataRow("NaN", "NaN")]
    public void TryGetDoubleReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDouble(out var result));
        Assert.AreEqual(double.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow("-Infinity")]
    [DataRow(".")]
    [DataRow("0x1p3")]
    [DataRow("1 000")]
    [DataRow("1,234.5")]
    [DataRow("1,5")]
    [DataRow("1.2.3")]
    [DataRow("1d")]
    [DataRow("1e")]
    [DataRow("1e3.5")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("Infinity")]
    [DataRow("\u221e")]
    [DataRow("e3")]
    public void TryGetDoubleReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDouble(out _));
    }

    [TestMethod]
    [DataRow("-INF", "-Infinity")]
    public void GetDoubleReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(double.Parse(expected, CultureInfo.InvariantCulture), reader.GetDouble());
    }

    [TestMethod]
    [DataRow("50%")]
    public void GetDoubleThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDouble());
    }

    [TestMethod]
    public void TryGetDoubleUsesFormatProvider()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo("fr-CA") };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("1,5"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDouble(out var result));
        Assert.AreEqual(1.5, result);
    }

    [TestMethod]
    [DataRow("+1.5", "1.5")]
    [DataRow("+79228162514264337593543950335", "79228162514264337593543950335")]
    [DataRow("-0", "0")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-79228162514264337593543950335", "-79228162514264337593543950335")]
    [DataRow(".5", "0.5")]
    [DataRow("0", "0")]
    [DataRow("007.50", "7.50")]
    [DataRow("5.", "5")]
    public void TryGetDecimalReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDecimal(out var result));
        Assert.AreEqual(decimal.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow(".")]
    [DataRow("1,234.5")]
    [DataRow("1.2.3")]
    [DataRow("1E3")]
    [DataRow("1e3")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("INF")]
    [DataRow("NaN")]
    public void TryGetDecimalReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDecimal(out _));
    }

    [TestMethod]
    [DataRow("-79228162514264337593543950335", "-79228162514264337593543950335")]
    public void GetDecimalReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(decimal.Parse(expected, CultureInfo.InvariantCulture), reader.GetDecimal());
    }

    [TestMethod]
    [DataRow("1e3")]
    public void GetDecimalThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDecimal());
    }

    [TestMethod]
    [DataRow("+1234.5", "1234.5")]
    [DataRow("+9.999999e+96", "9.999999E+96")]
    [DataRow("+INF", "Infinity")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-9.999999e+96", "-9.999999E+96")]
    [DataRow("-INF", "-Infinity")]
    [DataRow(".5", ".5")]
    [DataRow("0", "0")]
    [DataRow("1e3", "1E+3")]
    [DataRow("5.", "5.")]
    [DataRow("INF", "Infinity")]
    [DataRow("NaN", "NaN")]
    public void TryGetDecimal32ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDecimal32(out var result));
        Assert.AreEqual(Decimal32.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow(".")]
    [DataRow("0x1p3")]
    [DataRow("1.2.3")]
    [DataRow("1e")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("Infinity")]
    [DataRow("\u221e")]
    [DataRow("e3")]
    public void TryGetDecimal32ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDecimal32(out _));
    }

    [TestMethod]
    [DataRow("-INF", "-Infinity")]
    public void GetDecimal32ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(Decimal32.Parse(expected, CultureInfo.InvariantCulture), reader.GetDecimal32());
    }

    [TestMethod]
    [DataRow("50%")]
    public void GetDecimal32ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDecimal32());
    }

    [TestMethod]
    [DataRow("+1234.5", "1234.5")]
    [DataRow("+9.999999999999999e+384", "9.999999999999999E+384")]
    [DataRow("+INF", "Infinity")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-9.999999999999999e+384", "-9.999999999999999E+384")]
    [DataRow("-INF", "-Infinity")]
    [DataRow(".5", ".5")]
    [DataRow("0", "0")]
    [DataRow("1e3", "1E+3")]
    [DataRow("5.", "5.")]
    [DataRow("INF", "Infinity")]
    [DataRow("NaN", "NaN")]
    public void TryGetDecimal64ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDecimal64(out var result));
        Assert.AreEqual(Decimal64.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow(".")]
    [DataRow("0x1p3")]
    [DataRow("1.2.3")]
    [DataRow("1e")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("Infinity")]
    [DataRow("\u221e")]
    [DataRow("e3")]
    public void TryGetDecimal64ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDecimal64(out _));
    }

    [TestMethod]
    [DataRow("-INF", "-Infinity")]
    public void GetDecimal64ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(Decimal64.Parse(expected, CultureInfo.InvariantCulture), reader.GetDecimal64());
    }

    [TestMethod]
    [DataRow("50%")]
    public void GetDecimal64ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDecimal64());
    }

    [TestMethod]
    [DataRow("+1234.5", "1234.5")]
    [DataRow("+9.999999999999999999999999999999999e+6144", "9.999999999999999999999999999999999E+6144")]
    [DataRow("+INF", "Infinity")]
    [DataRow("-1.5", "-1.5")]
    [DataRow("-9.999999999999999999999999999999999e+6144", "-9.999999999999999999999999999999999E+6144")]
    [DataRow("-INF", "-Infinity")]
    [DataRow(".5", ".5")]
    [DataRow("0", "0")]
    [DataRow("1e3", "1E+3")]
    [DataRow("5.", "5.")]
    [DataRow("INF", "Infinity")]
    [DataRow("NaN", "NaN")]
    public void TryGetDecimal128ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDecimal128(out var result));
        Assert.AreEqual(Decimal128.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 1 ")]
    [DataRow("''")]
    [DataRow("+")]
    [DataRow(".")]
    [DataRow("0x1p3")]
    [DataRow("1.2.3")]
    [DataRow("1e")]
    [DataRow("25\u2030")]
    [DataRow("50%")]
    [DataRow("Infinity")]
    [DataRow("\u221e")]
    [DataRow("e3")]
    public void TryGetDecimal128ReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDecimal128(out _));
    }

    [TestMethod]
    [DataRow("-INF", "-Infinity")]
    public void GetDecimal128ReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(Decimal128.Parse(expected, CultureInfo.InvariantCulture), reader.GetDecimal128());
    }

    [TestMethod]
    [DataRow("50%")]
    public void GetDecimal128ThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDecimal128());
    }

    [TestMethod]
    [DataRow("-P10675199DT2H48M5.4775808S", "-10675199.02:48:05.4775808")]
    [DataRow("-P5D", "-05.00:00:00.0000000")]
    [DataRow("-P5DT2.1234567S", "-05.00:00:02.1234567")]
    [DataRow("-P5DT2S", "-05.00:00:02.0000000")]
    [DataRow("-P5DT3M", "-05.00:03:00.0000000")]
    [DataRow("-P5DT3M2.1234567S", "-05.00:03:02.1234567")]
    [DataRow("-P5DT3M2S", "-05.00:03:02.0000000")]
    [DataRow("-P5DT4H", "-05.04:00:00.0000000")]
    [DataRow("-P5DT4H2.1234567S", "-05.04:00:02.1234567")]
    [DataRow("-P5DT4H2S", "-05.04:00:02.0000000")]
    [DataRow("-P5DT4H3M", "-05.04:03:00.0000000")]
    [DataRow("-P5DT4H3M2.1234567S", "-05.04:03:02.1234567")]
    [DataRow("-P5DT4H3M2S", "-05.04:03:02.0000000")]
    [DataRow("-PT2.1234567S", "-00.00:00:02.1234567")]
    [DataRow("-PT2S", "-00.00:00:02.0000000")]
    [DataRow("-PT3M", "-00.00:03:00.0000000")]
    [DataRow("-PT3M2.1234567S", "-00.00:03:02.1234567")]
    [DataRow("-PT3M2S", "-00.00:03:02.0000000")]
    [DataRow("-PT4H", "-00.04:00:00.0000000")]
    [DataRow("-PT4H2.1234567S", "-00.04:00:02.1234567")]
    [DataRow("-PT4H2S", "-00.04:00:02.0000000")]
    [DataRow("-PT4H3M", "-00.04:03:00.0000000")]
    [DataRow("-PT4H3M2.1234567S", "-00.04:03:02.1234567")]
    [DataRow("-PT4H3M2S", "-00.04:03:02.0000000")]
    [DataRow("P0D", "00.00:00:00.0000000")]
    [DataRow("P0DT0H0M2.0000000S", "00.00:00:02.0000000")]
    [DataRow("P10675199DT2H48M5.4775807S", "10675199.02:48:05.4775807")]
    [DataRow("P1DT36H", "02.12:00:00.0000000")]
    [DataRow("P5D", "05.00:00:00.0000000")]
    [DataRow("P5DT2.1234567S", "05.00:00:02.1234567")]
    [DataRow("P5DT2S", "05.00:00:02.0000000")]
    [DataRow("P5DT3M", "05.00:03:00.0000000")]
    [DataRow("P5DT3M2.1234567S", "05.00:03:02.1234567")]
    [DataRow("P5DT3M2S", "05.00:03:02.0000000")]
    [DataRow("P5DT4H", "05.04:00:00.0000000")]
    [DataRow("P5DT4H2.1234567S", "05.04:00:02.1234567")]
    [DataRow("P5DT4H2S", "05.04:00:02.0000000")]
    [DataRow("P5DT4H3M", "05.04:03:00.0000000")]
    [DataRow("P5DT4H3M2.1234567S", "05.04:03:02.1234567")]
    [DataRow("P5DT4H3M2S", "05.04:03:02.0000000")]
    [DataRow("PT0.0000001S", "00.00:00:00.0000001")]
    [DataRow("PT00010S", "00.00:00:10.0000000")]
    [DataRow("PT0S", "00.00:00:00.0000000")]
    [DataRow("PT1.5S", "00.00:00:01.5000000")]
    [DataRow("PT100.5S", "00.00:01:40.5000000")]
    [DataRow("PT2.1234567S", "00.00:00:02.1234567")]
    [DataRow("PT2S", "00.00:00:02.0000000")]
    [DataRow("PT36H", "01.12:00:00.0000000")]
    [DataRow("PT3M", "00.00:03:00.0000000")]
    [DataRow("PT3M2.1234567S", "00.00:03:02.1234567")]
    [DataRow("PT3M2S", "00.00:03:02.0000000")]
    [DataRow("PT4H", "00.04:00:00.0000000")]
    [DataRow("PT4H2.1234567S", "00.04:00:02.1234567")]
    [DataRow("PT4H2S", "00.04:00:02.0000000")]
    [DataRow("PT4H3M", "00.04:03:00.0000000")]
    [DataRow("PT4H3M2.1234567S", "00.04:03:02.1234567")]
    [DataRow("PT4H3M2S", "00.04:03:02.0000000")]
    [DataRow("PT90M", "00.01:30:00.0000000")]
    public void TryGetTimeSpanReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetTimeSpan(out var result));
        Assert.AreEqual(TimeSpan.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" PT1S")]
    [DataRow("''")]
    [DataRow("+P1D")]
    [DataRow("-")]
    [DataRow("-P")]
    [DataRow("-P10675199DT2H48M5.4775809S")]
    [DataRow("00:00:01")]
    [DataRow("1D")]
    [DataRow("P")]
    [DataRow("P-1D")]
    [DataRow("P1.5D")]
    [DataRow("P10675199DT2H48M5.4775808S")]
    [DataRow("P1DT")]
    [DataRow("P1DT1H1M1S1")]
    [DataRow("P1M")]
    [DataRow("P1W")]
    [DataRow("P1Y")]
    [DataRow("P1Y2M3D")]
    [DataRow("PT")]
    [DataRow("PT-1S")]
    [DataRow("PT.5S")]
    [DataRow("PT1.5H")]
    [DataRow("PT1.5M")]
    [DataRow("PT1H1H")]
    [DataRow("PT1M1H")]
    [DataRow("PT1S ")]
    [DataRow("PT1S1M")]
    [DataRow("PT1s")]
    [DataRow("PT5.12345678S")]
    [DataRow("PT5.S")]
    [DataRow("T1H")]
    [DataRow("p1d")]
    public void TryGetTimeSpanReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetTimeSpan(out _));
    }

    [TestMethod]
    [DataRow("-P5DT4H3M2.1234567S", "-05.04:03:02.1234567")]
    public void GetTimeSpanReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(TimeSpan.Parse(expected, CultureInfo.InvariantCulture), reader.GetTimeSpan());
    }

    [TestMethod]
    [DataRow("PT5.S")]
    public void GetTimeSpanThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetTimeSpan());
    }

    [TestMethod]
    [DataRow("fa-IR")]
    [DataRow("fr-CA")]
    [DataRow("sv-SE")]
    public void TryGetTimeSpanIgnoresFormatProvider(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("-P0DT0H0M1.5000000S"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetTimeSpan(out var result));
        Assert.AreEqual(-TimeSpan.FromSeconds(1.5), result);
    }

    [TestMethod]
    [DataRow("00:00:00", "00:00:00.0000000")]
    [DataRow("01:00:00+02:00", "01:00:00.0000000")]
    [DataRow("04:05:06", "04:05:06.0000000")]
    [DataRow("04:05:06+02:00", "04:05:06.0000000")]
    [DataRow("04:05:06.0000007", "04:05:06.0000007")]
    [DataRow("04:05:06.0000007+14:00", "04:05:06.0000007")]
    [DataRow("04:05:06.1", "04:05:06.1000000")]
    [DataRow("04:05:06Z", "04:05:06.0000000")]
    [DataRow("23:00:00-14:00", "23:00:00.0000000")]
    [DataRow("23:59:59.9999999", "23:59:59.9999999")]
    public void TryGetTimeOnlyReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetTimeOnly(out var result));
        Assert.AreEqual(TimeOnly.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 04:05:06")]
    [DataRow("''")]
    [DataRow("0001-02-03T04:05:06")]
    [DataRow("04-05-06")]
    [DataRow("040506")]
    [DataRow("04:05")]
    [DataRow("04:05:06 ")]
    [DataRow("04:05:06+02")]
    [DataRow("04:05:06+15:00")]
    [DataRow("04:05:06.12345678")]
    [DataRow("04:05:06z")]
    [DataRow("04:05:6")]
    [DataRow("04:5:06")]
    [DataRow("04:60:06")]
    [DataRow("25:00:00")]
    [DataRow("4:05:06")]
    public void TryGetTimeOnlyReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetTimeOnly(out _));
    }

    [TestMethod]
    [DataRow("04:05:06.0000007", "04:05:06.0000007")]
    public void GetTimeOnlyReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(TimeOnly.Parse(expected, CultureInfo.InvariantCulture), reader.GetTimeOnly());
    }

    [TestMethod]
    [DataRow("04:05")]
    public void GetTimeOnlyThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetTimeOnly());
    }

    [TestMethod]
    [DataRow("04.05.06", "HH'.'mm'.'ss", "04:05:06.0000000")]
    public void TryGetTimeOnlyWithFormatReturnsValue(string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularTimeOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreEqual(TimeOnly.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow("04:05:06", "HH'.'mm'.'ss")]
    public void TryGetTimeOnlyWithFormatReturnsFalse(string content, string format)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularTimeOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGet(converter, out _));
    }

    [TestMethod]
    [DataRow("0001-01-01", "0001-01-01")]
    [DataRow("0001-02-03", "0001-02-03")]
    [DataRow("2024-01-02+02:00", "2024-01-02")]
    [DataRow("2024-01-02+14:00", "2024-01-02")]
    [DataRow("2024-01-02-14:00", "2024-01-02")]
    [DataRow("2024-01-02Z", "2024-01-02")]
    [DataRow("2024-02-29", "2024-02-29")]
    [DataRow("9999-12-31", "9999-12-31")]
    [DataRow("9999-12-31-14:00", "9999-12-31")]
    public void TryGetDateOnlyReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateOnly(out var result));
        Assert.AreEqual(DateOnly.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow(" 2024-01-02")]
    [DataRow("''")]
    [DataRow("2023-02-29")]
    [DataRow("2024-00-10")]
    [DataRow("2024-01-00")]
    [DataRow("2024-01-02 ")]
    [DataRow("2024-01-02+02")]
    [DataRow("2024-01-02+15:00")]
    [DataRow("2024-01-02T00:00:00")]
    [DataRow("2024-01-02z")]
    [DataRow("2024-01-2")]
    [DataRow("2024-1-02")]
    [DataRow("2024-13-01")]
    [DataRow("2024/01/02")]
    [DataRow("20240102")]
    [DataRow("24-01-02")]
    public void TryGetDateOnlyReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDateOnly(out _));
    }

    [TestMethod]
    [DataRow("0001-02-03", "0001-02-03")]
    public void GetDateOnlyReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(DateOnly.Parse(expected, CultureInfo.InvariantCulture), reader.GetDateOnly());
    }

    [TestMethod]
    [DataRow("2024/01/02")]
    public void GetDateOnlyThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDateOnly());
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public void TryGetDateOnlyIgnoresFormatProviderCalendar(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("2024-01-02"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateOnly(out var result));
        Assert.AreEqual(new DateOnly(2024, 1, 2), result);
    }

    [TestMethod]
    [DataRow("03.02.0001", "dd'.'MM'.'yyyy", "0001-02-03")]
    public void TryGetDateOnlyWithFormatReturnsValue(string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreEqual(DateOnly.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow("0001-02-03", "dd'.'MM'.'yyyy")]
    public void TryGetDateOnlyWithFormatReturnsFalse(string content, string format)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGet(converter, out _));
    }

    [TestMethod]
    [DataRow("fr-CA", "03 f\u00e9vrier 2024", "dd MMMM yyyy", "2024-02-03")]
    public void TryGetDateOnlyWithFormatUsesFormatProvider(string cultureName, string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreEqual(DateOnly.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    public void TryGetDateOnlyUsesConverterFromOptions()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("03.02.0001"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateOnly(out var result));
        Assert.AreEqual(new DateOnly(1, 2, 3), result);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06", "0001-02-03T04:05:06.0000000")]
    [DataRow("0001-02-03T04:05:06+00:00", "0001-02-03T04:05:06.0000000Z")]
    [DataRow("0001-02-03T04:05:06+08:09", "0001-02-02T19:56:06.0000000Z")]
    [DataRow("0001-02-03T04:05:06-08:09", "0001-02-03T12:14:06.0000000Z")]
    [DataRow("0001-02-03T04:05:06.0000007", "0001-02-03T04:05:06.0000007")]
    [DataRow("0001-02-03T04:05:06.0000007+08:09", "0001-02-02T19:56:06.0000007Z")]
    [DataRow("0001-02-03T04:05:06.0000007Z", "0001-02-03T04:05:06.0000007Z")]
    [DataRow("0001-02-03T04:05:06.1", "0001-02-03T04:05:06.1000000")]
    [DataRow("0001-02-03T04:05:06Z", "0001-02-03T04:05:06.0000000Z")]
    public void TryGetDateTimeReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateTime(out var result));
        Assert.AreEqual(DateTime.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), result);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06+00:00")]
    [DataRow("0001-02-03T04:05:06+08:09")]
    [DataRow("0001-02-03T04:05:06-08:09")]
    [DataRow("0001-02-03T04:05:06.0000007+08:09")]
    [DataRow("0001-02-03T04:05:06Z")]
    public void TryGetDateTimeReturnsUtcForTimeZone(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateTime(out var result));
        Assert.AreEqual(DateTimeKind.Utc, result.Kind);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06")]
    [DataRow("0001-02-03T04:05:06.0000007")]
    public void TryGetDateTimeReturnsUnspecifiedWithoutTimeZone(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateTime(out var result));
        Assert.AreEqual(DateTimeKind.Unspecified, result.Kind);
    }

    [TestMethod]
    [DataRow(" 0001-02-03T04:05:06")]
    [DataRow("''")]
    [DataRow("0001-02-03 04:05:06")]
    [DataRow("0001-02-03")]
    [DataRow("0001-02-03T04:05")]
    [DataRow("0001-02-03T04:05:06 ")]
    [DataRow("0001-02-03T04:05:06.12345678")]
    [DataRow("0001-02-03T04:05:06.12345678Z")]
    [DataRow("0001-02-03T25:05:06")]
    [DataRow("0001-02-03T4:05:06")]
    [DataRow("0001-02-30T04:05:06")]
    [DataRow("0001/02/03T04:05:06")]
    [DataRow("00010203T040506")]
    public void TryGetDateTimeReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDateTime(out _));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000007Z", "0001-02-03T04:05:06.0000007Z")]
    public void GetDateTimeReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(DateTime.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), reader.GetDateTime());
    }

    [TestMethod]
    [DataRow("0001-02-03 04:05:06")]
    public void GetDateTimeThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDateTime());
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public void TryGetDateTimeIgnoresFormatProviderCalendar(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("2024-01-02T03:04:05Z"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateTime(out var result));
        Assert.AreEqual(new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc), result);
    }

    [TestMethod]
    [DataRow("0001/02/03 04:05", "yyyy'/'MM'/'dd HH':'mm", "0001-02-03T04:05:00.0000000")]
    public void TryGetDateTimeWithFormatReturnsValue(string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreEqual(DateTime.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), result);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06", "yyyy'/'MM'/'dd HH':'mm")]
    public void TryGetDateTimeWithFormatReturnsFalse(string content, string format)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGet(converter, out _));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06+08:09", "0001-02-03T04:05:06.0000000+08:09")]
    [DataRow("0001-02-03T04:05:06-08:09", "0001-02-03T04:05:06.0000000-08:09")]
    [DataRow("0001-02-03T04:05:06.0000007+08:09", "0001-02-03T04:05:06.0000007+08:09")]
    [DataRow("0001-02-03T04:05:06.0000007Z", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow("0001-02-03T04:05:06.1+08:09", "0001-02-03T04:05:06.1000000+08:09")]
    [DataRow("0001-02-03T04:05:06Z", "0001-02-03T04:05:06.0000000+00:00")]
    public void TryGetDateTimeOffsetReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var expectedValue = DateTimeOffset.Parse(expected, CultureInfo.InvariantCulture);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateTimeOffset(out var result));
        Assert.AreEqual(expectedValue, result);
        Assert.AreEqual(expectedValue.Offset, result.Offset);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06", "0001-02-03T04:05:06.0000000+00:00")]
    [DataRow("0001-02-03T04:05:06.0000007", "0001-02-03T04:05:06.0000007+00:00")]
    public void TryGetDateTimeOffsetReturnsUtcWithoutTimeZone(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        var expectedValue = DateTimeOffset.Parse(expected, CultureInfo.InvariantCulture);

        Assert.IsTrue(reader.TryGetDateTimeOffset(out var result));
        Assert.AreEqual(expectedValue, result);
        Assert.AreEqual(TimeSpan.Zero, result.Offset);
    }

    [TestMethod]
    [DataRow(" 0001-02-03T04:05:06")]
    [DataRow("''")]
    [DataRow("0001-02-03 04:05:06")]
    [DataRow("0001-02-03")]
    [DataRow("0001-02-03T04:05")]
    [DataRow("0001-02-03T04:05:06 ")]
    [DataRow("0001-02-03T04:05:06.12345678")]
    [DataRow("0001-02-03T04:05:06.12345678Z")]
    [DataRow("0001-02-03T25:05:06")]
    [DataRow("0001-02-03T4:05:06")]
    [DataRow("0001-02-30T04:05:06")]
    [DataRow("0001/02/03T04:05:06")]
    [DataRow("00010203T040506")]
    public void TryGetDateTimeOffsetReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetDateTimeOffset(out _));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000007+08:09", "0001-02-03T04:05:06.0000007+08:09")]
    public void GetDateTimeOffsetReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var expectedValue = DateTimeOffset.Parse(expected, CultureInfo.InvariantCulture);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        var result = reader.GetDateTimeOffset();

        Assert.AreEqual(expectedValue, result);
        Assert.AreEqual(expectedValue.Offset, result.Offset);
    }

    [TestMethod]
    [DataRow("0001-02-03 04:05:06")]
    public void GetDateTimeOffsetThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetDateTimeOffset());
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public void TryGetDateTimeOffsetIgnoresFormatProviderCalendar(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("2024-01-02T03:04:05+01:00"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetDateTimeOffset(out var result));
        Assert.AreEqual(new DateTimeOffset(2024, 1, 2, 3, 4, 5, TimeSpan.FromHours(1)), result);
        Assert.AreEqual(TimeSpan.FromHours(1), result.Offset);
    }

    [TestMethod]
    [DataRow("0001-02-03 04:05 +08:09", "yyyy'-'MM'-'dd HH':'mm zzz", "0001-02-03T04:05:00.0000000+08:09")]
    public void TryGetDateTimeOffsetWithFormatReturnsValue(string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeOffsetConverter(format);
        var expectedValue = DateTimeOffset.Parse(expected, CultureInfo.InvariantCulture);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreEqual(expectedValue, result);
        Assert.AreEqual(expectedValue.Offset, result.Offset);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:00+08:09", "yyyy'-'MM'-'dd HH':'mm zzz")]
    public void TryGetDateTimeOffsetWithFormatReturnsFalse(string content, string format)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeOffsetConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGet(converter, out _));
    }

    [TestMethod]
    [DataRow("00000000-0000-0000-0000-000000000000", "00000000-0000-0000-0000-000000000000")]
    [DataRow("123E4567-E89B-12D3-A456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    [DataRow("123E4567-e89b-12D3-a456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    public void TryGetGuidReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetGuid(out var result));
        Assert.AreEqual(Guid.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow("''")]
    [DataRow("(123e4567-e89b-12d3-a456-426614174000)")]
    [DataRow("123e456-7e89b-12d3-a456-426614174000")]
    [DataRow("123e4567-e89b-12d3-a456-42661417400")]
    [DataRow("123e4567-e89b-12d3-a456-4266141740000")]
    [DataRow("123e4567-e89b-12d3-a456-42661417400g")]
    [DataRow("123e4567e89b-12d3-a456-426614174000")]
    [DataRow("123e4567e89b12d3a456426614174000")]
    [DataRow("urn:uuid:123e4567-e89b-12d3-a456-426614174000")]
    [DataRow("{123e4567-e89b-12d3-a456-426614174000}")]
    public void TryGetGuidReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetGuid(out _));
    }

    [TestMethod]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    public void GetGuidReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(Guid.Parse(expected, CultureInfo.InvariantCulture), reader.GetGuid());
    }

    [TestMethod]
    [DataRow("123e4567e89b12d3a456426614174000")]
    public void GetGuidThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetGuid());
    }

    [TestMethod]
    [DataRow("(123e4567-e89b-12d3-a456-426614174000)", "P", "123e4567-e89b-12d3-a456-426614174000")]
    [DataRow("123e4567e89b12d3a456426614174000", "N", "123e4567-e89b-12d3-a456-426614174000")]
    [DataRow("{123e4567-e89b-12d3-a456-426614174000}", "B", "123e4567-e89b-12d3-a456-426614174000")]
    public void TryGetGuidWithFormatReturnsValue(string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreEqual(Guid.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "N")]
    public void TryGetGuidWithFormatReturnsFalse(string content, string format)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGet(converter, out _));
    }

    [TestMethod]
    [DataRow("../a", "../a")]
    [DataRow("a/b", "a/b")]
    [DataRow("https://example.com/", "https://example.com/")]
    [DataRow("https://example.com/a%20b?q=a%20b", "https://example.com/a%20b?q=a%20b")]
    [DataRow("urn:com.example", "urn:com.example")]
    public void TryGetUriReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetUri(out var result));
        Assert.AreEqual(new Uri(expected, UriKind.RelativeOrAbsolute), result);
    }

    [TestMethod]
    [DataRow("http://[")]
    [DataRow("https://exa mple.com/")]
    public void TryGetUriReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetUri(out _));
    }

    [TestMethod]
    [DataRow("https://example.com/", "https://example.com/")]
    public void GetUriReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(new Uri(expected, UriKind.RelativeOrAbsolute), reader.GetUri());
    }

    [TestMethod]
    [DataRow("http://[")]
    public void GetUriThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetUri());
    }

    [TestMethod]
    [DataRow("''", "")]
    [DataRow("00FF", "00FF")]
    [DataRow("00ff", "00FF")]
    [DataRow("123E4567E89B12D3A456426614174000", "123E4567E89B12D3A456426614174000")]
    [DataRow("123e4567e89b12d3a456426614174000", "123E4567E89B12D3A456426614174000")]
    [DataRow("aB", "AB")]
    public void TryGetBase16ArrayReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBase16Array(out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result);
    }

    [TestMethod]
    [DataRow(" 1234 ")]
    [DataRow("0x12")]
    [DataRow("12 34 ")]
    [DataRow("a")]
    [DataRow("abc")]
    [DataRow("zz")]
    public void TryGetBase16ArrayReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBase16Array(out _));
    }

    [TestMethod]
    [DataRow("00ff", "00FF")]
    public void GetBase16ArrayReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreSequenceEqual(Convert.FromHexString(expected), reader.GetBase16Array());
    }

    [TestMethod]
    [DataRow("abc")]
    public void GetBase16ArrayThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBase16Array());
    }

    [TestMethod]
    [DataRow("''", "")]
    [DataRow("+/+/", "FBFFBF")]
    [DataRow("MTk4NC0wNS0wNA==", "313938342D30352D3034")]
    [DataRow("Zg==", "66")]
    [DataRow("Zm8=", "666F")]
    [DataRow("Zm9v", "666F6F")]
    public void TryGetBase64ArrayReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBase64Array(out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result);
    }

    [TestMethod]
    [DataRow("-_-_")]
    [DataRow("Z===")]
    [DataRow("Zg")]
    [DataRow("Zg=")]
    [DataRow("Zg==Zg==")]
    [DataRow("Zm9")]
    [DataRow("Zm9v!")]
    [DataRow("Zm9v=")]
    public void TryGetBase64ArrayReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBase64Array(out _));
    }

    [TestMethod]
    [DataRow("Zm9v", "666F6F")]
    public void GetBase64ArrayReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreSequenceEqual(Convert.FromHexString(expected), reader.GetBase64Array());
    }

    [TestMethod]
    [DataRow("Zm9")]
    public void GetBase64ArrayThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBase64Array());
    }

    [TestMethod]
    [DataRow("''", "")]
    [DataRow("00ff", "00FF")]
    public void TryGetBase16MemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBase16Memory(out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow("abc")]
    public void TryGetBase16MemoryReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBase16Memory(out _));
    }

    [TestMethod]
    [DataRow("00ff", "00FF")]
    public void GetBase16MemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreSequenceEqual(Convert.FromHexString(expected), reader.GetBase16Memory().ToArray());
    }

    [TestMethod]
    [DataRow("abc")]
    public void GetBase16MemoryThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBase16Memory());
    }

    [TestMethod]
    [DataRow("''", "")]
    [DataRow("Zm9v", "666F6F")]
    public void TryGetBase64MemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBase64Memory(out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow("Zm9")]
    public void TryGetBase64MemoryReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBase64Memory(out _));
    }

    [TestMethod]
    [DataRow("Zm9v", "666F6F")]
    public void GetBase64MemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreSequenceEqual(Convert.FromHexString(expected), reader.GetBase64Memory().ToArray());
    }

    [TestMethod]
    [DataRow("Zm9")]
    public void GetBase64MemoryThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBase64Memory());
    }

    [TestMethod]
    [DataRow("''", "")]
    [DataRow("00ff", "00FF")]
    public void TryGetBase16ReadOnlyMemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBase16ReadOnlyMemory(out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow("abc")]
    public void TryGetBase16ReadOnlyMemoryReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBase16ReadOnlyMemory(out _));
    }

    [TestMethod]
    [DataRow("00ff", "00FF")]
    public void GetBase16ReadOnlyMemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreSequenceEqual(Convert.FromHexString(expected), reader.GetBase16ReadOnlyMemory().ToArray());
    }

    [TestMethod]
    [DataRow("abc")]
    public void GetBase16ReadOnlyMemoryThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBase16ReadOnlyMemory());
    }

    [TestMethod]
    [DataRow("''", "")]
    [DataRow("Zm9v", "666F6F")]
    public void TryGetBase64ReadOnlyMemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGetBase64ReadOnlyMemory(out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow("Zm9")]
    public void TryGetBase64ReadOnlyMemoryReturnsFalse(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGetBase64ReadOnlyMemory(out _));
    }

    [TestMethod]
    [DataRow("Zm9v", "666F6F")]
    public void GetBase64ReadOnlyMemoryReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreSequenceEqual(Convert.FromHexString(expected), reader.GetBase64ReadOnlyMemory().ToArray());
    }

    [TestMethod]
    [DataRow("Zm9")]
    public void GetBase64ReadOnlyMemoryThrowsFormatException(string content)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.GetBase64ReadOnlyMemory());
    }

    [TestMethod]
    [DataRow("00ff", "00FF")]
    public void TryGetWithBase16MemoryConverterReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase16MemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow("00ff", "00FF")]
    public void TryGetWithBase16ReadOnlyMemoryConverterReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase16ReadOnlyMemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow("Zm9v", "666F6F")]
    public void TryGetWithBase64MemoryConverterReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase64MemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow("Zm9v", "666F6F")]
    public void TryGetWithBase64ReadOnlyMemoryConverterReturnsValue(string content, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase64ReadOnlyMemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreSequenceEqual(Convert.FromHexString(expected), result.ToArray());
    }

    [TestMethod]
    [DataRow(" 03.02.0001 ", "dd'.'MM'.'yyyy", "0001-02-03")]
    public void TryGetWithTrimWhitespaceReturnsValue(string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { TrimWhitespace = true };
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(converter, out var result));
        Assert.AreEqual(DateOnly.Parse(expected, CultureInfo.InvariantCulture), result);
    }

    [TestMethod]
    [DataRow("03.02.0001", "dd'.'MM'.'yyyy", "0001-02-03")]
    public void GetReturnsValue(string content, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.AreEqual(DateOnly.Parse(expected, CultureInfo.InvariantCulture), reader.Get(converter));
    }

    [TestMethod]
    [DataRow("0001-02-03", "dd'.'MM'.'yyyy")]
    public void GetThrowsFormatException(string content, string format)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.Get(converter));
    }

    [TestMethod]
    public void TryGetThrowsArgumentNullExceptionForNullConverter()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<ArgumentNullException>(() => reader.TryGet<string>(null!, out _));
    }

    [TestMethod]
    public void GetThrowsArgumentNullExceptionForNullConverter()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<ArgumentNullException>(() => reader.Get<string>(null!));
    }

    [TestMethod]
    public void ResolveConverterReturnsBuiltInConverter()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("0001-02-03"));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(reader.ResolveConverter<DateOnly>(), out var result));
        Assert.AreEqual(new DateOnly(1, 2, 3), result);
    }

    [TestMethod]
    public void ResolveConverterReturnsConverterFromOptions()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(Memory<byte>)] = new TabularBase16MemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("0aff"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsTrue(reader.TryGet(reader.ResolveConverter<Memory<byte>>(), out var result));
        Assert.AreSequenceEqual(Convert.FromHexString("0aff"), result.ToArray());
    }

    [TestMethod]
    public void TryGetWithResolvedConverterReturnsFalse()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(Memory<byte>)] = new TabularBase16MemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("0g"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.IsFalse(reader.TryGet(reader.ResolveConverter<Memory<byte>>(), out _));
    }

    [TestMethod]
    public void ResolveConverterThrowsInvalidOperationExceptionForTypeWithoutConverter()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("0aff"));
        using var reader = new TabularReader(stream, dialect);

        reader.TryReadField();

        Assert.ThrowsExactly<InvalidOperationException>(() => reader.ResolveConverter<Memory<byte>>());
    }

    [TestMethod]
    public void GetWithResolvedConverterReturnsValue()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(Memory<byte>)] = new TabularBase16MemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("0aff"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.AreSequenceEqual(Convert.FromHexString("0aff"), reader.Get(reader.ResolveConverter<Memory<byte>>()).ToArray());
    }

    [TestMethod]
    public void GetWithResolvedConverterThrowsFormatException()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(Memory<byte>)] = new TabularBase16MemoryConverter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("0g"));
        using var reader = new TabularReader(stream, dialect, options);

        reader.TryReadField();

        Assert.ThrowsExactly<FormatException>(() => reader.Get(reader.ResolveConverter<Memory<byte>>()));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForConverterOfOtherType()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularInt32Converter();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));

        Assert.ThrowsExactly<ArgumentException>(() => new TabularReader(stream, dialect, options));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForNullConverter()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = null!;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v"));

        Assert.ThrowsExactly<ArgumentException>(() => new TabularReader(stream, dialect, options));
    }

    private static TabularDialect CreateValueDialect()
    {
        return new("$", '^', '\'');
    }
}
