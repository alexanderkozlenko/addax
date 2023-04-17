// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular;

/// <summary>Represents a registry of tabular data field converters.</summary>
public sealed class TabularFieldConverterRegistry : ConcurrentDictionary<Type, TabularFieldConverter>
{
    private static readonly TabularFieldConverterRegistry _shared = Create();

    internal Dictionary<Type, TabularFieldConverter> AppendTo(IEnumerable<TabularFieldConverter>? source)
    {
        var result = new Dictionary<Type, TabularFieldConverter>(this);

        if ((source is not null) && (!source.TryGetNonEnumeratedCount(out var count) || (count != 0)))
        {
            foreach (var converter in source)
            {
                result[converter.FieldType] = converter;
            }
        }

        return result;
    }

    private static TabularFieldConverterRegistry Create()
    {
        var registry = new TabularFieldConverterRegistry();

        registry.TryAdd(typeof(BigInteger), new TabularBigIntegerConverter());
        registry.TryAdd(typeof(bool), new TabularBooleanConverter());
        registry.TryAdd(typeof(byte), new TabularByteConverter());
        registry.TryAdd(typeof(char), new TabularCharConverter());
        registry.TryAdd(typeof(Complex), new TabularComplexConverter());
        registry.TryAdd(typeof(DateOnly), new TabularDateOnlyConverter());
        registry.TryAdd(typeof(DateTime), new TabularDateTimeConverter());
        registry.TryAdd(typeof(DateTimeOffset), new TabularDateTimeOffsetConverter());
        registry.TryAdd(typeof(decimal), new TabularDecimalConverter());
        registry.TryAdd(typeof(double), new TabularDoubleConverter());
        registry.TryAdd(typeof(Guid), new TabularGuidConverter());
        registry.TryAdd(typeof(Half), new TabularHalfConverter());
        registry.TryAdd(typeof(short), new TabularInt16Converter());
        registry.TryAdd(typeof(int), new TabularInt32Converter());
        registry.TryAdd(typeof(long), new TabularInt64Converter());
        registry.TryAdd(typeof(Int128), new TabularInt128Converter());
        registry.TryAdd(typeof(Rune), new TabularRuneConverter());
        registry.TryAdd(typeof(sbyte), new TabularSByteConverter());
        registry.TryAdd(typeof(float), new TabularSingleConverter());
        registry.TryAdd(typeof(string), new TabularStringConverter());
        registry.TryAdd(typeof(TimeOnly), new TabularTimeOnlyConverter());
        registry.TryAdd(typeof(TimeSpan), new TabularTimeSpanConverter());
        registry.TryAdd(typeof(ushort), new TabularUInt16Converter());
        registry.TryAdd(typeof(uint), new TabularUInt32Converter());
        registry.TryAdd(typeof(ulong), new TabularUInt64Converter());
        registry.TryAdd(typeof(UInt128), new TabularUInt128Converter());

        return registry;
    }

    /// <summary>Gets a shared registry.</summary>
    /// <value>A shared <see cref="TabularFieldConverterRegistry" /> instance.</value>
    public static TabularFieldConverterRegistry Shared
    {
        get
        {
            return _shared;
        }
    }
}
