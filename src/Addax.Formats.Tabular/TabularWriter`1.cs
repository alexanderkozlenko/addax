// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, write-only access to tabular data records. This class cannot be inherited.</summary>
/// <typeparam name="T">The type of an object that represents a record.</typeparam>
public sealed class TabularWriter<T> : IDisposable, IAsyncDisposable
    where T : notnull
{
    private readonly TabularWriter _fieldWriter;
    private readonly TabularHandler<T> _recordHandler;

    /// <summary>Initializes a new instance of the <see cref="TabularWriter{T}" /> class for the specified stream using the provided dialect, options, and record handler.</summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <param name="handler">The handler to write a <typeparamref name="T" /> instance to a record.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public TabularWriter(Stream stream, TabularDialect dialect, TabularOptions? options = null, TabularHandler<T>? handler = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);

        _fieldWriter = new(stream, dialect, options);
        _recordHandler = handler ?? SelectHandler();
    }

    /// <summary>Releases the resources used by the current instance of the <see cref="TabularWriter{T}" /> class.</summary>
    /// <remarks>If the record handler defines a header, it will be automatically written.</remarks>
    public void Dispose()
    {
        if (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream)
        {
            WriteHeader();
        }

        _fieldWriter.Dispose();
    }

    /// <summary>Asynchronously releases the resources used by the current instance of the <see cref="TabularWriter{T}" /> class.</summary>
    /// <returns>A task object.</returns>
    /// <remarks>If the record handler defines a header, it will be automatically written.</remarks>
    public async ValueTask DisposeAsync()
    {
        if (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream)
        {
            await WriteHeaderAsync(default).ConfigureAwait(false);
        }

        await _fieldWriter.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>Writes the next record represented as <typeparamref name="T" />.</summary>
    /// <param name="record">The record to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="record" /> is <see langword="null" />.</exception>
    /// <remarks>If the record handler defines a header, it will be automatically written.</remarks>
    public void WriteRecord(in T record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream)
        {
            WriteHeader();
        }

        _recordHandler.Write(_fieldWriter, record);
        _fieldWriter.FinishRecord();
    }

    /// <summary>Asynchronously writes the next record represented as <typeparamref name="T" />.</summary>
    /// <param name="record">The record to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="record" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>If the record handler defines a header, it will be automatically written.</remarks>
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    public async ValueTask WriteRecordAsync(T record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream)
        {
            await WriteHeaderAsync(cancellationToken).ConfigureAwait(false);
        }

        await _recordHandler.WriteAsync(_fieldWriter, record, cancellationToken).ConfigureAwait(false);

        _fieldWriter.FinishRecord();
    }

    /// <inheritdoc cref="TabularWriter.Flush()" />
    public void Flush()
    {
        _fieldWriter.Flush();
    }

    /// <inheritdoc cref="TabularWriter.FlushAsync(CancellationToken)" />
    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        return _fieldWriter.FlushAsync(cancellationToken);
    }

    private void WriteHeader()
    {
        var header = _recordHandler.Header;

        if (header is not null)
        {
            foreach (var name in header)
            {
                _fieldWriter.WriteString(name);
            }

            _fieldWriter.FinishRecord();
        }
    }

    private async ValueTask WriteHeaderAsync(CancellationToken cancellationToken)
    {
        var header = _recordHandler.Header;

        if (header is not null)
        {
            foreach (var name in header)
            {
                await _fieldWriter.WriteStringAsync(name, cancellationToken).ConfigureAwait(false);
            }

            _fieldWriter.FinishRecord();
        }
    }

    private static TabularHandler<T> SelectHandler()
    {
        if (!TabularRegistry.Handlers.TryGetValue(typeof(T), out var handler) || (handler is not TabularHandler<T> handlerT))
        {
            ThrowHandlerNotFoundException();
        }

        return handlerT;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowHandlerNotFoundException()
        {
            throw new InvalidOperationException($"A record handler for type '{typeof(T)}' cannot be found in the registry.");
        }
    }

    /// <inheritdoc cref="TabularWriter.BytesCommitted" />
    public long BytesCommitted
    {
        get
        {
            return _fieldWriter.BytesCommitted;
        }
    }

    /// <inheritdoc cref="TabularWriter.RecordsWritten" />
    public long RecordsWritten
    {
        get
        {
            return _fieldWriter.RecordsWritten;
        }
    }
}
