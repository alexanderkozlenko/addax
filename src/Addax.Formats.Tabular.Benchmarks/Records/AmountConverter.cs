using System.Globalization;

namespace Addax.Formats.Tabular.Benchmarks.Records;

public sealed class AmountConverter : TabularConverter<Amount>
{
    public AmountConverter()
    {
    }

    public override bool TryFormat(Amount value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        return value.Value.TryFormat(destination, out charsWritten, default, provider);
    }

    public override bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out Amount value)
    {
        var result = long.TryParse(source, NumberStyles.AllowLeadingSign, provider, out var number);

        value = new(number);

        return result;
    }
}
