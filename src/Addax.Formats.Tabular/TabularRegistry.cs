// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular;

/// <summary>Represents a registry of shared components for reading and writing tabular data. This class cannot be inherited.</summary>
public static partial class TabularRegistry
{
    private static readonly ConcurrentDictionary<Type, object> s_handlers = CreateHandlers();

    internal static TabularHandler<T> SelectHandler<T>()
        where T : notnull
    {
        if (!s_handlers.TryGetValue(typeof(T), out var handler) || (handler is not TabularHandler<T> handlerT))
        {
            ThrowHandlerNotFoundException();
        }

        return handlerT;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowHandlerNotFoundException()
        {
            throw new InvalidOperationException($"A record handler for type '{typeof(T)}' cannot be found in the registry.");
        }
    }

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
