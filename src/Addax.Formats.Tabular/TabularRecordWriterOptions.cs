// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Defines a set of options to configure <see cref="TabularRecordWriter" />.</summary>
public class TabularRecordWriterOptions : TabularFieldWriterOptions
{
    /// <summary>Initializes a new instance of the <see cref="TabularRecordWriterOptions" /> class with optional arguments.</summary>
    /// <param name="encoding">The character encoding for decoding to a stream.</param>
    /// <param name="bufferSize">The buffer size in bytes for writing to a stream.</param>
    /// <param name="leaveOpen">The flag that indicates whether the stream should be left open after a writer is disposed.</param>
    /// <param name="converters">The converters to use for converting records to tabular data.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is less than or equals to zero or is greater than <see cref="Array.MaxLength" />.</exception>
    public TabularRecordWriterOptions(Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false, IEnumerable<TabularRecordConverter>? converters = null)
        : base(encoding, bufferSize, leaveOpen)
    {
        Converters = TabularRecordConverterRegistry.Shared.AppendTo(converters);
    }

    /// <summary>Gets the converters to use for converting records to tabular data.</summary>
    /// <value>A collection of specified converters extended by the shared converter registry.</value>
    public IReadOnlyDictionary<Type, TabularRecordConverter> Converters
    {
        get;
    }
}
