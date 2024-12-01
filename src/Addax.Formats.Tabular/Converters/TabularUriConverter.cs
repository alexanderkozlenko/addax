// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Uri" /> instance from or to a character sequence.</summary>
public class TabularUriConverter : TabularConverter<Uri>
{
    internal static readonly TabularUriConverter Instance = new();

    private readonly TabularStringFactory _stringFactory;

    /// <summary>Initializes a new instance of the <see cref="TabularUriConverter" /> class.</summary>
    public TabularUriConverter()
    {
        _stringFactory = TabularStringFactory.Default;
    }

    /// <summary>Initializes a new instance of the <see cref="TabularUriConverter" /> class with the specified string factory.</summary>
    /// <param name="stringFactory">The factory that creates <see cref="string" /> instances from character sequences.</param>
    protected TabularUriConverter(TabularStringFactory? stringFactory)
    {
        _stringFactory = stringFactory ?? TabularStringFactory.Default;
    }

    /// <inheritdoc />
    public override bool TryFormat(Uri? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        if (value is null)
        {
            charsWritten = 0;

            return true;
        }

        return value.TryFormat(destination, out charsWritten);
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, [NotNullWhen(true)] out Uri? value)
    {
        return Uri.TryCreate(_stringFactory.Create(source), UriKind.RelativeOrAbsolute, out value);
    }
}
