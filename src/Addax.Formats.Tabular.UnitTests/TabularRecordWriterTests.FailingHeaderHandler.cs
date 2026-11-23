namespace Addax.Formats.Tabular.UnitTests;

public sealed partial class TabularRecordWriterTests
{
    private sealed class FailingHeaderHandler : TabularHandler<string[]>
    {
        public override IEnumerable<string?>? Header
        {
            get
            {
                yield return "h1";

                throw new FormatException();
            }
        }
    }
}
