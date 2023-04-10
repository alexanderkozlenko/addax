// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Converts an object to or from a tabular data record.</summary>
public abstract class TabularRecordConverter
{
    private protected TabularRecordConverter()
    {
    }

    /// <summary>Get the type of object handled by the converter.</summary>
    public abstract Type RecordType
    {
        get;
    }
}
