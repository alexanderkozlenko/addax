// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1822

using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

/// <summary>Provides creation of tabular converters.</summary>
public sealed class TabularConverterFactory
{
    internal static readonly TabularConverterFactory Instance = new();

    private TabularConverterFactory()
    {
    }

    /// <summary>Creates a field converter of type <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of converter.</typeparam>
    /// <returns>An instance of type <typeparamref name="T" />.</returns>
    public T CreateFieldConverter<T>()
        where T : TabularFieldConverter, new()
    {
        return Singleton<T>.Instance;
    }
}
