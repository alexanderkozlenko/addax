// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Provides options to configure <see cref="TabularRecordReader" />.</summary>
public class TabularRecordReaderOptions : TabularFieldReaderOptions
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldReaderOptions" /> class with optional arguments.</summary>
    /// <param name="encoding">The character encoding for decoding a stream.</param>
    /// <param name="bufferSize">The buffer size in bytes for reading from a stream.</param>
    /// <param name="leaveOpen">The flag that indicates whether the stream should be left open after a reader is disposed.</param>
    /// <param name="consumeComments">The flag that indicates whether consuming comments as <see cref="string" /> values is enabled for a reader.</param>
    /// <param name="fieldConverters">The converters to use for converting fields from tabular data.</param>
    /// <param name="recordConverters">The converters to use for converting records from tabular data.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is less than or equals to zero or is greater than <see cref="Array.MaxLength" />.</exception>
    public TabularRecordReaderOptions(Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false, bool consumeComments = false, IEnumerable<TabularFieldConverter>? fieldConverters = null, IEnumerable<TabularRecordConverter>? recordConverters = null)
        : base(encoding, bufferSize, leaveOpen, fieldConverters)
    {
        ConsumeComments = consumeComments;
        RecordConverters = TabularRecordConverterRegistry.Shared.AppendTo(recordConverters);
    }

    /// <summary>Gets a value indicating whether consuming comments as <see cref="string" /> values is enabled for a reader.</summary>
    /// <value><see langword="true" /> if consuming comments as <see cref="string" /> values is enabled for a reader; <see langword="false" /> otherwise.</value>
    public bool ConsumeComments
    {
        get;
    }

    /// <summary>Gets an aggregate of shared and custom tabular record converters that will be used by a reader.</summary>
    /// <value>An instance of <see cref="IReadOnlyDictionary{Type, TabularRecordConverter}" />.</value>
    public IReadOnlyDictionary<Type, TabularRecordConverter> RecordConverters
    {
        get;
    }
}
