// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Provides options to configure <see cref="TabularRecordWriter" />.</summary>
public class TabularRecordWriterOptions : TabularFieldWriterOptions
{
    /// <summary>Initializes a new instance of the <see cref="TabularRecordWriterOptions" /> class with optional arguments.</summary>
    /// <param name="encoding">The character encoding for encoding a stream.</param>
    /// <param name="bufferSize">The buffer size in bytes for writing to a stream.</param>
    /// <param name="leaveOpen">The flag that indicates whether the stream should be left open after a writer is disposed.</param>
    /// <param name="fieldConverters">The converters to use for converting fields from tabular data.</param>
    /// <param name="recordConverters">The converters to use for converting records to tabular data.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is less than or equals to zero or is greater than <see cref="Array.MaxLength" />.</exception>
    public TabularRecordWriterOptions(Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false, IEnumerable<TabularFieldConverter>? fieldConverters = null, IEnumerable<TabularRecordConverter>? recordConverters = null)
        : base(encoding, bufferSize, leaveOpen, fieldConverters)
    {
        RecordConverters = TabularRecordConverterRegistry.Shared.AppendTo(recordConverters);
    }

    /// <summary>Gets an aggregate of shared and custom tabular record converters that will be used by a writer.</summary>
    /// <value>An instance of <see cref="IReadOnlyDictionary{Type, TabularRecordConverter}" />.</value>
    public IReadOnlyDictionary<Type, TabularRecordConverter> RecordConverters
    {
        get;
    }
}
