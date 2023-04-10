// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular;

internal static class TabularFieldConverters
{
    public static readonly TabularFieldConverter<char> Char = new TabularCharConverter();
    public static readonly TabularFieldConverter<Rune> Rune = new TabularRuneConverter();
    public static readonly TabularFieldConverter<bool> Boolean = new TabularBooleanConverter();
    public static readonly TabularFieldConverter<sbyte> SByte = new TabularSByteConverter();
    public static readonly TabularFieldConverter<byte> Byte = new TabularByteConverter();
    public static readonly TabularFieldConverter<short> Int16 = new TabularInt16Converter();
    public static readonly TabularFieldConverter<ushort> UInt16 = new TabularUInt16Converter();
    public static readonly TabularFieldConverter<int> Int32 = new TabularInt32Converter();
    public static readonly TabularFieldConverter<uint> UInt32 = new TabularUInt32Converter();
    public static readonly TabularFieldConverter<long> Int64 = new TabularInt64Converter();
    public static readonly TabularFieldConverter<ulong> UInt64 = new TabularUInt64Converter();
    public static readonly TabularFieldConverter<Int128> Int128 = new TabularInt128Converter();
    public static readonly TabularFieldConverter<UInt128> UInt128 = new TabularUInt128Converter();
    public static readonly TabularFieldConverter<BigInteger> BigInteger = new TabularBigIntegerConverter();
    public static readonly TabularFieldConverter<Half> Half = new TabularHalfConverter();
    public static readonly TabularFieldConverter<float> Single = new TabularSingleConverter();
    public static readonly TabularFieldConverter<double> Double = new TabularDoubleConverter();
    public static readonly TabularFieldConverter<decimal> Decimal = new TabularDecimalConverter();
    public static readonly TabularFieldConverter<Complex> Complex = new TabularComplexConverter();
    public static readonly TabularFieldConverter<TimeSpan> TimeSpan = new TabularTimeSpanConverter();
    public static readonly TabularFieldConverter<TimeOnly> TimeOnly = new TabularTimeOnlyConverter();
    public static readonly TabularFieldConverter<DateOnly> DateOnly = new TabularDateOnlyConverter();
    public static readonly TabularFieldConverter<DateTime> DateTime = new TabularDateTimeConverter();
    public static readonly TabularFieldConverter<DateTimeOffset> DateTimeOffset = new TabularDateTimeOffsetConverter();
    public static readonly TabularFieldConverter<Guid> Guid = new TabularGuidConverter();
}
