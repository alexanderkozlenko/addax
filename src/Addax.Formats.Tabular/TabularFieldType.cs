// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Specifies the type of a tabular field.</summary>
public enum TabularFieldType
{
    /// <summary>Indicates that the field is not defined.</summary>
    None,

    /// <summary>A value.</summary>
    Value,

    /// <summary>An annotation.</summary>
    Annotation,
}
