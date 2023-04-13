// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using Addax.Formats.Tabular.Primitives;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, write-only access to a tabular data on the field level.</summary>
public sealed partial class TabularFieldWriter : IAsyncDisposable
{
    private readonly TabularStreamWriter _streamWriter;
    private readonly TabularStreamFormatter _streamFormatter;

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
    }

    /// <summary>Initializes a new instance of the <see cref="TabularFieldWriter" /> class using the specified stream, dialect, and default options.</summary>
    /// <param name="stream">The stream to write tabular data to.</param>
    /// <param name="dialect">The tabular data dialect to use.</param>
    public TabularFieldWriter(Stream stream, TabularDataDialect dialect)
        : this(stream, dialect, new())
    {
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;
        _positionType = TabularPositionType.EndOfStream;
        _fieldType = TabularFieldType.None;
        _position = 0;

        return _streamWriter.DisposeAsync();
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
        _fieldType = TabularFieldType.None;
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

    private void Write<T>(T value, TabularFieldConverter<T> converter)
        where T : struct
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        EnsureCanWriteValue();

        var bufferLength = converter.GetFormatBufferLength(value);

        if (bufferLength < 0)
        {
            throw new FormatException("The specified value cannot be formatted into a sequence of characters.");
        }

        using var formattingBuffer = bufferLength <= TabularDataInfo.StackBufferLength ?
            HybridBuffer<char>.Create(stackalloc char[bufferLength]) :
            HybridBuffer<char>.Create(bufferLength);

        if (!converter.TryFormat(value, formattingBuffer.AsSpan(), TabularDataInfo.DefaultFormatProvider, out var charsWritten))
        {
            throw new FormatException("The specified value cannot be formatted into a sequence of characters.");
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
    }

    /// <summary>Writes the specified <typeparamref name="T" /> value to the internal buffer.</summary>
    /// <param name="value">The value to be written.</param>
    /// <param name="func">A function that parses a sequence of characters to a value.</param>
    /// <param name="provider">An optional object that provides culture-specific formatting information.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void Write<T>(T? value, TabularTryFormatFunc<T> func, IFormatProvider? provider)
    {
        ArgumentNullException.ThrowIfNull(func);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        EnsureCanWriteValue();

        provider ??= TabularDataInfo.DefaultFormatProvider;

        var bufferLength = TabularDataInfo.StackBufferLength;

        while (bufferLength <= Array.MaxLength)
        {
            if (TryWrite(value, func, provider, bufferLength))
            {
                return;
            }

            bufferLength *= 2;

            if ((uint)bufferLength > (uint)Array.MaxLength)
            {
                bufferLength = Math.Max(bufferLength + 1, Array.MaxLength);
            }
        }

        throw new FormatException("The specified value cannot be formatted into a sequence of characters.");
    }

    /// <summary>Writes the specified <typeparamref name="T" /> value to the internal buffer.</summary>
    /// <param name="value">The value to be written.</param>
    /// <param name="func">A function that parses a sequence of characters to a value.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void Write<T>(T? value, TabularTryFormatFunc<T> func)
    {
        Write(value, func, TabularDataInfo.DefaultFormatProvider);
    }

    private bool TryWrite<T>(T? value, TabularTryFormatFunc<T> func, IFormatProvider provider, int bufferLength)
    {
        using var formattingBuffer = bufferLength <= TabularDataInfo.StackBufferLength ?
            HybridBuffer<char>.Create(stackalloc char[bufferLength]) :
            HybridBuffer<char>.Create(bufferLength);

        if (!func.Invoke(value, formattingBuffer.AsSpan(), provider, out var charsWritten))
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
