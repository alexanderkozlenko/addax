using System.Globalization;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;
using Addax.Formats.Tabular.UnitTests.Streams;

namespace Addax.Formats.Tabular.UnitTests;

public sealed partial class TabularWriterTests
{
    [TestMethod]
    [DataRow(false, "false")]
    [DataRow(true, "true")]
    public void WriteBooleanWritesCanonicalForm(bool value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBoolean(value);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow(true, "true")]
    public async Task WriteBooleanWritesCanonicalFormAsync(bool value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBooleanAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow('\u00e9', "\u00e9")]
    [DataRow('a', "a")]
    public void WriteCharWritesCanonicalForm(char value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteChar(value);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow('a', "a")]
    public async Task WriteCharWritesCanonicalFormAsync(char value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteCharAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("\ud83d\udd2e", "\ud83d\udd2e")]
    [DataRow("v", "v")]
    public void WriteRuneWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteRune(Rune.GetRuneAt(value, 0));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("\ud83d\udd2e", "\ud83d\udd2e")]
    public async Task WriteRuneWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteRuneAsync(Rune.GetRuneAt(value, 0));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+127", "127")]
    [DataRow("-128", "-128")]
    [DataRow("0", "0")]
    public void WriteSByteWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteSByte(sbyte.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-128", "-128")]
    public async Task WriteSByteWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteSByteAsync(sbyte.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+255", "255")]
    [DataRow("0", "0")]
    public void WriteByteWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteByte(byte.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+255", "255")]
    public async Task WriteByteWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteByteAsync(byte.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+32767", "32767")]
    [DataRow("-32768", "-32768")]
    [DataRow("0", "0")]
    public void WriteInt16WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteInt16(short.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-32768", "-32768")]
    public async Task WriteInt16WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteInt16Async(short.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+65535", "65535")]
    [DataRow("0", "0")]
    public void WriteUInt16WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteUInt16(ushort.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+65535", "65535")]
    public async Task WriteUInt16WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteUInt16Async(ushort.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+2147483647", "2147483647")]
    [DataRow("-2147483648", "-2147483648")]
    [DataRow("0", "0")]
    public void WriteInt32WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteInt32(int.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-2147483648", "-2147483648")]
    public async Task WriteInt32WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteInt32Async(int.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+4294967295", "4294967295")]
    [DataRow("0", "0")]
    public void WriteUInt32WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteUInt32(uint.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+4294967295", "4294967295")]
    public async Task WriteUInt32WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteUInt32Async(uint.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+9223372036854775807", "9223372036854775807")]
    [DataRow("-9223372036854775808", "-9223372036854775808")]
    [DataRow("0", "0")]
    public void WriteInt64WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteInt64(long.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-9223372036854775808", "-9223372036854775808")]
    public async Task WriteInt64WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteInt64Async(long.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+18446744073709551615", "18446744073709551615")]
    [DataRow("0", "0")]
    public void WriteUInt64WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteUInt64(ulong.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+18446744073709551615", "18446744073709551615")]
    public async Task WriteUInt64WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteUInt64Async(ulong.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+170141183460469231731687303715884105727", "170141183460469231731687303715884105727")]
    [DataRow("-170141183460469231731687303715884105728", "-170141183460469231731687303715884105728")]
    [DataRow("0", "0")]
    public void WriteInt128WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteInt128(Int128.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-170141183460469231731687303715884105728", "-170141183460469231731687303715884105728")]
    public async Task WriteInt128WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteInt128Async(Int128.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+340282366920938463463374607431768211455", "340282366920938463463374607431768211455")]
    [DataRow("0", "0")]
    public void WriteUInt128WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteUInt128(UInt128.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+340282366920938463463374607431768211455", "340282366920938463463374607431768211455")]
    public async Task WriteUInt128WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteUInt128Async(UInt128.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+127", "127")]
    [DataRow("+999999999999999999999999999999999999999", "999999999999999999999999999999999999999")]
    [DataRow("-128", "-128")]
    [DataRow("-999999999999999999999999999999999999999", "-999999999999999999999999999999999999999")]
    [DataRow("0", "0")]
    public void WriteBigIntegerWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBigInteger(BigInteger.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-999999999999999999999999999999999999999", "-999999999999999999999999999999999999999")]
    public async Task WriteBigIntegerWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBigIntegerAsync(BigInteger.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+65500", "65500")]
    [DataRow("+Infinity", "INF")]
    [DataRow("-65500", "-65500")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("0.5", "0.5")]
    [DataRow("NaN", "NaN")]
    public void WriteHalfWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteHalf(Half.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-Infinity", "-INF")]
    public async Task WriteHalfWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteHalfAsync(Half.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+3.39E+38", "3.39e+38")]
    [DataRow("+Infinity", "INF")]
    [DataRow("-3.39E+38", "-3.39e+38")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("1.5", "1.5")]
    [DataRow("NaN", "NaN")]
    public void WriteBFloat16WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBFloat16(BFloat16.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-Infinity", "-INF")]
    public async Task WriteBFloat16WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBFloat16Async(BFloat16.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+3.402823E+38", "3.402823e+38")]
    [DataRow("+Infinity", "INF")]
    [DataRow("-3.402823E+38", "-3.402823e+38")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("1.5", "1.5")]
    [DataRow("1E-07", "1e-07")]
    [DataRow("NaN", "NaN")]
    public void WriteSingleWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteSingle(float.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-Infinity", "-INF")]
    public async Task WriteSingleWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteSingleAsync(float.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+1.7976931348623157E+308", "1.7976931348623157e+308")]
    [DataRow("+Infinity", "INF")]
    [DataRow("-1.7976931348623157E+308", "-1.7976931348623157e+308")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("0.1", "0.1")]
    [DataRow("1.5", "1.5")]
    [DataRow("1E-07", "1e-07")]
    [DataRow("NaN", "NaN")]
    public void WriteDoubleWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDouble(double.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-Infinity", "-INF")]
    public async Task WriteDoubleWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDoubleAsync(double.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void WriteDoubleUsesFormatProvider()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo("fr-CA") };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteDouble(1.5);
        writer.Flush();

        Assert.AreEqual("1,5", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task WriteDoubleUsesFormatProviderAsync()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo("fr-CA") };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteDoubleAsync(1.5);
        await writer.FlushAsync();

        Assert.AreEqual("1,5", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+79228162514264337593543950335", "79228162514264337593543950335")]
    [DataRow("-79228162514264337593543950335", "-79228162514264337593543950335")]
    [DataRow("0", "0")]
    [DataRow("1.50", "1.50")]
    public void WriteDecimalWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDecimal(decimal.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-79228162514264337593543950335", "-79228162514264337593543950335")]
    public async Task WriteDecimalWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDecimalAsync(decimal.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+9.999999E+96", "9.999999e+96")]
    [DataRow("+Infinity", "INF")]
    [DataRow("-1.50", "-1.50")]
    [DataRow("-9.999999E+96", "-9.999999e+96")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("NaN", "NaN")]
    public void WriteDecimal32WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDecimal32(Decimal32.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-Infinity", "-INF")]
    public async Task WriteDecimal32WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDecimal32Async(Decimal32.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+9.999999999999999E+384", "9.999999999999999e+384")]
    [DataRow("+Infinity", "INF")]
    [DataRow("-1.50", "-1.50")]
    [DataRow("-9.999999999999999E+384", "-9.999999999999999e+384")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("NaN", "NaN")]
    public void WriteDecimal64WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDecimal64(Decimal64.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-Infinity", "-INF")]
    public async Task WriteDecimal64WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDecimal64Async(Decimal64.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("+9.999999999999999999999999999999999E+6144", "9.999999999999999999999999999999999e+6144")]
    [DataRow("+Infinity", "INF")]
    [DataRow("-1.50", "-1.50")]
    [DataRow("-9.999999999999999999999999999999999E+6144", "-9.999999999999999999999999999999999e+6144")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("NaN", "NaN")]
    public void WriteDecimal128WritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDecimal128(Decimal128.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-Infinity", "-INF")]
    public async Task WriteDecimal128WritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDecimal128Async(Decimal128.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-00.00:00:02.0000000", "-P0DT0H0M2.0000000S")]
    [DataRow("-00.00:00:02.1234567", "-P0DT0H0M2.1234567S")]
    [DataRow("-00.00:03:00.0000000", "-P0DT0H3M0.0000000S")]
    [DataRow("-00.00:03:02.0000000", "-P0DT0H3M2.0000000S")]
    [DataRow("-00.00:03:02.1234567", "-P0DT0H3M2.1234567S")]
    [DataRow("-00.04:00:00.0000000", "-P0DT4H0M0.0000000S")]
    [DataRow("-00.04:00:02.0000000", "-P0DT4H0M2.0000000S")]
    [DataRow("-00.04:00:02.1234567", "-P0DT4H0M2.1234567S")]
    [DataRow("-00.04:03:00.0000000", "-P0DT4H3M0.0000000S")]
    [DataRow("-00.04:03:02.0000000", "-P0DT4H3M2.0000000S")]
    [DataRow("-00.04:03:02.1234567", "-P0DT4H3M2.1234567S")]
    [DataRow("-05.00:00:00.0000000", "-P5DT0H0M0.0000000S")]
    [DataRow("-05.00:00:02.0000000", "-P5DT0H0M2.0000000S")]
    [DataRow("-05.00:00:02.1234567", "-P5DT0H0M2.1234567S")]
    [DataRow("-05.00:03:00.0000000", "-P5DT0H3M0.0000000S")]
    [DataRow("-05.00:03:02.0000000", "-P5DT0H3M2.0000000S")]
    [DataRow("-05.00:03:02.1234567", "-P5DT0H3M2.1234567S")]
    [DataRow("-05.04:00:00.0000000", "-P5DT4H0M0.0000000S")]
    [DataRow("-05.04:00:02.0000000", "-P5DT4H0M2.0000000S")]
    [DataRow("-05.04:00:02.1234567", "-P5DT4H0M2.1234567S")]
    [DataRow("-05.04:03:00.0000000", "-P5DT4H3M0.0000000S")]
    [DataRow("-05.04:03:02.0000000", "-P5DT4H3M2.0000000S")]
    [DataRow("-05.04:03:02.1234567", "-P5DT4H3M2.1234567S")]
    [DataRow("-10675199.02:48:05.4775808", "-P10675199DT2H48M5.4775808S")]
    [DataRow("00.00:00:00.0000000", "P0DT0H0M0.0000000S")]
    [DataRow("00.00:00:02.0000000", "P0DT0H0M2.0000000S")]
    [DataRow("00.00:00:02.1234567", "P0DT0H0M2.1234567S")]
    [DataRow("00.00:03:00.0000000", "P0DT0H3M0.0000000S")]
    [DataRow("00.00:03:02.0000000", "P0DT0H3M2.0000000S")]
    [DataRow("00.00:03:02.1234567", "P0DT0H3M2.1234567S")]
    [DataRow("00.04:00:00.0000000", "P0DT4H0M0.0000000S")]
    [DataRow("00.04:00:02.0000000", "P0DT4H0M2.0000000S")]
    [DataRow("00.04:00:02.1234567", "P0DT4H0M2.1234567S")]
    [DataRow("00.04:03:00.0000000", "P0DT4H3M0.0000000S")]
    [DataRow("00.04:03:02.0000000", "P0DT4H3M2.0000000S")]
    [DataRow("00.04:03:02.1234567", "P0DT4H3M2.1234567S")]
    [DataRow("01.12:00:00.0000000", "P1DT12H0M0.0000000S")]
    [DataRow("05.00:00:00.0000000", "P5DT0H0M0.0000000S")]
    [DataRow("05.00:00:02.0000000", "P5DT0H0M2.0000000S")]
    [DataRow("05.00:00:02.1234567", "P5DT0H0M2.1234567S")]
    [DataRow("05.00:03:00.0000000", "P5DT0H3M0.0000000S")]
    [DataRow("05.00:03:02.0000000", "P5DT0H3M2.0000000S")]
    [DataRow("05.00:03:02.1234567", "P5DT0H3M2.1234567S")]
    [DataRow("05.04:00:00.0000000", "P5DT4H0M0.0000000S")]
    [DataRow("05.04:00:02.0000000", "P5DT4H0M2.0000000S")]
    [DataRow("05.04:00:02.1234567", "P5DT4H0M2.1234567S")]
    [DataRow("05.04:03:00.0000000", "P5DT4H3M0.0000000S")]
    [DataRow("05.04:03:02.0000000", "P5DT4H3M2.0000000S")]
    [DataRow("05.04:03:02.1234567", "P5DT4H3M2.1234567S")]
    [DataRow("10675199.02:48:05.4775807", "P10675199DT2H48M5.4775807S")]
    public void WriteTimeSpanWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteTimeSpan(TimeSpan.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("-05.04:03:02.1234567", "-P5DT4H3M2.1234567S")]
    public async Task WriteTimeSpanWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteTimeSpanAsync(TimeSpan.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("fa-IR")]
    [DataRow("fr-CA")]
    [DataRow("sv-SE")]
    public void WriteTimeSpanIgnoresFormatProvider(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteTimeSpan(-TimeSpan.FromSeconds(1.5));
        writer.Flush();

        Assert.AreEqual("-P0DT0H0M1.5000000S", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("fa-IR")]
    [DataRow("fr-CA")]
    [DataRow("sv-SE")]
    public async Task WriteTimeSpanIgnoresFormatProviderAsync(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteTimeSpanAsync(-TimeSpan.FromSeconds(1.5));
        await writer.FlushAsync();

        Assert.AreEqual("-P0DT0H0M1.5000000S", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("04:05:06.0000000", "04:05:06.0000000")]
    [DataRow("04:05:06.0000007", "04:05:06.0000007")]
    public void WriteTimeOnlyWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteTimeOnly(TimeOnly.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("04:05:06.0000007", "04:05:06.0000007")]
    public async Task WriteTimeOnlyWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteTimeOnlyAsync(TimeOnly.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("04:05:06.0000000", "HH'.'mm'.'ss", "04.05.06")]
    public void WriteTimeOnlyWithFormatWritesFormattedValue(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularTimeOnlyConverter(format);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(TimeOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("04:05:06.0000000", "HH'.'mm'.'ss", "04.05.06")]
    public async Task WriteTimeOnlyWithFormatWritesFormattedValueAsync(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularTimeOnlyConverter(format);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(TimeOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03", "0001-02-03")]
    [DataRow("9999-12-31", "9999-12-31")]
    public void WriteDateOnlyWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDateOnly(DateOnly.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03", "0001-02-03")]
    public async Task WriteDateOnlyWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDateOnlyAsync(DateOnly.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public void WriteDateOnlyIgnoresFormatProviderCalendar(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteDateOnly(new(2024, 1, 2));
        writer.Flush();

        Assert.AreEqual("2024-01-02", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public async Task WriteDateOnlyIgnoresFormatProviderCalendarAsync(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteDateOnlyAsync(new(2024, 1, 2));
        await writer.FlushAsync();

        Assert.AreEqual("2024-01-02", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03", "dd'.'MM'.'yyyy", "03.02.0001")]
    public void WriteDateOnlyWithFormatWritesFormattedValue(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(DateOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03", "dd'.'MM'.'yyyy", "03.02.0001")]
    public async Task WriteDateOnlyWithFormatWritesFormattedValueAsync(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(DateOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("fr-CA", "2024-02-03", "dd MMMM yyyy", "03 f\u00e9vrier 2024")]
    public void WriteDateOnlyWithFormatUsesFormatProvider(string cultureName, string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.Write(DateOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("fr-CA", "2024-02-03", "dd MMMM yyyy", "03 f\u00e9vrier 2024")]
    public async Task WriteDateOnlyWithFormatUsesFormatProviderAsync(string cultureName, string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };
        var converter = new TabularDateOnlyConverter(format);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteAsync(DateOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void WriteDateOnlyUsesConverterFromOptions()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteDateOnly(new(1, 2, 3));
        writer.Flush();

        Assert.AreEqual("03.02.0001", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task WriteDateOnlyUsesConverterFromOptionsAsync()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularDateOnlyConverter("dd'.'MM'.'yyyy");

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteDateOnlyAsync(new(1, 2, 3));
        await writer.FlushAsync();

        Assert.AreEqual("03.02.0001", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000", "0001-02-03T04:05:06.0000000")]
    [DataRow("0001-02-03T04:05:06.0000000Z", "0001-02-03T04:05:06.0000000Z")]
    [DataRow("0001-02-03T04:05:06.0000007", "0001-02-03T04:05:06.0000007")]
    [DataRow("0001-02-03T04:05:06.0000007Z", "0001-02-03T04:05:06.0000007Z")]
    public void WriteDateTimeWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDateTime(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000007Z", "0001-02-03T04:05:06.0000007Z")]
    public async Task WriteDateTimeWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDateTimeAsync(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public void WriteDateTimeIgnoresFormatProviderCalendar(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteDateTime(new(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
        writer.Flush();

        Assert.AreEqual("2024-01-02T03:04:05.0000000Z", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public async Task WriteDateTimeIgnoresFormatProviderCalendarAsync(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteDateTimeAsync(new(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc));
        await writer.FlushAsync();

        Assert.AreEqual("2024-01-02T03:04:05.0000000Z", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000", "yyyy'/'MM'/'dd HH':'mm", "0001/02/03 04:05")]
    public void WriteDateTimeWithFormatWritesFormattedValue(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeConverter(format);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000", "yyyy'/'MM'/'dd HH':'mm", "0001/02/03 04:05")]
    public async Task WriteDateTimeWithFormatWritesFormattedValueAsync(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeConverter(format);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000+00:00", "0001-02-03T04:05:06.0000000+00:00")]
    [DataRow("0001-02-03T04:05:06.0000000+00:09", "0001-02-03T04:05:06.0000000+00:09")]
    [DataRow("0001-02-03T04:05:06.0000007+00:00", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow("0001-02-03T04:05:06.0000007+08:09", "0001-02-03T04:05:06.0000007+08:09")]
    [DataRow("0001-02-03T04:05:06.0000007-08:09", "0001-02-03T04:05:06.0000007-08:09")]
    public void WriteDateTimeOffsetWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteDateTimeOffset(DateTimeOffset.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000007+08:09", "0001-02-03T04:05:06.0000007+08:09")]
    public async Task WriteDateTimeOffsetWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteDateTimeOffsetAsync(DateTimeOffset.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public void WriteDateTimeOffsetIgnoresFormatProviderCalendar(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.WriteDateTimeOffset(new(2024, 1, 2, 3, 4, 5, TimeSpan.FromHours(1)));
        writer.Flush();

        Assert.AreEqual("2024-01-02T03:04:05.0000000+01:00", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("ar-SA")]
    [DataRow("fa-IR")]
    [DataRow("th-TH")]
    public async Task WriteDateTimeOffsetIgnoresFormatProviderCalendarAsync(string cultureName)
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { FormatProvider = CultureInfo.GetCultureInfo(cultureName) };

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteDateTimeOffsetAsync(new(2024, 1, 2, 3, 4, 5, TimeSpan.FromHours(1)));
        await writer.FlushAsync();

        Assert.AreEqual("2024-01-02T03:04:05.0000000+01:00", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000+08:09", "yyyy'-'MM'-'dd HH':'mm zzz", "0001-02-03 04:05 +08:09")]
    public void WriteDateTimeOffsetWithFormatWritesFormattedValue(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeOffsetConverter(format);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(DateTimeOffset.Parse(value, CultureInfo.InvariantCulture), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000+08:09", "yyyy'-'MM'-'dd HH':'mm zzz", "0001-02-03 04:05 +08:09")]
    public async Task WriteDateTimeOffsetWithFormatWritesFormattedValueAsync(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateTimeOffsetConverter(format);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(DateTimeOffset.Parse(value, CultureInfo.InvariantCulture), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    [DataRow("123E4567-E89B-12D3-A456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    public void WriteGuidWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteGuid(Guid.Parse(value, CultureInfo.InvariantCulture));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("123E4567-E89B-12D3-A456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    public async Task WriteGuidWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteGuidAsync(Guid.Parse(value, CultureInfo.InvariantCulture));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "B", "{123e4567-e89b-12d3-a456-426614174000}")]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "N", "123e4567e89b12d3a456426614174000")]
    public void WriteGuidWithFormatWritesFormattedValue(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter(format);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(Guid.Parse(value, CultureInfo.InvariantCulture), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "B", "{123e4567-e89b-12d3-a456-426614174000}")]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "N", "123e4567e89b12d3a456426614174000")]
    public async Task WriteGuidWithFormatWritesFormattedValueAsync(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter(format);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(Guid.Parse(value, CultureInfo.InvariantCulture), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("a b/c", "a b/c")]
    [DataRow("https://example.com/", "https://example.com/")]
    [DataRow("https://example.com/\u00e4", "https://example.com/%C3%A4")]
    [DataRow("https://example.com/a b?q=a b", "https://example.com/a%20b?q=a%20b")]
    [DataRow("https://example.com/a%20b", "https://example.com/a%20b")]
    [DataRow("urn:com.example", "urn:com.example")]
    public void WriteUriWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteUri(new(value, UriKind.RelativeOrAbsolute));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("https://example.com/a b?q=a b", "https://example.com/a%20b?q=a%20b")]
    public async Task WriteUriWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteUriAsync(new(value, UriKind.RelativeOrAbsolute));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void WriteUriWritesEmptyFieldForNull()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteUri(null);
        writer.Flush();

        Assert.AreEqual("", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task WriteUriWritesEmptyFieldForNullAsync()
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteUriAsync(null);
        await writer.FlushAsync();

        Assert.AreEqual("", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("00FF", "00ff")]
    [DataRow("123E4567E89B12D3A456426614174000", "123e4567e89b12d3a456426614174000")]
    public void WriteBase16ArrayWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase16Array(Convert.FromHexString(value));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("00FF", "00ff")]
    public async Task WriteBase16ArrayWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase16ArrayAsync(Convert.FromHexString(value));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void WriteBase16ArrayWritesEmptyFieldForNull()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase16Array(null);
        writer.Flush();

        Assert.AreEqual("", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task WriteBase16ArrayWritesEmptyFieldForNullAsync()
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase16ArrayAsync(null);
        await writer.FlushAsync();

        Assert.AreEqual("", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("313938342D30352D3034", "MTk4NC0wNS0wNA==")]
    [DataRow("66", "Zg==")]
    [DataRow("666F", "Zm8=")]
    [DataRow("666F6F", "Zm9v")]
    [DataRow("FBFFBF", "+/+/")]
    public void WriteBase64ArrayWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase64Array(Convert.FromHexString(value));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("66", "Zg==")]
    public async Task WriteBase64ArrayWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase64ArrayAsync(Convert.FromHexString(value));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void WriteBase64ArrayWritesEmptyFieldForNull()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase64Array(null);
        writer.Flush();

        Assert.AreEqual("", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task WriteBase64ArrayWritesEmptyFieldForNullAsync()
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase64ArrayAsync(null);
        await writer.FlushAsync();

        Assert.AreEqual("", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("00FF", "00ff")]
    public void WriteBase16MemoryWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase16Memory(Convert.FromHexString(value).AsMemory());
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("00FF", "00ff")]
    public async Task WriteBase16MemoryWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase16MemoryAsync(Convert.FromHexString(value).AsMemory());
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("666F6F", "Zm9v")]
    public void WriteBase64MemoryWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase64Memory(Convert.FromHexString(value).AsMemory());
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("666F6F", "Zm9v")]
    public async Task WriteBase64MemoryWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase64MemoryAsync(Convert.FromHexString(value).AsMemory());
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("AA00FFAA", "00ff")]
    [DataRow("AAAA", "")]
    public void WriteBase16ReadOnlyMemoryWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase16ReadOnlyMemory(Convert.FromHexString(value).AsMemory()[1..^1]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("AA00FFAA", "00ff")]
    public async Task WriteBase16ReadOnlyMemoryWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase16ReadOnlyMemoryAsync(Convert.FromHexString(value).AsMemory()[1..^1]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("AA666F6FAA", "Zm9v")]
    [DataRow("AAAA", "")]
    public void WriteBase64ReadOnlyMemoryWritesCanonicalForm(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase64ReadOnlyMemory(Convert.FromHexString(value).AsMemory()[1..^1]);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("AA666F6FAA", "Zm9v")]
    public async Task WriteBase64ReadOnlyMemoryWritesCanonicalFormAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase64ReadOnlyMemoryAsync(Convert.FromHexString(value).AsMemory()[1..^1]);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("00FF", "00ff")]
    public void WriteWithBase16MemoryConverterWritesEncodedValue(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase16MemoryConverter();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(new Memory<byte>(Convert.FromHexString(value)), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("00FF", "00ff")]
    public async Task WriteWithBase16MemoryConverterWritesEncodedValueAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase16MemoryConverter();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(new Memory<byte>(Convert.FromHexString(value)), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("00FF", "00ff")]
    public void WriteWithBase16ReadOnlyMemoryConverterWritesEncodedValue(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase16ReadOnlyMemoryConverter();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(new ReadOnlyMemory<byte>(Convert.FromHexString(value)), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("00FF", "00ff")]
    public async Task WriteWithBase16ReadOnlyMemoryConverterWritesEncodedValueAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase16ReadOnlyMemoryConverter();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(new ReadOnlyMemory<byte>(Convert.FromHexString(value)), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("666F6F", "Zm9v")]
    public void WriteWithBase64MemoryConverterWritesEncodedValue(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase64MemoryConverter();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(new Memory<byte>(Convert.FromHexString(value)), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("666F6F", "Zm9v")]
    public async Task WriteWithBase64MemoryConverterWritesEncodedValueAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase64MemoryConverter();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(new Memory<byte>(Convert.FromHexString(value)), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("666F6F", "Zm9v")]
    public void WriteWithBase64ReadOnlyMemoryConverterWritesEncodedValue(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase64ReadOnlyMemoryConverter();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(new ReadOnlyMemory<byte>(Convert.FromHexString(value)), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("666F6F", "Zm9v")]
    public async Task WriteWithBase64ReadOnlyMemoryConverterWritesEncodedValueAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularBase64ReadOnlyMemoryConverter();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(new ReadOnlyMemory<byte>(Convert.FromHexString(value)), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow(100)]
    [DataRow(1000)]
    public void WriteBase16ArrayWritesLongValue(int length)
    {
        var dialect = CreateValueDialect();
        var value = Enumerable.Range(0, length).Select(static x => (byte)x).ToArray();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase16Array(value);
        writer.Flush();

        Assert.AreEqual(Convert.ToHexStringLower(value), Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow(100)]
    [DataRow(1000)]
    public async Task WriteBase16ArrayWritesLongValueAsync(int length)
    {
        var dialect = CreateValueDialect();
        var value = Enumerable.Range(0, length).Select(static x => (byte)x).ToArray();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase16ArrayAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(Convert.ToHexStringLower(value), Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow(100)]
    [DataRow(1000)]
    public void WriteBase64ArrayWritesLongValue(int length)
    {
        var dialect = CreateValueDialect();
        var value = Enumerable.Range(0, length).Select(static x => (byte)x).ToArray();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteBase64Array(value);
        writer.Flush();

        Assert.AreEqual(Convert.ToBase64String(value), Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow(100)]
    [DataRow(1000)]
    public async Task WriteBase64ArrayWritesLongValueAsync(int length)
    {
        var dialect = CreateValueDialect();
        var value = Enumerable.Range(0, length).Select(static x => (byte)x).ToArray();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteBase64ArrayAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(Convert.ToBase64String(value), Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow('$', "'$'")]
    [DataRow('\'', "''''")]
    [DataRow('^', "'^'")]
    public void WriteCharQuotesValue(char value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteChar(value);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow('$', "'$'")]
    [DataRow('\'', "''''")]
    [DataRow('^', "'^'")]
    public async Task WriteCharQuotesValueAsync(char value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteCharAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("a'b", "'a''b'")]
    public void WriteUriQuotesValue(string value, string expected)
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteUri(new(value, UriKind.RelativeOrAbsolute));
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("a'b", "'a''b'")]
    public async Task WriteUriQuotesValueAsync(string value, string expected)
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteUriAsync(new(value, UriKind.RelativeOrAbsolute));
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("2024-02-03", "yyyy'^'MM", "'2024^02'")]
    [DataRow("2024-02-03", "yyyy\\'MM", "'2024''02'")]
    public void WriteWithConverterQuotesFormattedValue(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(DateOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("2024-02-03", "yyyy'^'MM", "'2024^02'")]
    [DataRow("2024-02-03", "yyyy\\'MM", "'2024''02'")]
    public async Task WriteWithConverterQuotesFormattedValueAsync(string value, string format, string expected)
    {
        var dialect = CreateValueDialect();
        var converter = new TabularDateOnlyConverter(format);

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteAsync(DateOnly.Parse(value, CultureInfo.InvariantCulture), converter);
        await writer.FlushAsync();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void WriteThrowsArgumentNullExceptionForNullConverter()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        Assert.ThrowsExactly<ArgumentNullException>(() => writer.Write("v", null!));
    }

    [TestMethod]
    public async Task WriteThrowsArgumentNullExceptionForNullConverterAsync()
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect);

        Assert.ThrowsExactly<ArgumentNullException>(() => writer.WriteAsync("v", null!).AsTask());
    }

    [TestMethod]
    public void ResolveConverterReturnsBuiltInConverter()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        writer.Write(new DateOnly(1, 2, 3), writer.ResolveConverter<DateOnly>());
        writer.Flush();

        Assert.AreEqual("0001-02-03", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void ResolveConverterReturnsConverterFromOptions()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(Memory<byte>)] = new TabularBase16MemoryConverter();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect, options);

        writer.Write(new Memory<byte>([0x0a, 0xff]), writer.ResolveConverter<Memory<byte>>());
        writer.Flush();

        Assert.AreEqual("0aff", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task ResolveConverterReturnsConverterFromOptionsAsync()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(Memory<byte>)] = new TabularBase16MemoryConverter();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteAsync(new Memory<byte>([0x0a, 0xff]), writer.ResolveConverter<Memory<byte>>());
        await writer.FlushAsync();

        Assert.AreEqual("0aff", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public void ResolveConverterThrowsInvalidOperationExceptionForTypeWithoutConverter()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        Assert.ThrowsExactly<InvalidOperationException>(() => writer.ResolveConverter<Memory<byte>>());
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForConverterOfOtherType()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = new TabularInt32Converter();

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularWriter(stream, dialect, options));
    }

    [TestMethod]
    public void ConstructorThrowsArgumentExceptionForNullConverter()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions();

        options.Converters[typeof(DateOnly)] = null!;

        using var stream = new MemoryStream();

        Assert.ThrowsExactly<ArgumentException>(() => new TabularWriter(stream, dialect, options));
    }

    [TestMethod]
    public void WriteInt32ThrowsObjectDisposedExceptionAfterDispose()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect);

        writer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.WriteInt32(1));
    }

    [TestMethod]
    public async Task WriteInt32ThrowsObjectDisposedExceptionAfterDisposeAsync()
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect);

        await writer.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteInt32Async(1).AsTask());
    }

    [TestMethod]
    public void WriteThrowsObjectDisposedExceptionAfterDispose()
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter("N");

        using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect);

        writer.Dispose();

        Assert.ThrowsExactly<ObjectDisposedException>(() => writer.Write(Guid.Empty, converter));
    }

    [TestMethod]
    public async Task WriteThrowsObjectDisposedExceptionAfterDisposeAsync()
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter("N");

        await using var stream = new MemoryStream();

        var writer = new TabularWriter(stream, dialect);

        await writer.DisposeAsync();

        await Assert.ThrowsExactlyAsync<ObjectDisposedException>(() => writer.WriteAsync(Guid.Empty, converter).AsTask());
    }

    [TestMethod]
    public void WriteInt32ThrowsInvalidOperationExceptionAfterFault()
    {
        var dialect = CreateValueDialect();

        using var stream = new MemoryStream([]);
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");

        Assert.ThrowsExactly<NotSupportedException>(() => writer.Flush());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.WriteInt32(1));
    }

    [TestMethod]
    public async Task WriteInt32ThrowsInvalidOperationExceptionAfterFaultAsync()
    {
        var dialect = CreateValueDialect();

        await using var stream = new MemoryStream([]);
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => writer.FlushAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteInt32Async(1).AsTask());
    }

    [TestMethod]
    public void WriteThrowsInvalidOperationExceptionAfterFault()
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter("N");

        using var stream = new MemoryStream([]);
        using var writer = new TabularWriter(stream, dialect);

        writer.WriteString("v");

        Assert.ThrowsExactly<NotSupportedException>(() => writer.Flush());
        Assert.ThrowsExactly<InvalidOperationException>(() => writer.Write(Guid.Empty, converter));
    }

    [TestMethod]
    public async Task WriteThrowsInvalidOperationExceptionAfterFaultAsync()
    {
        var dialect = CreateValueDialect();
        var converter = new TabularGuidConverter("N");

        await using var stream = new MemoryStream([]);
        await using var writer = new TabularWriter(stream, dialect);

        await writer.WriteStringAsync("v");

        await Assert.ThrowsExactlyAsync<NotSupportedException>(() => writer.FlushAsync().AsTask());
        await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => writer.WriteAsync(Guid.Empty, converter).AsTask());
    }

    [TestMethod]
    public async Task WriteInt32AsyncDoesNotUseSynchronousIO()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { BufferSize = 1 };

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteInt32Async(123);
        await writer.WriteInt32Async(-45);
        await writer.FlushAsync();

        Assert.AreEqual("123^-45", Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    public async Task WriteBase16ArrayAsyncDoesNotUseSynchronousIO()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { BufferSize = 1 };
        var value = Enumerable.Range(0, 200).Select(static x => (byte)x).ToArray();

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);
        await using var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteBase16ArrayAsync(value);
        await writer.FlushAsync();

        Assert.AreEqual(Convert.ToHexStringLower(value), Encoding.UTF8.GetString(target.ToArray()));
    }

    [TestMethod]
    public async Task WriteAsyncWithConverterDoesNotUseSynchronousIO()
    {
        var dialect = CreateValueDialect();
        var options = new TabularOptions { BufferSize = 1 };
        var converter = new TabularGuidConverter("N");

        await using var target = new MemoryStream();
        await using var stream = new AsyncOnlyStream(target);

        var writer = new TabularWriter(stream, dialect, options);

        await writer.WriteAsync(Guid.Empty, converter);
        await writer.WriteAsync(Guid.Empty, converter);
        await writer.DisposeAsync();

        Assert.AreEqual("00000000000000000000000000000000^00000000000000000000000000000000$", Encoding.UTF8.GetString(target.ToArray()));
    }

    private static TabularDialect CreateValueDialect()
    {
        return new("$", '^', '\'');
    }
}
