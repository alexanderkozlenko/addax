// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Defines the type of logical position in tabular data.</summary>
public enum TabularPositionType
{
    /// <summary>Indicates that the cursor is at the beginning of a stream.</summary>
    BeginningOfStream,

    /// <summary>Indicates that the cursor is at the beginning of a record.</summary>
    BeginningOfRecord,

    /// <summary>Indicates that the cursor is at the field separator.</summary>
    FieldSeparator,

    /// <summary>Indicates that the cursor is at the end of a record.</summary>
    EndOfRecord,

    /// <summary>Indicates that the cursor is at the end of a stream.</summary>
    EndOfStream,
}
