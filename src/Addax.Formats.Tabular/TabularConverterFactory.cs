// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

/// <summary>Provides creation of tabular converters.</summary>
public class TabularConverterFactory
{
    internal static readonly TabularConverterFactory Default = new();

    /// <summary>Initializes a new instance of the <see cref="TabularConverterFactory" /> class.</summary>
    public TabularConverterFactory()
    {
    }

    /// <summary>Creates a field converter of type <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of converter.</typeparam>
    /// <returns>An instance of type <typeparamref name="T" />.</returns>
    public virtual T CreateFieldConverter<T>()
        where T : TabularFieldConverter, new()
    {
        return RuntimeSingleton<T>.Instance;
    }
}
