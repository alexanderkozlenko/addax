// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Handlers;

internal sealed class TabularStringArrayHandler : TabularHandler<string?[]>
{
    public override TabularRecord<string?[]> Read(TabularReader reader)
    {
        Debug.Assert(reader is not null);

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

        return new(builder.Build());
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public override async ValueTask<TabularRecord<string?[]>> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        Debug.Assert(reader is not null);

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

        return new(builder.Build());
    }

    public override void Write(TabularWriter writer, string?[] record)
    {
        Debug.Assert(writer is not null);
        Debug.Assert(record is not null);

        for (var i = 0; i < record.Length; i++)
        {
            writer.WriteString(record[i]);
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public override async ValueTask WriteAsync(TabularWriter writer, string?[] record, CancellationToken cancellationToken)
    {
        Debug.Assert(writer is not null);
        Debug.Assert(record is not null);

        for (var i = 0; i < record.Length; i++)
        {
            await writer.WriteStringAsync(record[i], cancellationToken).ConfigureAwait(false);
        }
    }
}
