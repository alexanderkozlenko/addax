// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using Addax.Formats.Tabular.Handlers;

namespace Addax.Formats.Tabular;

/// <summary>Represents a registry of shared components for reading and writing tabular data. This class cannot be inherited.</summary>
public static class TabularRegistry
{
    private static readonly ConcurrentDictionary<Type, object> s_handlers = new()
    {
        [typeof(string?[])] = new TabularStringArrayHandler(),
    };

    /// <summary>Gets the collection of shared record handlers.</summary>
    /// <value>A thread-safe dictionary with a record type as the key.</value>
    public static IDictionary<Type, object> Handlers
    {
        get
        {
            return s_handlers;
        }
    }
}
