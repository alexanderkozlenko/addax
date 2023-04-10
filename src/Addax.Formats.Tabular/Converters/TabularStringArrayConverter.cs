// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular.Converters;

internal sealed class TabularStringArrayConverter<T> : TabularRecordConverter<T>
    where T : IEnumerable<string>
{
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public override async ValueTask<TabularRecord<T>> ReadRecordAsync(TabularFieldReader reader, TabularRecordReaderContext context, CancellationToken cancellationToken)
    {
        using var builder = ArrayBuilder<string>.Create(capacity: 32);

        while (await reader.ReadFieldAsync(cancellationToken).ConfigureAwait(false))
        {
            if (reader.FieldType is TabularFieldType.Comment)
            {
                return TabularRecord<T>.AsComment(context.ConsumeComments ? reader.GetString() : null);
            }

            builder.Add(reader.GetString());
        }

        return TabularRecord<T>.AsContent((T)(IEnumerable<string>)builder.ToArray());
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public override async ValueTask WriteRecordAsync(TabularFieldWriter writer, T record, CancellationToken cancellationToken)
    {
        foreach (var value in record)
        {
            writer.WriteString(value);

            await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
