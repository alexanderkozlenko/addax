// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;
using Addax.Formats.Tabular.Handlers;

namespace Addax.Formats.Tabular;

public partial class TabularRegistry
{
    private static ConcurrentDictionary<Type, object> CreateHandlers()
    {
        return new()
        {
            [typeof(string?[])] = new TabularStringArrayHandler(),
            [typeof(BigInteger[])] = new TabularArrayHandler<BigInteger>(TabularBigIntegerConverter.Instance),
            [typeof(BigInteger?[])] = new TabularSparseArrayHandler<BigInteger>(TabularBigIntegerConverter.Instance),
            [typeof(bool[])] = new TabularArrayHandler<bool>(TabularBooleanConverter.Instance),
            [typeof(bool?[])] = new TabularSparseArrayHandler<bool>(TabularBooleanConverter.Instance),
            [typeof(byte[])] = new TabularArrayHandler<byte>(TabularByteConverter.Instance),
            [typeof(byte?[])] = new TabularSparseArrayHandler<byte>(TabularByteConverter.Instance),
            [typeof(char[])] = new TabularArrayHandler<char>(TabularCharConverter.Instance),
            [typeof(char?[])] = new TabularSparseArrayHandler<char>(TabularCharConverter.Instance),
            [typeof(DateOnly[])] = new TabularArrayHandler<DateOnly>(TabularDateOnlyConverter.Instance),
            [typeof(DateOnly?[])] = new TabularSparseArrayHandler<DateOnly>(TabularDateOnlyConverter.Instance),
            [typeof(DateTime[])] = new TabularArrayHandler<DateTime>(TabularDateTimeConverter.Instance),
            [typeof(DateTime?[])] = new TabularSparseArrayHandler<DateTime>(TabularDateTimeConverter.Instance),
            [typeof(DateTimeOffset[])] = new TabularArrayHandler<DateTimeOffset>(TabularDateTimeOffsetConverter.Instance),
            [typeof(DateTimeOffset?[])] = new TabularSparseArrayHandler<DateTimeOffset>(TabularDateTimeOffsetConverter.Instance),
            [typeof(decimal[])] = new TabularArrayHandler<decimal>(TabularDecimalConverter.Instance),
            [typeof(decimal?[])] = new TabularSparseArrayHandler<decimal>(TabularDecimalConverter.Instance),
            [typeof(double[])] = new TabularArrayHandler<double>(TabularDoubleConverter.Instance),
            [typeof(double?[])] = new TabularSparseArrayHandler<double>(TabularDoubleConverter.Instance),
            [typeof(Guid[])] = new TabularArrayHandler<Guid>(TabularGuidConverter.Instance),
            [typeof(Guid?[])] = new TabularSparseArrayHandler<Guid>(TabularGuidConverter.Instance),
            [typeof(Half[])] = new TabularArrayHandler<Half>(TabularHalfConverter.Instance),
            [typeof(Half?[])] = new TabularSparseArrayHandler<Half>(TabularHalfConverter.Instance),
            [typeof(Int128[])] = new TabularArrayHandler<Int128>(TabularInt128Converter.Instance),
            [typeof(Int128?[])] = new TabularSparseArrayHandler<Int128>(TabularInt128Converter.Instance),
            [typeof(short[])] = new TabularArrayHandler<short>(TabularInt16Converter.Instance),
            [typeof(short?[])] = new TabularSparseArrayHandler<short>(TabularInt16Converter.Instance),
            [typeof(int[])] = new TabularArrayHandler<int>(TabularInt32Converter.Instance),
            [typeof(int?[])] = new TabularSparseArrayHandler<int>(TabularInt32Converter.Instance),
            [typeof(long[])] = new TabularArrayHandler<long>(TabularInt64Converter.Instance),
            [typeof(long?[])] = new TabularSparseArrayHandler<long>(TabularInt64Converter.Instance),
            [typeof(sbyte[])] = new TabularArrayHandler<sbyte>(TabularSByteConverter.Instance),
            [typeof(sbyte?[])] = new TabularSparseArrayHandler<sbyte>(TabularSByteConverter.Instance),
            [typeof(float[])] = new TabularArrayHandler<float>(TabularSingleConverter.Instance),
            [typeof(float?[])] = new TabularSparseArrayHandler<float>(TabularSingleConverter.Instance),
            [typeof(TimeOnly[])] = new TabularArrayHandler<TimeOnly>(TabularTimeOnlyConverter.Instance),
            [typeof(TimeOnly?[])] = new TabularSparseArrayHandler<TimeOnly>(TabularTimeOnlyConverter.Instance),
            [typeof(TimeSpan[])] = new TabularArrayHandler<TimeSpan>(TabularTimeSpanConverter.Instance),
            [typeof(TimeSpan?[])] = new TabularSparseArrayHandler<TimeSpan>(TabularTimeSpanConverter.Instance),
            [typeof(UInt128[])] = new TabularArrayHandler<UInt128>(TabularUInt128Converter.Instance),
            [typeof(UInt128?[])] = new TabularSparseArrayHandler<UInt128>(TabularUInt128Converter.Instance),
            [typeof(ushort[])] = new TabularArrayHandler<ushort>(TabularUInt16Converter.Instance),
            [typeof(ushort?[])] = new TabularSparseArrayHandler<ushort>(TabularUInt16Converter.Instance),
            [typeof(uint[])] = new TabularArrayHandler<uint>(TabularUInt32Converter.Instance),
            [typeof(uint?[])] = new TabularSparseArrayHandler<uint>(TabularUInt32Converter.Instance),
            [typeof(ulong[])] = new TabularArrayHandler<ulong>(TabularUInt64Converter.Instance),
            [typeof(ulong?[])] = new TabularSparseArrayHandler<ulong>(TabularUInt64Converter.Instance),
            [typeof(Uri?[])] = new TabularArrayHandler<Uri>(TabularUriConverter.Instance),
            [typeof(Rune[])] = new TabularArrayHandler<Rune>(TabularRuneConverter.Instance),
            [typeof(Rune?[])] = new TabularSparseArrayHandler<Rune>(TabularRuneConverter.Instance),
        };
    }
}
