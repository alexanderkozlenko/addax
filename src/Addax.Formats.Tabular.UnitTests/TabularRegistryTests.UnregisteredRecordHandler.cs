namespace Addax.Formats.Tabular.UnitTests;

public sealed partial class TabularRegistryTests
{
    private sealed class UnregisteredRecordHandler : TabularHandler<UnregisteredRecord>
    {
        public override UnregisteredRecord Read(TabularReader reader)
        {
            var fields = new List<string>();

            while (reader.TryReadField())
            {
                fields.Add(reader.GetString());
            }

            return new(string.Join('|', fields));
        }

        public override async ValueTask<UnregisteredRecord> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
        {
            var fields = new List<string>();

            while (await reader.TryReadFieldAsync(cancellationToken))
            {
                fields.Add(reader.GetString());
            }

            return new(string.Join('|', fields));
        }

        public override void Write(TabularWriter writer, UnregisteredRecord record)
        {
            foreach (var field in record.Value.Split('|'))
            {
                writer.WriteString(field);
            }
        }

        public override async ValueTask WriteAsync(TabularWriter writer, UnregisteredRecord record, CancellationToken cancellationToken)
        {
            foreach (var field in record.Value.Split('|'))
            {
                await writer.WriteStringAsync(field, cancellationToken);
            }
        }
    }
}
