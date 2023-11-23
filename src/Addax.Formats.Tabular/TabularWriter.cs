// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Addax.Formats.Tabular.Buffers;
using Addax.Formats.Tabular.IO;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, write-only access to tabular data fields. This class cannot be inherited.</summary>
public sealed partial class TabularWriter : IDisposable, IAsyncDisposable
{
    private readonly LiteTextWriter _textWriter;
    private readonly TabularFormatter _tabularFormatter;
    private readonly IFormatProvider _formatProvider;
    private readonly bool _trimWhitespace;

    private long _fieldsWritten;
    private long _recordsWritten;
    private TabularPositionType _currentPositionType;
    private bool _isDisposed;

    /// <summary>Initializes a new instance of the <see cref="TabularWriter" /> class for the specified stream using the provided dialect and options.</summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <exception cref="ArgumentException"><paramref name="stream"/> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    public TabularWriter(Stream stream, TabularDialect dialect, TabularOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(dialect);

        if (!stream.CanWrite)
        {
            ThrowUnwritableStreamException();
        }

        options ??= TabularOptions.Default;

        var encoding = options.Encoding ?? TabularFormatInfo.DefaultEncoding;
        var bufferSize = Math.Max(1, Math.Min(options.BufferSize, Array.MaxLength));

        _textWriter = new(stream, encoding, bufferSize, options.LeaveOpen);
        _currentPositionType = _textWriter.IsStartOfStream ? TabularPositionType.StartOfStream : TabularPositionType.EndOfRecord;
        _tabularFormatter = new(dialect);
        _formatProvider = options.FormatProvider ?? TabularFormatInfo.DefaultCulture;
        _trimWhitespace = options.TrimWhitespace;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnwritableStreamException()
        {
            throw new ArgumentException("The stream does not support writing.");
        }
    }

    /// <summary>Releases the resources used by the current instance of the <see cref="TabularWriter" /> class.</summary>
    public void Dispose()
    {
        _isDisposed = true;
        _textWriter.Dispose();
    }

    /// <summary>Asynchronously releases the resources used by the current instance of the <see cref="TabularWriter" /> class.</summary>
    /// <returns>A task object.</returns>
    public ValueTask DisposeAsync()
    {
        _isDisposed = true;

        return _textWriter.DisposeAsync();
    }

    /// <summary>Writes an empty sequence of characters as the next value field of the current record.</summary>
    public void WriteEmpty()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(default);
    }

    /// <summary>Asynchronously writes an empty sequence of characters as the next value field of the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteEmptyAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(default, cancellationToken);
    }

    /// <summary>Writes a sequence of characters as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ReadOnlySpan{T}" /> instance to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    public void WriteString(ReadOnlySpan<char> value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value);
    }

    /// <summary>Writes a <see cref="string" /> instance as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    public void WriteString(string? value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value);
    }

    /// <summary>Asynchronously writes a sequence of characters as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ReadOnlyMemory{T}" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteStringAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="string" /> instance as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteStringAsync(string? value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value.AsMemory(), cancellationToken);
    }

    /// <summary>Writes <typeparamref name="T" /> value as the next value field of the current record.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="value"><typeparamref name="T" /> value to write.</param>
    /// <param name="converter">The converter to format the value with.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    public void Write<T>(T? value, TabularConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, converter);
    }

    /// <summary>Asynchronously writes <typeparamref name="T" /> value as the next value field of the current record.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="value"><typeparamref name="T" /> value to write.</param>
    /// <param name="converter">The converter to format the value with.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteAsync<T>(T? value, TabularConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, converter, cancellationToken);
    }

    /// <summary>Writes a sequence of characters as the next record with annotation.</summary>
    /// <param name="value">A <see cref="ReadOnlySpan{T}" /> instance to write.</param>
    /// <exception cref="ArgumentException">The value cannot be formatted into a sequence of characters or the formatted value exceeds the supported field length.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation.</exception>
    public void WriteAnnotation(ReadOnlySpan<char> value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteAnnotationCore(value);
    }

    /// <summary>Writes a <see cref="string" /> instance as the next record with annotation.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <exception cref="ArgumentException">The value cannot be formatted into a sequence of characters or the formatted value exceeds the supported field length.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation.</exception>
    public void WriteAnnotation(string? value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteAnnotationCore(value);
    }

    /// <summary>Asynchronously writes a sequence of characters as the next record with annotation.</summary>
    /// <param name="value">A <see cref="ReadOnlyMemory{T}" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException">The value cannot be formatted into a sequence of characters or the formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteAnnotationAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteAnnotationCoreAsync(value, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="string" /> instance as the next record with annotation.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException">The value cannot be formatted into a sequence of characters or the formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteAnnotationAsync(string? value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteAnnotationCoreAsync(value.AsMemory(), cancellationToken);
    }

    /// <summary>Instructs the writer to finish accepting fields for the current record.</summary>
    public void FinishRecord()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _currentPositionType = TabularPositionType.EndOfRecord;
        _fieldsWritten = 0;
        _recordsWritten++;
    }

    /// <summary>Writes any buffered data to the underlying stream.</summary>
    public void Flush()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        _textWriter.Flush();
    }

    /// <summary>Asynchronously writes any buffered data to the underlying stream.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return _textWriter.FlushAsync(cancellationToken);
    }

    private void WriteValueCore(ReadOnlySpan<char> value)
    {
        if (_trimWhitespace)
        {
            value = value.Trim();
        }

        var textInfo = _tabularFormatter.GetTextInfo(value);

        if ((uint)textInfo.CharsRequired > (uint)Array.MaxLength)
        {
            ThrowUnsupportedFieldLengthException();
        }

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            if (_textWriter.UnusedBufferSize < 1)
            {
                _textWriter.Flush();
            }

            _tabularFormatter.WriteDelimiter(_textWriter.BufferWriter);
        }
        else if (_currentPositionType == TabularPositionType.EndOfRecord)
        {
            if (_textWriter.UnusedBufferSize < _tabularFormatter.LineTerminatorLength)
            {
                _textWriter.Flush();
            }

            _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);
        }

        if (_textWriter.UnusedBufferSize < textInfo.CharsRequired)
        {
            _textWriter.Flush();
        }

        _tabularFormatter.WriteValue(value, _textWriter.BufferWriter, textInfo);

        if (_textWriter.WrittenBufferSize >= _textWriter.FlushThreshold)
        {
            _textWriter.Flush();
        }

        _currentPositionType = TabularPositionType.Delimiter;
        _fieldsWritten++;
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask WriteValueCoreAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken)
    {
        if (_trimWhitespace)
        {
            value = value.Trim();
        }

        var textInfo = _tabularFormatter.GetTextInfo(value.Span);

        if ((uint)textInfo.CharsRequired > (uint)Array.MaxLength)
        {
            ThrowUnsupportedFieldLengthException();
        }

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            if (_textWriter.UnusedBufferSize < 1)
            {
                await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            _tabularFormatter.WriteDelimiter(_textWriter.BufferWriter);
        }
        else if (_currentPositionType == TabularPositionType.EndOfRecord)
        {
            if (_textWriter.UnusedBufferSize < _tabularFormatter.LineTerminatorLength)
            {
                await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);
        }

        if (_textWriter.UnusedBufferSize < textInfo.CharsRequired)
        {
            await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        _tabularFormatter.WriteValue(value.Span, _textWriter.BufferWriter, textInfo);

        if (_textWriter.WrittenBufferSize >= _textWriter.FlushThreshold)
        {
            await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        _currentPositionType = TabularPositionType.Delimiter;
        _fieldsWritten++;
    }

    private void WriteValueCore<T>(T? value, TabularConverter<T> converter)
    {
        var bufferLength = 128;

        {
            var buffer = (Span<char>)stackalloc char[bufferLength];

            if (converter.TryFormat(value, buffer, _formatProvider, out var charsWritten))
            {
                WriteValueCore(buffer.Slice(0, charsWritten));

                return;
            }
        }

        while (bufferLength < Array.MaxLength)
        {
            bufferLength = (int)Math.Min(2 * (uint)bufferLength, (uint)Array.MaxLength);

            using var buffer = ArrayFactory<char>.Create(bufferLength);

            if (converter.TryFormat(value, buffer.AsSpan(), _formatProvider, out var charsWritten))
            {
                WriteValueCore(buffer.AsReadOnlySpan(0, charsWritten));

                return;
            }
        }

        ThrowFieldFormatException();
    }

    [AsyncMethodBuilder(typeof(PoolingAsyncValueTaskMethodBuilder))]
    private async ValueTask WriteValueCoreAsync<T>(T? value, TabularConverter<T> converter, CancellationToken cancellationToken)
    {
        var bufferLength = 128;

        while (bufferLength < Array.MaxLength)
        {
            bufferLength = (int)Math.Min(2 * (uint)bufferLength, (uint)Array.MaxLength);

            using var buffer = ArrayFactory<char>.Create(bufferLength);

            if (converter.TryFormat(value, buffer.AsSpan(), _formatProvider, out var charsWritten))
            {
                await WriteValueCoreAsync(buffer.AsReadOnlyMemory(0, charsWritten), cancellationToken).ConfigureAwait(false);

                return;
            }
        }

        ThrowFieldFormatException();
    }

    private void WriteAnnotationCore(ReadOnlySpan<char> value)
    {
        if (!_tabularFormatter.SupportsAnnotations)
        {
            ThrowUnsupportedOperationException();
        }

        if (_trimWhitespace)
        {
            value = value.Trim();
        }

        if ((uint)(value.Length + 1) > (uint)Array.MaxLength)
        {
            ThrowUnsupportedFieldLengthException();
        }

        if (!_tabularFormatter.CanWriteAnnotation(value))
        {
            ThrowFieldFormatException();
        }

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            if (_textWriter.UnusedBufferSize < _tabularFormatter.LineTerminatorLength)
            {
                _textWriter.Flush();
            }

            _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);

            if (_textWriter.UnusedBufferSize < value.Length + 1)
            {
                _textWriter.Flush();
            }

            _fieldsWritten = 0;
            _recordsWritten++;
        }

        _tabularFormatter.WriteAnnotation(value, _textWriter.BufferWriter);

        if (_textWriter.WrittenBufferSize >= _textWriter.FlushThreshold)
        {
            _textWriter.Flush();
        }

        _currentPositionType = TabularPositionType.EndOfRecord;
        _recordsWritten++;
    }

    private async ValueTask WriteAnnotationCoreAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken)
    {
        if (!_tabularFormatter.SupportsAnnotations)
        {
            ThrowUnsupportedOperationException();
        }

        if (_trimWhitespace)
        {
            value = value.Trim();
        }

        if ((uint)(value.Length + 1) > (uint)Array.MaxLength)
        {
            ThrowUnsupportedFieldLengthException();
        }

        if (!_tabularFormatter.CanWriteAnnotation(value.Span))
        {
            ThrowFieldFormatException();
        }

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            if (_textWriter.UnusedBufferSize < _tabularFormatter.LineTerminatorLength)
            {
                await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);

            if (_textWriter.UnusedBufferSize < value.Length + 1)
            {
                await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            _fieldsWritten = 0;
            _recordsWritten++;
        }

        _tabularFormatter.WriteAnnotation(value.Span, _textWriter.BufferWriter);

        if (_textWriter.WrittenBufferSize >= _textWriter.FlushThreshold)
        {
            await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }

        _currentPositionType = TabularPositionType.EndOfRecord;
        _recordsWritten++;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnsupportedFieldLengthException()
    {
        throw new ArgumentException("The formatted value exceeds the supported field length.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnsupportedOperationException()
    {
        throw new InvalidOperationException("The current dialect does not support the operation.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowFieldFormatException()
    {
        throw new FormatException("The value cannot be formatted into a sequence of characters.");
    }

    /// <summary>Gets the type of the current writer position.</summary>
    /// <value>A position type enumeration value.</value>
    public TabularPositionType CurrentPositionType
    {
        get
        {
            return _currentPositionType;
        }
    }

    /// <summary>Gets the number of bytes committed so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    /// <remarks>Might be greater than the number of bytes written to the underlying stream.</remarks>
    public long BytesCommitted
    {
        get
        {
            return _textWriter.BytesCommitted;
        }
    }

    /// <summary>Gets the number of fields written to the current record so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    public long FieldsWritten
    {
        get
        {
            return _fieldsWritten;
        }
    }

    /// <summary>Gets the number of records written so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    public long RecordsWritten
    {
        get
        {
            return _recordsWritten;
        }
    }
}
