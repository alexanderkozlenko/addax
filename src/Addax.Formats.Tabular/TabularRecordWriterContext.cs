// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Represents a context for <see cref="TabularRecordWriter" /> operations.</summary>
public sealed class TabularRecordWriterContext
{
    internal TabularRecordWriterContext(TabularConverterFactory converterFactory, long flushThreshold)
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
