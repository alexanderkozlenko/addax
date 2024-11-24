// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Frozen;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Addax.Formats.Tabular.Buffers;

namespace Addax.Formats.Tabular;

/// <summary>Provides static methods for working with collections of tabular data records and inferring dialects. This class cannot be inherited.</summary>
public static class TabularData
{
    /// <summary>Reads all records that can be represented as <typeparamref name="T" /> from the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <param name="handler">The handler to read a <typeparamref name="T" /> instance from a record.</param>
    /// <returns>An array of records.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static T[] ReadRecords<T>(Stream stream, TabularDialect dialect, TabularOptions? options, TabularHandler<T>? handler)
        where T : notnull
    {
        using (var reader = new TabularReader<T>(stream, dialect, options, handler))
        {
            using var builder = new ArrayBuilder<T>(32);

            while (reader.TryReadRecord())
            {
                builder.Add(reader.CurrentRecord);
            }

            return builder.Build();
        }
    }

    /// <summary>Reads all records that can be represented as <typeparamref name="T" /> from the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <returns>An array of records.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static T[] ReadRecords<T>(Stream stream, TabularDialect dialect, TabularOptions? options)
        where T : notnull
    {
        return ReadRecords<T>(stream, dialect, options, null);
    }

    /// <summary>Reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <returns>An array of records.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static T[] ReadRecords<T>(Stream stream, TabularDialect dialect)
        where T : notnull
    {
        return ReadRecords<T>(stream, dialect, null, null);
    }

    /// <summary>Asynchronously reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <param name="handler">The handler to read a <typeparamref name="T" /> instance from a record.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces an array of records.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static async ValueTask<T[]> ReadRecordsAsync<T>(Stream stream, TabularDialect dialect, TabularOptions? options, TabularHandler<T>? handler, CancellationToken cancellationToken = default)
        where T : notnull
    {
        var reader = new TabularReader<T>(stream, dialect, options, handler);

        await using (reader.ConfigureAwait(false))
        {
            using var builder = new ArrayBuilder<T>(32);

            while (await reader.TryReadRecordAsync(cancellationToken).ConfigureAwait(false))
            {
                builder.Add(reader.CurrentRecord);
            }

            return builder.Build();
        }
    }

