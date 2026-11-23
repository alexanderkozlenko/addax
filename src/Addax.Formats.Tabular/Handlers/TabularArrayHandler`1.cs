// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Handlers;

internal sealed class TabularArrayHandler<T> : TabularHandler<T?[]>
{
    public TabularArrayHandler()
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
            if (!typeof(T).IsValueType && reader.CurrentField.IsEmpty)
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
            if (!typeof(T).IsValueType && reader.CurrentField.IsEmpty)
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
            writer.Write(record[i], converter);
        }
    }

    public override async ValueTask WriteAsync(TabularWriter writer, T?[] record, CancellationToken cancellationToken)
    {
        var converter = writer.ResolveConverter<T>();

        for (var i = 0; i < record.Length; i++)
        {
            await writer.WriteAsync(record[i], converter, cancellationToken).ConfigureAwait(false);
        }
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowFieldConversionException(long recordIndex, int fieldIndex)
    {
        throw new TabularException($"The field {fieldIndex} of the record {recordIndex} cannot be converted to '{typeof(T)}'.");
    }
}
