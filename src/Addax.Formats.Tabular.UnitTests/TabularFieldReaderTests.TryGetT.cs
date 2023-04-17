using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

public partial class TabularFieldReaderTests
{
    private delegate bool TryGetAsFunc<T>(out T? result);

    private static async Task AssertTryGetAsAsync<T>(string buffer, T? expected, Func<TabularFieldReader, TryGetAsFunc<T>> selector, CancellationToken cancellationToken)
    {
        var dialect = new TabularDataDialect("\u000a", '\u001a', '\u001b', '\u001c');

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(buffer));
        await using var reader = new TabularFieldReader(stream, dialect);

        var method = selector.Invoke(reader);

        await reader.MoveNextRecordAsync(cancellationToken);
        await reader.ReadFieldAsync(cancellationToken);

        Assert.IsTrue(method.Invoke(out var result));
        Assert.AreEqual(expected, result);
    }

    [DataTestMethod]
    [DataRow(" v ", " v ")]
    public Task TryGetString(string buffer, string expected)
    {
        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetString, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" false ", "False")]
    [DataRow(" FALSE ", "False")]
    [DataRow(" true ", "True")]
    [DataRow(" TRUE ", "True")]
    [DataRow(" 0 ", "False")]
    [DataRow(" 1 ", "True")]
    public Task TryGetBoolean(string buffer, string projection)
    {
        var expected = bool.Parse(projection);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetBoolean, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("a", 'a')]
    public Task TryGetChar(string buffer, char expected)
    {
        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetChar, CancellationToken);
    }

    [DataTestMethod]
    [DataRow("a", "a")]
    [DataRow("\ud83e\udd70", "\ud83e\udd70")]
    public Task TryGetRune(string buffer, string projection)
    {
        Rune.DecodeFromUtf16(projection, out var expected, out _);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetRune, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -128 ", "-128")]
    [DataRow(" +127 ", "+127")]
    [DataRow(" -128\u0025 ", "-1")]
    [DataRow(" -128\u2030 ", "0")]
    [DataRow(" +127\u0025 ", "+1")]
    [DataRow(" +127\u2030 ", "0")]
    public Task TryGetSByte(string buffer, string projection)
    {
        var expected = sbyte.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetSByte, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +255 ", "+255")]
    [DataRow(" +0\u0025 ", "0")]
    [DataRow(" +0\u2030 ", "0")]
    [DataRow(" +255\u0025 ", "+2")]
    [DataRow(" +255\u2030 ", "0")]
    public Task TryGetByte(string buffer, string projection)
    {
        var expected = byte.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetByte, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -32,768 ", "-32768")]
    [DataRow(" +32,767 ", "+32767")]
    [DataRow(" -32,768\u0025 ", "-327")]
    [DataRow(" -32,768\u2030 ", "-32")]
    [DataRow(" +32,767\u0025 ", "+327")]
    [DataRow(" +32,767\u2030 ", "+32")]
    public Task TryGetInt16(string buffer, string projection)
    {
        var expected = short.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetInt16, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +65,535 ", "+65535")]
    [DataRow(" +0\u0025 ", "0")]
    [DataRow(" +0\u2030 ", "0")]
    [DataRow(" +65,535\u0025 ", "+655")]
    [DataRow(" +65,535\u2030 ", "+65")]
    public Task TryGetUInt16(string buffer, string projection)
    {
        var expected = ushort.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetUInt16, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -2,147,483,648 ", "-2147483648")]
    [DataRow(" +2,147,483,647 ", "+2147483647")]
    [DataRow(" -2,147,483,648\u0025 ", "-21474836")]
    [DataRow(" -2,147,483,648\u2030 ", "-2147483")]
    [DataRow(" +2,147,483,647\u0025 ", "+21474836")]
    [DataRow(" +2,147,483,647\u2030 ", "+2147483")]
    public Task TryGetInt32(string buffer, string projection)
    {
        var expected = int.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetInt32, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +4,294,967,295 ", "+4294967295")]
    [DataRow(" +0\u0025 ", "0")]
    [DataRow(" +0\u2030 ", "0")]
    [DataRow(" +4,294,967,295\u0025 ", "+42949672")]
    [DataRow(" +4,294,967,295\u2030 ", "+4294967")]
    public Task TryGetUInt32(string buffer, string projection)
    {
        var expected = uint.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetUInt32, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -9,223,372,036,854,775,808 ", "-9223372036854775808")]
    [DataRow(" +9,223,372,036,854,775,807 ", "+9223372036854775807")]
    [DataRow(" -9,223,372,036,854,775,808\u0025 ", "-92233720368547758")]
    [DataRow(" -9,223,372,036,854,775,808\u2030 ", "-9223372036854775")]
    [DataRow(" +9,223,372,036,854,775,807\u0025 ", "+92233720368547758")]
    [DataRow(" +9,223,372,036,854,775,807\u2030 ", "+9223372036854775")]
    public Task TryGetInt64(string buffer, string projection)
    {
        var expected = long.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetInt64, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +18,446,744,073,709,551,615 ", "+18446744073709551615")]
    [DataRow(" +0\u0025 ", "0")]
    [DataRow(" +0\u2030 ", "0")]
    [DataRow(" +18,446,744,073,709,551,615\u0025 ", "+184467440737095516")]
    [DataRow(" +18,446,744,073,709,551,615\u2030 ", "+18446744073709551")]
    public Task TryGetUInt64(string buffer, string projection)
    {
        var expected = ulong.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetUInt64, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -170,141,183,460,469,231,731,687,303,715,884,105,728 ", "-170141183460469231731687303715884105728")]
    [DataRow(" +170,141,183,460,469,231,731,687,303,715,884,105,727 ", "+170141183460469231731687303715884105727")]
    [DataRow(" -170,141,183,460,469,231,731,687,303,715,884,105,728\u0025 ", "-1701411834604692317316873037158841057")]
    [DataRow(" -170,141,183,460,469,231,731,687,303,715,884,105,728\u2030 ", "-170141183460469231731687303715884105")]
    [DataRow(" +170,141,183,460,469,231,731,687,303,715,884,105,727\u0025 ", "+1701411834604692317316873037158841057")]
    [DataRow(" +170,141,183,460,469,231,731,687,303,715,884,105,727\u2030 ", "+170141183460469231731687303715884105")]
    public Task TryGetInt128(string buffer, string projection)
    {
        var expected = Int128.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetInt128, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +340,282,366,920,938,463,463,374,607,431,768,211,455 ", "+340282366920938463463374607431768211455")]
    [DataRow(" +0\u0025 ", "0")]
    [DataRow(" +0\u2030 ", "0")]
    [DataRow(" +340,282,366,920,938,463,463,374,607,431,768,211,455\u0025 ", "+3402823669209384634633746074317682114")]
    [DataRow(" +340,282,366,920,938,463,463,374,607,431,768,211,455\u2030 ", "+340282366920938463463374607431768211")]
    public Task TryGetUInt128(string buffer, string projection)
    {
        var expected = UInt128.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetUInt128, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -999,999,999,999,999,999,999,999,999,999,999,999,999 ", "-999999999999999999999999999999999999999")]
    [DataRow(" +999,999,999,999,999,999,999,999,999,999,999,999,999 ", "+999999999999999999999999999999999999999")]
    [DataRow(" -999,999,999,999,999,999,999,999,999,999,999,999,999\u0025 ", "-9999999999999999999999999999999999999")]
    [DataRow(" -999,999,999,999,999,999,999,999,999,999,999,999,999\u2030 ", "-999999999999999999999999999999999999")]
    [DataRow(" +999,999,999,999,999,999,999,999,999,999,999,999,999\u0025 ", "+9999999999999999999999999999999999999")]
    [DataRow(" +999,999,999,999,999,999,999,999,999,999,999,999,999\u2030 ", "+999999999999999999999999999999999999")]
    public Task TryGetBigInteger(string buffer, string projection)
    {
        var expected = BigInteger.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetBigInteger, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -65,500 ", "-65500")]
    [DataRow(" +65,500 ", "+65500")]
    [DataRow(" +INF ", "+Infinity")]
    [DataRow(" -65,500\u0025 ", "-655")]
    [DataRow(" -65,500\u2030 ", "-65.5")]
    [DataRow(" +65,500\u0025 ", "+655")]
    [DataRow(" +65,500\u2030 ", "+65.5")]
    public Task TryGetHalf(string buffer, string projection)
    {
        var expected = Half.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetHalf, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -3.402823e+38 ", "-3.402823E+38")]
    [DataRow(" +3.402823e+38 ", "+3.402823E+38")]
    [DataRow(" +INF ", "+Infinity")]
    [DataRow(" -3.402823e+38\u0025 ", "-3.402823E+36")]
    [DataRow(" -3.402823e+38\u2030 ", "-3.402823E+35")]
    [DataRow(" +3.402823e+38\u0025 ", "+3.402823E+36")]
    [DataRow(" +3.402823e+38\u2030 ", "+3.402823E+35")]
    public Task TryGetSingle(string buffer, string projection)
    {
        var expected = float.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetSingle, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -1.7976931348623157e+308 ", "-1.7976931348623157E+308")]
    [DataRow(" +1.7976931348623157e+308 ", "+1.7976931348623157E+308")]
    [DataRow(" +INF ", "+Infinity")]
    [DataRow(" -1.7976931348623157e+308\u0025 ", "-1.7976931348623157E+306")]
    [DataRow(" -1.7976931348623157e+308\u2030 ", "-1.7976931348623157E+305")]
    [DataRow(" +1.7976931348623157e+308\u0025 ", "+1.7976931348623157E+306")]
    [DataRow(" +1.7976931348623157e+308\u2030 ", "+1.7976931348623157E+305")]
    public Task TryGetDouble(string buffer, string projection)
    {
        var expected = double.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetDouble, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -79,228,162,514,264,337,593,543,950,335 ", "-79228162514264337593543950335")]
    [DataRow(" +79,228,162,514,264,337,593,543,950,335 ", "+79228162514264337593543950335")]
    [DataRow(" -79,228,162,514,264,337,593,543,950,335\u0025 ", "-792281625142643375935439503.35")]
    [DataRow(" -79,228,162,514,264,337,593,543,950,335\u2030 ", "-79228162514264337593543950.335")]
    [DataRow(" +79,228,162,514,264,337,593,543,950,335\u0025 ", "+792281625142643375935439503.35")]
    [DataRow(" +79,228,162,514,264,337,593,543,950,335\u2030 ", "+79228162514264337593543950.335")]
    public Task TryGetDecimal(string buffer, string projection)
    {
        var expected = decimal.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetDecimal, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" -1,000 ", "-1000", "0")]
    [DataRow(" +1,000 ", "+1000", "0")]
    [DataRow(" 1,000 ", "+1000", "0")]
    [DataRow(" -2,000i ", "0", "-2000")]
    [DataRow(" -2,000I ", "0", "-2000")]
    [DataRow(" +2,000i ", "0", "+2000")]
    [DataRow(" +2,000I ", "0", "+2000")]
    [DataRow(" 2,000i ", "0", "+2000")]
    [DataRow(" 2,000I ", "0", "+2000")]
    [DataRow(" -i ", "0", "-1")]
    [DataRow(" -I ", "0", "-1")]
    [DataRow(" +i ", "0", "+1")]
    [DataRow(" +I ", "0", "+1")]
    [DataRow(" i ", "0", "+1")]
    [DataRow(" I ", "0", "+1")]
    [DataRow(" -1,000-2,000i ", "-1000", "-2000")]
    [DataRow(" -1,000-2,000I ", "-1000", "-2000")]
    [DataRow(" -1,000+2,000i ", "-1000", "+2000")]
    [DataRow(" -1,000+2,000I ", "-1000", "+2000")]
    [DataRow(" +1,000-2,000i ", "+1000", "-2000")]
    [DataRow(" +1,000-2,000I ", "+1000", "-2000")]
    [DataRow(" +1,000+2,000i ", "+1000", "+2000")]
    [DataRow(" +1,000+2,000I ", "+1000", "+2000")]
    [DataRow(" -1,000-i ", "-1000", "-1")]
    [DataRow(" -1,000-I ", "-1000", "-1")]
    [DataRow(" -1,000+i ", "-1000", "+1")]
    [DataRow(" -1,000+I ", "-1000", "+1")]
    [DataRow(" +1,000-i ", "+1000", "-1")]
    [DataRow(" +1,000-I ", "+1000", "-1")]
    [DataRow(" +1,000+i ", "+1000", "+1")]
    [DataRow(" +1,000+I ", "+1000", "+1")]
    [DataRow(" -1.1e+100 ", "-1.1e+100", "0")]
    [DataRow(" -1.1e-100 ", "-1.1e-100", "0")]
    [DataRow(" +1.1e+100 ", "+1.1e+100", "0")]
    [DataRow(" +1.1e-100 ", "+1.1e-100", "0")]
    [DataRow(" 1.1e+100 ", "+1.1e+100", "0")]
    [DataRow(" 1.1e-100 ", "+1.1e-100", "0")]
    [DataRow(" -2.2e+100i ", "0", "-2.2e+100")]
    [DataRow(" -2.2e-100i ", "0", "-2.2e-100")]
    [DataRow(" +2.2e+100i ", "0", "+2.2e+100")]
    [DataRow(" +2.2e-100i ", "0", "+2.2e-100")]
    [DataRow(" 2.2e+100i ", "0", "+2.2e+100")]
    [DataRow(" 2.2e-100i ", "0", "+2.2e-100")]
    [DataRow(" -1.1e+100-2.2e+100i ", "-1.1e+100", "-2.2e+100")]
    [DataRow(" -1.1e+100-2.2e-100i ", "-1.1e+100", "-2.2e-100")]
    [DataRow(" -1.1e-100-2.2e+100i ", "-1.1e-100", "-2.2e+100")]
    [DataRow(" -1.1e-100-2.2e-100i ", "-1.1e-100", "-2.2e-100")]
    [DataRow(" -1.1e+100+2.2e+100i ", "-1.1e+100", "+2.2e+100")]
    [DataRow(" -1.1e+100+2.2e-100i ", "-1.1e+100", "+2.2e-100")]
    [DataRow(" -1.1e-100+2.2e+100i ", "-1.1e-100", "+2.2e+100")]
    [DataRow(" -1.1e-100+2.2e-100i ", "-1.1e-100", "+2.2e-100")]
    [DataRow(" +1.1e+100-2.2e+100i ", "+1.1e+100", "-2.2e+100")]
    [DataRow(" +1.1e+100-2.2e-100i ", "+1.1e+100", "-2.2e-100")]
    [DataRow(" +1.1e-100-2.2e+100i ", "+1.1e-100", "-2.2e+100")]
    [DataRow(" +1.1e-100-2.2e-100i ", "+1.1e-100", "-2.2e-100")]
    [DataRow(" +1.1e+100+2.2e+100i ", "+1.1e+100", "+2.2e+100")]
    [DataRow(" +1.1e+100+2.2e-100i ", "+1.1e+100", "+2.2e-100")]
    [DataRow(" +1.1e-100+2.2e+100i ", "+1.1e-100", "+2.2e+100")]
    [DataRow(" +1.1e-100+2.2e-100i ", "+1.1e-100", "+2.2e-100")]
    [DataRow(" 1.1e+100-2.2e+100i ", "+1.1e+100", "-2.2e+100")]
    [DataRow(" 1.1e+100-2.2e-100i ", "+1.1e+100", "-2.2e-100")]
    [DataRow(" 1.1e-100-2.2e+100i ", "+1.1e-100", "-2.2e+100")]
    [DataRow(" 1.1e-100-2.2e-100i ", "+1.1e-100", "-2.2e-100")]
    [DataRow(" 1.1e+100+2.2e+100i ", "+1.1e+100", "+2.2e+100")]
    [DataRow(" 1.1e+100+2.2e-100i ", "+1.1e+100", "+2.2e-100")]
    [DataRow(" 1.1e-100+2.2e+100i ", "+1.1e-100", "+2.2e+100")]
    [DataRow(" 1.1e-100+2.2e-100i ", "+1.1e-100", "+2.2e-100")]
    [DataRow(" -1-2.2e+100i ", "-1", "-2.2e+100")]
    [DataRow(" -1-2.2e-100i ", "-1", "-2.2e-100")]
    [DataRow(" -1+2.2e+100i ", "-1", "+2.2e+100")]
    [DataRow(" -1+2.2e-100i ", "-1", "+2.2e-100")]
    [DataRow(" +1-2.2e+100i ", "+1", "-2.2e+100")]
    [DataRow(" +1-2.2e-100i ", "+1", "-2.2e-100")]
    [DataRow(" +1+2.2e+100i ", "+1", "+2.2e+100")]
    [DataRow(" +1+2.2e-100i ", "+1", "+2.2e-100")]
    [DataRow(" 1-2.2e+100i ", "+1", "-2.2e+100")]
    [DataRow(" 1-2.2e-100i ", "+1", "-2.2e-100")]
    [DataRow(" 1+2.2e+100i ", "+1", "+2.2e+100")]
    [DataRow(" 1+2.2e-100i ", "+1", "+2.2e-100")]
    [DataRow(" -1.1e+100-2i ", "-1.1e+100", "-2")]
    [DataRow(" -1.1e+100-2i ", "-1.1e+100", "-2")]
    [DataRow(" -1.1e+100+2i ", "-1.1e+100", "+2")]
    [DataRow(" -1.1e+100+2i ", "-1.1e+100", "+2")]
    [DataRow(" +1.1e+100-2i ", "+1.1e+100", "-2")]
    [DataRow(" +1.1e+100-2i ", "+1.1e+100", "-2")]
    [DataRow(" +1.1e+100+2i ", "+1.1e+100", "+2")]
    [DataRow(" +1.1e+100+2i ", "+1.1e+100", "+2")]
    [DataRow(" 1.1e+100-2i ", "+1.1e+100", "-2")]
    [DataRow(" 1.1e+100-2i ", "+1.1e+100", "-2")]
    [DataRow(" 1.1e+100+2i ", "+1.1e+100", "+2")]
    [DataRow(" 1.1e+100+2i ", "+1.1e+100", "+2")]
    public Task TryGetComplex(string buffer, string expectedR, string expectedI)
    {
        var expected = new Complex(
            double.Parse(expectedR, CultureInfo.InvariantCulture),
            double.Parse(expectedI, CultureInfo.InvariantCulture));

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetComplex, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" P5DT4H3M2.1234567S ", "05.04:03:02.1234567")]
    [DataRow(" P5DT4H3M2.12S ", "05.04:03:02.1200000")]
    [DataRow(" P5DT4H3M2S ", "05.04:03:02.0000000")]
    [DataRow(" P5DT4H3M ", "05.04:03:00.0000000")]
    [DataRow(" P5DT4H2.1234567S ", "05.04:00:02.1234567")]
    [DataRow(" P5DT4H2.12S ", "05.04:00:02.1200000")]
    [DataRow(" P5DT4H2S ", "05.04:00:02.0000000")]
    [DataRow(" P5DT4H ", "05.04:00:00.0000000")]
    [DataRow(" P5DT3M2.1234567S ", "05.00:03:02.1234567")]
    [DataRow(" P5DT3M2.12S ", "05.00:03:02.1200000")]
    [DataRow(" P5DT3M2S ", "05.00:03:02.0000000")]
    [DataRow(" P5DT3M ", "05.00:03:00.0000000")]
    [DataRow(" P5DT2.1234567S ", "05.00:00:02.1234567")]
    [DataRow(" P5DT2.12S ", "05.00:00:02.1200000")]
    [DataRow(" P5DT2S ", "05.00:00:02.0000000")]
    [DataRow(" P5D ", "05.00:00:00.0000000")]
    [DataRow(" PT4H3M2.1234567S ", "00.04:03:02.1234567")]
    [DataRow(" PT4H3M2.12S ", "00.04:03:02.1200000")]
    [DataRow(" PT4H3M2S ", "00.04:03:02.0000000")]
    [DataRow(" PT4H3M ", "00.04:03:00.0000000")]
    [DataRow(" PT4H2.1234567S ", "00.04:00:02.1234567")]
    [DataRow(" PT4H2.12S ", "00.04:00:02.1200000")]
    [DataRow(" PT4H2S ", "00.04:00:02.0000000")]
    [DataRow(" PT4H ", "00.04:00:00.0000000")]
    [DataRow(" PT3M2.1234567S ", "00.00:03:02.1234567")]
    [DataRow(" PT3M2.12S ", "00.00:03:02.1200000")]
    [DataRow(" PT3M2S ", "00.00:03:02.0000000")]
    [DataRow(" PT3M ", "00.00:03:00.0000000")]
    [DataRow(" PT2.1234567S ", "00.00:00:02.1234567")]
    [DataRow(" PT2.12S ", "00.00:00:02.1200000")]
    [DataRow(" PT2S ", "00.00:00:02.0000000")]
    [DataRow(" P0DT0H0M0S ", "00.00:00:00.0000000")]
    [DataRow(" P0DT0H0M0.0000000S ", "00.00:00:00.0000000")]
    [DataRow(" P0D ", "00.00:00:00.0000000")]
    [DataRow(" PT0H ", "00.00:00:00.0000000")]
    [DataRow(" PT0M ", "00.00:00:00.0000000")]
    [DataRow(" PT0S ", "00.00:00:00.0000000")]
    [DataRow(" PT0.0000000S ", "00.00:00:00.0000000")]
    [DataRow(" -P5DT4H3M2.1234567S ", "-05.04:03:02.1234567")]
    [DataRow(" -P5DT4H3M2.12S ", "-05.04:03:02.1200000")]
    [DataRow(" -P5DT4H3M2S ", "-05.04:03:02.0000000")]
    [DataRow(" -P5DT4H3M ", "-05.04:03:00.0000000")]
    [DataRow(" -P5DT4H2.1234567S ", "-05.04:00:02.1234567")]
    [DataRow(" -P5DT4H2.12S ", "-05.04:00:02.1200000")]
    [DataRow(" -P5DT4H2S ", "-05.04:00:02.0000000")]
    [DataRow(" -P5DT4H ", "-05.04:00:00.0000000")]
    [DataRow(" -P5DT3M2.1234567S ", "-05.00:03:02.1234567")]
    [DataRow(" -P5DT3M2.12S ", "-05.00:03:02.1200000")]
    [DataRow(" -P5DT3M2S ", "-05.00:03:02.0000000")]
    [DataRow(" -P5DT3M ", "-05.00:03:00.0000000")]
    [DataRow(" -P5DT2.1234567S ", "-05.00:00:02.1234567")]
    [DataRow(" -P5DT2.12S ", "-05.00:00:02.1200000")]
    [DataRow(" -P5DT2S ", "-05.00:00:02.0000000")]
    [DataRow(" -P5D ", "-05.00:00:00.0000000")]
    [DataRow(" -PT4H3M2.1234567S ", "-00.04:03:02.1234567")]
    [DataRow(" -PT4H3M2.12S ", "-00.04:03:02.1200000")]
    [DataRow(" -PT4H3M2S ", "-00.04:03:02.0000000")]
    [DataRow(" -PT4H3M ", "-00.04:03:00.0000000")]
    [DataRow(" -PT4H2.1234567S ", "-00.04:00:02.1234567")]
    [DataRow(" -PT4H2.12S ", "-00.04:00:02.1200000")]
    [DataRow(" -PT4H2S ", "-00.04:00:02.0000000")]
    [DataRow(" -PT4H ", "-00.04:00:00.0000000")]
    [DataRow(" -PT3M2.1234567S ", "-00.00:03:02.1234567")]
    [DataRow(" -PT3M2.12S ", "-00.00:03:02.1200000")]
    [DataRow(" -PT3M2S ", "-00.00:03:02.0000000")]
    [DataRow(" -PT3M ", "-00.00:03:00.0000000")]
    [DataRow(" -PT2.1234567S ", "-00.00:00:02.1234567")]
    [DataRow(" -PT2.12S ", "-00.00:00:02.1200000")]
    [DataRow(" -PT2S ", "-00.00:00:02.0000000")]
    [DataRow(" -P0D ", "00.00:00:00.0000000")]
    [DataRow(" -PT0H ", "00.00:00:00.0000000")]
    [DataRow(" -PT0M ", "00.00:00:00.0000000")]
    [DataRow(" -PT0S ", "00.00:00:00.0000000")]
    [DataRow(" -P0DT0H0M0S ", "00.00:00:00.0000000")]
    [DataRow(" -P0DT0H0M0.0000000S ", "00.00:00:00.0000000")]
    [DataRow(" -P0D ", "00.00:00:00.0000000")]
    [DataRow(" -PT0H ", "00.00:00:00.0000000")]
    [DataRow(" -PT0M ", "00.00:00:00.0000000")]
    [DataRow(" -PT0S ", "00.00:00:00.0000000")]
    [DataRow(" -PT0.0000000S ", "00.00:00:00.0000000")]
    public Task TryGetTimeSpan(string buffer, string projection)
    {
        var expected = TimeSpan.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetTimeSpan, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" 00:00:00 ", "00:00:00.0000000")]
    [DataRow(" 04:05:06 ", "04:05:06.0000000")]
    [DataRow(" 04:05:06.07 ", "04:05:06.0700000")]
    [DataRow(" 04:05:06.0000007 ", "04:05:06.0000007")]
    [DataRow(" 0001-02-03T00:00:00 ", "00:00:00.0000000")]
    [DataRow(" 0001-02-03T04:05:06 ", "04:05:06.0000000")]
    [DataRow(" 0001-02-03T04:05:06Z ", "04:05:06.0000000")]
    [DataRow(" 0001-02-03T04:05:06+08:09 ", "04:05:06.0000000")]
    [DataRow(" 0001-02-03T04:05:06.07 ", "04:05:06.0700000")]
    [DataRow(" 0001-02-03T04:05:06.07Z ", "04:05:06.0700000")]
    [DataRow(" 0001-02-03T04:05:06.07+08:09 ", "04:05:06.0700000")]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "04:05:06.0000007")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "04:05:06.0000007")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "04:05:06.0000007")]
    public Task TryGetTimeOnly(string buffer, string projection)
    {
        var expected = TimeOnly.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.None);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetTimeOnly, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03 ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06 ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06Z ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06+08:09 ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06.07 ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06.07Z ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06.07+08:09 ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "0001-02-03")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "0001-02-03")]
    public Task TryGetDateOnly(string buffer, string projection)
    {
        var expected = DateOnly.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.None);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetDateOnly, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03T04:05:06 ", "0001-02-03T04:05:06.0000000")]
    [DataRow(" 0001-02-03T04:05:06Z ", "0001-02-03T04:05:06.0000000Z")]
    [DataRow(" 0001-02-03T04:05:06+08:09 ", "0001-02-02T19:56:06.0000000Z")]
    [DataRow(" 0001-02-03T04:05:06.07 ", "0001-02-03T04:05:06.0700000")]
    [DataRow(" 0001-02-03T04:05:06.07Z ", "0001-02-03T04:05:06.0700000Z")]
    [DataRow(" 0001-02-03T04:05:06.07+08:09 ", "0001-02-02T19:56:06.0700000Z")]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "0001-02-03T04:05:06.0000007")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "0001-02-03T04:05:06.0000007Z")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "0001-02-02T19:56:06.0000007Z")]
    public Task TryGetDateTime(string buffer, string projection)
    {
        var expected = DateTime.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetDateTime, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03T04:05:06 ", "0001-02-03T04:05:06.0000000+00:00")]
    [DataRow(" 0001-02-03T04:05:06Z ", "0001-02-03T04:05:06.0000000+00:00")]
    [DataRow(" 0001-02-03T04:05:06+08:09 ", "0001-02-03T04:05:06.0000000+08:09")]
    [DataRow(" 0001-02-03T04:05:06.07 ", "0001-02-03T04:05:06.0700000+00:00")]
    [DataRow(" 0001-02-03T04:05:06.07Z ", "0001-02-03T04:05:06.0700000+00:00")]
    [DataRow(" 0001-02-03T04:05:06.07+08:09 ", "0001-02-03T04:05:06.0700000+08:09")]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "0001-02-03T04:05:06.0000007+08:09")]
    public Task TryGetDateTimeOffset(string buffer, string projection)
    {
        var expected = DateTimeOffset.Parse(projection, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetDateTimeOffset, CancellationToken);
    }

    [DataTestMethod]
    [DataRow(" 123e4567-e89b-12d3-a456-426614174000 ", "123e4567-e89b-12d3-a456-426614174000")]
    public Task TryGetGuid(string buffer, string projection)
    {
        var expected = Guid.Parse(projection, CultureInfo.InvariantCulture);

        return AssertTryGetAsAsync(buffer, expected, x => x.TryGetGuid, CancellationToken);
    }
}
