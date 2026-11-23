// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to tabular data records. This class cannot be inherited.</summary>
/// <typeparam name="T">The type of an object that represents a record.</typeparam>
public sealed class TabularReader<T> : IDisposable, IAsyncDisposable
    where T : notnull
{
    private readonly TabularReader _fieldReader;
    private readonly TabularHandler<T> _recordHandler;

    private T? _currentRecord;
    private bool _currentRecordHasValue;
    private TabularEngineState _engineState;

    /// <summary>Initializes a new instance of the <see cref="TabularReader{T}" /> class for the specified stream using the provided dialect and options.</summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading, or <see cref="TabularOptions.Converters" /> of <paramref name="options" /> contains a value that is <see langword="null" /> or is not a converter for its key, or the value for <typeparamref name="T" /> in <see cref="TabularOptions.Handlers" /> of <paramref name="options" /> is <see langword="null" /> or is not a record handler for <typeparamref name="T" />.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">Neither <see cref="TabularOptions.Handlers" /> of <paramref name="options" /> nor <see cref="TabularRegistry.Handlers" /> contains a record handler for <typeparamref name="T" />, or the registry entry for <typeparamref name="T" /> is not a record handler for that type.</exception>
    public TabularReader(Stream stream, TabularDialect dialect, TabularOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);

        _recordHandler = TabularRegistry.SelectHandler<T>(options);
        _fieldReader = new(stream, dialect, options);
    }

    /// <summary>Releases all resources used by the current instance of the <see cref="TabularReader{T}" /> class.</summary>
    public void Dispose()
    {
        if (_engineState == TabularEngineState.Disposed)
        {
            return;
        }

        _engineState = TabularEngineState.Disposed;

        ResetCurrentRecord();

        _fieldReader.Dispose();
    }

    /// <summary>Asynchronously releases all resources used by the current instance of the <see cref="TabularReader{T}" /> class.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        if (_engineState == TabularEngineState.Disposed)
        {
            return ValueTask.CompletedTask;
        }

        _engineState = TabularEngineState.Disposed;

        ResetCurrentRecord();

        return _fieldReader.DisposeAsync();
    }

    /// <summary>Tries to skip the next record.</summary>
    /// <returns><see langword="true" /> if the record was successfully skipped; <see langword="false" /> if there are no more records to skip.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length.</exception>
    /// <remarks>If the record handler defines a header, the first record is skipped as the header.</remarks>
    public bool TrySkipRecord()
    {
        ObjectDisposedException.ThrowIf(_engineState == TabularEngineState.Disposed, this);
        ResetCurrentRecord();

        if ((_fieldReader.CurrentPositionType == TabularPositionType.StartOfStream) && (_recordHandler.Header is not null) && !TrySkipNextRecord())
        {
            return false;
        }

        return TrySkipNextRecord();
    }

    /// <summary>Asynchronously tries to skip the next record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous skip operation. The value of its <see cref="ValueTask{TResult}.Result" /> property contains <see langword="true" /> if the record was successfully skipped; <see langword="false" /> if there are no more records to skip.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed. This exception is stored into the returned task.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <remarks>If the record handler defines a header, the first record is skipped as the header.</remarks>
    public async ValueTask<bool> TrySkipRecordAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_engineState == TabularEngineState.Disposed, this);
        ResetCurrentRecord();

        if ((_fieldReader.CurrentPositionType == TabularPositionType.StartOfStream) && (_recordHandler.Header is not null) && !await TrySkipNextRecordAsync(cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        return await TrySkipNextRecordAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Tries to read the next record that can be represented as <typeparamref name="T" />.</summary>
    /// <returns><see langword="true" /> if the record was successfully read; <see langword="false" /> if there are no more records to read.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    /// <exception cref="TabularException">A record does not match <typeparamref name="T" />, or a field exceeds the supported field length.</exception>
    /// <remarks>If the record handler defines a header, the first record is skipped as the header.</remarks>
    public bool TryReadRecord()
    {
        ObjectDisposedException.ThrowIf(_engineState == TabularEngineState.Disposed, this);
        ResetCurrentRecord();

        if ((_fieldReader.CurrentPositionType == TabularPositionType.StartOfStream) && (_recordHandler.Header is not null) && !TrySkipNextRecord())
        {
            return false;
        }

        while (_fieldReader.TryMoveToNextRecord())
        {
            if (_fieldReader.CurrentFieldType == TabularFieldType.Comment)
            {
                continue;
            }

            var record = _recordHandler.Read(_fieldReader);

            while (_fieldReader.TrySkipField())
            {
            }

            _currentRecord = record;
            _currentRecordHasValue = true;

            return true;
        }

        return false;
    }

    /// <summary>Asynchronously tries to read the next record that can be represented as <typeparamref name="T" />.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation. The value of its <see cref="ValueTask{TResult}.Result" /> property contains <see langword="true" /> if the record was successfully read; <see langword="false" /> if there are no more records to read.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed. This exception is stored into the returned task.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularException">A record does not match <typeparamref name="T" />, or a field exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <remarks>If the record handler defines a header, the first record is skipped as the header.</remarks>
    public async ValueTask<bool> TryReadRecordAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_engineState == TabularEngineState.Disposed, this);
        ResetCurrentRecord();

        if ((_fieldReader.CurrentPositionType == TabularPositionType.StartOfStream) && (_recordHandler.Header is not null) && !await TrySkipNextRecordAsync(cancellationToken).ConfigureAwait(false))
        {
            return false;
        }

        while (await _fieldReader.TryMoveToNextRecordAsync(cancellationToken).ConfigureAwait(false))
        {
            if (_fieldReader.CurrentFieldType == TabularFieldType.Comment)
            {
                continue;
            }

            var record = await _recordHandler.ReadAsync(_fieldReader, cancellationToken).ConfigureAwait(false);

            while (await _fieldReader.TrySkipFieldAsync(cancellationToken).ConfigureAwait(false))
            {
            }

            _currentRecord = record;
            _currentRecordHasValue = true;

            return true;
        }

        return false;
    }

    private bool TrySkipNextRecord()
    {
        while (_fieldReader.TryMoveToNextRecord())
        {
            if (_fieldReader.CurrentFieldType == TabularFieldType.Comment)
            {
                continue;
            }

            while (_fieldReader.TrySkipField())
            {
            }

            return true;
        }

        return false;
    }

    private async ValueTask<bool> TrySkipNextRecordAsync(CancellationToken cancellationToken)
    {
        while (await _fieldReader.TryMoveToNextRecordAsync(cancellationToken).ConfigureAwait(false))
        {
            if (_fieldReader.CurrentFieldType == TabularFieldType.Comment)
            {
                continue;
            }

            while (await _fieldReader.TrySkipFieldAsync(cancellationToken).ConfigureAwait(false))
            {
            }

            return true;
        }

        return false;
    }

    private void ResetCurrentRecord()
    {
        _currentRecord = default;
        _currentRecordHasValue = false;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowRecordNotSetException()
    {
        throw new InvalidOperationException("The current record is not set.");
    }

    /// <summary>Gets the current record that is represented as <typeparamref name="T" />.</summary>
    /// <value>A <typeparamref name="T" /> instance.</value>
    /// <exception cref="InvalidOperationException">The current record is not set.</exception>
    public T CurrentRecord
    {
        get
        {
            if (!_currentRecordHasValue)
            {
                ThrowRecordNotSetException();
            }

            return _currentRecord!;
        }
    }

    /// <summary>Gets a reference to the current record that is represented as <typeparamref name="T" />.</summary>
    /// <value>A read-only reference to a <typeparamref name="T" /> instance.</value>
    /// <exception cref="InvalidOperationException">The current record is not set.</exception>
    public ref readonly T CurrentRecordRef
    {
        get
        {
            if (!_currentRecordHasValue)
            {
                ThrowRecordNotSetException();
            }

            return ref _currentRecord!;
        }
    }

    /// <inheritdoc cref="TabularReader.BytesConsumed" />
    public long BytesConsumed
    {
        get
        {
            return _fieldReader.BytesConsumed;
        }
    }

    /// <inheritdoc cref="TabularReader.RecordsRead" />
    public long RecordsRead
    {
        get
        {
            return _fieldReader.RecordsRead;
        }
    }
}
