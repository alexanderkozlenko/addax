namespace Addax.Formats.Tabular.UnitTests;

public sealed partial class TabularRecordReaderTests
{
    private sealed class FieldlessHandler : TabularHandler<string>
    {
        public override string Read(TabularReader reader)
        {
            return "v";
        }

        public override ValueTask<string> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
        {
            return ValueTask.FromResult("v");
        }
    }
}
