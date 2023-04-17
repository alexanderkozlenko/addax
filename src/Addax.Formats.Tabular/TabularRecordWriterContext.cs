// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Represents a context for <see cref="TabularRecordWriter" /> operations.</summary>
public sealed class TabularRecordWriterContext
{
    internal TabularRecordWriterContext(TabularConverterFactory converterFactory)
    {
        ConverterFactory = converterFactory;
    }

    /// <summary>Gets the current converter factory.</summary>
    /// <value>An instance of <see cref="TabularConverterFactory" />.</value>
    public TabularConverterFactory ConverterFactory
    {
        get;
    }
}
