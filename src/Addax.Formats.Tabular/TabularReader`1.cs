// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to tabular data records. This class cannot be inherited.</summary>
/// <typeparam name="T">The type of an object that represents a record.</typeparam>
public sealed class TabularReader<T> : IDisposable, IAsyncDisposable
    where T : notnull
{
    private readonly TabularReader _fieldReader;
    private readonly TabularHandler<T> _recordHandler;

    private TabularRecord<T> _currentRecord;

    /// <summary>Initializes a new instance of the <see cref="TabularReader{T}" /> class for the specified stream using the provided dialect, options, and record handler.</summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <param name="handler">The handler to read a <typeparamref name="T" /> instance from a record.</param>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public TabularReader(Stream stream, TabularDialect dialect, TabularOptions? options = null, TabularHandler<T>? handler = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);

        _fieldReader = new(stream, dialect, options);
        _recordHandler = handler ?? SelectHandler();
    }

    /// <summary>Releases the resources used by the current instance of the <see cref="TabularReader{T}" /> class.</summary>
    public void Dispose()
    {
        _currentRecord = default;
        _fieldReader.Dispose();
    }

    /// <summary>Asynchronously releases the resources used by the current instance of the <see cref="TabularReader{T}" /> class.</summary>
    /// <returns>A task object.</returns>
    public ValueTask DisposeAsync()
    {
        _currentRecord = default;

        return _fieldReader.DisposeAsync();
    }

    /// <summary>Tries to advance the reader to the next record.</summary>
    /// <returns><see langword="true" /> if the reader was successfully advanced; otherwise, <see langword="false" />.</returns>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public bool TrySkipRecord()
    {
        if (_fieldReader.TryPickRecord())
        {
            _currentRecord = default;

            while (_fieldReader.TrySkipField())
            {
            }

            return true;
        }

        return false;
    }

    /// <summary>Asynchronously tries to advance the reader to the next record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces <see langword="true" /> if the reader was successfully advanced; otherwise, <see langword="false" />.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public async ValueTask<bool> TrySkipRecordAsync(CancellationToken cancellationToken = default)
    {
        if (_fieldReader.TryPickRecord())
        {
            _currentRecord = default;

            while (await _fieldReader.TrySkipFieldAsync(cancellationToken).ConfigureAwait(false))
            {
            }

            return true;
        }

        return false;
    }

    /// <summary>Tries to read the next record that can be represented as <typeparamref name="T" />.</summary>
    /// <returns><see langword="true" /> if the record was successfully read; otherwise, <see langword="false" />.</returns>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    /// <remarks>If the record handler defines a header, the first record will be automatically skipped.</remarks>
    public bool TryReadRecord()
    {
        while (true)
        {
            if (_fieldReader.CurrentPositionType == TabularPositionType.StartOfStream)
            {
                if (!TryReadHeader())
                {
                    return false;
                }
            }

            if (_fieldReader.TryPickRecord())
            {
                _currentRecord = default;
                _currentRecord = _recordHandler.Read(_fieldReader);

                if (_fieldReader.CurrentPositionType == TabularPositionType.Delimiter)
                {
                    while (_fieldReader.TrySkipField())
                    {
                    }
                }

                if (_currentRecord.HasValue)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>Asynchronously reads the next record that can be represented as <typeparamref name="T" />.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces <see langword="true" /> if the record was successfully read; otherwise, <see langword="false" />.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    /// <remarks>If the record handler defines a header, the first record will be automatically skipped.</remarks>
    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    public async ValueTask<bool> TryReadRecordAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (_fieldReader.CurrentPositionType == TabularPositionType.StartOfStream)
            {
                if (!await TryReadHeaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    return false;
                }
            }

            if (_fieldReader.TryPickRecord())
            {
                _currentRecord = default;
                _currentRecord = await _recordHandler.ReadAsync(_fieldReader, cancellationToken).ConfigureAwait(false);

                if (_fieldReader.CurrentPositionType == TabularPositionType.Delimiter)
                {
                    while (await _fieldReader.TrySkipFieldAsync(cancellationToken).ConfigureAwait(false))
                    {
                    }
                }

                if (_currentRecord.HasValue)
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }

    private bool TryReadHeader()
    {
        if (_recordHandler.Header is not null)
        {
            if (_fieldReader.TryPickRecord())
            {
                _currentRecord = default;

                while (_fieldReader.TrySkipField())
                {
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }

    private async ValueTask<bool> TryReadHeaderAsync(CancellationToken cancellationToken)
    {
        if (_recordHandler.Header is not null)
        {
            if (_fieldReader.TryPickRecord())
            {
                _currentRecord = default;

                while (await _fieldReader.TrySkipFieldAsync(cancellationToken).ConfigureAwait(false))
                {
                }
            }
            else
            {
                return false;
            }
        }

        return true;
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

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowRecordNotSetException()
    {
        throw new InvalidOperationException("The current record is not set.");
    }

    /// <summary>Gets the last read record that is represented as <typeparamref name="T" />.</summary>
    /// <value>A <typeparamref name="T" /> instance. An exception is thrown if the current record is not set.</value>
    /// <exception cref="InvalidOperationException">The current record is not set.</exception>
    public T CurrentRecord
    {
        get
        {
            if (!_currentRecord.HasValue)
            {
                ThrowRecordNotSetException();
            }

            return _currentRecord.Value!;
        }
    }

    /// <summary>Gets a reference to the last read record that is represented as <typeparamref name="T" />.</summary>
    /// <value>A read-only reference to a <typeparamref name="T" /> instance. An exception is thrown if the current record is not set.</value>
    /// <exception cref="InvalidOperationException">The current record is not set.</exception>
    public ref readonly T CurrentRecordRef
    {
        get
        {
            if (!_currentRecord.HasValue)
            {
                ThrowRecordNotSetException();
            }

            return ref _currentRecord.Value!;
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
