// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Defines a set of options to configure <see cref="TabularFieldWriter" />.</summary>
public class TabularFieldWriterOptions
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldWriterOptions" /> class with optional arguments.</summary>
    /// <param name="encoding">The character encoding for decoding to a stream.</param>
    /// <param name="bufferSize">The buffer size in bytes for writing to a stream.</param>
    /// <param name="leaveOpen">The flag that indicates whether the stream should be left open after a writer is disposed.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="bufferSize" /> is less than or equals to zero or is greater than <see cref="Array.MaxLength" />.</exception>
    public TabularFieldWriterOptions(Encoding? encoding = null, int bufferSize = 4096, bool leaveOpen = false)
    {
        if ((bufferSize <= 0) || (bufferSize > Array.MaxLength))
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize), bufferSize, "The buffer size must be greater than zero and less than or equal to 'System.Array.MaxLength'.");
        }

        Encoding = encoding ?? TabularDataInfo.DefaultEncoding;
        BufferSize = bufferSize;
        LeaveOpen = leaveOpen;
    }

    /// <summary>Gets the character encoding for decoding to a stream.</summary>
    /// <value>An instance of <see cref="System.Text.Encoding" />.</value>
    public Encoding Encoding
    {
        get;
    }

    /// <summary>Gets the buffer size in bytes for writing to a stream.</summary>
    /// <value>A positive zero-based <see cref="int" /> value.</value>
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
}
