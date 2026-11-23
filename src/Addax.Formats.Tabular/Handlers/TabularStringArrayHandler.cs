// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Handlers;

internal sealed class TabularStringArrayHandler : TabularHandler<string?[]>
{
    public TabularStringArrayHandler()
    {
    }

    public override string?[] Read(TabularReader reader)
    {
        using var builder = new ArrayBuilder<string?>(32);

        while (reader.TryReadField())
        {
            reader.TryGetString(out var field);
            builder.Add(string.IsNullOrEmpty(field) ? null : field);
        }

        return builder.Build();
    }

    public override async ValueTask<string?[]> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        using var builder = new ArrayBuilder<string?>(32);

        while (await reader.TryReadFieldAsync(cancellationToken).ConfigureAwait(false))
        {
            reader.TryGetString(out var field);
            builder.Add(string.IsNullOrEmpty(field) ? null : field);
        }

        return builder.Build();
    }

    public override void Write(TabularWriter writer, string?[] record)
    {
        for (var i = 0; i < record.Length; i++)
        {
            writer.WriteString(record[i]);
        }
    }

    public override async ValueTask WriteAsync(TabularWriter writer, string?[] record, CancellationToken cancellationToken)
    {
        for (var i = 0; i < record.Length; i++)
        {
            await writer.WriteStringAsync(record[i], cancellationToken).ConfigureAwait(false);
        }
    }
}
