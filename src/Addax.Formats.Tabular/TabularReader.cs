// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Addax.Formats.Tabular.IO;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to tabular data fields. This class cannot be inherited.</summary>
public sealed partial class TabularReader : IDisposable, IAsyncDisposable
{
    private const int s_fieldCapacity = 4096;

    private static readonly Encoding s_defaultEncoding = new UTF8Encoding(true, false);

    private readonly BufferedTextReader _textReader;
    private readonly TabularParser _tabularParser;
    private readonly TabularStringFactory _stringFactory;
    private readonly TabularConverterResolver _converters;
    private readonly IFormatProvider _formatProvider;
    private readonly char _delimiter;
    private readonly int _lineTerminatorLength;
    private readonly bool _trimWhitespace;

    private int[] _fieldEnds;
    private TabularParserState _parserState;
    private ArraySegment<char> _buffer;
    private long _charsConsumed;
    private long _fieldsRead;
    private long _recordsRead;
    private int _fieldStart;
    private int _fieldIndex;
    private int _fieldCount;
    private int _currentFieldStart;
    private int _currentFieldLength;
    private TabularPositionType _currentPositionType;
    private TabularFieldType _currentFieldType;
    private TabularEngineState _engineState;

    /// <summary>Initializes a new instance of the <see cref="TabularReader" /> class for the specified stream using the provided dialect and options.</summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading, or <see cref="TabularOptions.Converters" /> of <paramref name="options" /> contains a value that is <see langword="null" /> or is not a converter for its key.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    public TabularReader(Stream stream, TabularDialect dialect, TabularOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);

        if (!stream.CanRead)
        {
            ThrowUnreadableStreamException();
        }

        options ??= TabularOptions.Default;

        _converters = new(options.Converters);

        var encoding = options.Encoding ?? s_defaultEncoding;
        var bufferSize = Math.Max(1, Math.Min(options.BufferSize, Array.MaxLength));

