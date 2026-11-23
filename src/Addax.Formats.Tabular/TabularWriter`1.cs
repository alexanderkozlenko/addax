// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, write-only access to tabular data records. This class cannot be inherited.</summary>
/// <typeparam name="T">The type of an object that represents a record.</typeparam>
public sealed class TabularWriter<T> : IDisposable, IAsyncDisposable
    where T : notnull
{
    private readonly TabularWriter _fieldWriter;
    private readonly TabularHandler<T> _recordHandler;

    private TabularEngineState _engineState;

    /// <summary>Initializes a new instance of the <see cref="TabularWriter{T}" /> class for the specified stream using the provided dialect and options.</summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing, or <see cref="TabularOptions.Converters" /> of <paramref name="options" /> contains a value that is <see langword="null" /> or is not a converter for its key, or the value for <typeparamref name="T" /> in <see cref="TabularOptions.Handlers" /> of <paramref name="options" /> is <see langword="null" /> or is not a record handler for <typeparamref name="T" />.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">Neither <see cref="TabularOptions.Handlers" /> of <paramref name="options" /> nor <see cref="TabularRegistry.Handlers" /> contains a record handler for <typeparamref name="T" />, or the registry entry for <typeparamref name="T" /> is not a record handler for that type.</exception>
    public TabularWriter(Stream stream, TabularDialect dialect, TabularOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);

        _recordHandler = TabularRegistry.SelectHandler<T>(options);
        _fieldWriter = new(stream, dialect, options);
    }

    /// <summary>Releases all resources used by the current instance of the <see cref="TabularWriter{T}" /> class.</summary>
    /// <remarks>If the record handler defines a header and no records were written, the header is written, unless a previous write operation failed.</remarks>
    public void Dispose()
    {
        if (_engineState == TabularEngineState.Disposed)
        {
            return;
        }

        var engineState = _engineState;

        _engineState = TabularEngineState.Disposed;

        try
        {
            if ((engineState == TabularEngineState.Active) && (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream))
            {
                WriteHeader();
            }
        }
        finally
        {
            _fieldWriter.Dispose();
        }
    }

    /// <summary>Asynchronously releases all resources used by the current instance of the <see cref="TabularWriter{T}" /> class.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    /// <remarks>If the record handler defines a header and no records were written, the header is written, unless a previous write operation failed.</remarks>
    public async ValueTask DisposeAsync()
    {
        if (_engineState == TabularEngineState.Disposed)
        {
            return;
        }

        var engineState = _engineState;

        _engineState = TabularEngineState.Disposed;

        try
        {
            if ((engineState == TabularEngineState.Active) && (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream))
            {
                await WriteHeaderAsync(default).ConfigureAwait(false);
            }
        }
        finally
        {
            await _fieldWriter.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Writes the next record represented as <typeparamref name="T" />.</summary>
    /// <param name="record">The record to write.</param>
    /// <exception cref="ArgumentNullException"><paramref name="record" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>If the record handler defines a header, the header is written before the first record.</remarks>
    public void WriteRecord(T record)
    {
        ArgumentNullException.ThrowIfNull(record);
        ThrowIfNotActive();

        try
        {
            if (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream)
            {
                WriteHeader();
            }

            _recordHandler.Write(_fieldWriter, record);
            _fieldWriter.WriteEndOfRecord();
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
    }

    /// <summary>Asynchronously writes the next record represented as <typeparamref name="T" />.</summary>
    /// <param name="record">The record to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="record" /> is <see langword="null" />. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed. This exception is stored into the returned task.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>If the record handler defines a header, the header is written before the first record.</remarks>
    public async ValueTask WriteRecordAsync(T record, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(record);
        ThrowIfNotActive();

        try
        {
            if (_fieldWriter.CurrentPositionType == TabularPositionType.StartOfStream)
            {
                await WriteHeaderAsync(cancellationToken).ConfigureAwait(false);
            }

            await _recordHandler.WriteAsync(_fieldWriter, record, cancellationToken).ConfigureAwait(false);
            await _fieldWriter.WriteEndOfRecordAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
    }

    /// <summary>Writes any buffered data to the underlying stream and flushes the stream.</summary>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>Buffered data is also written to the underlying stream when the buffer fills, without flushing the stream.</remarks>
    public void Flush()
    {
        ThrowIfNotActive();

        try
        {
            _fieldWriter.Flush();
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
    }

    /// <summary>Asynchronously writes any buffered data to the underlying stream and flushes the stream.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>Buffered data is also written to the underlying stream when the buffer fills, without flushing the stream.</remarks>
    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return FlushCoreAsync(cancellationToken);

        async ValueTask FlushCoreAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _fieldWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                _engineState = TabularEngineState.Faulted;

                throw;
            }
        }
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

            _fieldWriter.WriteEndOfRecord();
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

            await _fieldWriter.WriteEndOfRecordAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private void ThrowIfNotActive()
    {
        if (_engineState != TabularEngineState.Active)
        {
            ObjectDisposedException.ThrowIf(_engineState == TabularEngineState.Disposed, this);
            ThrowFaultedException();
        }
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowFaultedException()
    {
        throw new InvalidOperationException("The writer cannot be used after a failed write operation.");
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
