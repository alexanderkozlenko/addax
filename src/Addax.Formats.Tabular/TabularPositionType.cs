// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Specifies the type of a position in tabular data.</summary>
public enum TabularPositionType
{
    /// <summary>The start of a tabular stream.</summary>
    StartOfStream,

    /// <summary>The start of a tabular record.</summary>
    StartOfRecord,

    /// <summary>A delimiter between tabular fields.</summary>
    Delimiter,

    /// <summary>The end of a tabular record.</summary>
    EndOfRecord,

    /// <summary>The end of a tabular stream.</summary>
    EndOfStream,
}
