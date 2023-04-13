// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to a tabular data on the field level.</summary>
public sealed partial class TabularFieldReader : IAsyncDisposable
{
    private readonly TabularStreamReader _streamReader;
    private readonly TabularStreamParser _streamParser;
    private readonly SequenceSource<char> _bufferSource = new(minimumSegmentSize: 64);

    private ReadOnlySequence<char> _value;
    private long _position;
    private TabularPositionType _positionType;
    private TabularFieldType _fieldType;
    private BufferKind _bufferKind;
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="TabularFieldReader" /> class using the specified stream, dialect, and options.</summary>
    /// <param name="stream">The stream to read tabular data from.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    /// <param name="options">The options to configure the reader.</param>
    public TabularFieldReader(Stream stream, TabularDataDialect dialect, TabularFieldReaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);
        ArgumentNullException.ThrowIfNull(options);

        _streamReader = new(stream, options.Encoding, options.BufferSize, options.LeaveOpen);
        _streamParser = new(dialect);
    }

    /// <summary>Initializes a new instance of the <see cref="TabularFieldReader" /> class using the specified stream, dialect, and default options.</summary>
    /// <param name="stream">The stream to read tabular data from.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    public TabularFieldReader(Stream stream, TabularDataDialect dialect)
        : this(stream, dialect, new())
    {
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;
        _value = ReadOnlySequence<char>.Empty;
        _bufferSource.Dispose();
        _bufferKind = BufferKind.None;
        _positionType = TabularPositionType.EndOfStream;
        _fieldType = TabularFieldType.None;
        _position = 0;

        return _streamReader.DisposeAsync();
    }

    /// <summary>Asynchronously advances the reader to the next record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a result of <see langword="true" /> if the reader was successfully advanced, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask<bool> MoveNextRecordAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _positionType switch
        {
            TabularPositionType.BeginningOfStream => AssumeNextRecordAsync(),
            TabularPositionType.BeginningOfRecord => MoveNextRecordAsyncCore(cancellationToken),
            TabularPositionType.FieldSeparator => MoveNextRecordAsyncCore(cancellationToken),
            TabularPositionType.EndOfRecord => AssumeNextRecordAsync(),
            _ => new(false),
        };

        [StackTraceHidden]
        ValueTask<bool> AssumeNextRecordAsync()
        {
            ReleaseValue();

            _positionType = TabularPositionType.BeginningOfRecord;

            return new(true);
        }

        [StackTraceHidden]
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<bool> MoveNextRecordAsyncCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            while (_positionType is not (TabularPositionType.EndOfRecord or TabularPositionType.EndOfStream))
            {
                await AdvanceNextTokenAsync(consume: false, cancellationToken).ConfigureAwait(false);
            }

            if (_positionType is TabularPositionType.EndOfRecord)
            {
                _positionType = TabularPositionType.BeginningOfRecord;
            }

            return _positionType is TabularPositionType.BeginningOfRecord;
        }
    }

    /// <summary>Asynchronously advances the reader to the next field in the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a result of <see langword="true" /> if the reader was successfully advanced, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask<bool> MoveNextFieldAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _positionType switch
        {
            TabularPositionType.BeginningOfRecord => MoveNextFieldAsyncCore(cancellationToken),
            TabularPositionType.FieldSeparator => MoveNextFieldAsyncCore(cancellationToken),
            _ => new(false),
        };

        [StackTraceHidden]
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<bool> MoveNextFieldAsyncCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            await AdvanceNextTokenAsync(consume: false, cancellationToken).ConfigureAwait(false);

            return _positionType is TabularPositionType.FieldSeparator;
        }
    }

    /// <summary>Asynchronously reads the next field in the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a result of <see langword="true" /> if the value was successfully read, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public ValueTask<bool> ReadFieldAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _positionType switch
        {
            TabularPositionType.BeginningOfRecord => ReadFieldAsyncCore(cancellationToken),
            TabularPositionType.FieldSeparator => ReadFieldAsyncCore(cancellationToken),
            _ => new(false),
        };

        [StackTraceHidden]
        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<bool> ReadFieldAsyncCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            await AdvanceNextTokenAsync(consume: true, cancellationToken).ConfigureAwait(false);

            return true;
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask AdvanceNextTokenAsync(bool consume, CancellationToken cancellationToken)
    {
        var examinedLength = 0L;

        var parserState = new TabularStreamParserState
        {
            IsBeginningOfLine = _positionType is TabularPositionType.BeginningOfRecord,
        };

        while (_positionType is not TabularPositionType.EndOfStream)
        {
            await _streamReader.ReadAsync(cancellationToken).ConfigureAwait(false);

            var readingBuffer = _streamReader.Buffer;
            var parsingBuffer = readingBuffer.Slice(_streamReader.Examined);
            var parsingStatus = _streamParser.Parse(parsingBuffer, ref parserState, out var parsedLength);

            examinedLength += parsedLength;

            if (parsingStatus == TabularStreamParsingStatus.NeedMoreData)
            {
                if (!_streamReader.IsCompleted)
                {
                    _streamReader.Advance(0, examinedLength);

                    continue;
                }
                if (parserState.IsIncomplete)
                {
                    var position = _position + examinedLength - 1;

                    throw new TabularDataException($"The reader encountered an unexpected end of stream at position {position}.", position);
                }
            }
            else if (parsingStatus == TabularStreamParsingStatus.FoundInvalidData)
            {
                var position = _position + examinedLength - 1;

                throw new TabularDataException($"The reader encountered an unexpected character at position {position}.", position);
            }

            _fieldType = !parserState.IsCommentPrefixFound ? TabularFieldType.Content : TabularFieldType.Comment;

            if (consume)
            {
                var valueBuffer = readingBuffer.Slice(0, examinedLength);

                _value = _streamParser.Extract(valueBuffer, parsingStatus, parserState, _bufferSource, out _bufferKind);
            }

            if (_bufferKind == BufferKind.Shared)
            {
                _streamReader.Advance(0, examinedLength);
            }
            else
            {
                _streamReader.Advance(examinedLength, examinedLength);
            }

            _positionType = parsingStatus switch
            {
                TabularStreamParsingStatus.FoundFieldSeparation => TabularPositionType.FieldSeparator,
                TabularStreamParsingStatus.FoundRecordSeparation => TabularPositionType.EndOfRecord,
                _ => TabularPositionType.EndOfStream,
            };

            _position += examinedLength;

            return;
        }
    }

    private bool TryGet<T>(TabularFieldConverter<T> converter, out T result)
        where T : struct
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_bufferKind == BufferKind.None)
        {
            throw new InvalidOperationException("The current reader value can only be accessed following a read operation.");
        }

        var value = _value;
        var valueLength = value.Length;
        var valueLengthLimit = converter.GetParseBufferLength();

        if (valueLengthLimit < 0)
        {
            throw new FormatException($"The current reader value cannot be represented as '{typeof(T)}'.");
        }

        if (valueLength > valueLengthLimit)
        {
            result = default;

            return false;
        }

        if (value.IsSingleSegment || (value.FirstSpan.Length == valueLength))
        {
            return converter.TryParse(value.FirstSpan, TabularDataInfo.DefaultFormatProvider, out result);
        }
        else
        {
            using var parsingBuffer = valueLength <= TabularDataInfo.StackBufferLength ?
                HybridBuffer<char>.Create(stackalloc char[(int)valueLength]) :
                HybridBuffer<char>.Create((int)valueLength);

            value.CopyTo(parsingBuffer.AsSpan());

            return converter.TryParse(parsingBuffer.AsSpan(), TabularDataInfo.DefaultFormatProvider, out result);
        }
    }

    /// <summary>Tries to retrieve the current reader value as <typeparamref name="T" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="func">A function that tries to parse a sequence of characters into a value.</param>
    /// <param name="provider">An optional object that provides culture-specific formatting information.</param>
    /// <param name="result">When this method returns, contains the <typeparamref name="T" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    public bool TryGet<T>(TabularTryParseFunc<T> func, IFormatProvider? provider, out T? result)
    {
        ArgumentNullException.ThrowIfNull(func);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_bufferKind == BufferKind.None)
        {
            throw new InvalidOperationException("The current reader value can only be accessed following a read operation.");
        }

        provider ??= TabularDataInfo.DefaultFormatProvider;

        var value = _value;
        var valueLength = value.Length;

        if (valueLength > Array.MaxLength)
        {
            result = default;

            return false;
        }

        if (value.IsSingleSegment || (value.FirstSpan.Length == valueLength))
        {
            return func.Invoke(value.FirstSpan, provider, out result);
        }
        else
        {
            using var parsingBuffer = valueLength <= TabularDataInfo.StackBufferLength ?
                HybridBuffer<char>.Create(stackalloc char[(int)valueLength]) :
                HybridBuffer<char>.Create((int)valueLength);

            value.CopyTo(parsingBuffer.AsSpan());

            return func.Invoke(parsingBuffer.AsSpan(), provider, out result);
        }
    }

    /// <summary>Tries to retrieve the current reader value as <typeparamref name="T" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="func">A function that tries to parse a sequence of characters into a value.</param>
    /// <param name="result">When this method returns, contains the <typeparamref name="T" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    public bool TryGet<T>(TabularTryParseFunc<T> func, out T? result)
    {
        return TryGet(func, TabularDataInfo.DefaultFormatProvider, out result);
    }

    private T Get<T>(TabularFieldConverter<T> converter)
        where T : struct
    {
        if (!TryGet(converter, out var result))
        {
            ThrowFormatException();
        }

        return result;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowFormatException()
        {
            throw new FormatException($"The current reader value cannot be represented as '{typeof(T)}'.");
        }
    }

    /// <summary>Retrieves the current reader value as <typeparamref name="T" />.</summary>
    /// <param name="func">A function that parses a sequence of characters into a value.</param>
    /// <param name="provider">An optional object that provides culture-specific formatting information.</param>
    /// <returns>The <typeparamref name="T" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <typeparamref name="T" />.</exception>
    public T? Get<T>(TabularParseFunc<T> func, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(func);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_bufferKind == BufferKind.None)
        {
            throw new InvalidOperationException("The current reader value can only be accessed following a read operation.");
        }

        provider ??= TabularDataInfo.DefaultFormatProvider;

        var value = _value;
        var valueLength = value.Length;

        if (valueLength > Array.MaxLength)
        {
            throw new FormatException($"The current reader value cannot be represented as '{typeof(T)}'.");
        }

        if (value.IsSingleSegment || (value.FirstSpan.Length == valueLength))
        {
            return func.Invoke(value.FirstSpan, provider);
        }
        else
        {
            using var parsingBuffer = valueLength <= TabularDataInfo.StackBufferLength ?
                HybridBuffer<char>.Create(stackalloc char[(int)valueLength]) :
                HybridBuffer<char>.Create((int)valueLength);

            value.CopyTo(parsingBuffer.AsSpan());

            return func.Invoke(parsingBuffer.AsSpan(), provider);
        }
    }

    /// <summary>Retrieves the current reader value as <typeparamref name="T" />.</summary>
    /// <param name="func">A function that parses a sequence of characters into a value.</param>
    /// <returns>The <typeparamref name="T" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <typeparamref name="T" />.</exception>
    public T? Get<T>(TabularParseFunc<T> func)
    {
        return Get(func, TabularDataInfo.DefaultFormatProvider);
    }

    private void ReleaseValue()
    {
        _fieldType = TabularFieldType.None;
        _value = ReadOnlySequence<char>.Empty;

        switch (_bufferKind)
        {
            case BufferKind.Shared:
                {
                    _streamReader.Advance(_streamReader.Examined, _streamReader.Examined);
                }
                break;
            case BufferKind.Private:
                {
                    _bufferSource.Dispose();
                }
                break;
        }

        _bufferKind = BufferKind.None;
    }

    /// <summary>Gets a type of the last processed field.</summary>
    /// <value>A <see cref="TabularFieldType" /> value.</value>
    public TabularFieldType FieldType
    {
        get
        {
            return _fieldType;
        }
    }

    /// <summary>Gets a value of the last read field as a sequence of contiguous character series.</summary>
    /// <value>A <see cref="ReadOnlySequence{T}" /> value.</value>
    public ReadOnlySequence<char> Value
    {
        get
        {
            return _value;
        }
    }

    /// <summary>Gets the current type of position in tabular data.</summary>
    /// <value>A <see cref="TabularPositionType" /> value.</value>
    public TabularPositionType PositionType
    {
        get
        {
            return _positionType;
        }
    }

    /// <summary>Gets the total number of characters consumed so far by the reader.</summary>
    /// <value>A non-negative zero-based number.</value>
    public long Position
    {
        get
        {
            return _position;
        }
    }
}
