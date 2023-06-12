// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Represents a context for a tabular stream reader.</summary>
public sealed class TabularReaderContext
{
    internal TabularReaderContext(TabularConverterFactory converterFactory, bool consumeComments)
    {
        ConverterFactory = converterFactory;
        ConsumeComments = consumeComments;
    }

    /// <summary>Gets the current converter factory.</summary>
    /// <value>An instance of <see cref="TabularConverterFactory" />.</value>
    public TabularConverterFactory ConverterFactory
    {
        get;
    }

    /// <summary>Gets a value indicating whether consuming comments as <see cref="string" /> values is enabled for the current record reader.</summary>
    /// <value><see langword="true" /> if consuming comments as <see cref="string" /> values is enabled for the current record reader; <see langword="false" /> otherwise.</value>
    public bool ConsumeComments
    {
        get;
    }
}
