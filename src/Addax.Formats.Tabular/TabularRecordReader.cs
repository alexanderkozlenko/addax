// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to a tabular data on the record level.</summary>
public sealed class TabularRecordReader : IDisposable, IAsyncDisposable
{
    private readonly TabularFieldReader _fieldReader;
    private readonly TabularReaderContext _context;
    private readonly IReadOnlyDictionary<Type, TabularRecordConverter> _converters;

    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="TabularRecordReader" /> class using the specified stream, dialect, and options.</summary>
    /// <param name="stream">The stream to read tabular data from.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    /// <param name="options">The options to configure the reader.</param>
    public TabularRecordReader(Stream stream, TabularDataDialect dialect, TabularReaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);
        ArgumentNullException.ThrowIfNull(options);

        _fieldReader = new(stream, dialect, options);
        _context = new(options.ConverterFactory, options.ConsumeComments);
        _converters = options.RecordConverters;
    }

    /// <summary>Initializes a new instance of the <see cref="TabularRecordReader" /> class using the specified stream, dialect, and default options.</summary>
    /// <param name="stream">The stream to read tabular data from.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    public TabularRecordReader(Stream stream, TabularDataDialect dialect)
        : this(stream, dialect, new())
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _isDisposed = true;
        _fieldReader.Dispose();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;

        return _fieldReader.DisposeAsync();
    }

    /// <summary>Advances the reader to the next record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true" /> if the reader was successfully advanced, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public bool SkipRecord(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _fieldReader.MoveNextRecord(cancellationToken);
    }

    /// <summary>Asynchronously advances the reader to the next record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a result of <see langword="true" /> if the reader was successfully advanced, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask<bool> SkipRecordAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _fieldReader.MoveNextRecordAsync(cancellationToken);
    }

    /// <summary>Reads a record from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A record as a <see cref="TabularRecord{T}" /> instance.</returns>
    /// <exception cref="InvalidOperationException">There is no more records available in the stream.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public TabularRecord<T> ReadRecord<T>(TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (!_fieldReader.MoveNextRecord(cancellationToken))
        {
            throw new InvalidOperationException("There is no more records available in the stream.");
        }

        return converter.ReadRecord(_fieldReader, _context, cancellationToken);
    }

    /// <summary>Asynchronously reads a record from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a record as a <see cref="TabularRecord{T}" /> instance.</returns>
    /// <exception cref="InvalidOperationException">There is no more records available in the stream.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask<TabularRecord<T>> ReadRecordAsync<T>(TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return ReadRecordAsyncCore(converter, cancellationToken);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<TabularRecord<T>> ReadRecordAsyncCore(TabularRecordConverter<T> converter, CancellationToken cancellationToken)
        {
            if (!await _fieldReader.MoveNextRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                throw new InvalidOperationException("There is no more records available in the stream.");
            }

            return await converter.ReadRecordAsync(_fieldReader, _context, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>Reads a record from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A record as a <see cref="TabularRecord{T}" /> instance.</returns>
    /// <exception cref="InvalidOperationException">There is no more records available in the stream.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public TabularRecord<T> ReadRecord<T>(CancellationToken cancellationToken = default)
    {
        return ReadRecord(SelectConverter<T>(), cancellationToken);
    }

    /// <summary>Asynchronously reads a record from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a record as a <see cref="TabularRecord{T}" /> instance.</returns>
    /// <exception cref="InvalidOperationException">There is no more records available in the stream.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask<TabularRecord<T>> ReadRecordAsync<T>(CancellationToken cancellationToken = default)
    {
        return ReadRecordAsync(SelectConverter<T>(), cancellationToken);
    }

    /// <summary>Reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IEnumerable<TabularRecord<T>> ReadRecords<T>(TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_fieldReader.PositionType is TabularPositionType.EndOfStream)
        {
            return EmptyEnumerable<TabularRecord<T>>.Instance;
        }

        return new TabularRecordEnumerable<T>(_fieldReader, _context, converter, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_fieldReader.PositionType is TabularPositionType.EndOfStream)
        {
            return EmptyAsyncEnumerable<TabularRecord<T>>.Instance;
        }

        return new TabularRecordAsyncEnumerable<T>(_fieldReader, _context, converter, cancellationToken);
    }

    /// <summary>Reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IEnumerable<TabularRecord<T>> ReadRecords<T>(CancellationToken cancellationToken = default)
    {
        return ReadRecords(SelectConverter<T>(), cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(SelectConverter<T>(), cancellationToken);
    }

    private TabularRecordConverter<T> SelectConverter<T>()
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularRecordConverter<T> converterT))
        {
            ThrowInvalidOperationException();
        }

        return converterT;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException($"A record converter for type '{typeof(T)}' is not registered.");
        }
    }

    /// <summary>Gets the current type of position in tabular data.</summary>
    /// <value>A <see cref="TabularPositionType" /> value.</value>
    public TabularPositionType PositionType
    {
        get
        {
            return _fieldReader.PositionType;
        }
    }

    /// <summary>Gets the total number of characters consumed so far by the reader.</summary>
    /// <value>A non-negative zero-based number.</value>
    public long Position
    {
        get
        {
            return _fieldReader.Position;
        }
    }
}
