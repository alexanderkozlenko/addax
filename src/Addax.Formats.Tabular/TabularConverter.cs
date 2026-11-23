// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Represents the base type of converters that convert values to or from character sequences.</summary>
public abstract class TabularConverter
{
    private protected TabularConverter()
    {
    }

    internal abstract Type ValueType
    {
        get;
    }
}
