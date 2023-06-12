// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Represents a context for a tabular stream writer.</summary>
public sealed class TabularWriterContext
{
    internal TabularWriterContext(TabularConverterFactory converterFactory, long flushThreshold)
    {
        ConverterFactory = converterFactory;
        FlushThreshold = flushThreshold;
    }

    /// <summary>Gets the current converter factory.</summary>
    /// <value>An instance of <see cref="TabularConverterFactory" />.</value>
    public TabularConverterFactory ConverterFactory
    {
        get;
    }

    /// <summary>Gets the number of characters at which a flush operation should be triggered when exceeded.</summary>
    /// <value>A non-negative zero-based number.</value>
    public long FlushThreshold
    {
        get;
    }
}
