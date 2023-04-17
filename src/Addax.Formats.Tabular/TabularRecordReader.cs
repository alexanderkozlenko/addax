// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to a tabular data on the record level.</summary>
public sealed class TabularRecordReader : IAsyncDisposable
{
    private readonly TabularFieldReader _fieldReader;
    private readonly TabularRecordReaderContext _context;
    private readonly IReadOnlyDictionary<Type, TabularRecordConverter> _converters;

    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="TabularRecordReader" /> class using the specified stream, dialect, and options.</summary>
    /// <param name="stream">The stream to read tabular data from.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    /// <param name="options">The options to configure the reader.</param>
    public TabularRecordReader(Stream stream, TabularDataDialect dialect, TabularRecordReaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);
        ArgumentNullException.ThrowIfNull(options);

        _fieldReader = new(stream, dialect, options);
        _context = new(TabularConverterFactory.Instance, options.ConsumeComments);
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
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;

        return _fieldReader.DisposeAsync();
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

    /// <summary>Asynchronously tries to read a record from the stream.</summary>
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

        [StackTraceHidden]
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

    /// <summary>Asynchronously tries to read a record from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a record as a <see cref="TabularRecord{T}" /> instance.</returns>
    /// <exception cref="InvalidOperationException">There is no more records available in the stream.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask<TabularRecord<T>> ReadRecordAsync<T>(CancellationToken cancellationToken = default)
    {
        return ReadRecordAsync(SelectConverter<T>(), cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="predicate">An optional predicate to filter the sequence of records.</param>
    /// <param name="skip">The number of records to skip in the filtered sequence of records.</param>
    /// <param name="take">The number of records to take from the filtered sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> or <paramref name="take" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(TabularRecordConverter<T> converter, Predicate<TabularRecord<T>>? predicate, long skip, long take, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);

        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), skip, "The number of records to skip must be greater than or equal to zero.");
        }
        if (take < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(take), take, "The number of records to take must be greater than or equal to zero.");
        }

        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if ((take == 0) || !HasMoreRecords)
        {
            return EmptyAsyncEnumerable<TabularRecord<T>>.Instance;
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new TabularRecordEnumerable<T>(_fieldReader, _context, converter, predicate, skip, take, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="predicate">An optional predicate to filter the sequence of records.</param>
    /// <param name="skip">The number of records to skip in the filtered sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(TabularRecordConverter<T> converter, Predicate<TabularRecord<T>>? predicate, long skip, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);

        if (skip < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(skip), skip, "The number of records to skip must be greater than or equal to zero.");
        }

        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (!HasMoreRecords)
        {
            return EmptyAsyncEnumerable<TabularRecord<T>>.Instance;
        }

        cancellationToken.ThrowIfCancellationRequested();

        return new TabularRecordEnumerable<T>(_fieldReader, _context, converter, predicate, skip, take: -1, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="predicate">An optional predicate to filter the sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(TabularRecordConverter<T> converter, Predicate<TabularRecord<T>>? predicate, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(converter, predicate, skip: 0, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="skip">The number of records to skip in the sequence of records.</param>
    /// <param name="take">The number of records to take from the sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> or <paramref name="take" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(TabularRecordConverter<T> converter, long skip, long take, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(converter, predicate: null, skip, take, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="skip">The number of records to skip in the sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(TabularRecordConverter<T> converter, long skip, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(converter, predicate: null, skip, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular record to <typeparamref name="T" />.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(TabularRecordConverter<T> converter, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(converter, predicate: null, skip: 0, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="predicate">A predicate to filter the sequence of records.</param>
    /// <param name="skip">The number of records to skip in the filtered sequence of records.</param>
    /// <param name="take">The number of records to take from the filtered sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> or <paramref name="take" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(Predicate<TabularRecord<T>>? predicate, long skip, long take, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(SelectConverter<T>(), predicate, skip, take, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="predicate">A predicate to filter the sequence of records.</param>
    /// <param name="skip">The number of records to skip in the filtered sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(Predicate<TabularRecord<T>>? predicate, long skip, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(SelectConverter<T>(), predicate, skip, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="predicate">A predicate to filter the sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(Predicate<TabularRecord<T>>? predicate, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(SelectConverter<T>(), predicate, skip: 0, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="skip">The number of records to skip in the sequence of records.</param>
    /// <param name="take">The number of records to take from the sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> or <paramref name="take" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(long skip, long take, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(SelectConverter<T>(), predicate: null, skip, take, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="skip">The number of records to skip in the sequence of records.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="skip" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(long skip, CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(SelectConverter<T>(), predicate: null, skip, cancellationToken);
    }

    /// <summary>Asynchronously reads records from the stream.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular record.</typeparam>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An object that provides asynchronous iteration over a sequence of records constructed from tabular data.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public IAsyncEnumerable<TabularRecord<T>> ReadRecordsAsync<T>(CancellationToken cancellationToken = default)
    {
        return ReadRecordsAsync(SelectConverter<T>(), predicate: null, skip: 0, cancellationToken);
    }

    private TabularRecordConverter<T> SelectConverter<T>()
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularRecordConverter<T> converterT))
        {
            throw new InvalidOperationException($"A record converter for type '{typeof(T)}' is not registered.");
        }

        return converterT;
    }

    /// <summary>Gets the flag that indicates whether there is more records available in the stream.</summary>
    /// <value><see langword="true" /> if there is more records available in the stream; <see langword="false" /> otherwise.</value>
    public bool HasMoreRecords
    {
        get
        {
            return _fieldReader.PositionType is not TabularPositionType.EndOfStream;
        }
    }
}
