// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Represents a context for tabular records reading.</summary>
public sealed class TabularRecordReaderContext
{
    internal TabularRecordReaderContext(bool consumeComments)
    {
        ConsumeComments = consumeComments;
    }

    /// <summary> Gets a value indicating whether consuming comments as <see cref="string" /> values is enabled for the current record reader.</summary>
    /// <value><see langword="true" /> if consuming comments as <see cref="string" /> values is enabled for the current record reader; <see langword="false" /> otherwise.</value>
    public bool ConsumeComments
    {
        get;
    }
}
