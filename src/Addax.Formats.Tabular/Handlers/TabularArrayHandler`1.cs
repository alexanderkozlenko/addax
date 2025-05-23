﻿// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular.Handlers;

/// <summary>Reads an array of <typeparamref name="T" /> from or writes an array of <typeparamref name="T" /> to a tabular record. This class cannot be inherited.</summary>
/// <typeparam name="T">The type of field value handled by the record handler.</typeparam>
public sealed class TabularArrayHandler<T> : TabularHandler<T?[]>
{
    private readonly TabularConverter<T> _converter;

    /// <summary>Initializes a new instance of the <see cref="TabularArrayHandler{T}" /> class with the specified converter.</summary>
    /// <param name="converter">The converter to parse and format values with.</param>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    public TabularArrayHandler(TabularConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        _converter = converter;
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="reader" /> is <see langword="null" />.</exception>
    public override TabularRecord<T?[]> Read(TabularReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        using var builder = new ArrayBuilder<T?>(32);

        while (reader.TryReadField())
        {
            if (reader.CurrentFieldType != TabularFieldType.Value)
            {
                return default;
            }

            reader.TryGet(_converter, out var field);
            builder.Add(field);
        }

        return new(builder.Build());
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="reader" /> is <see langword="null" />.</exception>
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public override async ValueTask<TabularRecord<T?[]>> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(reader);

        using var builder = new ArrayBuilder<T?>(32);

        while (await reader.TryReadFieldAsync(cancellationToken).ConfigureAwait(false))
        {
            if (reader.CurrentFieldType != TabularFieldType.Value)
            {
                return default;
            }

            reader.TryGet(_converter, out var field);
            builder.Add(field);
        }

        return new(builder.Build());
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="writer" /> or <paramref name="record" /> is <see langword="null" />.</exception>
    public override void Write(TabularWriter writer, T?[] record)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(record);

        for (var i = 0; i < record.Length; i++)
        {
            writer.Write(record[i], _converter);
        }
    }

    /// <inheritdoc />
    /// <exception cref="ArgumentNullException"><paramref name="writer" /> or <paramref name="record" /> is <see langword="null" />.</exception>
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public override async ValueTask WriteAsync(TabularWriter writer, T?[] record, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(record);

        for (var i = 0; i < record.Length; i++)
        {
            await writer.WriteAsync(record[i], _converter, cancellationToken).ConfigureAwait(false);
        }
    }
}
