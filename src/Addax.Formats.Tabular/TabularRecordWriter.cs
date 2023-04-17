﻿// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, write-only access to a tabular data on the record level.</summary>
public sealed class TabularRecordWriter : IAsyncDisposable
{
    private readonly TabularFieldWriter _fieldWriter;
    private readonly TabularRecordWriterContext _context;
    private readonly IReadOnlyDictionary<Type, TabularRecordConverter> _converters;

    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="TabularRecordWriter" /> class using the specified stream, dialect, and options.</summary>
    /// <param name="stream">The stream to write tabular data to.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    /// <param name="options">The options to configure the writer.</param>
    public TabularRecordWriter(Stream stream, TabularDataDialect dialect, TabularRecordWriterOptions options)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);
        ArgumentNullException.ThrowIfNull(options);

        _fieldWriter = new(stream, dialect, options);
        _context = new(TabularConverterFactory.Instance);
        _converters = options.RecordConverters;
    }

    /// <summary>Initializes a new instance of the <see cref="TabularRecordWriter" /> class using the specified stream, dialect, and default options.</summary>
    /// <param name="stream">The stream to write tabular data to.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    public TabularRecordWriter(Stream stream, TabularDataDialect dialect)
        : this(stream, dialect, new())
    {
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;

        return _fieldWriter.DisposeAsync();
    }

    /// <summary>Asynchronously writes the record to the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="record">The record to be written.</param>
    /// <param name="converter">The converter to use for converting <typeparamref name="T" /> to a tabular record.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask WriteRecordAsync<T>(T record, TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteRecordAsyncCore(record, converter, cancellationToken);

        [StackTraceHidden]
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask WriteRecordAsyncCore(T record, TabularRecordConverter<T> converter, CancellationToken cancellationToken)
        {
            _fieldWriter.BeginRecord();

            await converter.WriteRecordAsync(_fieldWriter, record, _context, cancellationToken).ConfigureAwait(false);
            await _fieldWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>Asynchronously writes the record to the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="record">The record to be written.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask WriteRecordAsync<T>(T record, CancellationToken cancellationToken = default)
    {
        return WriteRecordAsync(record, SelectConverter<T>(), cancellationToken);
    }

    /// <summary>Asynchronously writes the record enumeration to the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="records">The record enumeration to be written.</param>
    /// <param name="converter">The converter to use for converting <typeparamref name="T" /> to a tabular record.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask WriteRecordsAsync<T>(IEnumerable<T> records, TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (records.TryGetNonEnumeratedCount(out var count) && (count == 0))
        {
            return ValueTask.CompletedTask;
        }

        return WriteRecordsAsyncCore(records, converter, cancellationToken);

        [StackTraceHidden]
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask WriteRecordsAsyncCore(IEnumerable<T> records, TabularRecordConverter<T> converter, CancellationToken cancellationToken)
        {
            foreach (var record in records)
            {
                if (record is null)
                {
                    throw new ArgumentException("The enumeration contains a null reference.", nameof(records));
                }

                _fieldWriter.BeginRecord();

                await converter.WriteRecordAsync(_fieldWriter, record, _context, cancellationToken).ConfigureAwait(false);
                await _fieldWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>Asynchronously writes the record enumeration to the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="records">The record enumeration to be written.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask WriteRecordsAsync<T>(IEnumerable<T> records, CancellationToken cancellationToken = default)
    {
        return WriteRecordsAsync(records, SelectConverter<T>(), cancellationToken);
    }

    /// <summary>Asynchronously writes the records to the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="records">The record enumeration to be written.</param>
    /// <param name="converter">The converter to use for converting <typeparamref name="T" /> to a tabular record.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask WriteRecordsAsync<T>(IAsyncEnumerable<T> records, TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(records);
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteRecordsAsyncCore(records, converter, cancellationToken);

        [StackTraceHidden]
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
        async ValueTask WriteRecordsAsyncCore(IAsyncEnumerable<T> records, TabularRecordConverter<T> converter, CancellationToken cancellationToken)
        {
            await foreach (var record in records.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                if (record is null)
                {
                    throw new ArgumentException("The enumeration contains a null reference.", nameof(records));
                }

                _fieldWriter.BeginRecord();

                await converter.WriteRecordAsync(_fieldWriter, record, _context, cancellationToken).ConfigureAwait(false);
                await _fieldWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>Asynchronously writes the records to the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="records">The record enumeration to be written.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask WriteRecordsAsync<T>(IAsyncEnumerable<T> records, CancellationToken cancellationToken = default)
    {
        return WriteRecordsAsync(records, SelectConverter<T>(), cancellationToken);
    }

    private TabularRecordConverter<T> SelectConverter<T>()
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularRecordConverter<T> converterT))
        {
            throw new InvalidOperationException($"A record converter for type '{typeof(T)}' is not registered.");
        }

        return converterT;
    }
}
