using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

public partial class TabularReaderTests
{
    private delegate bool TryGetFunc<T>(out T? result);

    private static void TryGet<T>(Func<TabularReader, TryGetFunc<T>> selector, string content, T? expected)
    {
        var dialect = new TabularDialect("\u000a", '\u000b', '\u000c', '\u000d');
        var options = new TabularOptions { Encoding = Encoding.ASCII };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var method = selector.Invoke(reader);

        reader.TryReadField();

        Assert.IsTrue(method.Invoke(out var result));

        if (typeof(T).IsAssignableTo(typeof(ICollection)))
        {
            CollectionAssert.AreEqual(expected as ICollection, result as ICollection);
        }
        else
        {
            Assert.AreEqual(expected, result);
        }
    }

    private static void Get<T>(Func<TabularReader, Func<T?>> selector, string content, T? expected)
    {
        var dialect = new TabularDialect("\u000a", '\u000b', '\u000c', '\u000d');
        var options = new TabularOptions { Encoding = Encoding.ASCII };

        using var stream = new MemoryStream(Encoding.ASCII.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var method = selector.Invoke(reader);

        reader.TryReadField();

        var result = method.Invoke();

        if (typeof(T).IsAssignableTo(typeof(ICollection)))
        {
            CollectionAssert.AreEqual(expected as ICollection, result as ICollection);
        }
        else
        {
            Assert.AreEqual(expected, result);
        }
    }

    [DataTestMethod]
    [DataRow("", "")]
    [DataRow(" v ", " v ")]
    public void TryGetString(string content, string expected)
    {
        TryGet(x => x.TryGetString, content, expected);
    }

    [DataTestMethod]
    [DataRow(" false ", "False")]
    [DataRow(" true ", "True")]
    [DataRow(" 0 ", "False")]
    [DataRow(" 1 ", "True")]
    public void TryGetBoolean(string content, string expected)
    {
        TryGet(x => x.TryGetBoolean, content, bool.Parse(expected));
    }

    [DataTestMethod]
    [DataRow("v", "v")]
    public void TryGetChar(string content, string expected)
    {
        TryGet(x => x.TryGetChar, content, char.Parse(expected));
    }

    [DataTestMethod]
    [DataRow(" -128 ", "-128")]
    [DataRow(" +127 ", "+127")]
    public void TryGetSByte(string content, string expected)
    {
        TryGet(x => x.TryGetSByte, content, sbyte.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +255 ", "+255")]
    public void TryGetByte(string content, string expected)
    {
        TryGet(x => x.TryGetByte, content, byte.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -32,768 ", "-32768")]
    [DataRow(" +32,767 ", "+32767")]
    public void TryGetInt16(string content, string expected)
    {
        TryGet(x => x.TryGetInt16, content, short.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +65,535 ", "+65535")]
    public void TryGetUInt16(string content, string expected)
    {
        TryGet(x => x.TryGetUInt16, content, ushort.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -2,147,483,648 ", "-2147483648")]
    [DataRow(" +2,147,483,647 ", "+2147483647")]
    public void TryGetInt32(string content, string expected)
    {
        TryGet(x => x.TryGetInt32, content, int.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +4,294,967,295 ", "+4294967295")]
    public void TryGetUInt32(string content, string expected)
    {
        TryGet(x => x.TryGetUInt32, content, uint.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -9,223,372,036,854,775,808 ", "-9223372036854775808")]
    [DataRow(" +9,223,372,036,854,775,807 ", "+9223372036854775807")]
    public void TryGetInt64(string content, string expected)
    {
        TryGet(x => x.TryGetInt64, content, long.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +18,446,744,073,709,551,615 ", "+18446744073709551615")]
    public void TryGetUInt64(string content, string expected)
    {
        TryGet(x => x.TryGetUInt64, content, ulong.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -170,141,183,460,469,231,731,687,303,715,884,105,728 ", "-170141183460469231731687303715884105728")]
    [DataRow(" +170,141,183,460,469,231,731,687,303,715,884,105,727 ", "+170141183460469231731687303715884105727")]
    public void TryGetInt128(string content, string expected)
    {
        TryGet(x => x.TryGetInt128, content, Int128.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +340,282,366,920,938,463,463,374,607,431,768,211,455 ", "+340282366920938463463374607431768211455")]
    public void TryGetUInt128(string content, string expected)
    {
        TryGet(x => x.TryGetUInt128, content, UInt128.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -999,999,999,999,999,999,999,999,999,999,999,999,999 ", "-999999999999999999999999999999999999999")]
    [DataRow(" +999,999,999,999,999,999,999,999,999,999,999,999,999 ", "+999999999999999999999999999999999999999")]
    public void TryGetBigInteger(string content, string expected)
    {
        TryGet(x => x.TryGetBigInteger, content, BigInteger.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -65,500 ", "-65500")]
    [DataRow(" +65,500 ", "+65500")]
    [DataRow(" +INF ", "+Infinity")]
    public void TryGetHalf(string content, string expected)
    {
        TryGet(x => x.TryGetHalf, content, Half.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -3.402823e+38 ", "-3.402823E+38")]
    [DataRow(" +3.402823e+38 ", "+3.402823E+38")]
    [DataRow(" +INF ", "+Infinity")]
    public void TryGetSingle(string content, string expected)
    {
        TryGet(x => x.TryGetSingle, content, float.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -1.7976931348623157e+308 ", "-1.7976931348623157E+308")]
    [DataRow(" +1.7976931348623157e+308 ", "+1.7976931348623157E+308")]
    [DataRow(" +INF ", "+Infinity")]
    public void TryGetDouble(string content, string expected)
    {
        TryGet(x => x.TryGetDouble, content, double.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -79,228,162,514,264,337,593,543,950,335 ", "-79228162514264337593543950335")]
    [DataRow(" +79,228,162,514,264,337,593,543,950,335 ", "+79228162514264337593543950335")]
    public void TryGetDecimal(string content, string expected)
    {
        TryGet(x => x.TryGetDecimal, content, decimal.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" PT2S ", "00.00:00:02.0000000")]
    [DataRow(" PT3M2S ", "00.00:03:02.0000000")]
    [DataRow(" PT4H3M2S ", "00.04:03:02.0000000")]
    [DataRow(" P5DT4H3M2S ", "05.04:03:02.0000000")]
    [DataRow(" PT2.1234567S ", "00.00:00:02.1234567")]
    [DataRow(" PT3M2.1234567S ", "00.00:03:02.1234567")]
    [DataRow(" PT4H3M2.1234567S ", "00.04:03:02.1234567")]
    [DataRow(" P5DT4H3M2.1234567S ", "05.04:03:02.1234567")]
    [DataRow(" PT3M ", "00.00:03:00.0000000")]
    [DataRow(" PT4H ", "00.04:00:00.0000000")]
    [DataRow(" PT4H3M ", "00.04:03:00.0000000")]
    [DataRow(" PT4H2S ", "00.04:00:02.0000000")]
    [DataRow(" P5D ", "05.00:00:00.0000000")]
    [DataRow(" P5DT4H ", "05.04:00:00.0000000")]
    [DataRow(" P5DT4H3M ", "05.04:03:00.0000000")]
    [DataRow(" P5DT4H2S ", "05.04:00:02.0000000")]
    [DataRow(" P5DT3M ", "05.00:03:00.0000000")]
    [DataRow(" P5DT3M2S ", "05.00:03:02.0000000")]
    [DataRow(" P5DT2S ", "05.00:00:02.0000000")]
    [DataRow(" PT4H2.1234567S ", "00.04:00:02.1234567")]
    [DataRow(" P5DT4H2.1234567S ", "05.04:00:02.1234567")]
    [DataRow(" P5DT3M2.1234567S ", "05.00:03:02.1234567")]
    [DataRow(" P5DT2.1234567S ", "05.00:00:02.1234567")]
    [DataRow(" -PT2S ", "-00.00:00:02.0000000")]
    [DataRow(" -PT3M2S ", "-00.00:03:02.0000000")]
    [DataRow(" -PT4H3M2S ", "-00.04:03:02.0000000")]
    [DataRow(" -P5DT4H3M2S ", "-05.04:03:02.0000000")]
    [DataRow(" -PT2.1234567S ", "-00.00:00:02.1234567")]
    [DataRow(" -PT3M2.1234567S ", "-00.00:03:02.1234567")]
    [DataRow(" -PT4H3M2.1234567S ", "-00.04:03:02.1234567")]
    [DataRow(" -P5DT4H3M2.1234567S ", "-05.04:03:02.1234567")]
    [DataRow(" -PT3M ", "-00.00:03:00.0000000")]
    [DataRow(" -PT4H ", "-00.04:00:00.0000000")]
    [DataRow(" -PT4H3M ", "-00.04:03:00.0000000")]
    [DataRow(" -PT4H2S ", "-00.04:00:02.0000000")]
    [DataRow(" -P5D ", "-05.00:00:00.0000000")]
    [DataRow(" -P5DT4H ", "-05.04:00:00.0000000")]
    [DataRow(" -P5DT4H3M ", "-05.04:03:00.0000000")]
    [DataRow(" -P5DT4H2S ", "-05.04:00:02.0000000")]
    [DataRow(" -P5DT3M ", "-05.00:03:00.0000000")]
    [DataRow(" -P5DT3M2S ", "-05.00:03:02.0000000")]
    [DataRow(" -P5DT2S ", "-05.00:00:02.0000000")]
    [DataRow(" -PT4H2.1234567S ", "-00.04:00:02.1234567")]
    [DataRow(" -P5DT4H2.1234567S ", "-05.04:00:02.1234567")]
    [DataRow(" -P5DT3M2.1234567S ", "-05.00:03:02.1234567")]
    [DataRow(" -P5DT2.1234567S ", "-05.00:00:02.1234567")]
    public void TryGetTimeSpan(string content, string expected)
    {
        TryGet(x => x.TryGetTimeSpan, content, TimeSpan.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" 04:05:06.0000007 ", "04:05:06.0000007")]
    public void TryGetTimeOnly(string content, string expected)
    {
        TryGet(x => x.TryGetTimeOnly, content, TimeOnly.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03 ", "0001-02-03")]
    public void TryGetDateOnly(string content, string expected)
    {
        TryGet(x => x.TryGetDateOnly, content, DateOnly.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "0001-02-03T04:05:06.0000007")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "0001-02-03T04:05:06.0000007Z")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "0001-02-02T19:56:06.0000007Z")]
    public void TryGetDateTime(string content, string expected)
    {
        TryGet(x => x.TryGetDateTime, content, DateTime.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "0001-02-03T04:05:06.0000007+08:09")]
    public void TryGetDateTimeOffset(string content, string expected)
    {
        TryGet(x => x.TryGetDateTimeOffset, content, DateTimeOffset.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
    }

    [DataTestMethod]
    [DataRow(" 123e4567-e89b-12d3-a456-426614174000 ", "123e4567-e89b-12d3-a456-426614174000")]
    [DataRow(" 123E4567-E89B-12D3-A456-426614174000 ", "123e4567-e89b-12d3-a456-426614174000")]
    public void TryGetGuid(string content, string expected)
    {
        TryGet(x => x.TryGetGuid, content, Guid.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" urn:com.example ", "urn:com.example")]
    [DataRow(" https://example.com/ ", "https://example.com/")]
    public void TryGetUri(string content, string expected)
    {
        TryGet(x => x.TryGetUri, content, new Uri(expected, UriKind.RelativeOrAbsolute));
    }

    [DataTestMethod]
    [DataRow(" ", "")]
    [DataRow(" 123e4567e89b12d3a456426614174000 ", "123e4567e89b12d3a456426614174000")]
    [DataRow(" 123E4567E89B12D3A456426614174000 ", "123e4567e89b12d3a456426614174000")]
    public void TryGetBase16Binary(string content, string expected)
    {
        TryGet(x => x.TryGetBase16Binary, content, Convert.FromHexString(expected));
    }

    [DataTestMethod]
    [DataRow(" ", "")]
    [DataRow(" MTk4NC0wNS0wNA== ", "MTk4NC0wNS0wNA==")]
    public void TryGetBase64Binary(string content, string expected)
    {
        TryGet(x => x.TryGetBase64Binary, content, Convert.FromBase64String(expected));
    }

    [DataTestMethod]
    [DataRow("", "")]
    [DataRow(" v ", " v ")]
    public void GetString(string content, string expected)
    {
        Get(x => x.GetString, content, expected);
    }

    [DataTestMethod]
    [DataRow(" false ", "False")]
    [DataRow(" true ", "True")]
    [DataRow(" 0 ", "False")]
    [DataRow(" 1 ", "True")]
    public void GetBoolean(string content, string expected)
    {
        Get(x => x.GetBoolean, content, bool.Parse(expected));
    }

    [DataTestMethod]
    [DataRow("v", "v")]
    public void GetChar(string content, string expected)
    {
        Get(x => x.GetChar, content, char.Parse(expected));
    }

    [DataTestMethod]
    [DataRow(" -128 ", "-128")]
    [DataRow(" +127 ", "+127")]
    public void GetSByte(string content, string expected)
    {
        Get(x => x.GetSByte, content, sbyte.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +255 ", "+255")]
    public void GetByte(string content, string expected)
    {
        Get(x => x.GetByte, content, byte.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -32,768 ", "-32768")]
    [DataRow(" +32,767 ", "+32767")]
    public void GetInt16(string content, string expected)
    {
        Get(x => x.GetInt16, content, short.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +65,535 ", "+65535")]
    public void GetUInt16(string content, string expected)
    {
        Get(x => x.GetUInt16, content, ushort.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -2,147,483,648 ", "-2147483648")]
    [DataRow(" +2,147,483,647 ", "+2147483647")]
    public void GetInt32(string content, string expected)
    {
        Get(x => x.GetInt32, content, int.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +4,294,967,295 ", "+4294967295")]
    public void GetUInt32(string content, string expected)
    {
        Get(x => x.GetUInt32, content, uint.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -9,223,372,036,854,775,808 ", "-9223372036854775808")]
    [DataRow(" +9,223,372,036,854,775,807 ", "+9223372036854775807")]
    public void GetInt64(string content, string expected)
    {
        Get(x => x.GetInt64, content, long.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +18,446,744,073,709,551,615 ", "+18446744073709551615")]
    public void GetUInt64(string content, string expected)
    {
        Get(x => x.GetUInt64, content, ulong.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -170,141,183,460,469,231,731,687,303,715,884,105,728 ", "-170141183460469231731687303715884105728")]
    [DataRow(" +170,141,183,460,469,231,731,687,303,715,884,105,727 ", "+170141183460469231731687303715884105727")]
    public void GetInt128(string content, string expected)
    {
        Get(x => x.GetInt128, content, Int128.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" +0 ", "0")]
    [DataRow(" +340,282,366,920,938,463,463,374,607,431,768,211,455 ", "+340282366920938463463374607431768211455")]
    public void GetUInt128(string content, string expected)
    {
        Get(x => x.GetUInt128, content, UInt128.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -999,999,999,999,999,999,999,999,999,999,999,999,999 ", "-999999999999999999999999999999999999999")]
    [DataRow(" +999,999,999,999,999,999,999,999,999,999,999,999,999 ", "+999999999999999999999999999999999999999")]
    public void GetBigInteger(string content, string expected)
    {
        Get(x => x.GetBigInteger, content, BigInteger.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -65,500 ", "-65500")]
    [DataRow(" +65,500 ", "+65500")]
    [DataRow(" +INF ", "+Infinity")]
    public void GetHalf(string content, string expected)
    {
        Get(x => x.GetHalf, content, Half.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -3.402823e+38 ", "-3.402823E+38")]
    [DataRow(" +3.402823e+38 ", "+3.402823E+38")]
    [DataRow(" +INF ", "+Infinity")]
    public void GetSingle(string content, string expected)
    {
        Get(x => x.GetSingle, content, float.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" NaN ", "NaN")]
    [DataRow(" -INF ", "-Infinity")]
    [DataRow(" -1.7976931348623157e+308 ", "-1.7976931348623157E+308")]
    [DataRow(" +1.7976931348623157e+308 ", "+1.7976931348623157E+308")]
    [DataRow(" +INF ", "+Infinity")]
    public void GetDouble(string content, string expected)
    {
        Get(x => x.GetDouble, content, double.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" -79,228,162,514,264,337,593,543,950,335 ", "-79228162514264337593543950335")]
    [DataRow(" +79,228,162,514,264,337,593,543,950,335 ", "+79228162514264337593543950335")]
    public void GetDecimal(string content, string expected)
    {
        Get(x => x.GetDecimal, content, decimal.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" PT2S ", "00.00:00:02.0000000")]
    [DataRow(" PT3M2S ", "00.00:03:02.0000000")]
    [DataRow(" PT4H3M2S ", "00.04:03:02.0000000")]
    [DataRow(" P5DT4H3M2S ", "05.04:03:02.0000000")]
    [DataRow(" PT2.1234567S ", "00.00:00:02.1234567")]
    [DataRow(" PT3M2.1234567S ", "00.00:03:02.1234567")]
    [DataRow(" PT4H3M2.1234567S ", "00.04:03:02.1234567")]
    [DataRow(" P5DT4H3M2.1234567S ", "05.04:03:02.1234567")]
    [DataRow(" PT3M ", "00.00:03:00.0000000")]
    [DataRow(" PT4H ", "00.04:00:00.0000000")]
    [DataRow(" PT4H3M ", "00.04:03:00.0000000")]
    [DataRow(" PT4H2S ", "00.04:00:02.0000000")]
    [DataRow(" P5D ", "05.00:00:00.0000000")]
    [DataRow(" P5DT4H ", "05.04:00:00.0000000")]
    [DataRow(" P5DT4H3M ", "05.04:03:00.0000000")]
    [DataRow(" P5DT4H2S ", "05.04:00:02.0000000")]
    [DataRow(" P5DT3M ", "05.00:03:00.0000000")]
    [DataRow(" P5DT3M2S ", "05.00:03:02.0000000")]
    [DataRow(" P5DT2S ", "05.00:00:02.0000000")]
    [DataRow(" PT4H2.1234567S ", "00.04:00:02.1234567")]
    [DataRow(" P5DT4H2.1234567S ", "05.04:00:02.1234567")]
    [DataRow(" P5DT3M2.1234567S ", "05.00:03:02.1234567")]
    [DataRow(" P5DT2.1234567S ", "05.00:00:02.1234567")]
    [DataRow(" -PT2S ", "-00.00:00:02.0000000")]
    [DataRow(" -PT3M2S ", "-00.00:03:02.0000000")]
    [DataRow(" -PT4H3M2S ", "-00.04:03:02.0000000")]
    [DataRow(" -P5DT4H3M2S ", "-05.04:03:02.0000000")]
    [DataRow(" -PT2.1234567S ", "-00.00:00:02.1234567")]
    [DataRow(" -PT3M2.1234567S ", "-00.00:03:02.1234567")]
    [DataRow(" -PT4H3M2.1234567S ", "-00.04:03:02.1234567")]
    [DataRow(" -P5DT4H3M2.1234567S ", "-05.04:03:02.1234567")]
    [DataRow(" -PT3M ", "-00.00:03:00.0000000")]
    [DataRow(" -PT4H ", "-00.04:00:00.0000000")]
    [DataRow(" -PT4H3M ", "-00.04:03:00.0000000")]
    [DataRow(" -PT4H2S ", "-00.04:00:02.0000000")]
    [DataRow(" -P5D ", "-05.00:00:00.0000000")]
    [DataRow(" -P5DT4H ", "-05.04:00:00.0000000")]
    [DataRow(" -P5DT4H3M ", "-05.04:03:00.0000000")]
    [DataRow(" -P5DT4H2S ", "-05.04:00:02.0000000")]
    [DataRow(" -P5DT3M ", "-05.00:03:00.0000000")]
    [DataRow(" -P5DT3M2S ", "-05.00:03:02.0000000")]
    [DataRow(" -P5DT2S ", "-05.00:00:02.0000000")]
    [DataRow(" -PT4H2.1234567S ", "-00.04:00:02.1234567")]
    [DataRow(" -P5DT4H2.1234567S ", "-05.04:00:02.1234567")]
    [DataRow(" -P5DT3M2.1234567S ", "-05.00:03:02.1234567")]
    [DataRow(" -P5DT2.1234567S ", "-05.00:00:02.1234567")]
    public void GetTimeSpan(string content, string expected)
    {
        Get(x => x.GetTimeSpan, content, TimeSpan.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" 04:05:06.0000007 ", "04:05:06.0000007")]
    public void GetTimeOnly(string content, string expected)
    {
        Get(x => x.GetTimeOnly, content, TimeOnly.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03 ", "0001-02-03")]
    public void GetDateOnly(string content, string expected)
    {
        Get(x => x.GetDateOnly, content, DateOnly.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.None));
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "0001-02-03T04:05:06.0000007")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "0001-02-03T04:05:06.0000007Z")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "0001-02-02T19:56:06.0000007Z")]
    public void GetDateTime(string content, string expected)
    {
        Get(x => x.GetDateTime, content, DateTime.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal));
    }

    [DataTestMethod]
    [DataRow(" 0001-02-03T04:05:06.0000007 ", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow(" 0001-02-03T04:05:06.0000007Z ", "0001-02-03T04:05:06.0000007+00:00")]
    [DataRow(" 0001-02-03T04:05:06.0000007+08:09 ", "0001-02-03T04:05:06.0000007+08:09")]
    public void GetDateTimeOffset(string content, string expected)
    {
        Get(x => x.GetDateTimeOffset, content, DateTimeOffset.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal));
    }

    [DataTestMethod]
    [DataRow(" 123e4567-e89b-12d3-a456-426614174000 ", "123e4567-e89b-12d3-a456-426614174000")]
    [DataRow(" 123E4567-E89B-12D3-A456-426614174000 ", "123e4567-e89b-12d3-a456-426614174000")]
    public void GetGuid(string content, string expected)
    {
        Get(x => x.GetGuid, content, Guid.Parse(expected, CultureInfo.InvariantCulture));
    }

    [DataTestMethod]
    [DataRow(" urn:com.example ", "urn:com.example")]
    [DataRow(" https://example.com/ ", "https://example.com/")]
    public void GetUri(string content, string expected)
    {
        Get(x => x.GetUri, content, new Uri(expected, UriKind.RelativeOrAbsolute));
    }

    [DataTestMethod]
    [DataRow(" ", "")]
    [DataRow(" 123e4567e89b12d3a456426614174000 ", "123e4567e89b12d3a456426614174000")]
    [DataRow(" 123E4567E89B12D3A456426614174000 ", "123e4567e89b12d3a456426614174000")]
    public void GetBase16Binary(string content, string expected)
    {
        Get(x => x.GetBase16Binary, content, Convert.FromHexString(expected));
    }

    [DataTestMethod]
    [DataRow(" ", "")]
    [DataRow(" MTk4NC0wNS0wNA== ", "MTk4NC0wNS0wNA==")]
    public void GetBase64Binary(string content, string expected)
    {
        Get(x => x.GetBase64Binary, content, Convert.FromBase64String(expected));
    }
}
