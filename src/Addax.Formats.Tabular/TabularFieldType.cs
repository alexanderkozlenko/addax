// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Defines the type of a tabular field.</summary>
public enum TabularFieldType
{
    /// <summary>Indicates that the field type is not defined.</summary>
    None,

    /// <summary>Indicates that the field contains data.</summary>
    Content,

    /// <summary>Indicates that the field contains a comment.</summary>
    Comment,
}
