#pragma warning disable IDE1006

using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

public partial class TabularFieldWriterTests
{
    private delegate void WriteAction<in T>(T value);

    private static async Task AssertWriteAsync<T>(T value, string expected, Func<TabularFieldWriter, WriteAction<T>> selector, CancellationToken cancellationToken)
    {
        var dialect = new TabularDataDialect("\u000a", '\u001a', '\u001b', '\u001c');

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect);

        var method = selector.Invoke(writer);

        writer.BeginRecord();
        method.Invoke(value);

        await writer.FlushAsync(cancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, Encoding.UTF8);
        var result = await reader.ReadToEndAsync(cancellationToken);

        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public async Task WriteT()
    {
        var dialect = new TabularDataDialect("\u000a", '\u001a', '\u001b', '\u001c');

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect);

        writer.BeginRecord();
        writer.Write('a', TabularTryFormatFunc);

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        CollectionAssert.AreEqual("a"u8.ToArray(), stream.ToArray());

        static bool TabularTryFormatFunc(char value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
        {
            if (buffer.Length > 512)
            {
                buffer[0] = value;
                charsWritten = 1;

                return true;
            }
            else
            {
                charsWritten = 0;

                return false;
            }
        }
    }

    [DataTestMethod]
    [DataRow("False", "false")]
    [DataRow("True", "true")]
    public Task WriteBoolean(string projection, string expected)
    {
        var value = bool.Parse(projection);

        return AssertWriteAsync(value, expected, writer => writer.WriteBoolean, CancellationToken);
    }

