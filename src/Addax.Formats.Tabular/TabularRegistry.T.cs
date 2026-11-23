// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;
using Addax.Formats.Tabular.Handlers;

namespace Addax.Formats.Tabular;

public partial class TabularRegistry
{
    private static ConcurrentDictionary<Type, TabularConverter> CreateConverters()
    {
        return new()
        {
            [typeof(BFloat16)] = new TabularBFloat16Converter(),
            [typeof(BigInteger)] = new TabularBigIntegerConverter(),
            [typeof(bool)] = new TabularBooleanConverter(),
            [typeof(byte)] = new TabularByteConverter(),
            [typeof(char)] = new TabularCharConverter(),
            [typeof(DateOnly)] = new TabularDateOnlyConverter(),
            [typeof(DateTime)] = new TabularDateTimeConverter(),
            [typeof(DateTimeOffset)] = new TabularDateTimeOffsetConverter(),
            [typeof(decimal)] = new TabularDecimalConverter(),
            [typeof(Decimal32)] = new TabularDecimal32Converter(),
            [typeof(Decimal64)] = new TabularDecimal64Converter(),
            [typeof(Decimal128)] = new TabularDecimal128Converter(),
            [typeof(double)] = new TabularDoubleConverter(),
            [typeof(Guid)] = new TabularGuidConverter(),
            [typeof(Half)] = new TabularHalfConverter(),
            [typeof(short)] = new TabularInt16Converter(),
            [typeof(int)] = new TabularInt32Converter(),
            [typeof(long)] = new TabularInt64Converter(),
            [typeof(Int128)] = new TabularInt128Converter(),
            [typeof(Rune)] = new TabularRuneConverter(),
            [typeof(sbyte)] = new TabularSByteConverter(),
            [typeof(float)] = new TabularSingleConverter(),
            [typeof(TimeOnly)] = new TabularTimeOnlyConverter(),
            [typeof(TimeSpan)] = new TabularTimeSpanConverter(),
            [typeof(ushort)] = new TabularUInt16Converter(),
            [typeof(uint)] = new TabularUInt32Converter(),
            [typeof(ulong)] = new TabularUInt64Converter(),
            [typeof(UInt128)] = new TabularUInt128Converter(),
            [typeof(Uri)] = new TabularUriConverter(),
        };
    }

    private static ConcurrentDictionary<Type, TabularHandler> CreateHandlers()
    {
        return new()
        {
            [typeof(string?[])] = new TabularStringArrayHandler(),
            [typeof(BFloat16[])] = new TabularArrayHandler<BFloat16>(),
            [typeof(BFloat16?[])] = new TabularNullableArrayHandler<BFloat16>(),
            [typeof(BigInteger[])] = new TabularArrayHandler<BigInteger>(),
            [typeof(BigInteger?[])] = new TabularNullableArrayHandler<BigInteger>(),
            [typeof(bool[])] = new TabularArrayHandler<bool>(),
            [typeof(bool?[])] = new TabularNullableArrayHandler<bool>(),
            [typeof(byte[])] = new TabularArrayHandler<byte>(),
            [typeof(byte?[])] = new TabularNullableArrayHandler<byte>(),
            [typeof(char[])] = new TabularArrayHandler<char>(),
            [typeof(char?[])] = new TabularNullableArrayHandler<char>(),
            [typeof(DateOnly[])] = new TabularArrayHandler<DateOnly>(),
            [typeof(DateOnly?[])] = new TabularNullableArrayHandler<DateOnly>(),
            [typeof(DateTime[])] = new TabularArrayHandler<DateTime>(),
            [typeof(DateTime?[])] = new TabularNullableArrayHandler<DateTime>(),
            [typeof(DateTimeOffset[])] = new TabularArrayHandler<DateTimeOffset>(),
            [typeof(DateTimeOffset?[])] = new TabularNullableArrayHandler<DateTimeOffset>(),
            [typeof(decimal[])] = new TabularArrayHandler<decimal>(),
            [typeof(decimal?[])] = new TabularNullableArrayHandler<decimal>(),
            [typeof(Decimal128[])] = new TabularArrayHandler<Decimal128>(),
            [typeof(Decimal128?[])] = new TabularNullableArrayHandler<Decimal128>(),
            [typeof(Decimal32[])] = new TabularArrayHandler<Decimal32>(),
            [typeof(Decimal32?[])] = new TabularNullableArrayHandler<Decimal32>(),
            [typeof(Decimal64[])] = new TabularArrayHandler<Decimal64>(),
            [typeof(Decimal64?[])] = new TabularNullableArrayHandler<Decimal64>(),
            [typeof(double[])] = new TabularArrayHandler<double>(),
            [typeof(double?[])] = new TabularNullableArrayHandler<double>(),
            [typeof(Guid[])] = new TabularArrayHandler<Guid>(),
            [typeof(Guid?[])] = new TabularNullableArrayHandler<Guid>(),
            [typeof(Half[])] = new TabularArrayHandler<Half>(),
            [typeof(Half?[])] = new TabularNullableArrayHandler<Half>(),
            [typeof(Int128[])] = new TabularArrayHandler<Int128>(),
            [typeof(Int128?[])] = new TabularNullableArrayHandler<Int128>(),
            [typeof(short[])] = new TabularArrayHandler<short>(),
            [typeof(short?[])] = new TabularNullableArrayHandler<short>(),
            [typeof(int[])] = new TabularArrayHandler<int>(),
            [typeof(int?[])] = new TabularNullableArrayHandler<int>(),
            [typeof(long[])] = new TabularArrayHandler<long>(),
            [typeof(long?[])] = new TabularNullableArrayHandler<long>(),
            [typeof(sbyte[])] = new TabularArrayHandler<sbyte>(),
            [typeof(sbyte?[])] = new TabularNullableArrayHandler<sbyte>(),
            [typeof(float[])] = new TabularArrayHandler<float>(),
            [typeof(float?[])] = new TabularNullableArrayHandler<float>(),
            [typeof(TimeOnly[])] = new TabularArrayHandler<TimeOnly>(),
            [typeof(TimeOnly?[])] = new TabularNullableArrayHandler<TimeOnly>(),
            [typeof(TimeSpan[])] = new TabularArrayHandler<TimeSpan>(),
            [typeof(TimeSpan?[])] = new TabularNullableArrayHandler<TimeSpan>(),
            [typeof(UInt128[])] = new TabularArrayHandler<UInt128>(),
            [typeof(UInt128?[])] = new TabularNullableArrayHandler<UInt128>(),
            [typeof(ushort[])] = new TabularArrayHandler<ushort>(),
            [typeof(ushort?[])] = new TabularNullableArrayHandler<ushort>(),
            [typeof(uint[])] = new TabularArrayHandler<uint>(),
            [typeof(uint?[])] = new TabularNullableArrayHandler<uint>(),
            [typeof(ulong[])] = new TabularArrayHandler<ulong>(),
            [typeof(ulong?[])] = new TabularNullableArrayHandler<ulong>(),
            [typeof(Uri?[])] = new TabularArrayHandler<Uri>(),
            [typeof(Rune[])] = new TabularArrayHandler<Rune>(),
            [typeof(Rune?[])] = new TabularNullableArrayHandler<Rune>(),
        };
    }
}
