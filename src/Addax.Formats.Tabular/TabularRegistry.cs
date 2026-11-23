// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular;

/// <summary>Represents a registry of shared components for reading and writing tabular data.</summary>
/// <remarks>All public members of the type are thread-safe and may be used concurrently from multiple threads.</remarks>
public static partial class TabularRegistry
{
    private static readonly ConcurrentDictionary<Type, TabularConverter> s_converters = CreateConverters();
    private static readonly ConcurrentDictionary<Type, TabularHandler> s_handlers = CreateHandlers();

    internal static TabularHandler<T> SelectHandler<T>(TabularOptions? options)
        where T : notnull
    {
        if ((options is not null) && options.Handlers.TryGetValue(typeof(T), out var substitute))
        {
            if (substitute is null)
            {
                ThrowNullSubstituteException();
            }

            if (substitute is not TabularHandler<T> substituteT)
            {
                ThrowInvalidSubstituteException(substitute);
            }

            return substituteT;
        }

        if (!s_handlers.TryGetValue(typeof(T), out var handler) || (handler is not TabularHandler<T> handlerT))
        {
            ThrowHandlerNotFoundException();
        }

        return handlerT;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowNullSubstituteException()
        {
            throw new ArgumentException($"The record handler for type '{typeof(T)}' in the options is null.");
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidSubstituteException(TabularHandler substitute)
        {
            throw new ArgumentException($"The record handler '{substitute.GetType()}' for type '{typeof(T)}' in the options is not a record handler for that type.");
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowHandlerNotFoundException()
        {
            throw new InvalidOperationException($"A record handler for type '{typeof(T)}' cannot be found in the options or the registry.");
        }
    }

    /// <summary>Gets the collection of shared value converters.</summary>
    /// <value>A thread-safe dictionary with a type as the key and a converter for values of that type as the value.</value>
    public static IDictionary<Type, TabularConverter> Converters
    {
        get
        {
            return s_converters;
        }
    }

    /// <summary>Gets the collection of shared record handlers.</summary>
    /// <value>A thread-safe dictionary with a record type as the key and a record handler for that type as the value.</value>
    public static IDictionary<Type, TabularHandler> Handlers
    {
        get
        {
            return s_handlers;
        }
    }
}
