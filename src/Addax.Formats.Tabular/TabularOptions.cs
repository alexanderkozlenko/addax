// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Globalization;
using System.Text;

namespace Addax.Formats.Tabular;

/// <summary>Specifies options to control the behavior of reading and writing tabular data. This class cannot be inherited.</summary>
public sealed class TabularOptions
{
    internal static readonly TabularOptions Default = new();
    internal static readonly CultureInfo DefaultCultureInfo = CreateDefaultCultureInfo();

    /// <summary>Initializes a new instance of the <see cref="TabularOptions" /> class.</summary>
    public TabularOptions()
    {
        Converters = new Dictionary<Type, TabularConverter>();
        Handlers = new Dictionary<Type, TabularHandler>();

        BufferSize = 0x00004000;
    }

    private static CultureInfo CreateDefaultCultureInfo()
    {
        var culture = CultureInfo.CreateSpecificCulture(string.Empty);

        culture.NumberFormat.PositiveInfinitySymbol = "INF";
        culture.NumberFormat.NegativeInfinitySymbol = "-INF";

        return CultureInfo.ReadOnly(culture);
    }

    /// <summary>Gets the value converters that readers and writers created with these options use instead of the converters in <see cref="TabularRegistry.Converters" />.</summary>
    /// <value>A dictionary with a type as the key and a converter for values of that type as the value. The default is an empty dictionary.</value>
    public IDictionary<Type, TabularConverter> Converters
    {
        get;
    }

    /// <summary>Gets the record handlers that record readers and writers created with these options use instead of the handlers in <see cref="TabularRegistry.Handlers" />.</summary>
    /// <value>A dictionary with a record type as the key and a record handler for that type as the value. The default is an empty dictionary.</value>
    public IDictionary<Type, TabularHandler> Handlers
    {
        get;
    }

    /// <summary>Gets or sets the character encoding for reading from and writing to a stream.</summary>
    /// <value>An <see cref="System.Text.Encoding" /> instance or <see langword="null" /> to use UTF-8 encoding, which skips a byte order mark (BOM) when reading and writes none. The default is <see langword="null" />.</value>
    public Encoding? Encoding
    {
        get;
        set;
    }

    /// <summary>Gets or sets the minimum size of the buffer in bytes for reading from and writing to a stream.</summary>
    /// <value>A 32-bit signed integer. A value less than one is treated as one, and a value greater than <see cref="Array.MaxLength" /> as <see cref="Array.MaxLength" />. The default is <c>16384</c>.</value>
    public int BufferSize
    {
        get;
        set;
    }

    /// <summary>Gets or sets the factory that creates <see cref="string" /> instances from character sequences when reading fields as strings.</summary>
    /// <value>A <see cref="TabularStringFactory" /> instance or <see langword="null" /> to use the default factory, which returns <see cref="string.Empty" /> or creates a new <see cref="string" /> instance. The default is <see langword="null" />.</value>
    public TabularStringFactory? StringFactory
    {
        get;
        set;
    }

    /// <summary>Gets or sets a value that indicates whether leading and trailing white-space characters should be removed from fields when reading and from strings when writing.</summary>
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
