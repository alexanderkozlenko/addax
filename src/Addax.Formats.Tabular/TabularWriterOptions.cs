// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Provides options to configure a tabular stream writer.</summary>
public sealed class TabularWriterOptions
{
    /// <summary>Initializes a new instance of the <see cref="TabularWriterOptions" /> class with optional arguments.</summary>
    /// <param name="encoding">The character encoding for encoding a stream.</param>
    /// <param name="bufferSize">The buffer size in bytes for writing to a stream.</param>
    /// <param name="leaveOpen">The flag that indicates whether the stream should be left open after a writer is disposed.</param>
    /// <param name="converterFactory">The custom tabular converter factory to use.</param>
    /// <param name="fieldConverters">The custom tabular field converters to use.</param>
    /// <param name="recordConverters">The custom tabular record converters to use.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is less than or equals to zero or is greater than <see cref="Array.MaxLength" />.</exception>
    public TabularWriterOptions(
        Encoding? encoding = null,
        int bufferSize = 4096,
        bool leaveOpen = false,
        TabularConverterFactory? converterFactory = null,
        IEnumerable<TabularFieldConverter>? fieldConverters = null,
        IEnumerable<TabularRecordConverter>? recordConverters = null)
    {
        if ((bufferSize <= 0) || (bufferSize > Array.MaxLength))
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "The buffer size must be greater than zero and less than or equal to 'System.Array.MaxLength'.");
        }

        Encoding = encoding ?? TabularFormatInfo.DefaultEncoding;
        BufferSize = bufferSize;
        LeaveOpen = leaveOpen;
        ConverterFactory = converterFactory ?? TabularConverterFactory.Default;
        FieldConverters = TabularFieldConverterRegistry.Shared.AppendTo(fieldConverters);
        RecordConverters = TabularRecordConverterRegistry.Shared.AppendTo(recordConverters);
    }

    /// <summary>Gets the character encoding for encoding a stream.</summary>
    /// <value>The provided or the default instance of <see cref="System.Text.Encoding" />.</value>
    public Encoding Encoding
    {
        get;
    }

    /// <summary>Gets the buffer size in bytes for writing to a stream.</summary>
    /// <value>A positive zero-based number.</value>
    public int BufferSize
    {
        get;
    }

    /// <summary>Gets the flag that indicates whether a stream should be left open after a writer is disposed.</summary>
    /// <value><see langword="true" /> if a stream should be left open; <see langword="false" /> otherwise.</value>
    public bool LeaveOpen
    {
        get;
    }

    /// <summary>Gets the current converter factory.</summary>
    /// <value>An instance of <see cref="TabularConverterFactory" />.</value>
    public TabularConverterFactory ConverterFactory
    {
        get;
    }

    /// <summary>Gets an aggregate of shared and custom tabular field converters that will be used by a writer.</summary>
    /// <value>An instance of <see cref="IReadOnlyDictionary{Type, TabularFieldConverter}" />.</value>
    public IReadOnlyDictionary<Type, TabularFieldConverter> FieldConverters
    {
        get;
    }

    /// <summary>Gets an aggregate of shared and custom tabular record converters that will be used by a writer.</summary>
    /// <value>An instance of <see cref="IReadOnlyDictionary{Type, TabularRecordConverter}" />.</value>
    public IReadOnlyDictionary<Type, TabularRecordConverter> RecordConverters
    {
        get;
    }
}
