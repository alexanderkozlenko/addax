// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular;

internal sealed partial class TabularConverterResolver
{
    private readonly FrozenDictionary<Type, TabularConverter>? _substitutes;

    private Dictionary<Type, TabularConverter>? _converters;

    public TabularConverterResolver(IDictionary<Type, TabularConverter> substitutes)
    {
        if (substitutes.Count != 0)
        {
            _substitutes = CreateSubstitutes(substitutes);
        }
    }

    public TabularConverter<T> Resolve<T>()
    {
        if ((_converters is not null) && _converters.TryGetValue(typeof(T), out var converter))
        {
            return (TabularConverter<T>)converter;
        }

        return Add<T>();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private TabularConverter<T> Resolve<T>(ref TabularConverter<T>? converter)
    {
        converter = Resolve<T>();

        return converter;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private TabularConverter<T> Add<T>()
    {
        if (((_substitutes is null) || !_substitutes.TryGetValue(typeof(T), out var converter)) && !TabularRegistry.Converters.TryGetValue(typeof(T), out converter))
        {
            ThrowConverterNotFoundException();
        }

        if (converter is not TabularConverter<T> converterT)
        {
            ThrowInvalidConverterException();
        }

        _converters ??= [];
        _converters.Add(typeof(T), converterT);

        return converterT;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowConverterNotFoundException()
        {
            throw new InvalidOperationException($"A converter for type '{typeof(T)}' cannot be found in the options or the registry.");
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidConverterException()
        {
            throw new InvalidOperationException($"The registry entry for type '{typeof(T)}' is not a converter for that type.");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static FrozenDictionary<Type, TabularConverter> CreateSubstitutes(IDictionary<Type, TabularConverter> substitutes)
    {
        var frozenSubstitutes = substitutes.ToFrozenDictionary();

        foreach (var (valueType, substitute) in frozenSubstitutes)
        {
            if (substitute is null)
            {
                ThrowNullSubstituteException(valueType);
            }

            if (substitute.ValueType != valueType)
            {
                ThrowInvalidSubstituteException(valueType, substitute);
            }
        }

        return frozenSubstitutes;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowNullSubstituteException(Type valueType)
        {
            throw new ArgumentException($"The converter for type '{valueType}' in the options is null.");
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidSubstituteException(Type valueType, TabularConverter substitute)
        {
            throw new ArgumentException($"The converter '{substitute.GetType()}' for type '{valueType}' in the options is not a converter for that type.");
        }
    }
}
