// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularStringEnumerableConverter : TabularRecordConverter<IEnumerable<string>>
{
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public override async ValueTask<TabularRecord<IEnumerable<string>>> ReadRecordAsync(TabularFieldReader reader, TabularRecordReaderContext context, CancellationToken cancellationToken)
    {
        using var builder = ArrayBuilder<string>.Create(capacity: 32);

        while (await reader.ReadFieldAsync(cancellationToken).ConfigureAwait(false))
        {
            if (reader.FieldType is TabularFieldType.Comment)
            {
                return TabularRecord<IEnumerable<string>>.AsComment(context.ConsumeComments ? reader.GetString() : null);
            }

            builder.Add(reader.GetString());
        }

        return TabularRecord<IEnumerable<string>>.AsContent(builder.ToArray());
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public override async ValueTask WriteRecordAsync(TabularFieldWriter writer, IEnumerable<string> record, TabularRecordWriterContext context, CancellationToken cancellationToken)
    {
        foreach (var value in record)
        {
            writer.WriteString(value);

            if (writer.UnflushedChars > context.FlushThreshold)
            {
                await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
