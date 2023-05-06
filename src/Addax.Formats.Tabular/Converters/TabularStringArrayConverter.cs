// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularStringArrayConverter : TabularRecordConverter<string[]>
{
    public override TabularRecord<string[]> ReadRecord(TabularFieldReader reader, TabularRecordReaderContext context, CancellationToken cancellationToken)
    {
        using var builder = ArrayBuilder<string>.Create(capacity: 32);

        while (reader.ReadField(cancellationToken))
        {
            if (reader.FieldType is TabularFieldType.Comment)
            {
                return TabularRecord<string[]>.FromComment(context.ConsumeComments ? reader.GetString() : null);
            }

            builder.Add(reader.GetString());
        }

        return TabularRecord<string[]>.FromContent(builder.ToArray());
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public override async ValueTask<TabularRecord<string[]>> ReadRecordAsync(TabularFieldReader reader, TabularRecordReaderContext context, CancellationToken cancellationToken)
    {
        using var builder = ArrayBuilder<string>.Create(capacity: 32);

        while (await reader.ReadFieldAsync(cancellationToken).ConfigureAwait(false))
        {
            if (reader.FieldType is TabularFieldType.Comment)
            {
                return TabularRecord<string[]>.FromComment(context.ConsumeComments ? reader.GetString() : null);
            }

            builder.Add(reader.GetString());
        }

        return TabularRecord<string[]>.FromContent(builder.ToArray());
    }

    public override void WriteRecord(TabularFieldWriter writer, string[] record, TabularRecordWriterContext context, CancellationToken cancellationToken)
    {
        for (var i = 0; i < record.Length; i++)
        {
            writer.WriteString(record[i]);

            if (writer.UnflushedChars > context.FlushThreshold)
            {
                writer.Flush(cancellationToken);
            }
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public override async ValueTask WriteRecordAsync(TabularFieldWriter writer, string[] record, TabularRecordWriterContext context, CancellationToken cancellationToken)
    {
        for (var i = 0; i < record.Length; i++)
        {
            writer.WriteString(record[i]);

            if (writer.UnflushedChars > context.FlushThreshold)
            {
                await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
