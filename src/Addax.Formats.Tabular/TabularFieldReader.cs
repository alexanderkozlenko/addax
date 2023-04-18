// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to a tabular data on the field level.</summary>
public sealed partial class TabularFieldReader : IAsyncDisposable
{
    private readonly TabularStreamReader _streamReader;
    private readonly TabularStreamParser _streamParser;
    private readonly SequenceSource<char> _bufferSource = new(minimumSegmentSize: 64);
    private readonly IReadOnlyDictionary<Type, TabularFieldConverter> _converters;

    private readonly TabularFieldConverter<BigInteger> _converterBigInteger;
    private readonly TabularFieldConverter<bool> _converterBoolean;
    private readonly TabularFieldConverter<byte> _converterByte;
    private readonly TabularFieldConverter<char> _converterChar;
    private readonly TabularFieldConverter<Complex> _converterComplex;
    private readonly TabularFieldConverter<DateOnly> _converterDateOnly;
    private readonly TabularFieldConverter<DateTime> _converterDateTime;
    private readonly TabularFieldConverter<DateTimeOffset> _converterDateTimeOffset;
    private readonly TabularFieldConverter<decimal> _converterDecimal;
    private readonly TabularFieldConverter<double> _converterDouble;
    private readonly TabularFieldConverter<Guid> _converterGuid;
    private readonly TabularFieldConverter<Half> _converterHalf;
    private readonly TabularFieldConverter<short> _converterInt16;
    private readonly TabularFieldConverter<int> _converterInt32;
    private readonly TabularFieldConverter<long> _converterInt64;
    private readonly TabularFieldConverter<Int128> _converterInt128;
    private readonly TabularFieldConverter<Rune> _converterRune;
    private readonly TabularFieldConverter<sbyte> _converterSByte;
    private readonly TabularFieldConverter<float> _converterSingle;
    private readonly TabularFieldConverter<TimeOnly> _converterTimeOnly;
    private readonly TabularFieldConverter<TimeSpan> _converterTimeSpan;
    private readonly TabularFieldConverter<uint> _converterUInt32;
    private readonly TabularFieldConverter<ulong> _converterUInt64;
    private readonly TabularFieldConverter<ushort> _converterUInt16;
    private readonly TabularFieldConverter<UInt128> _converterUInt128;

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
        _converters = options.FieldConverters;

        _converterBigInteger = SelectConverter<BigInteger>();
        _converterBoolean = SelectConverter<bool>();
        _converterByte = SelectConverter<byte>();
        _converterChar = SelectConverter<char>();
        _converterComplex = SelectConverter<Complex>();
        _converterDateOnly = SelectConverter<DateOnly>();
        _converterDateTime = SelectConverter<DateTime>();
        _converterDateTimeOffset = SelectConverter<DateTimeOffset>();
        _converterDecimal = SelectConverter<decimal>();
        _converterDouble = SelectConverter<double>();
        _converterGuid = SelectConverter<Guid>();
        _converterHalf = SelectConverter<Half>();
        _converterInt16 = SelectConverter<short>();
        _converterInt32 = SelectConverter<int>();
        _converterInt64 = SelectConverter<long>();
        _converterInt128 = SelectConverter<Int128>();
        _converterRune = SelectConverter<Rune>();
        _converterSByte = SelectConverter<sbyte>();
        _converterSingle = SelectConverter<float>();
        _converterTimeOnly = SelectConverter<TimeOnly>();
        _converterTimeSpan = SelectConverter<TimeSpan>();
        _converterUInt16 = SelectConverter<ushort>();
        _converterUInt32 = SelectConverter<uint>();
        _converterUInt64 = SelectConverter<ulong>();
        _converterUInt128 = SelectConverter<UInt128>();
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
        _fieldType = TabularFieldType.Undefined;
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
        ValueTask<bool> ReadFieldAsyncCore(CancellationToken cancellationToken)
        {
            ReleaseValue();

            return AdvanceNextTokenAsync(consume: true, cancellationToken);
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

        while (_positionType is not TabularPositionType.EndOfStream)
        {
            if (!_streamReader.TryRead())
            {
                await _streamReader.ReadAsync(cancellationToken).ConfigureAwait(false);
            }

            var readingBuffer = _streamReader.Buffer;
            var parsingBuffer = readingBuffer.Slice(_streamReader.Examined);
            var parsingStatus = _streamParser.Parse(parsingBuffer, ref parserState, out var parsedLength);

            examinedLength += parsedLength;

            if (parsingStatus is TabularStreamParsingStatus.NeedMoreData)
            {
                if (!_streamReader.IsCompleted)
                {
                    _streamReader.Advance(0, examinedLength);

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
                _value = _streamParser.Extract(readingBuffer, examinedLength, parsingStatus, parserState, _bufferSource, out _bufferKind);
            }

            if (_bufferKind is BufferKind.Shared)
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

            return consume;
        }

        return consume;

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

        if (_bufferKind is BufferKind.None)
        {
            throw new InvalidOperationException("The current reader value can only be accessed following a read operation.");
        }

        var value = _value;
        var bufferLength = value.Length;

        if (bufferLength > Array.MaxLength)
        {
            result = default;

            return false;
        }

        if (!converter.TryGetParseBufferLength(out var bufferLengthLimit))
        {
            result = default;

            return false;
        }

        if (bufferLengthLimit < 0)
        {
            throw new InvalidOperationException("The maximum buffer length for parsing a value must be equal to or greater than zero.");
        }

        if (bufferLength > bufferLengthLimit)
        {
            result = default;

            return false;
        }

        if (value.IsSingleSegment)
        {
            return converter.TryParse(value.FirstSpan, TabularDataInfo.DefaultFormatProvider, out result);
        }
        else
        {
            using var parsingBuffer = bufferLength <= TabularDataInfo.StackBufferLength ?
                HybridBuffer<char>.Create(stackalloc char[(int)bufferLength]) :
                HybridBuffer<char>.Create((int)bufferLength);

            value.CopyTo(parsingBuffer.AsSpan());

            return converter.TryParse(parsingBuffer.AsSpan(), TabularDataInfo.DefaultFormatProvider, out result);
        }
    }

    /// <summary>Tries to retrieve the current reader value as <typeparamref name="T" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular field.</typeparam>
    /// <param name="result">When this method returns, contains the <typeparamref name="T" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    public bool TryGet<T>(out T? result)
    {
        return TryGet(SelectConverter<T>(), out result);
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
        return Get(SelectConverter<T>());
    }

    private void ReleaseValue()
    {
        _fieldType = TabularFieldType.Undefined;
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

    private TabularFieldConverter<T> SelectConverter<T>()
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularFieldConverter<T> converterT))
        {
            throw new InvalidOperationException($"A field converter for type '{typeof(T)}' is not registered.");
        }

        return converterT;
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
