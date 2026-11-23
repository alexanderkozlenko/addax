namespace Addax.Formats.Tabular.UnitTests;

public sealed partial class TabularStringFactoryTests
{
    private sealed class BracketingStringFactory : TabularStringFactory
    {
        public override string Create(ReadOnlySpan<char> source)
        {
            return $"<{source}>";
        }
    }
}
