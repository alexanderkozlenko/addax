namespace Addax.Formats.Tabular.UnitTests;

public sealed partial class TabularStringFactoryTests
{
    private sealed class PrefixingStringFactory : TabularStringFactory
    {
        public override string Create(ReadOnlySpan<char> source)
        {
            return $"urn:{source}";
        }
    }
}