    /// <summary>Asynchronously reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="options">The options to control the behavior during reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces an array of records.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static ValueTask<T[]> ReadRecordsAsync<T>(Stream stream, TabularDialect dialect, TabularOptions? options, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return ReadRecordsAsync<T>(stream, dialect, options, null, cancellationToken);
    }

    /// <summary>Asynchronously reads all records from the stream that can be represented as <typeparamref name="T" />.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to read from.</param>
    /// <param name="dialect">The dialect to use for reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces an array of records.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" /> or <paramref name="dialect" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <exception cref="TabularContentException">An unexpected character or end of stream was encountered.</exception>
    public static ValueTask<T[]> ReadRecordsAsync<T>(Stream stream, TabularDialect dialect, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return ReadRecordsAsync<T>(stream, dialect, null, null, cancellationToken);
    }

    /// <summary>Writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <param name="handler">The handler to write a <typeparamref name="T" /> instance to a record.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public static void WriteRecords<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options, TabularHandler<T>? handler)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(records);

        using (var writer = new TabularWriter<T>(stream, dialect, options, handler))
        {
            foreach (var record in records)
            {
                if (record is not null)
                {
                    writer.WriteRecord(record);
                }
            }
        }
    }

    /// <summary>Writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public static void WriteRecords<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options)
        where T : notnull
    {
        WriteRecords(stream, dialect, records, options, null);
    }

    /// <summary>Writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    public static void WriteRecords<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records)
        where T : notnull
    {
        WriteRecords(stream, dialect, records, null, null);
    }

    /// <summary>Asynchronously writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <param name="handler">The handler to write a <typeparamref name="T" /> instance to a record.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public static async ValueTask WriteRecordsAsync<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options, TabularHandler<T>? handler, CancellationToken cancellationToken = default)
        where T : notnull
    {
        ArgumentNullException.ThrowIfNull(records);

        var writer = new TabularWriter<T>(stream, dialect, options, handler);

        await using (writer.ConfigureAwait(false))
        {
            foreach (var record in records)
            {
                if (record is not null)
                {
                    await writer.WriteRecordAsync(record, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>Asynchronously writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="options">The options to control the behavior during writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public static ValueTask WriteRecordsAsync<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, TabularOptions? options, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return WriteRecordsAsync(stream, dialect, records, options, null, cancellationToken);
    }

    /// <summary>Asynchronously writes a collection of records to the stream.</summary>
    /// <typeparam name="T">The type of an object that represents a record.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="dialect">The dialect to use for writing.</param>
    /// <param name="records">The records to write to the stream.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support writing.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="dialect" />, or <paramref name="records" /> is <see langword="null" />.</exception>
    /// <exception cref="InvalidOperationException">The record handler is not specified and cannot be found in the registry.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public static ValueTask WriteRecordsAsync<T>(Stream stream, TabularDialect dialect, IEnumerable<T> records, CancellationToken cancellationToken = default)
        where T : notnull
    {
        return WriteRecordsAsync(stream, dialect, records, null, null, cancellationToken);
    }

    /// <summary>Attempts to infer a dialect from the stream based on frequency of the eligible token values.</summary>
    /// <param name="stream">The stream to infer from.</param>
    /// <param name="lineTerminators">The eligible values for a line terminator.</param>
    /// <param name="delimiters">The eligible values for a delimiter.</param>
    /// <param name="quoteSymbols">The eligible values for a quote symbol.</param>
    /// <param name="sampleLength">The length of a sample in bytes.</param>
    /// <param name="encoding">The encoding for reading from the stream.</param>
    /// <returns>A successfully inferred dialect or <see langword="null" />.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="lineTerminators" />, <paramref name="delimiters" />, or <paramref name="quoteSymbols" /> is <see langword="null" />.</exception>
    public static TabularDialect? InferDialect(Stream stream, IEnumerable<string> lineTerminators, IEnumerable<char> delimiters, IEnumerable<char> quoteSymbols, int sampleLength, Encoding? encoding)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(lineTerminators);
        ArgumentNullException.ThrowIfNull(delimiters);
        ArgumentNullException.ThrowIfNull(quoteSymbols);

        if (!stream.CanRead)
        {
            ThrowUnreadableStreamException();
        }

        encoding ??= TabularFormatInfo.DefaultEncoding;

        using (stream)
        {
            var isStartOfStream = !stream.CanSeek || (stream.Position == 0);
            var byteBufferSize = Math.Max(1, Math.Min(sampleLength, Array.MaxLength));

            using var byteBuffer = new ArrayBuffer<byte>(byteBufferSize);

            var byteBufferUsedSize = stream.ReadAtLeast(byteBuffer.AsSpan(), byteBufferSize, false);
            var byteBufferUsed = byteBuffer.AsSpan(byteBufferUsedSize);

            if (isStartOfStream && byteBufferUsed.StartsWith(encoding.Preamble))
            {
                byteBufferUsed = byteBufferUsed.Slice(encoding.Preamble.Length);
            }

            var charBufferSize = encoding.GetMaxCharCount(Math.Min(byteBufferUsedSize, 0x00100000));

            using var charBuffer = new ArrayBuffer<char>(charBufferSize);

            var charBufferUsedSize = encoding.GetChars(byteBufferUsed, charBuffer.AsSpan());
            var charBufferUsed = charBuffer.AsSpan(charBufferUsedSize);

            return InferDialect(charBufferUsed, lineTerminators.ToFrozenSet(StringComparer.Ordinal), delimiters.ToFrozenSet(), quoteSymbols.ToFrozenSet());
        }
    }

    /// <summary>Attempts to infer a dialect from the stream based on frequency of the eligible token values.</summary>
    /// <param name="stream">The stream to infer from.</param>
    /// <param name="lineTerminators">The eligible values for a line terminator.</param>
    /// <param name="delimiters">The eligible values for a delimiter.</param>
    /// <param name="quoteSymbols">The eligible values for a quote symbol.</param>
    /// <returns>A successfully inferred dialect or <see langword="null" />.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="lineTerminators" />, <paramref name="delimiters" />, or <paramref name="quoteSymbols" /> is <see langword="null" />.</exception>
    /// <remarks>The operation consumes the first 65536 bytes as a sample and uses a UTF-8 encoding without byte order mark (BOM).</remarks>
    public static TabularDialect? InferDialect(Stream stream, IEnumerable<string> lineTerminators, IEnumerable<char> delimiters, IEnumerable<char> quoteSymbols)
    {
        return InferDialect(stream, lineTerminators, delimiters, quoteSymbols, 65536, TabularFormatInfo.DefaultEncoding);
    }

    /// <summary>Asynchronously attempts to infer a dialect from the stream based on frequency of the eligible token values.</summary>
    /// <param name="stream">The stream to infer from.</param>
    /// <param name="lineTerminators">The eligible values for a line terminator.</param>
    /// <param name="delimiters">The eligible values for a delimiter.</param>
    /// <param name="quoteSymbols">The eligible values for a quote symbol.</param>
    /// <param name="sampleLength">The length of a sample in bytes.</param>
    /// <param name="encoding">The encoding for reading from the stream.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces a successfully inferred dialect or <see langword="null" />.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="lineTerminators" />, <paramref name="delimiters" />, or <paramref name="quoteSymbols" /> is <see langword="null" />.</exception>
    public static async ValueTask<TabularDialect?> InferDialectAsync(Stream stream, IEnumerable<string> lineTerminators, IEnumerable<char> delimiters, IEnumerable<char> quoteSymbols, int sampleLength, Encoding? encoding, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(lineTerminators);
        ArgumentNullException.ThrowIfNull(delimiters);
        ArgumentNullException.ThrowIfNull(quoteSymbols);

        if (!stream.CanRead)
        {
            ThrowUnreadableStreamException();
        }

        encoding ??= TabularFormatInfo.DefaultEncoding;

        await using (stream.ConfigureAwait(false))
        {
            var isStartOfStream = !stream.CanSeek || (stream.Position == 0);
            var byteBufferSize = Math.Max(1, Math.Min(sampleLength, Array.MaxLength));

            using var byteBuffer = new ArrayBuffer<byte>(byteBufferSize);

            var byteBufferUsedSize = await stream.ReadAtLeastAsync(byteBuffer.AsMemory(), byteBufferSize, false, cancellationToken).ConfigureAwait(false);
            var byteBufferUsed = byteBuffer.AsMemory(byteBufferUsedSize);

            if (isStartOfStream && byteBufferUsed.Span.StartsWith(encoding.Preamble))
            {
                byteBufferUsed = byteBufferUsed.Slice(encoding.Preamble.Length);
            }

            var charBufferSize = encoding.GetMaxCharCount(Math.Min(byteBufferUsedSize, 0x00100000));

            using var charBuffer = new ArrayBuffer<char>(charBufferSize);

            var charBufferUsedSize = encoding.GetChars(byteBufferUsed.Span, charBuffer.AsSpan());
            var charBufferUsed = charBuffer.AsMemory(charBufferUsedSize);

            return InferDialect(charBufferUsed.Span, lineTerminators.ToFrozenSet(StringComparer.Ordinal), delimiters.ToFrozenSet(), quoteSymbols.ToFrozenSet());
        }
    }

    /// <summary>Asynchronously attempts to infer a dialect from the stream based on frequency of the eligible token values.</summary>
    /// <param name="stream">The stream to infer from.</param>
    /// <param name="lineTerminators">The eligible values for a line terminator.</param>
    /// <param name="delimiters">The eligible values for a delimiter.</param>
    /// <param name="quoteSymbols">The eligible values for a quote symbol.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object that, when awaited, produces a successfully inferred dialect or <see langword="null" />.</returns>
    /// <exception cref="ArgumentException"><paramref name="stream" /> does not support reading.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="stream" />, <paramref name="lineTerminators" />, <paramref name="delimiters" />, or <paramref name="quoteSymbols" /> is <see langword="null" />.</exception>
    /// <remarks>The operation consumes the first 65536 bytes as a sample and uses a UTF-8 encoding without byte order mark (BOM).</remarks>
    public static ValueTask<TabularDialect?> InferDialectAsync(Stream stream, IEnumerable<string> lineTerminators, IEnumerable<char> delimiters, IEnumerable<char> quoteSymbols, CancellationToken cancellationToken = default)
    {
        return InferDialectAsync(stream, lineTerminators, delimiters, quoteSymbols, 65536, TabularFormatInfo.DefaultEncoding, cancellationToken);
    }

    private static TabularDialect? InferDialect(ReadOnlySpan<char> source, FrozenSet<string> tokensT, FrozenSet<char> tokensD, FrozenSet<char> tokensQ)
    {
        var countersT = new Dictionary<string, int>(tokensT.Count, tokensT.Comparer);
        var countersD = new Dictionary<char, int>(tokensD.Count);
        var countersQ = new Dictionary<char, int>(tokensQ.Count);

        foreach (var token in tokensT.Where(static x => (x is not null) && TabularDialect.IsSupportedLineTerminator(x)))
        {
            countersT[token] = source.Count(token);
        }

        foreach (var token in tokensD)
        {
            countersD[token] = source.Count(token);
        }

        foreach (var token in tokensQ)
        {
            countersQ[token] = source.Count(token);
        }

        var choicesT = countersT
            .OrderByDescending(static x => x.Value)
            .ThenByDescending(static x => x.Key.Length);

        foreach (var (tokenT, _) in choicesT)
        {
            var choicesD = countersD
                .Where(x => !tokenT.Contains(x.Key, StringComparison.Ordinal))
                .OrderByDescending(static x => x.Value);

            foreach (var (tokenD, _) in choicesD)
            {
                var choicesQ = countersQ
                    .Where(x => !tokenT.Contains(x.Key, StringComparison.Ordinal) && (x.Key != tokenD))
                    .OrderByDescending(static x => x.Value);

                foreach (var (tokenQ, _) in choicesQ)
                {
                    return new(tokenT, tokenD, tokenQ);
                }
            }
        }

        return null;
    }

    [DoesNotReturn]
    [StackTraceHidden]
    private static void ThrowUnreadableStreamException()
    {
        throw new ArgumentException("The stream does not support reading.");
    }
}
