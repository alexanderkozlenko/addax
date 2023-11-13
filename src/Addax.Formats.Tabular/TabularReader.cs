// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Buffers;
using Addax.Formats.Tabular.Collections;
using Addax.Formats.Tabular.IO;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, read-only access to tabular data fields. This class cannot be inherited.</summary>
public sealed partial class TabularReader : IDisposable, IAsyncDisposable
{
    private readonly LiteTextReader _textReader;
    private readonly TabularParser _tabularParser;
    private readonly LiteQueue<TabularFieldInfo> _fieldsQueue = new(32);
    private readonly TabularStringFactory _stringFactory;
    private readonly IFormatProvider _formatProvider;
    private readonly bool _trimWhitespace;

    private TabularParserState _parserState;
    private ArrayRef<char> _currentField;
    private long _charsConsumed;
    private long _fieldsRead;
    private long _recordsRead;
    private int _currentFieldCharsUsed;
    private TabularPositionType _currentPositionType;
    private TabularFieldType _currentFieldType;
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="TabularReader" /> class for the specified stream using the provided dialect and options.</summary>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support reading.</exception>
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

        var encoding = options.Encoding ?? TabularFormatInfo.DefaultEncoding;
        var bufferSize = Math.Max(1, Math.Min(options.BufferSize, Array.MaxLength));

