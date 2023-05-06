// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, write-only access to a tabular data on the field level.</summary>
public sealed partial class TabularFieldWriter : IDisposable, IAsyncDisposable
{
    private readonly TabularStreamWriter _streamWriter;
    private readonly TabularStreamFormatter _streamFormatter;
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

    private long _position;
    private TabularPositionType _positionType;
    private TabularFieldType _fieldType;
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="TabularFieldWriter" /> class using the specified stream, dialect, and options.</summary>
    /// <param name="stream">The stream to write tabular data to.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    /// <param name="options">The options to configure the writer.</param>
    public TabularFieldWriter(Stream stream, TabularDataDialect dialect, TabularFieldWriterOptions options)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);
        ArgumentNullException.ThrowIfNull(options);

        _streamWriter = new(stream, options.Encoding, options.BufferSize, options.LeaveOpen);
        _streamFormatter = new(dialect);
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

    /// <summary>Initializes a new instance of the <see cref="TabularFieldWriter" /> class using the specified stream, dialect, and default options.</summary>
    /// <param name="stream">The stream to write tabular data to.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    public TabularFieldWriter(Stream stream, TabularDataDialect dialect)
        : this(stream, dialect, new())
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _isDisposed = true;
        _fieldType = TabularFieldType.Undefined;
        _positionType = TabularPositionType.EndOfStream;
        _position = 0;
        _streamWriter.Dispose();
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;
        _fieldType = TabularFieldType.Undefined;
        _positionType = TabularPositionType.EndOfStream;
        _position = 0;

        return _streamWriter.DisposeAsync();
    }

    /// <summary>Instructs the writer to write any buffered characters to the stream.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    public void Flush(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _streamWriter.Flush(cancellationToken);
    }

    /// <summary>Asynchronously instructs the writer to write any buffered characters to the stream.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _streamWriter.FlushAsync(cancellationToken);
    }

    /// <summary>Instructs the writer to begin a new record in the stream.</summary>
    public void BeginRecord()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_positionType is not TabularPositionType.BeginningOfStream)
        {
            _streamFormatter.WriteRecordSeparator(_streamWriter, out var committed);
            _position += committed;
        }

        _positionType = TabularPositionType.BeginningOfRecord;
        _fieldType = TabularFieldType.Undefined;
    }

    /// <summary>Writes the specified <see cref="ReadOnlySpan{T}" /> value as data to the internal buffer.</summary>
    /// <param name="value">The value to be written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void Write(ReadOnlySpan<char> value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        EnsureCanWriteValue();

        var committed = 0L;

        if (_positionType is not TabularPositionType.BeginningOfRecord)
        {
            _streamFormatter.WriteFieldSeparator(_streamWriter, out committed);
            _position += committed;
        }

        _streamFormatter.WriteValue(value, _streamWriter, out committed);
        _positionType = TabularPositionType.FieldSeparator;
        _fieldType = TabularFieldType.Content;
        _position += committed;
    }

    /// <summary>Writes the specified <see cref="ReadOnlySequence{T}" /> value as data to the internal buffer.</summary>
    /// <param name="value">The value to be written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void Write(in ReadOnlySequence<char> value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        EnsureCanWriteValue();

        var committed = 0L;

        if (_positionType is not TabularPositionType.BeginningOfRecord)
        {
            _streamFormatter.WriteFieldSeparator(_streamWriter, out committed);
            _position += committed;
        }

        _streamFormatter.WriteValue(value, _streamWriter, out committed);
        _positionType = TabularPositionType.FieldSeparator;
        _fieldType = TabularFieldType.Content;
        _position += committed;
    }

    /// <summary>Writes the specified <see cref="ReadOnlySpan{T}" /> value as a comment to the internal buffer.</summary>
    /// <param name="value">The comment to be written.</param>
    /// <exception cref="InvalidOperationException">The current dialect does not support comments, the comment contains the line terminator, or the current record contains a value.</exception>
    public void WriteComment(ReadOnlySpan<char> value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        EnsureCanWriteComment(value);

        _streamFormatter.WriteComment(value, _streamWriter, out var committed);
        _positionType = TabularPositionType.FieldSeparator;
        _fieldType = TabularFieldType.Comment;
        _position += committed;
    }

    /// <summary>Writes the specified <see cref="ReadOnlySequence{T}" /> value as a comment to the internal buffer.</summary>
    /// <param name="value">The comment to be written.</param>
    /// <exception cref="InvalidOperationException">The current dialect does not support comments, the comment contains the line terminator, or the current record contains a value.</exception>
    public void WriteComment(in ReadOnlySequence<char> value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        EnsureCanWriteComment(value);

        _streamFormatter.WriteComment(value, _streamWriter, out var committed);
        _positionType = TabularPositionType.FieldSeparator;
        _fieldType = TabularFieldType.Comment;
        _position += committed;
    }

    /// <summary>Writes the specified <typeparamref name="T" /> value to the internal buffer.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular field.</typeparam>
    /// <param name="converter">The converter to use for converting <typeparamref name="T" /> to a tabular field.</param>
    /// <param name="value">The object or value to be written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void Write<T>(T? value, TabularFieldConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        EnsureCanWriteValue();

        if (converter.TryGetFormatBufferLength(value, out var bufferLength))
        {
            if (bufferLength < 0)
            {
                throw new InvalidOperationException("The maximum buffer length for formatting a value must be equal to or greater than zero.");
            }

            while (bufferLength <= Array.MaxLength)
            {
                if (TryWrite(value, converter, bufferLength))
                {
                    return;
                }

                bufferLength *= 2;

                if ((uint)bufferLength > (uint)Array.MaxLength)
                {
                    bufferLength = Math.Max(bufferLength + 1, Array.MaxLength);
                }
            }
        }

        throw new FormatException("The specified value cannot be formatted to a sequence of characters.");
    }

    /// <summary>Writes the specified <typeparamref name="T" /> value to the internal buffer.</summary>
    /// <typeparam name="T">The type of an object or value that represents a tabular field.</typeparam>
    /// <param name="value">The object or value to be written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void Write<T>(T? value)
    {
        Write(value, SelectConverter<T>());
    }

    private bool TryWrite<T>(T? value, TabularFieldConverter<T> converter, int bufferLength)
    {
        using var formattingBuffer = bufferLength <= TabularDataInfo.StackBufferLength ?
            HybridBuffer<char>.Create(stackalloc char[bufferLength]) :
            HybridBuffer<char>.Create(bufferLength);

        if (!converter.TryFormat(value, formattingBuffer.AsSpan(), TabularDataInfo.DefaultFormatProvider, out var charsWritten))
        {
            return false;
        }

        var committed = 0L;

        if (_positionType is not TabularPositionType.BeginningOfRecord)
        {
            _streamFormatter.WriteFieldSeparator(_streamWriter, out committed);
            _position += committed;
        }

        _streamFormatter.WriteValue(formattingBuffer.AsSpan()[..charsWritten], _streamWriter, out committed);
        _positionType = TabularPositionType.FieldSeparator;
        _fieldType = TabularFieldType.Content;
        _position += committed;

        return true;
    }

    private void EnsureCanWriteValue()
    {
        if (_positionType is TabularPositionType.BeginningOfStream)
        {
            throw new InvalidOperationException("An explicitly started record is required.");
        }
        if (_fieldType is TabularFieldType.Comment)
        {
            throw new InvalidOperationException("A value cannot be written after a comment.");
        }
    }

    private void EnsureCanWriteComment(ReadOnlySpan<char> value)
    {
        if (!_streamFormatter.SupportsComments)
        {
            throw new InvalidOperationException("The current dialect does not support comments.");
        }
        if (!_streamFormatter.CanWriteComment(value))
        {
            throw new InvalidOperationException("A comment cannot contain the line terminator.");
        }
        if (_positionType is not TabularPositionType.BeginningOfRecord)
        {
            throw new InvalidOperationException("A comment can be written only to an empty record.");
        }
    }

    private void EnsureCanWriteComment(in ReadOnlySequence<char> value)
    {
        if (!_streamFormatter.SupportsComments)
        {
            throw new InvalidOperationException("The current dialect does not support comments.");
        }
        if (!_streamFormatter.CanWriteComment(value))
        {
            throw new InvalidOperationException("A comment cannot contain the line terminator.");
        }
        if (_positionType is not TabularPositionType.BeginningOfRecord)
        {
            throw new InvalidOperationException("A comment can be written only to an empty record.");
        }
    }

    private TabularFieldConverter<T> SelectConverter<T>()
    {
        if (!_converters.TryGetValue(typeof(T), out var converter) || (converter is not TabularFieldConverter<T> converterT))
        {
            throw new InvalidOperationException($"A field converter for type '{typeof(T)}' is not registered.");
        }

        return converterT;
    }

    /// <summary>Gets the count of unflushed characters within the current writer.</summary>
    /// <value>A non-negative zero-based number.</value>
    public long UnflushedChars
    {
        get
        {
            return _streamWriter.UnflushedChars;
        }
    }

    /// <summary>Gets a type of the last written field.</summary>
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

    /// <summary>Gets the total number of characters committed so far by the writer.</summary>
    /// <value>A non-negative zero-based number.</value>
    public long Position
    {
        get
        {
            return _position;
        }
    }
}
