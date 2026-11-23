// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;
using System.Text;
using Addax.Formats.Tabular.Buffers;
using Addax.Formats.Tabular.IO;

namespace Addax.Formats.Tabular;

/// <summary>Provides forward-only, write-only access to tabular data fields. This class cannot be inherited.</summary>
public sealed partial class TabularWriter : IDisposable, IAsyncDisposable
{
    private static readonly Encoding s_defaultEncoding = new UTF8Encoding(false, false);

    private readonly BufferedTextWriter _textWriter;
    private readonly TabularFormatter _tabularFormatter;
    private readonly TabularConverterResolver _converters;
    private readonly IFormatProvider _formatProvider;
    private readonly bool _trimWhitespace;

    private long _fieldsWritten;
    private long _recordsWritten;
    private TabularPositionType _currentPositionType;
    private TabularEngineState _engineState;

    /// <summary>Initializes a new instance of the <see cref="TabularWriter" /> class for the specified stream using the provided dialect and options.</summary>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing, or <see cref="TabularOptions.Converters" /> of <paramref name="options" /> contains a value that is <see langword="null" /> or is not a converter for its key.</exception>
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

        _converters = new(options.Converters);

        var encoding = options.Encoding ?? s_defaultEncoding;
        var bufferSize = Math.Max(1, Math.Min(options.BufferSize, Array.MaxLength));

