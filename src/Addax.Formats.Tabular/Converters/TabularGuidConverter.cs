// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular.Converters;

/// <summary>Converts a <see cref="Guid" /> value from or to a character sequence.</summary>
public class TabularGuidConverter : TabularConverter<Guid>
{
    internal static readonly TabularGuidConverter Instance = new();

    /// <summary>Initializes a new instance of the <see cref="TabularGuidConverter" /> class.</summary>
    public TabularGuidConverter()
    {
    }

    /// <inheritdoc />
    public override bool TryFormat(Guid value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.TryFormat(destination, out charsWritten, "d");
    }

    /// <inheritdoc />
    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Guid value)
    {
        return Guid.TryParseExact(source, "d", out value);
    }
}
