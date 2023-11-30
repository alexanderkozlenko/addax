// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Numerics;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular;

public partial class TabularWriter
{
    /// <summary>Writes a <see cref="char" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="char" /> value to write.</param>
    public void WriteChar(char value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularCharConverter.Instance);
    }

    /// <summary>Writes a <see cref="bool" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="bool" /> value to write.</param>
    /// <remarks>The value will be represented as one of the literals: <c>"false"</c>, <c>"true"</c>.</remarks>
    public void WriteBoolean(bool value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularBooleanConverter.Instance);
    }

    /// <summary>Writes a <see cref="sbyte" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="sbyte" /> value to write.</param>
    [CLSCompliant(false)]
    public void WriteSByte(sbyte value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularSByteConverter.Instance);
    }

    /// <summary>Writes a <see cref="byte" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="byte" /> value to write.</param>
    public void WriteByte(byte value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularByteConverter.Instance);
    }

    /// <summary>Writes a <see cref="short" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="short" /> value to write.</param>
    public void WriteInt16(short value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularInt16Converter.Instance);
    }

    /// <summary>Writes a <see cref="ushort" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="ushort" /> value to write.</param>
    [CLSCompliant(false)]
    public void WriteUInt16(ushort value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularUInt16Converter.Instance);
    }

    /// <summary>Writes a <see cref="int" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="int" /> value to write.</param>
    public void WriteInt32(int value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularInt32Converter.Instance);
    }

    /// <summary>Writes a <see cref="uint" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="uint" /> value to write.</param>
    [CLSCompliant(false)]
    public void WriteUInt32(uint value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularUInt32Converter.Instance);
    }

    /// <summary>Writes a <see cref="long" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="long" /> value to write.</param>
    public void WriteInt64(long value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularInt64Converter.Instance);
    }

    /// <summary>Writes a <see cref="ulong" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="ulong" /> value to write.</param>
    [CLSCompliant(false)]
    public void WriteUInt64(ulong value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularUInt64Converter.Instance);
    }

    /// <summary>Writes a <see cref="Int128" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="Int128" /> value to write.</param>
    public void WriteInt128(Int128 value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularInt128Converter.Instance);
    }

    /// <summary>Writes a <see cref="UInt128" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="UInt128" /> value to write.</param>
    [CLSCompliant(false)]
    public void WriteUInt128(UInt128 value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularUInt128Converter.Instance);
    }

    /// <summary>Writes a <see cref="BigInteger" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="BigInteger" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted field exceeds the supported field length.</exception>
    public void WriteBigInteger(BigInteger value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularBigIntegerConverter.Instance);
    }

    /// <summary>Writes a <see cref="Half" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Half" /> value to write.</param>
    public void WriteHalf(Half value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularHalfConverter.Instance);
    }

    /// <summary>Writes a <see cref="float" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="float" /> value to write.</param>
    public void WriteSingle(float value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularSingleConverter.Instance);
    }

    /// <summary>Writes a <see cref="double" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="double" /> value to write.</param>
    public void WriteDouble(double value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularDoubleConverter.Instance);
    }

    /// <summary>Writes a <see cref="decimal" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="decimal" /> value to write.</param>
    public void WriteDecimal(decimal value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularDecimalConverter.Instance);
    }

    /// <summary>Writes a <see cref="TimeSpan" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeSpan" /> value to write.</param>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public void WriteTimeSpan(TimeSpan value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularTimeSpanConverter.Instance);
    }

    /// <summary>Writes a <see cref="TimeOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeOnly" /> value to write.</param>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public void WriteTimeOnly(TimeOnly value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularTimeOnlyConverter.Instance);
    }

    /// <summary>Writes a <see cref="DateOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateOnly" /> value to write.</param>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public void WriteDateOnly(DateOnly value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularDateOnlyConverter.Instance);
    }

    /// <summary>Writes a <see cref="DateTime" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTime" /> value to write.</param>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public void WriteDateTime(DateTime value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularDateTimeConverter.Instance);
    }

    /// <summary>Writes a <see cref="DateTimeOffset" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTimeOffset" /> value to write.</param>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public void WriteDateTimeOffset(DateTimeOffset value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularDateTimeOffsetConverter.Instance);
    }

    /// <summary>Writes a <see cref="Guid" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Guid" /> value to write.</param>
    /// <remarks>The value will be formatted according to the RFC 4122.</remarks>
    public void WriteGuid(Guid value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularGuidConverter.Instance);
    }

    /// <summary>Writes an <see cref="Uri" /> instance as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="Uri" /> instance to write.</param>
    /// <remarks>The value will be formatted according to the RFC 3986.</remarks>
    public void WriteUri(Uri? value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularUriConverter.Instance);
    }

    /// <summary>Writes binary data encoded with "base16" ("hex") encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted field exceeds the supported field length.</exception>
    /// <remarks>The value will be formatted according to the RFC 4648.</remarks>
    public void WriteBase16Binary(byte[]? value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularBase16BinaryConverter.Instance);
    }

    /// <summary>Writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted field exceeds the supported field length.</exception>
    /// <remarks>The value will be formatted according to the RFC 4648.</remarks>
    public void WriteBase64Binary(byte[]? value)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        WriteValueCore(value, TabularBase64BinaryConverter.Instance);
    }

    /// <summary>Asynchronously writes a <see cref="char" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="char" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteCharAsync(char value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularCharConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="bool" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="bool" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be represented as one of the literals: <c>"false"</c>, <c>"true"</c>.</remarks>
    public ValueTask WriteBooleanAsync(bool value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularBooleanConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="sbyte" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="sbyte" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteSByteAsync(sbyte value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularSByteConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="byte" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="byte" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteByteAsync(byte value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularByteConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="short" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="short" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt16Async(short value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularInt16Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="ushort" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="ushort" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt16Async(ushort value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularUInt16Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="int" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="int" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt32Async(int value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularInt32Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="uint" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="uint" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt32Async(uint value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularUInt32Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="long" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="long" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt64Async(long value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularInt64Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="ulong" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="ulong" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt64Async(ulong value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularUInt64Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Int128" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="Int128" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt128Async(Int128 value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularInt128Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="UInt128" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="UInt128" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt128Async(UInt128 value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularUInt128Converter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="BigInteger" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="BigInteger" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteBigIntegerAsync(BigInteger value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularBigIntegerConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Half" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Half" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteHalfAsync(Half value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularHalfConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="float" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="float" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteSingleAsync(float value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularSingleConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="double" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="double" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteDoubleAsync(double value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularDoubleConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="decimal" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="decimal" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteDecimalAsync(decimal value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularDecimalConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="TimeSpan" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeSpan" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public ValueTask WriteTimeSpanAsync(TimeSpan value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularTimeSpanConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="TimeOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeOnly" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public ValueTask WriteTimeOnlyAsync(TimeOnly value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularTimeOnlyConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="DateOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateOnly" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public ValueTask WriteDateOnlyAsync(DateOnly value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularDateOnlyConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="DateTime" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTime" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public ValueTask WriteDateTimeAsync(DateTime value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularDateTimeConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="DateTimeOffset" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTimeOffset" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 3339.</remarks>
    public ValueTask WriteDateTimeOffsetAsync(DateTimeOffset value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularDateTimeOffsetConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Guid" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Guid" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 4122.</remarks>
    public ValueTask WriteGuidAsync(Guid value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularGuidConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes an <see cref="Uri" /> instance as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="Uri" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 3986.</remarks>
    public ValueTask WriteUriAsync(Uri? value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularUriConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base16" ("hex") encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 4648.</remarks>
    public ValueTask WriteBase16BinaryAsync(byte[]? value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularBase16BinaryConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task object.</returns>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value will be formatted according to the RFC 4648.</remarks>
    public ValueTask WriteBase64BinaryAsync(byte[]? value, CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        return WriteValueCoreAsync(value, TabularBase64BinaryConverter.Instance, cancellationToken);
    }
}