        _textWriter = new(stream, encoding, bufferSize);
        _currentPositionType = _textWriter.IsStartOfStream ? TabularPositionType.StartOfStream : TabularPositionType.EndOfRecord;
        _tabularFormatter = new(dialect);
        _formatProvider = options.FormatProvider ?? TabularOptions.DefaultCultureInfo;
        _trimWhitespace = options.TrimWhitespace;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowUnwritableStreamException()
        {
            throw new ArgumentException("The stream does not support writing.");
        }
    }

    /// <summary>Writes the end of the current record if it has fields and releases all resources used by the current instance of the <see cref="TabularWriter" /> class.</summary>
    public void Dispose()
    {
        if (_engineState == TabularEngineState.Disposed)
        {
            return;
        }

        try
        {
            if (_engineState == TabularEngineState.Active)
            {
                if (_currentPositionType == TabularPositionType.Delimiter)
                {
                    WriteEndOfRecordCore();
                }

                _textWriter.Flush();
            }
        }
        finally
        {
            _engineState = TabularEngineState.Disposed;
            _textWriter.Dispose();
        }
    }

    /// <summary>Asynchronously writes the end of the current record if it has fields and releases all resources used by the current instance of the <see cref="TabularWriter" /> class.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        if (_engineState == TabularEngineState.Disposed)
        {
            return;
        }

        try
        {
            if (_engineState == TabularEngineState.Active)
            {
                if (_currentPositionType == TabularPositionType.Delimiter)
                {
                    await WriteEndOfRecordCoreAsync(default).ConfigureAwait(false);
                }

                await _textWriter.FlushAsync(default).ConfigureAwait(false);
            }
        }
        finally
        {
            _engineState = TabularEngineState.Disposed;
            _textWriter.Dispose();
        }
    }

    /// <summary>Writes an empty character sequence as the next value field of the current record.</summary>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteEmpty()
    {
        ThrowIfNotActive();

        WriteValueCore(default);
    }

    /// <summary>Asynchronously writes an empty character sequence as the next value field of the current record.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteEmptyAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(default, cancellationToken);
    }

    /// <summary>Writes a character sequence as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ReadOnlySpan{T}" /> instance to write.</param>
    /// <exception cref="ArgumentException">The value exceeds the supported field length.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteString(ReadOnlySpan<char> value)
    {
        ThrowIfNotActive();

        WriteValueCore(value);
    }

    /// <summary>Writes a <see cref="string" /> instance as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <exception cref="ArgumentException">The value exceeds the supported field length.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteString(string? value)
    {
        ThrowIfNotActive();

        WriteValueCore(value);
    }

    /// <summary>Asynchronously writes a character sequence as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ReadOnlyMemory{T}" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteStringAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="string" /> instance as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteStringAsync(string? value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value.AsMemory(), cancellationToken);
    }

    /// <summary>Writes a value of type <typeparamref name="T" /> as the next value field of the current record.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="value">The value to write.</param>
    /// <param name="converter">The converter to format the value with.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void Write<T>(T? value, TabularConverter<T> converter)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ThrowIfNotActive();

        WriteValueCore(value, converter);
    }

    /// <summary>Asynchronously writes a value of type <typeparamref name="T" /> as the next value field of the current record.</summary>
    /// <typeparam name="T">The type of the value to write.</typeparam>
    /// <param name="value">The value to write.</param>
    /// <param name="converter">The converter to format the value with.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="converter" /> is <see langword="null" />.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteAsync<T>(T? value, TabularConverter<T> converter, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(converter);
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, converter, cancellationToken);
    }

    /// <summary>Resolves the converter that the writer uses for values of type <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of the values to convert.</typeparam>
    /// <returns>The converter for <typeparamref name="T" /> in <see cref="TabularOptions.Converters" /> of the options the writer was created with, or else in <see cref="TabularRegistry.Converters" />.</returns>
    /// <exception cref="InvalidOperationException">Neither <see cref="TabularOptions.Converters" /> of the options the writer was created with nor <see cref="TabularRegistry.Converters" /> contains a converter for <typeparamref name="T" />, or the registry entry for <typeparamref name="T" /> is not a converter for that type.</exception>
    /// <remarks>The converter for a type is resolved once per writer.</remarks>
    public TabularConverter<T> ResolveConverter<T>()
    {
        return _converters.Resolve<T>();
    }

    /// <summary>Writes a character sequence as the next comment record.</summary>
    /// <param name="value">A <see cref="ReadOnlySpan{T}" /> instance to write.</param>
    /// <exception cref="ArgumentException">The value exceeds the supported field length.</exception>
    /// <exception cref="ArgumentException">The value contains the line terminator of the current dialect.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteComment(ReadOnlySpan<char> value)
    {
        ThrowIfNotActive();

        WriteCommentCore(value);
    }

    /// <summary>Writes a <see cref="string" /> instance as the next comment record.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <exception cref="ArgumentException">The value exceeds the supported field length.</exception>
    /// <exception cref="ArgumentException">The value contains the line terminator of the current dialect.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteComment(string? value)
    {
        ThrowIfNotActive();

        WriteCommentCore(value);
    }

    /// <summary>Asynchronously writes a character sequence as the next comment record.</summary>
    /// <param name="value">A <see cref="ReadOnlyMemory{T}" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="ArgumentException">The value contains the line terminator of the current dialect. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteCommentAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteCommentCoreAsync(value, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="string" /> instance as the next comment record.</summary>
    /// <param name="value">A <see cref="string" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="ArgumentException">The value contains the line terminator of the current dialect. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">The current dialect does not support the operation. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteCommentAsync(string? value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteCommentCoreAsync(value.AsMemory(), cancellationToken);
    }

    /// <summary>Writes the end of the current record, which is an empty record if no fields were written.</summary>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteEndOfRecord()
    {
        ThrowIfNotActive();

        WriteEndOfRecordCore();
    }

    /// <summary>Asynchronously writes the end of the current record, which is an empty record if no fields were written.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteEndOfRecordAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteEndOfRecordCoreAsync(cancellationToken);
    }

    /// <summary>Writes any buffered data to the underlying stream and flushes the stream.</summary>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>Buffered data is also written to the underlying stream when the buffer fills, without flushing the stream.</remarks>
    public void Flush()
    {
        ThrowIfNotActive();

        try
        {
            _textWriter.Flush();
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
    }

    /// <summary>Asynchronously writes any buffered data to the underlying stream and flushes the stream.</summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous flush operation.</returns>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>Buffered data is also written to the underlying stream when the buffer fills, without flushing the stream.</remarks>
    public ValueTask FlushAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return FlushCoreAsync(cancellationToken);

        async ValueTask FlushCoreAsync(CancellationToken cancellationToken)
        {
            try
            {
                await _textWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                _engineState = TabularEngineState.Faulted;

                throw;
            }
        }
    }

    private bool TryWriteValueToBuffer(ReadOnlySpan<char> value)
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

        var isDelimited = _currentPositionType == TabularPositionType.Delimiter;

        if (_textWriter.RemainingCount < textInfo.CharsRequired + (isDelimited ? 1 : 0))
        {
            return false;
        }

        _tabularFormatter.WriteValue(value, _textWriter.BufferWriter, textInfo, isDelimited);
        _currentPositionType = TabularPositionType.Delimiter;
        _fieldsWritten++;

        return true;
    }

    private void WriteValueCore(ReadOnlySpan<char> value)
    {
        if (!TryWriteValueToBuffer(value))
        {
            if (_trimWhitespace)
            {
                value = value.Trim();
            }

            var textInfo = _tabularFormatter.GetTextInfo(value);

            if (_currentPositionType == TabularPositionType.Delimiter)
            {
                _tabularFormatter.WriteValue(default, _textWriter.BufferWriter, default, true);
            }

            FlushBuffer();

            _tabularFormatter.WriteValue(value, _textWriter.BufferWriter, textInfo, false);
            _currentPositionType = TabularPositionType.Delimiter;
            _fieldsWritten++;
        }

        if (_textWriter.IsFull)
        {
            FlushBuffer();
        }
    }

    private ValueTask WriteValueCoreAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken)
    {
        try
        {
            if (!TryWriteValueToBuffer(value.Span))
            {
                return FlushAndWriteValueAsync(value, cancellationToken);
            }
        }
        catch (Exception e)
        {
            return ValueTask.FromException(e);
        }

        return _textWriter.IsFull ? FlushBufferAsync(cancellationToken) : ValueTask.CompletedTask;

        async ValueTask FlushAndWriteValueAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken)
        {
            if (_trimWhitespace)
            {
                value = value.Trim();
            }

            var textInfo = _tabularFormatter.GetTextInfo(value.Span);

            if (_currentPositionType == TabularPositionType.Delimiter)
            {
                _tabularFormatter.WriteValue(default, _textWriter.BufferWriter, default, true);
            }

            await FlushBufferAsync(cancellationToken).ConfigureAwait(false);

            _tabularFormatter.WriteValue(value.Span, _textWriter.BufferWriter, textInfo, false);
            _currentPositionType = TabularPositionType.Delimiter;
            _fieldsWritten++;

            if (_textWriter.IsFull)
            {
                await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
            }
        }
    }

    private void WriteValueCore<T>(T? value, TabularConverter<T> converter)
    {
        var bufferLength = 128;

        {
            var buffer = (stackalloc char[bufferLength]);

            if (converter.TryFormat(value, buffer, _formatProvider, out var charsWritten))
            {
                WriteValueCore(buffer.Slice(0, charsWritten));

                return;
            }
        }

        while (bufferLength < Array.MaxLength)
        {
            bufferLength = (int)Math.Min(2 * (uint)bufferLength, (uint)Array.MaxLength);

            using var buffer = new ArrayBuffer<char>(bufferLength);

            if (converter.TryFormat(value, buffer.AsSpan(), _formatProvider, out var charsWritten))
            {
                WriteValueCore(buffer.AsSpan(charsWritten));

                return;
            }
        }

        ThrowFieldFormatException();
    }

    private ValueTask WriteValueCoreAsync<T>(T? value, TabularConverter<T> converter, CancellationToken cancellationToken)
    {
        try
        {
            var buffer = (stackalloc char[128]);

            if (converter.TryFormat(value, buffer, _formatProvider, out var charsWritten) && TryWriteValueToBuffer(buffer.Slice(0, charsWritten)))
            {
                return _textWriter.IsFull ? FlushBufferAsync(cancellationToken) : ValueTask.CompletedTask;
            }
        }
        catch (OperationCanceledException e)
        {
            return ThrowCanceledAsync(ExceptionDispatchInfo.Capture(e));
        }
        catch (Exception e)
        {
            return ValueTask.FromException(e);
        }

        return FormatAndWriteValueAsync(value, converter, cancellationToken);

        static async ValueTask ThrowCanceledAsync(ExceptionDispatchInfo dispatchInfo)
        {
            await ValueTask.CompletedTask.ConfigureAwait(false);

            dispatchInfo.Throw();
        }

        async ValueTask FormatAndWriteValueAsync(T? value, TabularConverter<T> converter, CancellationToken cancellationToken)
        {
            var bufferLength = 128;

            while (bufferLength < Array.MaxLength)
            {
                bufferLength = (int)Math.Min(2 * (uint)bufferLength, (uint)Array.MaxLength);

                using var buffer = new ArrayBuffer<char>(bufferLength);

                if (converter.TryFormat(value, buffer.AsSpan(), _formatProvider, out var charsWritten))
                {
                    await WriteValueCoreAsync(buffer.AsMemory(charsWritten), cancellationToken).ConfigureAwait(false);

                    return;
                }
            }

            ThrowFieldFormatException();
        }
    }

    private void WriteEndOfRecordCore()
    {
        Debug.Assert(_textWriter.RemainingCount >= _tabularFormatter.LineTerminatorLength);

        _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);
        _currentPositionType = TabularPositionType.EndOfRecord;
        _fieldsWritten = 0;
        _recordsWritten++;

        if (_textWriter.IsFull)
        {
            FlushBuffer();
        }
    }

    private ValueTask WriteEndOfRecordCoreAsync(CancellationToken cancellationToken)
    {
        Debug.Assert(_textWriter.RemainingCount >= _tabularFormatter.LineTerminatorLength);

        _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);
        _currentPositionType = TabularPositionType.EndOfRecord;
        _fieldsWritten = 0;
        _recordsWritten++;

        return _textWriter.IsFull ? FlushBufferAsync(cancellationToken) : ValueTask.CompletedTask;
    }

    private void WriteCommentCore(ReadOnlySpan<char> value)
    {
        if (!_tabularFormatter.SupportsComments)
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

        if (!_tabularFormatter.CanWriteComment(value))
        {
            ThrowInvalidCommentException();
        }

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            Debug.Assert(_textWriter.RemainingCount >= _tabularFormatter.LineTerminatorLength);

            _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);
            _fieldsWritten = 0;
            _recordsWritten++;
        }

        if (_textWriter.RemainingCount < value.Length + 1)
        {
            FlushBuffer();
        }

        _tabularFormatter.WriteComment(value, _textWriter.BufferWriter);

        if (_textWriter.RemainingCount < _tabularFormatter.LineTerminatorLength)
        {
            FlushBuffer();
        }

        _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);

        if (_textWriter.IsFull)
        {
            FlushBuffer();
        }

        _currentPositionType = TabularPositionType.EndOfRecord;
    }

    private async ValueTask WriteCommentCoreAsync(ReadOnlyMemory<char> value, CancellationToken cancellationToken)
    {
        if (!_tabularFormatter.SupportsComments)
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

        if (!_tabularFormatter.CanWriteComment(value.Span))
        {
            ThrowInvalidCommentException();
        }

        if (_currentPositionType == TabularPositionType.Delimiter)
        {
            Debug.Assert(_textWriter.RemainingCount >= _tabularFormatter.LineTerminatorLength);

            _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);
            _fieldsWritten = 0;
            _recordsWritten++;
        }

        if (_textWriter.RemainingCount < value.Length + 1)
        {
            await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
        }

        _tabularFormatter.WriteComment(value.Span, _textWriter.BufferWriter);

        if (_textWriter.RemainingCount < _tabularFormatter.LineTerminatorLength)
        {
            await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
        }

        _tabularFormatter.WriteLineTerminator(_textWriter.BufferWriter);

        if (_textWriter.IsFull)
        {
            await FlushBufferAsync(cancellationToken).ConfigureAwait(false);
        }

        _currentPositionType = TabularPositionType.EndOfRecord;
    }

    private void FlushBuffer()
    {
        try
        {
            _textWriter.FlushBuffer();
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
    }

    private async ValueTask FlushBufferAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _textWriter.FlushBufferAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            _engineState = TabularEngineState.Faulted;

            throw;
        }
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
        throw new InvalidOperationException("The writer cannot be used after a failed write operation.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnsupportedFieldLengthException()
    {
        throw new ArgumentException("The formatted value exceeds the supported field length.");
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowInvalidCommentException()
    {
        throw new ArgumentException("The comment contains the line terminator of the current dialect.");
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
        throw new FormatException("The value cannot be formatted into a character sequence.");
    }

    /// <summary>Gets the type of the current writer position.</summary>
    /// <value>One of the enumeration values that specifies the type of the current writer position.</value>
    public TabularPositionType CurrentPositionType
    {
        get
        {
            return _currentPositionType;
        }
    }

    /// <summary>Gets the number of bytes written to the underlying stream so far.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
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

    /// <summary>Gets the number of records written so far, excluding comment records.</summary>
    /// <value>A non-negative 64-bit signed integer.</value>
    public long RecordsWritten
    {
        get
        {
            return _recordsWritten;
        }
    }
}
