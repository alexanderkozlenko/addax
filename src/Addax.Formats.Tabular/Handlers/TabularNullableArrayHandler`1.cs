// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Handlers;

internal sealed class TabularNullableArrayHandler<T> : TabularHandler<T?[]>
    where T : struct
{
    public TabularNullableArrayHandler()
    {
    }

    public override T?[] Read(TabularReader reader)
    {
        using var builder = new ArrayBuilder<T?>(32);

        var converter = reader.ResolveConverter<T>();
        var recordIndex = reader.RecordsRead;
        var fieldIndex = 0;

        while (reader.TryReadField())
        {
            if (reader.CurrentField.IsEmpty)
            {
                builder.Add(default);
            }
            else
            {
                if (!reader.TryGet(converter, out var field))
                {
                    ThrowFieldConversionException(recordIndex, fieldIndex);
                }

                builder.Add(field);
            }

            fieldIndex++;
        }

        return builder.Build();
    }

    public override async ValueTask<T?[]> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        using var builder = new ArrayBuilder<T?>(32);

        var converter = reader.ResolveConverter<T>();
        var recordIndex = reader.RecordsRead;
        var fieldIndex = 0;

        while (await reader.TryReadFieldAsync(cancellationToken).ConfigureAwait(false))
        {
            if (reader.CurrentField.IsEmpty)
            {
                builder.Add(default);
            }
            else
            {
                if (!reader.TryGet(converter, out var field))
                {
                    ThrowFieldConversionException(recordIndex, fieldIndex);
                }

                builder.Add(field);
            }

            fieldIndex++;
        }

        return builder.Build();
    }

    public override void Write(TabularWriter writer, T?[] record)
    {
        var converter = writer.ResolveConverter<T>();

        for (var i = 0; i < record.Length; i++)
        {
            var field = record[i];

            if (field.HasValue)
            {
                writer.Write(field.Value, converter);
            }
            else
            {
                writer.WriteEmpty();
            }
        }
    }

    public override async ValueTask WriteAsync(TabularWriter writer, T?[] record, CancellationToken cancellationToken)
    {
        var converter = writer.ResolveConverter<T>();

        for (var i = 0; i < record.Length; i++)
        {
            var field = record[i];

            if (field.HasValue)
            {
                await writer.WriteAsync(field.Value, converter, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                await writer.WriteEmptyAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowFieldConversionException(long recordIndex, int fieldIndex)
    {
        throw new TabularException($"The field {fieldIndex} of the record {recordIndex} cannot be converted to '{typeof(T)}'.");
    }
}
