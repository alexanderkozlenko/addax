// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Handlers;

internal sealed class TabularStringArrayHandler : TabularHandler<string?[]>
{
    public override TabularRecord<string?[]> Read(TabularReader reader)
    {
        using var builder = new ArrayBuilder<string?>(32);

        while (reader.TryReadField())
        {
            if (reader.CurrentFieldType != TabularFieldType.Value)
            {
                return default;
            }

            reader.TryGetString(out var field);
            builder.Add(field);
        }

        return new(builder.ToArray());
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public override async ValueTask<TabularRecord<string?[]>> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        using var builder = new ArrayBuilder<string?>(32);

        while (await reader.TryReadFieldAsync(cancellationToken).ConfigureAwait(false))
        {
            if (reader.CurrentFieldType != TabularFieldType.Value)
            {
                return default;
            }

            reader.TryGetString(out var field);
            builder.Add(field);
        }

        return new(builder.ToArray());
    }

    public override void Write(TabularWriter writer, string?[] record)
    {
        for (var i = 0; i < record.Length; i++)
        {
            writer.WriteString(record[i]);
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public override async ValueTask WriteAsync(TabularWriter writer, string?[] record, CancellationToken cancellationToken)
    {
        for (var i = 0; i < record.Length; i++)
        {
            await writer.WriteStringAsync(record[i], cancellationToken).ConfigureAwait(false);
        }
    }
}
