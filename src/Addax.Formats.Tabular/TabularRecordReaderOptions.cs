// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Defines a set of options to configure <see cref="TabularRecordReader" />.</summary>
public class TabularRecordReaderOptions : TabularFieldReaderOptions
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldReaderOptions" /> class with optional arguments.</summary>
    /// <param name="encoding">The character encoding for decoding from a stream.</param>
    /// <param name="bufferSize">The buffer size in bytes for reading from a stream.</param>
    /// <param name="leaveOpen">The flag that indicates whether the stream should be left open after a reader is disposed.</param>
    /// <param name="converters">The converters to use for converting records from tabular data.</param>
    /// <param name="consumeComments">The flag that indicates whether consuming comments as <see cref="string" /> values is enabled for a reader.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is less than or equals to zero or is greater than <see cref="Array.MaxLength" />.</exception>
    public TabularRecordReaderOptions(Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false, IEnumerable<TabularRecordConverter>? converters = null, bool consumeComments = false)
        : base(encoding, bufferSize, leaveOpen)
    {
        Converters = TabularRecordConverterRegistry.Shared.AppendTo(converters);
        ConsumeComments = consumeComments;
    }

    /// <summary>Gets the converters to use for converting records from tabular data.</summary>
    /// <value>A collection of specified converters extended by the shared converter registry.</value>
    public IReadOnlyDictionary<Type, TabularRecordConverter> Converters
    {
        get;
    }

    /// <summary> Gets a value indicating whether consuming comments as <see cref="string" /> values is enabled for a reader.</summary>
    /// <value><see langword="true" /> if consuming comments as <see cref="string" /> values is enabled for a reader; <see langword="false" /> otherwise.</value>
    public bool ConsumeComments
    {
        get;
    }
}