        _textReader = new(stream, encoding, bufferSize, options.LeaveOpen);
        _currentPositionType = !_textReader.IsAppending ? TabularPositionType.StartOfStream : TabularPositionType.EndOfRecord;
        _tabularParser = new(dialect);
        _formatProvider = options.FormatProvider ?? TabularFormatInfo.DefaultCulture;
        _trimWhitespace = options.TrimWhitespace;
        _stringFactory = options.StringFactory ?? TabularStringFactory.Default;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnreadableStreamException()
        {
            throw new ArgumentException("The stream does not support reading.");
        }
    }

    /// <summary>Releases the resources used by the current instance of the <see cref="TabularReader" /> class.</summary>
    public void Dispose()
    {
        _isDisposed = true;
        _fieldsQueue.Dispose();
        _currentField.Dispose();
        _textReader.Dispose();
    }

    /// <summary>Asynchronously releases the resources used by the current instance of the <see cref="TabularReader" /> class.</summary>
    /// <returns>A task object.</returns>
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;
        _fieldsQueue.Dispose();
        _currentField.Dispose();

        return _textReader.DisposeAsync();
    }

    /// <summary>Tries to switch the reader to the next record.</summary>
    /// <returns><see langword="true" /> if the reader was successfully switched; otherwise, <see langword="false" />.</returns>
    public bool TryPickRecord()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if ((_currentPositionType == TabularPositionType.EndOfRecord) ||
            (_currentPositionType == TabularPositionType.StartOfStream))
        {
            ResetCurrentField();

            _currentPositionType = TabularPositionType.StartOfRecord;
            _fieldsRead = 0;

            return true;
        }

        return false;
    }

    /// <summary>Tries to advance the reader to the next field of the current record.</summary>
    /// <returns><see langword="true" /> if the reader was successfully advanced; otherwise, <see langword="false" />.</returns>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public bool TrySkipField()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.StartOfStream))
        {
            ResetCurrentField();

            if (_fieldsQueue.IsEmpty)
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
    /// <returns>A task object that, when awaited, produces <see langword="true" /> if the reader was successfully advanced; otherwise, <see langword="false" />.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    public ValueTask<bool> TrySkipFieldAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.StartOfStream))
        {
            ResetCurrentField();

            if (!_fieldsQueue.IsEmpty)
            {
                SkipNextField();

                return new(true);
            }

            return ParseAndSkipFieldAsync(cancellationToken);
        }

        return new(false);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<bool> ParseAndSkipFieldAsync(CancellationToken cancellationToken)
        {
            await ParseStreamAsync(cancellationToken).ConfigureAwait(false);

            SkipNextField();

            return true;
        }
    }

    /// <summary>Tries to read the content of the next field of the current record.</summary>
    /// <returns><see langword="true" /> if the field was successfully read; otherwise, <see langword="false" />.</returns>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public bool TryReadField()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.StartOfStream))
        {
            ResetCurrentField();

            if (_fieldsQueue.IsEmpty)
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
    /// <returns>A task object that, when awaited, produces <see langword="true" /> if the field was successfully read; otherwise, <see langword="false" />.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered. This exception is stored into the returned task.</exception>
    public ValueTask<bool> TryReadFieldAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if ((_currentPositionType == TabularPositionType.Delimiter) ||
            (_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.StartOfStream))
        {
            ResetCurrentField();

            if (!_fieldsQueue.IsEmpty)
            {
                ReadNextField();

                return new(true);
            }

            return ParseAndReadFieldAsync(cancellationToken);
        }

        return new(false);

        [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder<>))]
        async ValueTask<bool> ParseAndReadFieldAsync(CancellationToken cancellationToken)
        {
            await ParseStreamAsync(cancellationToken).ConfigureAwait(false);

            ReadNextField();

            return true;
        }
    }

    /// <summary>Tries to transcode the current field as <see cref="string" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="string" /> instance that represents the current field, or <see langword="null" />. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if a value was successfully transcoded; otherwise, <see langword="false" />.</returns>
    public bool TryGetString([NotNullWhen(true)] out string? value)
    {
        var source = _currentField.AsReadOnlySpan();

        if (_trimWhitespace)
        {
            source = source.Trim();
        }

        value = _stringFactory.Create(source);

        return value is not null;
    }

    /// <summary>Tries to parse the current field as <typeparamref name="T" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <typeparam name="T">The type to parse the field as.</typeparam>
    /// <param name="converter">The converter to parse the field with.</param>
    /// <param name="value">When this method returns, contains a <typeparamref name="T" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    public bool TryGet<T>(TabularConverter<T> converter, out T? value)
    {
        ArgumentNullException.ThrowIfNull(converter);

        return TryGet(_currentField.AsReadOnlySpan(), converter, out value);
    }

    /// <summary>Transcodes the current field as <see cref="string" />.</summary>
    /// <returns>A <see cref="string" /> instance.</returns>
    /// <exception cref="FormatException">The current field cannot be transcoded as <see cref="string" />.</exception>
    public string GetString()
    {
        if (!TryGetString(out var result))
        {
            ThrowFieldFormatException<string>();
        }

        return result;
    }

    /// <summary>Parses the current field as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type to parse the field as.</typeparam>
    /// <param name="converter">The converter to parse the field with.</param>
    /// <returns>A <typeparamref name="T" /> value.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    /// <exception cref="FormatException">The current field cannot be parsed as <typeparamref name="T" />.</exception>
    public T? Get<T>(TabularConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);

        if (!TryGet(converter, out var result))
        {
            ThrowFieldFormatException<T>();
        }

        return result;
    }

    private void ParseStream()
    {
        var parsingMode = TabularParsingMode.None;

        if ((_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.StartOfStream))
        {
            parsingMode |= TabularParsingMode.StartOfRecord;
        }

        var charsParsed = _parserState.CharsParsed;

        while (_fieldsQueue.IsEmpty && !_textReader.IsEndOfStream)
        {
            if (!_textReader.TryRead())
            {
                ThrowUnsupportedFieldLengthException(_charsConsumed + charsParsed - 1);
            }

            var source = _textReader.BufferSpan.Slice(charsParsed);

            if (_textReader.IsEndOfStream)
            {
                parsingMode |= TabularParsingMode.EndOfStream;
            }

            if (!_tabularParser.TryParse(source, parsingMode, ref _parserState, _fieldsQueue, ref charsParsed))
            {
                if ((parsingMode & TabularParsingMode.EndOfStream) == 0)
                {
                    ThrowUnexpectedCharacterException(_charsConsumed + charsParsed - 1);
                }
                else
                {
                    ThrowUnexpectedEndOfStreamException(_charsConsumed + charsParsed - 1);
                }
            }
        }
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask ParseStreamAsync(CancellationToken cancellationToken)
    {
        var parsingMode = TabularParsingMode.None;

        if ((_currentPositionType == TabularPositionType.StartOfRecord) ||
            (_currentPositionType == TabularPositionType.StartOfStream))
        {
            parsingMode |= TabularParsingMode.StartOfRecord;
        }

        var charsParsed = _parserState.CharsParsed;

        while (_fieldsQueue.IsEmpty && !_textReader.IsEndOfStream)
        {
            if (!await _textReader.TryReadAsync(cancellationToken).ConfigureAwait(false))
            {
                ThrowUnsupportedFieldLengthException(_charsConsumed + charsParsed - 1);
            }

            var source = _textReader.BufferMemory.Slice(charsParsed);

            if (_textReader.IsEndOfStream)
            {
                parsingMode |= TabularParsingMode.EndOfStream;
            }

            if (!_tabularParser.TryParse(source.Span, parsingMode, ref _parserState, _fieldsQueue, ref charsParsed))
            {
                if ((parsingMode & TabularParsingMode.EndOfStream) == 0)
                {
                    ThrowUnexpectedCharacterException(_charsConsumed + charsParsed - 1);
                }
                else
                {
                    ThrowUnexpectedEndOfStreamException(_charsConsumed + charsParsed - 1);
                }
            }
        }
    }

    private void SkipNextField()
    {
        var fieldInfo = _fieldsQueue.Dequeue();

        _textReader.Advance(fieldInfo.CharsUsed);

        _currentPositionType = fieldInfo.Separator switch
        {
            TabularSeparator.Delimiter => TabularPositionType.Delimiter,
            TabularSeparator.LineTerminator => TabularPositionType.EndOfRecord,
            _ => TabularPositionType.EndOfStream,
        };

        _currentFieldType = fieldInfo.IsAnnotation ? TabularFieldType.Annotation : TabularFieldType.Value;
        _charsConsumed += fieldInfo.CharsUsed;

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            _fieldsRead++;
        }
        else
        {
            _recordsRead++;
        }
    }

    private void ReadNextField()
    {
        var fieldInfo = _fieldsQueue.Dequeue();

        _tabularParser.ReadField(_textReader.BufferMemory, in fieldInfo, ref _currentField);

        if (_currentField.IsUntracked)
        {
            _currentFieldCharsUsed = fieldInfo.CharsUsed;
        }
        else
        {
            _textReader.Advance(fieldInfo.CharsUsed);
        }

        _currentPositionType = fieldInfo.Separator switch
        {
            TabularSeparator.Delimiter => TabularPositionType.Delimiter,
            TabularSeparator.LineTerminator => TabularPositionType.EndOfRecord,
            _ => TabularPositionType.EndOfStream,
        };

        _currentFieldType = fieldInfo.IsAnnotation ? TabularFieldType.Annotation : TabularFieldType.Value;
        _charsConsumed += fieldInfo.CharsUsed;

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            _fieldsRead++;
        }
        else
        {
            _recordsRead++;
        }
    }

    private void ResetCurrentField()
    {
        if (_currentFieldCharsUsed != 0)
        {
            _textReader.Advance(_currentFieldCharsUsed);
        }
        else
        {
            _currentField.Dispose();
        }

        _currentFieldType = TabularFieldType.None;
        _currentField = default;
        _currentFieldCharsUsed = 0;
    }

    private bool TryGet<T>(ReadOnlySpan<char> source, TabularConverter<T> converter, out T? result)
    {
        return converter.TryParse(source, _formatProvider, out result);
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnsupportedFieldLengthException(long position)
    {
        throw new NotSupportedException($"A field at position {position} exceeds the supported field length.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnexpectedCharacterException(long position)
    {
        throw new TabularContentException($"An unexpected character was encountered at position {position}.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnexpectedEndOfStreamException(long position)
    {
        throw new TabularContentException($"An unexpected end of stream was encountered at position {position}.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowFieldFormatException<T>()
    {
        throw new FormatException($"The current field cannot be parsed as {typeof(T)}.");
    }

    /// <summary>Gets the type of the current reader position.</summary>
    /// <value>A position type enumeration value.</value>
    public TabularPositionType CurrentPositionType
    {
        get
        {
            return _currentPositionType;
        }
    }

    /// <summary>Gets the type of the last consumed field.</summary>
    /// <value>A field type enumeration value.</value>
    public TabularFieldType CurrentFieldType
    {
        get
        {
            return _currentFieldType;
        }
    }

    /// <summary>Gets the last read field as a sequence of characters.</summary>
    /// <value>A read-only region of memory.</value>
    public ReadOnlyMemory<char> CurrentField
    {
        get
        {
            return _currentField.AsReadOnlyMemory();
        }
    }

    /// <summary>Gets the number of bytes consumed so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    /// <remarks>Might be less than the number of bytes read from the underlying stream.</remarks>
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

    /// <summary>Gets the number of records read so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    public long RecordsRead
    {
        get
        {
            return _recordsRead;
        }
    }
}
