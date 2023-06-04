// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular;

/// <summary>Represents a registry of tabular data record converters.</summary>
public sealed class TabularRecordConverterRegistry : ConcurrentDictionary<Type, TabularRecordConverter>
{
    private static readonly TabularRecordConverterRegistry _shared = Create();

    internal Dictionary<Type, TabularRecordConverter> AppendTo(IEnumerable<TabularRecordConverter>? source)
    {
        var result = new Dictionary<Type, TabularRecordConverter>(this);

        if ((source is not null) && (!source.TryGetNonEnumeratedCount(out var count) || (count != 0)))
        {
            foreach (var converter in source)
            {
                result[converter.RecordType] = converter;
            }
        }

        return result;
    }

    private static TabularRecordConverterRegistry Create()
    {
        var registry = new TabularRecordConverterRegistry();

        registry.TryAdd(typeof(string[]), new TabularStringArrayConverter());

        return registry;
    }

    /// <summary>Gets a shared registry.</summary>
    /// <value>A shared <see cref="TabularRecordConverterRegistry" /> instance.</value>
    public static TabularRecordConverterRegistry Shared
    {
        get
        {
            return _shared;
        }
    }
}
