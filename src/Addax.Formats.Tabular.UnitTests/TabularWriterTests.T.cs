using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

public partial class TabularWriterTests
{
    private delegate void WriteValue<in T>(T value);

    private static void Write<T>(Func<TabularWriter, WriteValue<T>> selector, T value, string expected)
    {
        var dialect = new TabularDialect("\u000a", '\u000b', '\u000c', '\u000d');

        using var stream = new MemoryStream();
        using var writer = new TabularWriter(stream, dialect);

        var method = selector.Invoke(writer);

        method.Invoke(value);
        writer.Flush();

        Assert.AreEqual(expected, Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    [DataRow("False", "false")]
    [DataRow("True", "true")]
    public void WriteBoolean(string value, string expected)
    {
        Write(x => x.WriteBoolean, bool.Parse(value), expected);
    }

    [TestMethod]
    [DataRow("a", "a")]
    public void WriteChar(string value, string expected)
    {
        Write(x => x.WriteChar, char.Parse(value), expected);
    }

    [TestMethod]
    [DataRow("-128", "-128")]
    [DataRow("+127", "127")]
    public void WriteSByte(string value, string expected)
    {
        Write(x => x.WriteSByte, sbyte.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("0", "0")]
    [DataRow("+255", "255")]
    public void WriteByte(string value, string expected)
    {
        Write(x => x.WriteByte, byte.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("-32768", "-32768")]
    [DataRow("+32767", "32767")]
    public void WriteInt16(string value, string expected)
    {
        Write(x => x.WriteInt16, short.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("0", "0")]
    [DataRow("+65535", "65535")]
    public void WriteUInt16(string value, string expected)
    {
        Write(x => x.WriteUInt16, ushort.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("-2147483648", "-2147483648")]
    [DataRow("+2147483647", "2147483647")]
    public void WriteInt32(string value, string expected)
    {
        Write(x => x.WriteInt32, int.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("0", "0")]
    [DataRow("+4294967295", "4294967295")]
    public void WriteUInt32(string value, string expected)
    {
        Write(x => x.WriteUInt32, uint.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("-9223372036854775808", "-9223372036854775808")]
    [DataRow("+9223372036854775807", "9223372036854775807")]
    public void WriteInt64(string value, string expected)
    {
        Write(x => x.WriteInt64, long.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("0", "0")]
    [DataRow("+18446744073709551615", "18446744073709551615")]
    public void WriteUInt64(string value, string expected)
    {
        Write(x => x.WriteUInt64, ulong.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("-170141183460469231731687303715884105728", "-170141183460469231731687303715884105728")]
    [DataRow("+170141183460469231731687303715884105727", "170141183460469231731687303715884105727")]
    public void WriteInt128(string value, string expected)
    {
        Write(x => x.WriteInt128, Int128.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("0", "0")]
    [DataRow("+340282366920938463463374607431768211455", "340282366920938463463374607431768211455")]
    public void WriteUInt128(string value, string expected)
    {
        Write(x => x.WriteUInt128, UInt128.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("-128", "-128")]
    [DataRow("+127", "127")]
    [DataRow("-999999999999999999999999999999999999999", "-999999999999999999999999999999999999999")]
    [DataRow("+999999999999999999999999999999999999999", "999999999999999999999999999999999999999")]
    public void WriteBigInteger(string value, string expected)
    {
        Write(x => x.WriteBigInteger, BigInteger.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("NaN", "NaN")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("-65500", "-65500")]
    [DataRow("+65500", "65500")]
    [DataRow("+Infinity", "INF")]
    public void WriteHalf(string value, string expected)
    {
        Write(x => x.WriteHalf, Half.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("NaN", "NaN")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("-3.402823E+38", "-3.402823e+38")]
    [DataRow("+3.402823E+38", "3.402823e+38")]
    [DataRow("+Infinity", "INF")]
    public void WriteSingle(string value, string expected)
    {
        Write(x => x.WriteSingle, float.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("NaN", "NaN")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("-1.7976931348623157E+308", "-1.7976931348623157e+308")]
    [DataRow("+1.7976931348623157E+308", "1.7976931348623157e+308")]
    [DataRow("+Infinity", "INF")]
    public void WriteDouble(string value, string expected)
    {
        Write(x => x.WriteDouble, double.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("-79228162514264337593543950335", "-79228162514264337593543950335")]
    [DataRow("+79228162514264337593543950335", "79228162514264337593543950335")]
    public void WriteDecimal(string value, string expected)
    {
        Write(x => x.WriteDecimal, decimal.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("00.00:00:02.0000000", "P0DT0H0M2.0000000S")]
    [DataRow("00.00:03:02.0000000", "P0DT0H3M2.0000000S")]
    [DataRow("00.04:03:02.0000000", "P0DT4H3M2.0000000S")]
    [DataRow("05.04:03:02.0000000", "P5DT4H3M2.0000000S")]
    [DataRow("00.00:00:02.1234567", "P0DT0H0M2.1234567S")]
    [DataRow("00.00:03:02.1234567", "P0DT0H3M2.1234567S")]
    [DataRow("00.04:03:02.1234567", "P0DT4H3M2.1234567S")]
    [DataRow("05.04:03:02.1234567", "P5DT4H3M2.1234567S")]
    [DataRow("00.00:03:00.0000000", "P0DT0H3M0.0000000S")]
    [DataRow("00.04:00:00.0000000", "P0DT4H0M0.0000000S")]
    [DataRow("00.04:03:00.0000000", "P0DT4H3M0.0000000S")]
    [DataRow("00.04:00:02.0000000", "P0DT4H0M2.0000000S")]
    [DataRow("05.00:00:00.0000000", "P5DT0H0M0.0000000S")]
    [DataRow("05.04:00:00.0000000", "P5DT4H0M0.0000000S")]
    [DataRow("05.04:03:00.0000000", "P5DT4H3M0.0000000S")]
    [DataRow("05.04:00:02.0000000", "P5DT4H0M2.0000000S")]
    [DataRow("05.00:03:00.0000000", "P5DT0H3M0.0000000S")]
    [DataRow("05.00:03:02.0000000", "P5DT0H3M2.0000000S")]
    [DataRow("05.00:00:02.0000000", "P5DT0H0M2.0000000S")]
    [DataRow("00.04:00:02.1234567", "P0DT4H0M2.1234567S")]
    [DataRow("05.04:00:02.1234567", "P5DT4H0M2.1234567S")]
    [DataRow("05.00:03:02.1234567", "P5DT0H3M2.1234567S")]
    [DataRow("05.00:00:02.1234567", "P5DT0H0M2.1234567S")]
    [DataRow("-00.00:00:02.0000000", "-P0DT0H0M2.0000000S")]
    [DataRow("-00.00:03:02.0000000", "-P0DT0H3M2.0000000S")]
    [DataRow("-00.04:03:02.0000000", "-P0DT4H3M2.0000000S")]
    [DataRow("-05.04:03:02.0000000", "-P5DT4H3M2.0000000S")]
    [DataRow("-00.00:00:02.1234567", "-P0DT0H0M2.1234567S")]
    [DataRow("-00.00:03:02.1234567", "-P0DT0H3M2.1234567S")]
    [DataRow("-00.04:03:02.1234567", "-P0DT4H3M2.1234567S")]
    [DataRow("-05.04:03:02.1234567", "-P5DT4H3M2.1234567S")]
    [DataRow("-00.00:03:00.0000000", "-P0DT0H3M0.0000000S")]
    [DataRow("-00.04:00:00.0000000", "-P0DT4H0M0.0000000S")]
    [DataRow("-00.04:03:00.0000000", "-P0DT4H3M0.0000000S")]
    [DataRow("-00.04:00:02.0000000", "-P0DT4H0M2.0000000S")]
    [DataRow("-05.00:00:00.0000000", "-P5DT0H0M0.0000000S")]
    [DataRow("-05.04:00:00.0000000", "-P5DT4H0M0.0000000S")]
    [DataRow("-05.04:03:00.0000000", "-P5DT4H3M0.0000000S")]
    [DataRow("-05.04:00:02.0000000", "-P5DT4H0M2.0000000S")]
    [DataRow("-05.00:03:00.0000000", "-P5DT0H3M0.0000000S")]
    [DataRow("-05.00:03:02.0000000", "-P5DT0H3M2.0000000S")]
    [DataRow("-05.00:00:02.0000000", "-P5DT0H0M2.0000000S")]
    [DataRow("-00.04:00:02.1234567", "-P0DT4H0M2.1234567S")]
    [DataRow("-05.04:00:02.1234567", "-P5DT4H0M2.1234567S")]
    [DataRow("-05.00:03:02.1234567", "-P5DT0H3M2.1234567S")]
    [DataRow("-05.00:00:02.1234567", "-P5DT0H0M2.1234567S")]
    public void WriteTimeSpan(string value, string expected)
    {
        Write(x => x.WriteTimeSpan, TimeSpan.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("04:05:06.0000000", "04:05:06.0000000")]
    [DataRow("04:05:06.0000007", "04:05:06.0000007")]
    public void WriteTimeOnly(string value, string expected)
    {
        Write(x => x.WriteTimeOnly, TimeOnly.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None), expected);
    }

    [TestMethod]
    [DataRow("0001-02-03", "0001-02-03")]
    public void WriteDateOnly(string value, string expected)
    {
        Write(x => x.WriteDateOnly, DateOnly.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None), expected);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000", "0001-02-03T04:05:06.0000000")]
    [DataRow("0001-02-03T04:05:06.0000000Z", "0001-02-03T04:05:06.0000000Z")]
    [DataRow("0001-02-03T04:05:06.0000007", "0001-02-03T04:05:06.0000007")]
    [DataRow("0001-02-03T04:05:06.0000007Z", "0001-02-03T04:05:06.0000007Z")]
    public void WriteDateTime(string value, string expected)
    {
        Write(x => x.WriteDateTime, DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal), expected);
    }

    [TestMethod]
    [DataRow("0001-02-03T04:05:06.0000000+00:00", "0001-02-03T04:05:06.0000000+00:00")]
    [DataRow("0001-02-03T04:05:06.0000000+00:09", "0001-02-03T04:05:06.0000000+00:09")]
    [DataRow("0001-02-03T04:05:06.0000007+00:00", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow("0001-02-03T04:05:06.0000007+08:09", "0001-02-03T04:05:06.0000007+08:09")]
    public void WriteDateTimeOffset(string value, string expected)
    {
        Write(x => x.WriteDateTimeOffset, DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal), expected);
    }

    [TestMethod]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    public void WriteGuid(string value, string expected)
    {
        Write(x => x.WriteGuid, Guid.Parse(value, CultureInfo.InvariantCulture), expected);
    }

    [TestMethod]
    [DataRow("urn:com.example", "urn:com.example")]
    [DataRow("https://example.com/", "https://example.com/")]
    public void WriteUri(string value, string expected)
    {
        Write(x => x.WriteUri, new Uri(value, UriKind.RelativeOrAbsolute), expected);
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("123e4567e89b12d3a456426614174000", "123e4567e89b12d3a456426614174000")]
    public void WriteBase16Binary(string value, string expected)
    {
        Write(x => x.WriteBase16Binary, Convert.FromHexString(value), expected);
    }

    [TestMethod]
    [DataRow("", "")]
    [DataRow("MTk4NC0wNS0wNA==", "MTk4NC0wNS0wNA==")]
    public void WriteBase64Binary(string value, string expected)
    {
        Write(x => x.WriteBase64Binary, Convert.FromBase64String(value), expected);
    }

    [TestMethod]
    [DataRow("v", "v")]
    [DataRow("\ud83d\udd2e", "\ud83d\udd2e")]
    public void WriteRune(string value, string expected)
    {
        Write(x => x.WriteRune, Rune.GetRuneAt(value, 0), expected);
    }
}
