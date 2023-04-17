// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Converts an object or value to or from tabular field.</summary>
public abstract class TabularFieldConverter
{
    private protected TabularFieldConverter()
    {
    }

    /// <summary>Get the type of object or value handled by the converter.</summary>
    public abstract Type FieldType
    {
        get;
    }
}
