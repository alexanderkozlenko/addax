// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to a tabular data on the field level.</summary>
public sealed partial class TabularFieldReader : IDisposable, IAsyncDisposable
{
    private readonly TabularStreamReader _streamReader;
    private readonly TabularStreamParser _streamParser;
    private readonly SequenceSource<char> _bufferSource = new(minimumSegmentSize: TabularFormatInfo.StackBufferSize);
    private readonly TabularStringFactory _stringFactory;
    private readonly IReadOnlyDictionary<Type, TabularFieldConverter> _converters;

    private ReadOnlySequence<char> _value;
    private long _position;
    private TabularPositionType _positionType;
    private TabularFieldType _fieldType;
    private StringBufferKind _bufferKind;
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
        _stringFactory = options.StringFactory;
        _converters = options.FieldConverters;
    }

    /// <summary>Initializes a new instance of the <see cref="TabularFieldReader" /> class using the specified stream, dialect, and default options.</summary>
    /// <param name="stream">The stream to read tabular data from.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    public TabularFieldReader(Stream stream, TabularDataDialect dialect)
        : this(stream, dialect, new())
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _isDisposed = true;
        _value = ReadOnlySequence<char>.Empty;
        _fieldType = TabularFieldType.Undefined;
        _positionType = TabularPositionType.EndOfStream;
        _position = 0;
        _bufferKind = StringBufferKind.None;
        _bufferSource.Clear();
        _streamReader.Dispose();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;
        _value = ReadOnlySequence<char>.Empty;
        _fieldType = TabularFieldType.Undefined;
        _positionType = TabularPositionType.EndOfStream;
        _position = 0;
        _bufferKind = StringBufferKind.None;
        _bufferSource.Clear();

        return _streamReader.DisposeAsync();
    }

    /// <summary>Advances the reader to the next record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true" /> if the reader was successfully advanced, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public bool MoveNextRecord(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _positionType switch
        {
            TabularPositionType.BeginningOfStream => AssumeNextRecord(),
            TabularPositionType.BeginningOfRecord => MoveNextRecordCore(cancellationToken),
            TabularPositionType.FieldSeparator => MoveNextRecordCore(cancellationToken),
            TabularPositionType.EndOfRecord => AssumeNextRecord(),
            _ => false,
        };

        bool AssumeNextRecord()
        {
            ReleaseValue();

            _positionType = TabularPositionType.BeginningOfRecord;

            return true;
        }

        bool MoveNextRecordCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            while (_positionType is not (TabularPositionType.EndOfRecord or TabularPositionType.EndOfStream))
            {
                AdvanceNextToken(consume: false, cancellationToken);
            }

            if (_positionType is TabularPositionType.EndOfRecord)
            {
                _positionType = TabularPositionType.BeginningOfRecord;
            }

            return _positionType is TabularPositionType.BeginningOfRecord;
        }
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

        ValueTask<bool> AssumeNextRecordAsync()
        {
            ReleaseValue();

            _positionType = TabularPositionType.BeginningOfRecord;

            return new(true);
        }

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

    /// <summary>Advances the reader to the next field in the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true" /> if the reader was successfully advanced, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public bool MoveNextField(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _positionType switch
        {
            TabularPositionType.BeginningOfRecord => MoveNextFieldCore(cancellationToken),
            TabularPositionType.FieldSeparator => MoveNextFieldCore(cancellationToken),
            _ => false,
        };

        bool MoveNextFieldCore(CancellationToken cancellationToken)
        {
            ReleaseValue();
            AdvanceNextToken(consume: false, cancellationToken);

            return _positionType is TabularPositionType.FieldSeparator;
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

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<bool> MoveNextFieldAsyncCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            await AdvanceNextTokenAsync(consume: false, cancellationToken).ConfigureAwait(false);

            return _positionType is TabularPositionType.FieldSeparator;
        }
    }

    /// <summary>Reads the next field in the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns><see langword="true" /> if the value was successfully read, or <see langword="false" /> otherwise.</returns>
    /// <exception cref="TabularDataException">The reader encountered an unexpected character or end of stream.</exception>
    public bool ReadField(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _positionType switch
        {
            TabularPositionType.BeginningOfRecord => ReadFieldCore(cancellationToken),
            TabularPositionType.FieldSeparator => ReadFieldCore(cancellationToken),
            _ => false,
        };

        bool ReadFieldCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            return AdvanceNextToken(consume: true, cancellationToken);
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

        ValueTask<bool> ReadFieldAsyncCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            return AdvanceNextTokenAsync(consume: true, cancellationToken);
        }
    }

    private bool AdvanceNextToken(bool consume, CancellationToken cancellationToken)
    {
        var examinedLength = 0L;

        var parserState = new TabularStreamParserState
        {
            IsBeginningOfLine = _positionType is TabularPositionType.BeginningOfRecord,
        };

        var streamReader = _streamReader;
        var streamParser = _streamParser;

        while (true)
        {
            streamReader.Read(cancellationToken);

            var readingBuffer = streamReader.Buffer;
            var parsingBuffer = readingBuffer.Slice(streamReader.ExaminedChars);
            var parsingStatus = TabularStreamParsingStatus.NeedMoreData;

            if (parsingBuffer.IsSingleSegment)
            {
                parsingStatus = streamParser.Parse(parsingBuffer.FirstSpan, ref parserState, out var parsedLength);
                examinedLength += parsedLength;
            }
            else
            {
                foreach (var parsingBufferSegment in parsingBuffer)
                {
                    parsingStatus = streamParser.Parse(parsingBufferSegment.Span, ref parserState, out var parsedLength);
                    examinedLength += parsedLength;

                    if (parsingStatus is not TabularStreamParsingStatus.NeedMoreData)
                    {
                        break;
                    }
                }
            }

            if (parsingStatus is TabularStreamParsingStatus.NeedMoreData)
            {
                if (!streamReader.IsCompleted)
                {
                    streamReader.Advance(0, examinedLength);

                    continue;
                }

                if (parserState.IsIncomplete)
                {
                    ThrowUnexpectedEndOfStreamException(_position + examinedLength - 1);
                }
            }
            else if (parsingStatus is TabularStreamParsingStatus.FoundInvalidData)
            {
                ThrowUnexpectedCharacterException(_position + examinedLength - 1);
            }

            _fieldType = !parserState.IsCommentPrefixFound ? TabularFieldType.Content : TabularFieldType.Comment;

            if (consume)
            {
                _value = streamParser.Extract(readingBuffer, examinedLength, parsingStatus, parserState, _bufferSource, out _bufferKind);
            }

            if (_bufferKind is StringBufferKind.Shared)
            {
                streamReader.Advance(0, examinedLength);
            }
            else
            {
                streamReader.Advance(examinedLength);
            }

            _positionType = parsingStatus switch
            {
                TabularStreamParsingStatus.FoundFieldSeparation => TabularPositionType.FieldSeparator,
                TabularStreamParsingStatus.FoundRecordSeparation => TabularPositionType.EndOfRecord,
                _ => TabularPositionType.EndOfStream,
            };

            _position += examinedLength;

            return consume;
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnexpectedCharacterException(long position)
        {
            throw new TabularDataException($"The reader encountered an unexpected character at position {position}.", position);
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnexpectedEndOfStreamException(long position)
        {
            throw new TabularDataException($"The reader encountered an unexpected end of stream at position {position}.", position);
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
    private async ValueTask<bool> AdvanceNextTokenAsync(bool consume, CancellationToken cancellationToken)
    {
        var examinedLength = 0L;

        var parserState = new TabularStreamParserState
        {
            IsBeginningOfLine = _positionType is TabularPositionType.BeginningOfRecord,
        };

        var streamReader = _streamReader;
        var streamParser = _streamParser;

        while (true)
        {
            var readingTask = streamReader.ReadAsync(cancellationToken);

            if (!readingTask.IsCompletedSuccessfully)
            {
                await readingTask.ConfigureAwait(false);
            }

            var readingBuffer = streamReader.Buffer;
            var parsingBuffer = readingBuffer.Slice(streamReader.ExaminedChars);
            var parsingStatus = TabularStreamParsingStatus.NeedMoreData;

            if (parsingBuffer.IsSingleSegment)
            {
                parsingStatus = streamParser.Parse(parsingBuffer.FirstSpan, ref parserState, out var parsedLength);
                examinedLength += parsedLength;
            }
            else
            {
                foreach (var parsingBufferSegment in parsingBuffer)
                {
                    parsingStatus = streamParser.Parse(parsingBufferSegment.Span, ref parserState, out var parsedLength);
                    examinedLength += parsedLength;

                    if (parsingStatus is not TabularStreamParsingStatus.NeedMoreData)
                    {
                        break;
                    }
                }
            }

            if (parsingStatus is TabularStreamParsingStatus.NeedMoreData)
            {
                if (!streamReader.IsCompleted)
                {
                    streamReader.Advance(0, examinedLength);

                    continue;
                }

                if (parserState.IsIncomplete)
                {
                    ThrowUnexpectedEndOfStreamException(_position + examinedLength - 1);
                }
            }
            else if (parsingStatus is TabularStreamParsingStatus.FoundInvalidData)
            {
                ThrowUnexpectedCharacterException(_position + examinedLength - 1);
            }

            _fieldType = !parserState.IsCommentPrefixFound ? TabularFieldType.Content : TabularFieldType.Comment;

            if (consume)
            {
                _value = streamParser.Extract(readingBuffer, examinedLength, parsingStatus, parserState, _bufferSource, out _bufferKind);
            }

            if (_bufferKind is StringBufferKind.Shared)
            {
                streamReader.Advance(0, examinedLength);
            }
            else
            {
                streamReader.Advance(examinedLength);
            }

            _positionType = parsingStatus switch
            {
                TabularStreamParsingStatus.FoundFieldSeparation => TabularPositionType.FieldSeparator,
                TabularStreamParsingStatus.FoundRecordSeparation => TabularPositionType.EndOfRecord,
                _ => TabularPositionType.EndOfStream,
            };

            _position += examinedLength;

            return consume;
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnexpectedCharacterException(long position)
        {
            throw new TabularDataException($"The reader encountered an unexpected character at position {position}.", position);
        }

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnexpectedEndOfStreamException(long position)
        {
            throw new TabularDataException($"The reader encountered an unexpected end of stream at position {position}.", position);
        }
    }

    /// <summary>Tries to retrieve the current reader value as <typeparamref name="T" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular field.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular field to <typeparamref name="T" />.</param>
    /// <param name="result">When this method returns, contains the <typeparamref name="T" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    public bool TryGet<T>(TabularFieldConverter<T> converter, out T? result)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_bufferKind is StringBufferKind.None)
        {
            throw new InvalidOperationException("The current reader value can only be accessed following a read operation.");
        }

        var value = _value;
        var bufferSize = value.Length;

        if (bufferSize > Array.MaxLength)
        {
            result = default;

            return false;
        }

        if (!converter.TryGetParseBufferSize(out var bufferSizeLimit))
        {
            result = default;

            return false;
        }

        if (bufferSizeLimit < 0)
        {
            throw new InvalidOperationException("The maximum buffer size for parsing a value must be equal to or greater than zero.");
        }

        if (bufferSize > bufferSizeLimit)
        {
            result = default;

            return false;
        }

        if (value.IsSingleSegment)
        {
            return converter.TryParse(value.FirstSpan, TabularFormatInfo.DefaultFormatProvider, out result);
        }
        else
        {
            using var parsingBuffer = bufferSize <= TabularFormatInfo.StackBufferSize ?
                new StringBuffer(stackalloc char[(int)bufferSize]) :
                new StringBuffer((int)bufferSize);

            value.CopyTo(parsingBuffer.AsSpan());

            return converter.TryParse(parsingBuffer.AsSpan(), TabularFormatInfo.DefaultFormatProvider, out result);
        }
    }

    /// <summary>Tries to retrieve the current reader value as <typeparamref name="T" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular field.</typeparam>
    /// <param name="result">When this method returns, contains the <typeparamref name="T" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    public bool TryGet<T>(out T? result)
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularFieldConverter<T> converterT))
        {
            if (typeof(T) == typeof(string))
            {
                // System.String does not have a default converter and is processed in a special way.

                if (TryGetString(out var resultT))
                {
                    result = Unsafe.As<string, T>(ref resultT);

                    return true;
                }
                else
                {
                    result = default;

                    return false;
                }
            }

            ThrowInvalidOperationException();
        }

        return TryGet(converterT, out result);

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException($"A field converter for type '{typeof(T)}' is not registered.");
        }
    }

    /// <summary>Retrieves the current reader value as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a field.</typeparam>
    /// <param name="converter">The converter to use for converting a tabular field to <typeparamref name="T" />.</param>
    /// <returns>The <typeparamref name="T" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <typeparamref name="T" />.</exception>
    public T? Get<T>(TabularFieldConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

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
    /// <typeparam name="T">The type of an object that represents a field.</typeparam>
    /// <returns>The <typeparamref name="T" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <typeparamref name="T" />.</exception>
    public T? Get<T>()
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularFieldConverter<T> converterT))
        {
            if (typeof(T) == typeof(string))
            {
                // System.String does not have a default converter and is processed in a special way.

                var resultT = GetString();

                return Unsafe.As<string, T>(ref resultT);
            }

            ThrowInvalidOperationException();
        }

        return Get(converterT);

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException($"A field converter for type '{typeof(T)}' is not registered.");
        }
    }

    private void ReleaseValue()
    {
        _fieldType = TabularFieldType.Undefined;
        _value = ReadOnlySequence<char>.Empty;

        switch (_bufferKind)
        {
            case StringBufferKind.Shared:
                {
                    _streamReader.Advance(_streamReader.ExaminedChars);
                }
                break;
            case StringBufferKind.Private:
                {
                    _bufferSource.Clear();
                }
                break;
        }

        _bufferKind = StringBufferKind.None;
    }

    private TabularFieldConverter<T> SelectConverter<T>()
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularFieldConverter<T> converterT))
        {
            ThrowInvalidOperationException();
        }

        return converterT;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException($"A field converter for type '{typeof(T)}' is not registered.");
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

    /// <summary>Gets a type of the last processed field.</summary>
    /// <value>A <see cref="TabularFieldType" /> value.</value>
    public TabularFieldType FieldType
    {
        get
        {
            return _fieldType;
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
