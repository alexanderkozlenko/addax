// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular;

/// <summary>Provides static methods for reading and writing collections of tabular data records.</summary>
public static class TabularData
{
    /// <summary>Reads all records that can be represented as <typeparamref name="T" /> from the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <param name="handler">The handler to read a <typeparamref name="T" /> instance from a record.</param>
    /// <returns>An array containing all records in the stream.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static T[] ReadRecords<T>(Stream stream, TabularDialect dialect, TabularOptions? options, TabularHandler<T>? handler)
        where T : notnull
    {
        using (var reader = new TabularReader<T>(stream, dialect, options, handler))
        {
            using var builder = new ArrayBuilder<T>(32);

            while (reader.TryReadRecord())
            {
                builder.Add(reader.CurrentRecord);
            }

            return builder.ToArray();
        }
    }

    /// <summary>Reads all records that can be represented as <typeparamref name="T" /> from the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <returns>An array containing all records in the stream.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static T[] ReadRecords<T>(Stream stream, TabularDialect dialect, TabularOptions? options)
        where T : notnull
    {
        return ReadRecords<T>(stream, dialect, options, null);
    }

    /// <summary>Reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <returns>An array containing all records in the stream.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static T[] ReadRecords<T>(Stream stream, TabularDialect dialect)
        where T : notnull
    {
        return ReadRecords<T>(stream, dialect, null, null);
    }

    /// <summary>Asynchronously reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <param name="handler">The handler to read a <typeparamref name="T" /> instance from a record.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces an array containing all records in the stream.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static async ValueTask<T[]> ReadRecordsAsync<T>(Stream stream, TabularDialect dialect, TabularOptions? options, TabularHandler<T>? handler, CancellationToken cancellationToken = default)
        where T : notnull
    {
        var reader = new TabularReader<T>(stream, dialect, options, handler);

        await using (reader.ConfigureAwait(false))
        {
            using var builder = new ArrayBuilder<T>(32);

            while (await reader.TryReadRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                builder.Add(reader.CurrentRecord);
            }

            return builder.ToArray();
        }
    }

    /// <summary>Asynchronously reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces an array containing all records in the stream.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static ValueTask<T[]> ReadRecordsAsync<T>(Stream stream, TabularDialect dialect, TabularOptions? options, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return ReadRecordsAsync<T>(stream, dialect, options, null, cancellationToken);
    }

    /// <summary>Asynchronously reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces an array containing all records in the stream.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static ValueTask<T[]> ReadRecordsAsync<T>(Stream stream, TabularDialect dialect, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return ReadRecordsAsync<T>(stream, dialect, null, null, cancellationToken);
    }

    /// <summary>Writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <param name="handler">The handler to write a <typeparamref name="T" /> instance to a record.</param>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public static void WriteRecords<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options, TabularHandler<T>? handler)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(records);

        using (var writer = new TabularWriter<T>(stream, dialect, options, handler))
        {
            foreach (var record in records)
            {
                if (record is not null)
                {
                    writer.WriteRecord(record);
                }
            }
        }
    }

    /// <summary>Writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public static void WriteRecords<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options)
        where T : notnull
    {
        WriteRecords(stream, dialect, records, options, null);
    }

    /// <summary>Writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public static void WriteRecords<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records)
        where T : notnull
    {
        WriteRecords(stream, dialect, records, null, null);
    }

    /// <summary>Asynchronously writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <param name="handler">The handler to write a <typeparamref name="T" /> instance to a record.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public static async ValueTask WriteRecordsAsync<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options, TabularHandler<T>? handler, CancellationToken cancellationToken = default)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(records);

        var writer = new TabularWriter<T>(stream, dialect, options, handler);

        await using (writer.ConfigureAwait(false))
        {
            foreach (var record in records)
            {
                if (record is not null)
                {
                    await writer.WriteRecordAsync(record, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>Asynchronously writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public static ValueTask WriteRecordsAsync<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return WriteRecordsAsync(stream, dialect, records, options, null, cancellationToken);
    }

    /// <summary>Asynchronously writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public static ValueTask WriteRecordsAsync<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return WriteRecordsAsync(stream, dialect, records, null, null, cancellationToken);
    }
}