        _textReader = new(stream, encoding, bufferSize);
        _fieldEnds = ArrayPool<int>.Shared.Rent(s_fieldCapacity);
        _currentPositionType = _textReader.IsStartOfStream ? TabularPositionType.StartOfStream : TabularPositionType.EndOfRecord;
        _tabularParser = new(dialect);
        _delimiter = dialect.Delimiter;
        _lineTerminatorLength = dialect.LineTerminator.Length;
        _formatProvider = options.FormatProvider ?? TabularOptions.DefaultCultureInfo;
        _trimWhitespace = options.TrimWhitespace;
        _stringFactory = options.StringFactory ?? TabularStringFactory.Default;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnreadableStreamException()
        {
            throw new ArgumentException("The stream does not support reading.");
        }
    }

    /// <summary>Releases all resources used by the current instance of the <see cref="TabularReader" /> class.</summary>
    public void Dispose()
    {
        if (_engineState == TabularEngineState.Disposed)
        {
            return;
        }

        _engineState = TabularEngineState.Disposed;

        ArrayPool<int>.Shared.Return(_fieldEnds);

        _textReader.Dispose();
    }

    /// <summary>Asynchronously releases all resources used by the current instance of the <see cref="TabularReader" /> class.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public ValueTask DisposeAsync()
    {
        Dispose();

        return ValueTask.CompletedTask;
    }

    /// <summary>Tries to advance the reader to the start of the next record, skipping the unread fields of the current record.</summary>
    /// <returns><see langword="true" /> if the reader was successfully advanced; <see langword="false" /> if the end of the stream was reached.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length.</exception>
    public bool TryMoveToNextRecord()
    {
        ThrowIfNotActive();

        while (
            (_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.Delimiter))
        {
            ResetCurrentField();

            if (_fieldIndex == _fieldCount)
            {
                ParseStream();
            }

            SkipNextField();
        }

        return TryStartRecord();
    }

    /// <summary>Asynchronously tries to advance the reader to the start of the next record, skipping the unread fields of the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation. The value of its <see cref="ValueTask{TResult}.Result" /> property contains <see langword="true" /> if the reader was successfully advanced; <see langword="false" /> if the end of the stream was reached.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length. This exception is stored into the returned task.</exception>
    public ValueTask<bool> TryMoveToNextRecordAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        if ((_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.Delimiter))
        {
            return SkipFieldsAndStartRecordAsync(cancellationToken);
        }

        return TryStartRecordAsync(cancellationToken);

        async ValueTask<bool> SkipFieldsAndStartRecordAsync(CancellationToken cancellationToken)
        {
            while (
                (_currentPositionType == TabularPositionType.StartOfRecord) ||
                (_currentPositionType == TabularPositionType.Delimiter))
            {
                await TrySkipFieldAsync(cancellationToken).ConfigureAwait(false);
            }

            return await TryStartRecordAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>Tries to advance the reader to the next field of the current record.</summary>
    /// <returns><see langword="true" /> if the reader was successfully advanced; <see langword="false" /> if there are no more fields in the current record.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length.</exception>
    public bool TrySkipField()
    {
        ThrowIfNotActive();

        if ((_currentPositionType == TabularPositionType.StartOfStream) && !TryStartRecord())
        {
            return false;
        }

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord))
        {
            ResetCurrentField();

            if (_fieldIndex == _fieldCount)
            {
                ParseStream();
            }

            SkipNextField();

            return true;
        }

        return false;
    }

    /// <summary>Asynchronously tries to advance the reader to the next field of the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous skip operation. The value of its <see cref="ValueTask{TResult}.Result" /> property contains <see langword="true" /> if the reader was successfully advanced; <see langword="false" /> if there are no more fields in the current record.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length. This exception is stored into the returned task.</exception>
    public ValueTask<bool> TrySkipFieldAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        if (_currentPositionType == TabularPositionType.StartOfStream)
        {
            return StartRecordAndSkipFieldAsync(cancellationToken);
        }

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord))
        {
            ResetCurrentField();

            if (_fieldIndex != _fieldCount)
            {
                SkipNextField();

                return new(true);
            }

            return ParseAndSkipFieldAsync(cancellationToken);
        }

        return new(false);

        async ValueTask<bool> StartRecordAndSkipFieldAsync(CancellationToken cancellationToken)
        {
            if (!await TryStartRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                return false;
            }

            return await TrySkipFieldAsync(cancellationToken).ConfigureAwait(false);
        }

        async ValueTask<bool> ParseAndSkipFieldAsync(CancellationToken cancellationToken)
        {
            await ParseStreamAsync(cancellationToken).ConfigureAwait(false);

            SkipNextField();

            return true;
        }
    }

    /// <summary>Tries to read the content of the next field of the current record.</summary>
    /// <returns><see langword="true" /> if the field was successfully read; <see langword="false" /> if there are no more fields in the current record.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length.</exception>
    public bool TryReadField()
    {
        ThrowIfNotActive();

        if ((_currentPositionType == TabularPositionType.StartOfStream) && !TryStartRecord())
        {
            return false;
        }

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord))
        {
            ResetCurrentField();

            if (_fieldIndex == _fieldCount)
            {
                ParseStream();
            }

            ReadNextField();

            return true;
        }

        return false;
    }

    /// <summary>Asynchronously tries to read the content of the next field of the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation. The value of its <see cref="ValueTask{TResult}.Result" /> property contains <see langword="true" /> if the field was successfully read; <see langword="false" /> if there are no more fields in the current record.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularException">A field exceeds the supported field length. This exception is stored into the returned task.</exception>
    public ValueTask<bool> TryReadFieldAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        if (_currentPositionType == TabularPositionType.StartOfStream)
        {
            return StartRecordAndReadFieldAsync(cancellationToken);
        }

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord))
        {
            ResetCurrentField();

            if (_fieldIndex != _fieldCount)
            {
                ReadNextField();

                return new(true);
            }

            return ParseAndReadFieldAsync(cancellationToken);
        }

        return new(false);

        async ValueTask<bool> StartRecordAndReadFieldAsync(CancellationToken cancellationToken)
        {
            if (!await TryStartRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                return false;
            }

            return await TryReadFieldAsync(cancellationToken).ConfigureAwait(false);
        }

        async ValueTask<bool> ParseAndReadFieldAsync(CancellationToken cancellationToken)
        {
            await ParseStreamAsync(cancellationToken).ConfigureAwait(false);

            ReadNextField();

            return true;
        }
    }

    /// <summary>Tries to transcode the current field as a <see cref="string" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="string" /> instance that represents the current field, or <see langword="null" /> on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if a value was successfully transcoded; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetString([NotNullWhen(true)] out string? value)
    {
        ThrowIfNotActive();

        var source = _buffer.AsSpan(_currentFieldStart, _currentFieldLength);

        if (_trimWhitespace)
        {
            source = source.Trim();
        }

        value = _stringFactory.Create(source);

        return value is not null;
    }

    /// <summary>Tries to parse the current field as a <typeparamref name="T" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <typeparam name="T">The type to parse the field as.</typeparam>
    /// <param name="converter">The converter to parse the value with.</param>
    /// <param name="value">When this method returns, contains a <typeparamref name="T" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGet<T>(TabularConverter<T> converter, out T? value)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ThrowIfNotActive();

        return TryParse(converter, out value);
    }

    /// <summary>Transcodes the current field as a <see cref="string" />.</summary>
    /// <returns>A <see cref="string" /> instance.</returns>
    /// <exception cref="FormatException">The current field cannot be transcoded as a <see cref="string" />.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public string GetString()
    {
        if (!TryGetString(out var result))
        {
            ThrowFieldFormatException<string>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type to parse the field as.</typeparam>
    /// <param name="converter">The converter to parse the value with.</param>
    /// <returns>A <typeparamref name="T" /> value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    /// <exception cref="FormatException">The current field cannot be parsed as <typeparamref name="T" />.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public T? Get<T>(TabularConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        if (!TryGet(converter, out var result))
        {
            ThrowFieldFormatException<T>();
        }

        return result;
    }

    /// <summary>Resolves the converter that the reader uses for values of type <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of the values to convert.</typeparam>
    /// <returns>The converter for <typeparamref name="T" /> in <see cref="TabularOptions.Converters" /> of the options the reader was created with, or else in <see cref="TabularRegistry.Converters" />.</returns>
    /// <exception cref="InvalidOperationException">Neither <see cref="TabularOptions.Converters" /> of the options the reader was created with nor <see cref="TabularRegistry.Converters" /> contains a converter for <typeparamref name="T" />, or the registry entry for <typeparamref name="T" /> is not a converter for that type.</exception>
    /// <remarks>The converter for a type is resolved once per reader.</remarks>
    public TabularConverter<T> ResolveConverter<T>()
    {
        return _converters.Resolve<T>();
    }

    private void ParseStream()
    {
        _textReader.Advance(_fieldStart);
        _charsConsumed += _fieldStart;
        _parserState.Position -= _fieldStart;
        _fieldStart = 0;

        try
        {
            while (!TryParseBuffer())
            {
                if (!_textReader.TryRead())
                {
                    ThrowUnsupportedFieldLengthException(_charsConsumed);
                }
            }
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
    }

    private async ValueTask ParseStreamAsync(CancellationToken cancellationToken)
    {
        _textReader.Advance(_fieldStart);
        _charsConsumed += _fieldStart;
        _parserState.Position -= _fieldStart;
        _fieldStart = 0;

        try
        {
            while (!TryParseBuffer())
            {
                if (!await _textReader.TryReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    ThrowUnsupportedFieldLengthException(_charsConsumed);
                }
            }
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
    }

    private bool TryParseBuffer()
    {
        _buffer = _textReader.BufferSegment;
        _fieldIndex = 0;

        var status = _tabularParser.Parse(_buffer, _textReader.IsEndOfStream, ref _parserState, _fieldEnds, out _fieldCount);

        while (status == OperationStatus.DestinationTooSmall)
        {
            var fieldEnds = ArrayPool<int>.Shared.Rent((int)Math.Max(Math.Min(2 * (uint)_fieldEnds.Length, (uint)Array.MaxLength), (uint)_fieldEnds.Length + 1));

            _fieldEnds.AsSpan(0, _fieldCount).CopyTo(fieldEnds);

            ArrayPool<int>.Shared.Return(_fieldEnds);

            _fieldEnds = fieldEnds;

            status = _tabularParser.Parse(_buffer, _textReader.IsEndOfStream, ref _parserState, _fieldEnds.AsSpan(_fieldCount), out var fieldCount);

            _fieldCount += fieldCount;
        }

        if (status == OperationStatus.InvalidData)
        {
            ThrowUnexpectedCharacterException(_charsConsumed + _parserState.Position);
        }

        if ((status == OperationStatus.NeedMoreData) && _textReader.IsEndOfStream)
        {
            ThrowUnexpectedEndOfStreamException(_charsConsumed + _parserState.Position);
        }

        return _fieldCount != 0;
    }

    private void SkipNextField()
    {
        var fieldEnd = _fieldEnds[_fieldIndex++];

        _currentFieldType = fieldEnd < 0 ? TabularFieldType.Comment : TabularFieldType.Value;

        if (fieldEnd < 0)
        {
            fieldEnd = ~fieldEnd;
        }

        var fieldStart = fieldEnd;

        if (fieldEnd == _buffer.Count)
        {
            _currentPositionType = TabularPositionType.EndOfStream;
        }
        else if (_buffer[fieldEnd] == _delimiter)
        {
            _currentPositionType = TabularPositionType.Delimiter;

            fieldStart += 1;
        }
        else
        {
            _currentPositionType = TabularPositionType.EndOfRecord;

            fieldStart += _lineTerminatorLength;
        }

        _fieldStart = fieldStart;
        _fieldsRead++;

        if ((_currentPositionType != TabularPositionType.Delimiter) && (_currentFieldType != TabularFieldType.Comment))
        {
            _recordsRead++;
        }
    }

    private void ReadNextField()
    {
        _currentFieldLength = _tabularParser.DecodeField(_buffer, _fieldStart, _fieldEnds[_fieldIndex], out _currentFieldStart);

        SkipNextField();
    }

    private bool TryStartRecord()
    {
        ResetCurrentField();

        if ((_currentPositionType != TabularPositionType.EndOfRecord) &&
            (_currentPositionType != TabularPositionType.StartOfStream))
        {
            return false;
        }

        if (_fieldIndex == _fieldCount)
        {
            ParseStream();
        }

        return TryStartQueuedRecord();
    }

    private ValueTask<bool> TryStartRecordAsync(CancellationToken cancellationToken)
    {
        ResetCurrentField();

        if ((_currentPositionType != TabularPositionType.EndOfRecord) &&
            (_currentPositionType != TabularPositionType.StartOfStream))
        {
            return new(false);
        }

        if (_fieldIndex != _fieldCount)
        {
            return new(TryStartQueuedRecord());
        }

        return ParseAndStartRecordAsync(cancellationToken);

        async ValueTask<bool> ParseAndStartRecordAsync(CancellationToken cancellationToken)
        {
            await ParseStreamAsync(cancellationToken).ConfigureAwait(false);

            return TryStartQueuedRecord();
        }
    }

    private bool TryStartQueuedRecord()
    {
        var fieldEnd = _fieldEnds[_fieldIndex];

        if ((fieldEnd == _fieldStart) && (fieldEnd == _buffer.Count))
        {
            _fieldIndex++;
            _currentPositionType = TabularPositionType.EndOfStream;

            return false;
        }

        _currentPositionType = TabularPositionType.StartOfRecord;
        _currentFieldType = fieldEnd < 0 ? TabularFieldType.Comment : TabularFieldType.Value;
        _fieldsRead = 0;

        return true;
    }

    private void ResetCurrentField()
    {
        _currentFieldType = TabularFieldType.None;
        _currentFieldStart = 0;
        _currentFieldLength = 0;
    }

    private bool TryParse<T>(TabularConverter<T> converter, out T? result)
    {
        var source = _buffer.AsSpan(_currentFieldStart, _currentFieldLength);

        if (_trimWhitespace)
        {
            source = source.Trim();
        }

        return converter.TryParse(source, _formatProvider, out result);
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
        throw new InvalidOperationException("The reader cannot be used after a failed read operation.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnsupportedFieldLengthException(long position)
    {
        throw new TabularException($"A field at position {position} exceeds the supported field length.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnexpectedCharacterException(long position)
    {
        throw new TabularContentException($"An unexpected character was encountered at position {position}.", position);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnexpectedEndOfStreamException(long position)
    {
        throw new TabularContentException($"An unexpected end of stream was encountered at position {position}.", position);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowFieldFormatException<T>()
    {
        throw new FormatException($"The current field cannot be parsed as {typeof(T)}.");
    }

    /// <summary>Gets the type of the current reader position.</summary>
    /// <value>One of the enumeration values that specifies the type of the current reader position.</value>
    public TabularPositionType CurrentPositionType
    {
        get
        {
            return _currentPositionType;
        }
    }

    /// <summary>Gets the type of the current field.</summary>
    /// <value>One of the enumeration values that specifies the type of the last consumed field, or, at the start of a record, the type of the first field of the record, or <see cref="TabularFieldType.None" /> before the first record and after an attempt to advance past the last record.</value>
    public TabularFieldType CurrentFieldType
    {
        get
        {
            return _currentFieldType;
        }
    }

    /// <summary>Gets the last read field as a character sequence.</summary>
    /// <value>A read-only region of memory, or an empty region if the last consumed field was skipped or no field of the current record was consumed.</value>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public ReadOnlyMemory<char> CurrentField
    {
        get
        {
            ThrowIfNotActive();

            var source = _buffer.AsMemory(_currentFieldStart, _currentFieldLength);

            return _trimWhitespace ? source.Trim() : source;
        }
    }

    /// <summary>Gets the number of bytes read from the underlying stream so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    public long BytesConsumed
    {
        get
        {
            return _textReader.BytesConsumed;
        }
    }

    /// <summary>Gets the number of fields read from the current record so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    public long FieldsRead
    {
        get
        {
            return _fieldsRead;
        }
    }

    /// <summary>Gets the number of records read so far, excluding comment records.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    public long RecordsRead
    {
        get
        {
            return _recordsRead;
        }
    }
}
