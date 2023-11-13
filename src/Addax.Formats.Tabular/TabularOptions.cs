// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Specifies options to control the behavior of reading and writing tabular data. This class cannot be inherited.</summary>
public sealed class TabularOptions
{
    internal static readonly TabularOptions Default = new();

    /// <summary>Initializes a new instance of the <see cref="TabularOptions" /> class.</summary>
    public TabularOptions()
    {
        BufferSize = 16384;
    }

    /// <summary>Gets or sets the character encoding for reading from and writing to a stream.</summary>
    /// <value>An <see cref="System.Text.Encoding" /> instance or <see langword="null" /> to use the default UTF-8 encoding without byte order mark (BOM). The default is <see langword="null" />.</value>
    public Encoding? Encoding
    {
        get;
        set;
    }

    /// <summary>Gets or sets the minimum size of the buffer in bytes for reading from and writing to a stream.</summary>
    /// <value>A 32-bit signed integer. Must be greater than zero and less than or equal to <see cref="Array.MaxLength" />. The default is <c>16384</c>.</value>
    public int BufferSize
    {
        get;
        set;
    }

    /// <summary>Gets or sets a value that indicates whether a stream should stay open after a reader or writer is disposed.</summary>
    /// <value><see langword="true" /> to leave a stream open after a reader or writer is disposed; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
    public bool LeaveOpen
    {
        get;
        set;
    }

    /// <summary>Gets or set the factory that creates <see cref="string" /> instances from character sequences when reading fields as strings.</summary>
    /// <value>A <see cref="TabularStringFactory" /> instance or <see langword="null" /> to use the default factory, which returns <see cref="string.Empty" /> or creates a new <see cref="string" /> instance. The default is <see langword="null" />.</value>
    public TabularStringFactory? StringFactory
    {
        get;
        set;
    }

    /// <summary>Gets or sets a value that indicates whether leading and trailing white-space characters should be removed when reading and writing fields as strings.</summary>
    /// <value><see langword="true" /> to remove leading and trailing white-space characters; otherwise, <see langword="false" />. The default is <see langword="false" />.</value>
    public bool TrimWhitespace
    {
        get;
        set;
    }

    /// <summary>Gets or sets the object that supplies culture-specific formatting information when reading and writing fields as typed values.</summary>
    /// <value>An <see cref="IFormatProvider" /> object or <see langword="null" /> to use the default culture-independent provider. The default is <see langword="null" />.</value>
    public IFormatProvider? FormatProvider
    {
        get;
        set;
    }
}