    [DataTestMethod]
    [DataRow('a', "a")]
    public Task WriteChar(char value, string expected)
    {
        return AssertWriteAsync(value, expected, writer => writer.WriteChar, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("a", "a")]
    [DataRow("\ud83e\udd70", "\ud83e\udd70")]
    public Task WriteRune(string projection, string expected)
    {
        Rune.DecodeFromUtf16(projection, out var value, out _);

        return AssertWriteAsync(value, expected, writer => writer.WriteRune, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("-128", "-128")]
    [DataRow("+127", "127")]
    public Task WriteSByte(string projection, string expected)
    {
        var value = sbyte.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteSByte, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0", "0")]
    [DataRow("+255", "255")]
    public Task WriteByte(string projection, string expected)
    {
        var value = byte.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteByte, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("-32768", "-32768")]
    [DataRow("+32767", "32767")]
    public Task WriteInt16(string projection, string expected)
    {
        var value = short.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteInt16, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0", "0")]
    [DataRow("+65535", "65535")]
    public Task WriteUInt16(string projection, string expected)
    {
        var value = ushort.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteUInt16, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("-2147483648", "-2147483648")]
    [DataRow("+2147483647", "2147483647")]
    public Task WriteInt32(string projection, string expected)
    {
        var value = int.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteInt32, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0", "0")]
    [DataRow("+4294967295", "4294967295")]
    public Task WriteUInt32(string projection, string expected)
    {
        var value = uint.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteUInt32, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("-9223372036854775808", "-9223372036854775808")]
    [DataRow("+9223372036854775807", "9223372036854775807")]
    public Task WriteInt64(string projection, string expected)
    {
        var value = long.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteInt64, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0", "0")]
    [DataRow("+18446744073709551615", "18446744073709551615")]
    public Task WriteUInt64(string projection, string expected)
    {
        var value = ulong.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteUInt64, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("-170141183460469231731687303715884105728", "-170141183460469231731687303715884105728")]
    [DataRow("+170141183460469231731687303715884105727", "170141183460469231731687303715884105727")]
    public Task WriteInt128(string projection, string expected)
    {
        var value = Int128.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteInt128, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0", "0")]
    [DataRow("+340282366920938463463374607431768211455", "340282366920938463463374607431768211455")]
    public Task WriteUInt128(string projection, string expected)
    {
        var value = UInt128.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteUInt128, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("-128", "-128")]
    [DataRow("+127", "127")]
    [DataRow("-999999999999999999999999999999999999999", "-999999999999999999999999999999999999999")]
    [DataRow("+999999999999999999999999999999999999999", "999999999999999999999999999999999999999")]
    public Task WriteBigInteger(string projection, string expected)
    {
        var value = BigInteger.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteBigInteger, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("NaN", "NaN")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("-65500", "-65500")]
    [DataRow("+65500", "65500")]
    [DataRow("+Infinity", "INF")]
    public Task WriteHalf(string projection, string expected)
    {
        var value = Half.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteHalf, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("NaN", "NaN")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("-3.402823E+38", "-3.402823e+38")]
    [DataRow("+3.402823E+38", "3.402823e+38")]
    [DataRow("+Infinity", "INF")]
    public Task WriteSingle(string projection, string expected)
    {
        var value = float.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteSingle, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("NaN", "NaN")]
    [DataRow("-Infinity", "-INF")]
    [DataRow("-1.7976931348623157E+308", "-1.7976931348623157e+308")]
    [DataRow("+1.7976931348623157E+308", "1.7976931348623157e+308")]
    [DataRow("+Infinity", "INF")]
    public Task WriteDouble(string projection, string expected)
    {
        var value = double.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteDouble, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("-79228162514264337593543950335", "-79228162514264337593543950335")]
    [DataRow("+79228162514264337593543950335", "79228162514264337593543950335")]
    public Task WriteDecimal(string projection, string expected)
    {
        var value = decimal.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteDecimal, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0", "0", "0")]
    [DataRow("-1", "0", "-1")]
    [DataRow("+1", "0", "1")]
    [DataRow("0", "-2", "-2i")]
    [DataRow("0", "+2", "2i")]
    [DataRow("0", "-1", "-i")]
    [DataRow("0", "+1", "i")]
    [DataRow("-1", "-2", "-1-2i")]
    [DataRow("-1", "+2", "-1+2i")]
    [DataRow("-1", "-1", "-1-i")]
    [DataRow("-1", "+1", "-1+i")]
    [DataRow("-1.1e+100", "0", "-1.1e+100")]
    [DataRow("-1.1e-100", "0", "-1.1e-100")]
    [DataRow("+1.1e+100", "0", "1.1e+100")]
    [DataRow("+1.1e-100", "0", "1.1e-100")]
    [DataRow("0", "-2.2e+100", "-2.2e+100i")]
    [DataRow("0", "-2.2e-100", "-2.2e-100i")]
    [DataRow("0", "+2.2e+100", "2.2e+100i")]
    [DataRow("0", "+2.2e-100", "2.2e-100i")]
    [DataRow("-1.1e+100", "-2.2e+100", "-1.1e+100-2.2e+100i")]
    [DataRow("-1.1e+100", "-2.2e-100", "-1.1e+100-2.2e-100i")]
    [DataRow("-1.1e-100", "-2.2e+100", "-1.1e-100-2.2e+100i")]
    [DataRow("-1.1e-100", "-2.2e-100", "-1.1e-100-2.2e-100i")]
    [DataRow("-1.1e+100", "+2.2e+100", "-1.1e+100+2.2e+100i")]
    [DataRow("-1.1e+100", "+2.2e-100", "-1.1e+100+2.2e-100i")]
    [DataRow("-1.1e-100", "+2.2e+100", "-1.1e-100+2.2e+100i")]
    [DataRow("-1.1e-100", "+2.2e-100", "-1.1e-100+2.2e-100i")]
    [DataRow("+1.1e+100", "-2.2e+100", "1.1e+100-2.2e+100i")]
    [DataRow("+1.1e+100", "-2.2e-100", "1.1e+100-2.2e-100i")]
    [DataRow("+1.1e-100", "-2.2e+100", "1.1e-100-2.2e+100i")]
    [DataRow("+1.1e-100", "-2.2e-100", "1.1e-100-2.2e-100i")]
    [DataRow("+1.1e+100", "+2.2e+100", "1.1e+100+2.2e+100i")]
    [DataRow("+1.1e+100", "+2.2e-100", "1.1e+100+2.2e-100i")]
    [DataRow("+1.1e-100", "+2.2e+100", "1.1e-100+2.2e+100i")]
    [DataRow("+1.1e-100", "+2.2e-100", "1.1e-100+2.2e-100i")]
    [DataRow("-1", "-2.2e+100", "-1-2.2e+100i")]
    [DataRow("-1", "-2.2e-100", "-1-2.2e-100i")]
    [DataRow("-1", "+2.2e+100", "-1+2.2e+100i")]
    [DataRow("-1", "+2.2e-100", "-1+2.2e-100i")]
    [DataRow("+1", "-2.2e+100", "1-2.2e+100i")]
    [DataRow("+1", "-2.2e-100", "1-2.2e-100i")]
    [DataRow("+1", "+2.2e+100", "1+2.2e+100i")]
    [DataRow("+1", "+2.2e-100", "1+2.2e-100i")]
    [DataRow("-1.1e+100", "-2", "-1.1e+100-2i")]
    [DataRow("-1.1e+100", "-2", "-1.1e+100-2i")]
    [DataRow("-1.1e+100", "+2", "-1.1e+100+2i")]
    [DataRow("-1.1e+100", "+2", "-1.1e+100+2i")]
    [DataRow("+1.1e+100", "-2", "1.1e+100-2i")]
    [DataRow("+1.1e+100", "-2", "1.1e+100-2i")]
    [DataRow("+1.1e+100", "+2", "1.1e+100+2i")]
    [DataRow("+1.1e+100", "+2", "1.1e+100+2i")]
    public Task WriteComplex(string projectionR, string projectionI, string expected)
    {
        var value = new Complex(
            double.Parse(projectionR, CultureInfo.InvariantCulture),
            double.Parse(projectionI, CultureInfo.InvariantCulture));

        return AssertWriteAsync(value, expected, writer => writer.WriteComplex, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("05.04:03:02.1234567", "P5DT4H3M2.1234567S")]
    [DataRow("05.04:03:02.1200000", "P5DT4H3M2.12S")]
    [DataRow("05.04:03:02.0000000", "P5DT4H3M2S")]
    [DataRow("05.04:03:00.0000000", "P5DT4H3M")]
    [DataRow("05.04:00:02.1234567", "P5DT4H2.1234567S")]
    [DataRow("05.04:00:02.1200000", "P5DT4H2.12S")]
    [DataRow("05.04:00:02.0000000", "P5DT4H2S")]
    [DataRow("05.04:00:00.0000000", "P5DT4H")]
    [DataRow("05.00:03:02.1234567", "P5DT3M2.1234567S")]
    [DataRow("05.00:03:02.1200000", "P5DT3M2.12S")]
    [DataRow("05.00:03:02.0000000", "P5DT3M2S")]
    [DataRow("05.00:03:00.0000000", "P5DT3M")]
    [DataRow("05.00:00:02.1234567", "P5DT2.1234567S")]
    [DataRow("05.00:00:02.1200000", "P5DT2.12S")]
    [DataRow("05.00:00:02.0000000", "P5DT2S")]
    [DataRow("05.00:00:00.0000000", "P5D")]
    [DataRow("00.04:03:02.1234567", "PT4H3M2.1234567S")]
    [DataRow("00.04:03:02.1200000", "PT4H3M2.12S")]
    [DataRow("00.04:03:02.0000000", "PT4H3M2S")]
    [DataRow("00.04:03:00.0000000", "PT4H3M")]
    [DataRow("00.04:00:02.1234567", "PT4H2.1234567S")]
    [DataRow("00.04:00:02.1200000", "PT4H2.12S")]
    [DataRow("00.04:00:02.0000000", "PT4H2S")]
    [DataRow("00.04:00:00.0000000", "PT4H")]
    [DataRow("00.00:03:02.1234567", "PT3M2.1234567S")]
    [DataRow("00.00:03:02.1200000", "PT3M2.12S")]
    [DataRow("00.00:03:02.0000000", "PT3M2S")]
    [DataRow("00.00:03:00.0000000", "PT3M")]
    [DataRow("00.00:00:02.1234567", "PT2.1234567S")]
    [DataRow("00.00:00:02.1200000", "PT2.12S")]
    [DataRow("00.00:00:02.0000000", "PT2S")]
    [DataRow("00.00:00:00.0000000", "PT0H")]
    [DataRow("-05.04:03:02.1234567", "-P5DT4H3M2.1234567S")]
    [DataRow("-05.04:03:02.1200000", "-P5DT4H3M2.12S")]
    [DataRow("-05.04:03:02.0000000", "-P5DT4H3M2S")]
    [DataRow("-05.04:03:00.0000000", "-P5DT4H3M")]
    [DataRow("-05.04:00:02.1234567", "-P5DT4H2.1234567S")]
    [DataRow("-05.04:00:02.1200000", "-P5DT4H2.12S")]
    [DataRow("-05.04:00:02.0000000", "-P5DT4H2S")]
    [DataRow("-05.04:00:00.0000000", "-P5DT4H")]
    [DataRow("-05.00:03:02.1234567", "-P5DT3M2.1234567S")]
    [DataRow("-05.00:03:02.1200000", "-P5DT3M2.12S")]
    [DataRow("-05.00:03:02.0000000", "-P5DT3M2S")]
    [DataRow("-05.00:03:00.0000000", "-P5DT3M")]
    [DataRow("-05.00:00:02.1234567", "-P5DT2.1234567S")]
    [DataRow("-05.00:00:02.1200000", "-P5DT2.12S")]
    [DataRow("-05.00:00:02.0000000", "-P5DT2S")]
    [DataRow("-05.00:00:00.0000000", "-P5D")]
    [DataRow("-00.04:03:02.1234567", "-PT4H3M2.1234567S")]
    [DataRow("-00.04:03:02.1200000", "-PT4H3M2.12S")]
    [DataRow("-00.04:03:02.0000000", "-PT4H3M2S")]
    [DataRow("-00.04:03:00.0000000", "-PT4H3M")]
    [DataRow("-00.04:00:02.1234567", "-PT4H2.1234567S")]
    [DataRow("-00.04:00:02.1200000", "-PT4H2.12S")]
    [DataRow("-00.04:00:02.0000000", "-PT4H2S")]
    [DataRow("-00.04:00:00.0000000", "-PT4H")]
    [DataRow("-00.00:03:02.1234567", "-PT3M2.1234567S")]
    [DataRow("-00.00:03:02.1200000", "-PT3M2.12S")]
    [DataRow("-00.00:03:02.0000000", "-PT3M2S")]
    [DataRow("-00.00:03:00.0000000", "-PT3M")]
    [DataRow("-00.00:00:02.1234567", "-PT2.1234567S")]
    [DataRow("-00.00:00:02.1200000", "-PT2.12S")]
    [DataRow("-00.00:00:02.0000000", "-PT2S")]
    [DataRow("-00.00:00:00.0000000", "PT0H")]
    public Task WriteTimeSpan(string projection, string expected)
    {
        var value = TimeSpan.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteTimeSpan, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("00:00:00.0000000", "00:00:00")]
    [DataRow("04:05:06.0000000", "04:05:06")]
    [DataRow("04:05:06.0700000", "04:05:06.07")]
    [DataRow("04:05:06.0000007", "04:05:06.0000007")]
    public Task WriteTimeOnly(string projection, string expected)
    {
        var value = TimeOnly.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.None);

        return AssertWriteAsync(value, expected, writer => writer.WriteTimeOnly, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0001-02-03", "0001-02-03")]
    public Task WriteDateOnly(string projection, string expected)
    {
        var value = DateOnly.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.None);

        return AssertWriteAsync(value, expected, writer => writer.WriteDateOnly, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0001-02-03T04:05:06.0000000", "0001-02-03T04:05:06")]
    [DataRow("0001-02-03T04:05:06.0000000Z", "0001-02-03T04:05:06Z")]
    [DataRow("0001-02-03T04:05:06.0700000", "0001-02-03T04:05:06.07")]
    [DataRow("0001-02-03T04:05:06.0700000Z", "0001-02-03T04:05:06.07Z")]
    [DataRow("0001-02-03T04:05:06.0000007", "0001-02-03T04:05:06.0000007")]
    [DataRow("0001-02-03T04:05:06.0000007Z", "0001-02-03T04:05:06.0000007Z")]
    public Task WriteDateTime(string projection, string expected)
    {
        var value = DateTime.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

        return AssertWriteAsync(value, expected, writer => writer.WriteDateTime, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("0001-02-03T04:05:06.0000000+00:00", "0001-02-03T04:05:06+00:00")]
    [DataRow("0001-02-03T04:05:06.0000000+08:09", "0001-02-03T04:05:06+08:09")]
    [DataRow("0001-02-03T04:05:06.0700000+00:00", "0001-02-03T04:05:06.07+00:00")]
    [DataRow("0001-02-03T04:05:06.0700000+08:09", "0001-02-03T04:05:06.07+08:09")]
    [DataRow("0001-02-03T04:05:06.0000007+00:00", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow("0001-02-03T04:05:06.0000007+08:09", "0001-02-03T04:05:06.0000007+08:09")]
    public Task WriteDateTimeOffset(string projection, string expected)
    {
        var value = DateTimeOffset.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        return AssertWriteAsync(value, expected, writer => writer.WriteDateTimeOffset, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("123e4567-e89b-12d3-a456-426614174000", "123e4567-e89b-12d3-a456-426614174000")]
    public Task WriteGuid(string projection, string expected)
    {
        var value = Guid.Parse(projection, CultureInfo.InvariantCulture);

        return AssertWriteAsync(value, expected, writer => writer.WriteGuid, CancellationToken);
    }
}
