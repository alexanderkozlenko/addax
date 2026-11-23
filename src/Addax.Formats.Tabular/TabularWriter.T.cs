// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular;

public partial class TabularWriter
{
    /// <summary>Writes a <see cref="char" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="char" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="char" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteChar(char value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Char);
    }

    /// <summary>Writes a <see cref="bool" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="bool" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="bool" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the value is written as one of the literals: <c>"false"</c>, <c>"true"</c>.</remarks>
    public void WriteBoolean(bool value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Boolean);
    }

    /// <summary>Writes an <see cref="sbyte" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="sbyte" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="sbyte" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public void WriteSByte(sbyte value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.SByte);
    }

    /// <summary>Writes a <see cref="byte" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="byte" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="byte" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteByte(byte value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Byte);
    }

    /// <summary>Writes a <see cref="short" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="short" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="short" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteInt16(short value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Int16);
    }

    /// <summary>Writes a <see cref="ushort" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ushort" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="ushort" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public void WriteUInt16(ushort value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.UInt16);
    }

    /// <summary>Writes an <see cref="int" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="int" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="int" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteInt32(int value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Int32);
    }

    /// <summary>Writes a <see cref="uint" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="uint" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="uint" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public void WriteUInt32(uint value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.UInt32);
    }

    /// <summary>Writes a <see cref="long" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="long" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="long" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteInt64(long value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Int64);
    }

    /// <summary>Writes a <see cref="ulong" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ulong" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="ulong" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public void WriteUInt64(ulong value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.UInt64);
    }

    /// <summary>Writes an <see cref="Int128" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="Int128" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Int128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteInt128(Int128 value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Int128);
    }

    /// <summary>Writes a <see cref="UInt128" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="UInt128" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="UInt128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public void WriteUInt128(UInt128 value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.UInt128);
    }

    /// <summary>Writes a <see cref="BigInteger" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="BigInteger" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="BigInteger" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteBigInteger(BigInteger value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.BigInteger);
    }

    /// <summary>Writes a <see cref="Half" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Half" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Half" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteHalf(Half value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Half);
    }

    /// <summary>Writes a <see cref="BFloat16" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="BFloat16" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="BFloat16" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteBFloat16(BFloat16 value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.BFloat16);
    }

    /// <summary>Writes a <see cref="float" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="float" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="float" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteSingle(float value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Single);
    }

    /// <summary>Writes a <see cref="double" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="double" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="double" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteDouble(double value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Double);
    }

    /// <summary>Writes a <see cref="decimal" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="decimal" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="decimal" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteDecimal(decimal value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Decimal);
    }

    /// <summary>Writes a <see cref="Decimal32" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Decimal32" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Decimal32" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteDecimal32(Decimal32 value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Decimal32);
    }

    /// <summary>Writes a <see cref="Decimal64" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Decimal64" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Decimal64" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteDecimal64(Decimal64 value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Decimal64);
    }

    /// <summary>Writes a <see cref="Decimal128" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Decimal128" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Decimal128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteDecimal128(Decimal128 value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Decimal128);
    }

    /// <summary>Writes a <see cref="TimeSpan" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeSpan" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="TimeSpan" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the value is formatted according to W3C.</remarks>
    public void WriteTimeSpan(TimeSpan value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.TimeSpan);
    }

    /// <summary>Writes a <see cref="TimeOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeOnly" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="TimeOnly" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public void WriteTimeOnly(TimeOnly value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.TimeOnly);
    }

    /// <summary>Writes a <see cref="DateOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateOnly" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="DateOnly" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public void WriteDateOnly(DateOnly value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.DateOnly);
    }

    /// <summary>Writes a <see cref="DateTime" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTime" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="DateTime" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public void WriteDateTime(DateTime value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.DateTime);
    }

    /// <summary>Writes a <see cref="DateTimeOffset" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTimeOffset" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="DateTimeOffset" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public void WriteDateTimeOffset(DateTimeOffset value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.DateTimeOffset);
    }

    /// <summary>Writes a <see cref="Guid" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Guid" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Guid" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the value is formatted according to RFC 9562.</remarks>
    public void WriteGuid(Guid value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Guid);
    }

    /// <summary>Writes a <see cref="Uri" /> instance as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Uri" /> instance to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Uri" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, an absolute URI is formatted according to RFC 3987, and a relative URI is formatted as its original string.</remarks>
    public void WriteUri(Uri? value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Uri);
    }

    /// <summary>Writes binary data encoded with "base16" encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public void WriteBase16Array(byte[]? value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, TabularBase16ArrayConverter.Instance);
    }

    /// <summary>Writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public void WriteBase64Array(byte[]? value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, TabularBase64ArrayConverter.Instance);
    }

    /// <summary>Writes binary data encoded with "base16" encoding as the next value field of the current record.</summary>
    /// <param name="value">A memory region of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public void WriteBase16Memory(Memory<byte> value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, TabularBase16MemoryConverter.Instance);
    }

    /// <summary>Writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">A memory region of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public void WriteBase64Memory(Memory<byte> value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, TabularBase64MemoryConverter.Instance);
    }

    /// <summary>Writes binary data encoded with "base16" encoding as the next value field of the current record.</summary>
    /// <param name="value">A read-only memory region of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public void WriteBase16ReadOnlyMemory(ReadOnlyMemory<byte> value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, TabularBase16ReadOnlyMemoryConverter.Instance);
    }

    /// <summary>Writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">A read-only memory region of bytes to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public void WriteBase64ReadOnlyMemory(ReadOnlyMemory<byte> value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, TabularBase64ReadOnlyMemoryConverter.Instance);
    }

    /// <summary>Writes a <see cref="Rune" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Rune" /> value to write.</param>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Rune" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public void WriteRune(Rune value)
    {
        ThrowIfNotActive();

        WriteValueCore(value, _converters.Rune);
    }

    /// <summary>Asynchronously writes a <see cref="char" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="char" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="char" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteCharAsync(char value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Char, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="bool" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="bool" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="bool" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, the value is written as one of the literals: <c>"false"</c>, <c>"true"</c>.</remarks>
    public ValueTask WriteBooleanAsync(bool value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Boolean, cancellationToken);
    }

    /// <summary>Asynchronously writes an <see cref="sbyte" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="sbyte" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="sbyte" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteSByteAsync(sbyte value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.SByte, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="byte" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="byte" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="byte" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteByteAsync(byte value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Byte, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="short" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="short" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="short" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt16Async(short value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Int16, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="ushort" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ushort" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="ushort" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt16Async(ushort value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.UInt16, cancellationToken);
    }

    /// <summary>Asynchronously writes an <see cref="int" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="int" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="int" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt32Async(int value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Int32, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="uint" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="uint" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="uint" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt32Async(uint value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.UInt32, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="long" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="long" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="long" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt64Async(long value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Int64, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="ulong" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="ulong" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="ulong" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt64Async(ulong value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.UInt64, cancellationToken);
    }

    /// <summary>Asynchronously writes an <see cref="Int128" /> value as the next value field of the current record.</summary>
    /// <param name="value">An <see cref="Int128" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Int128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteInt128Async(Int128 value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Int128, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="UInt128" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="UInt128" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="UInt128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    [CLSCompliant(false)]
    public ValueTask WriteUInt128Async(UInt128 value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.UInt128, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="BigInteger" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="BigInteger" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="BigInteger" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteBigIntegerAsync(BigInteger value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.BigInteger, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Half" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Half" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Half" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteHalfAsync(Half value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Half, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="BFloat16" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="BFloat16" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="BFloat16" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteBFloat16Async(BFloat16 value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.BFloat16, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="float" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="float" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="float" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteSingleAsync(float value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Single, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="double" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="double" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="double" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteDoubleAsync(double value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Double, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="decimal" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="decimal" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="decimal" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteDecimalAsync(decimal value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Decimal, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Decimal32" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Decimal32" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Decimal32" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteDecimal32Async(Decimal32 value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Decimal32, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Decimal64" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Decimal64" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Decimal64" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteDecimal64Async(Decimal64 value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Decimal64, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Decimal128" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Decimal128" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Decimal128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteDecimal128Async(Decimal128 value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Decimal128, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="TimeSpan" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeSpan" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="TimeSpan" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, the value is formatted according to W3C.</remarks>
    public ValueTask WriteTimeSpanAsync(TimeSpan value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.TimeSpan, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="TimeOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="TimeOnly" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="TimeOnly" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public ValueTask WriteTimeOnlyAsync(TimeOnly value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.TimeOnly, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="DateOnly" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateOnly" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="DateOnly" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public ValueTask WriteDateOnlyAsync(DateOnly value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.DateOnly, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="DateTime" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTime" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="DateTime" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public ValueTask WriteDateTimeAsync(DateTime value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.DateTime, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="DateTimeOffset" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="DateTimeOffset" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="DateTimeOffset" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, the value is formatted according to ISO 8601-1:2019.</remarks>
    public ValueTask WriteDateTimeOffsetAsync(DateTimeOffset value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.DateTimeOffset, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Guid" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Guid" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Guid" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, the value is formatted according to RFC 9562.</remarks>
    public ValueTask WriteGuidAsync(Guid value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Guid, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Uri" /> instance as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Uri" /> instance to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Uri" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>By default, an absolute URI is formatted according to RFC 3987, and a relative URI is formatted as its original string.</remarks>
    public ValueTask WriteUriAsync(Uri? value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Uri, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base16" encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public ValueTask WriteBase16ArrayAsync(byte[]? value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, TabularBase16ArrayConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">An array of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public ValueTask WriteBase64ArrayAsync(byte[]? value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, TabularBase64ArrayConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base16" encoding as the next value field of the current record.</summary>
    /// <param name="value">A memory region of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public ValueTask WriteBase16MemoryAsync(Memory<byte> value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, TabularBase16MemoryConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">A memory region of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public ValueTask WriteBase64MemoryAsync(Memory<byte> value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, TabularBase64MemoryConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base16" encoding as the next value field of the current record.</summary>
    /// <param name="value">A read-only memory region of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public ValueTask WriteBase16ReadOnlyMemoryAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, TabularBase16ReadOnlyMemoryConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes binary data encoded with "base64" encoding as the next value field of the current record.</summary>
    /// <param name="value">A read-only memory region of bytes to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    /// <remarks>The value is formatted according to RFC 4648.</remarks>
    public ValueTask WriteBase64ReadOnlyMemoryAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, TabularBase64ReadOnlyMemoryConverter.Instance, cancellationToken);
    }

    /// <summary>Asynchronously writes a <see cref="Rune" /> value as the next value field of the current record.</summary>
    /// <param name="value">A <see cref="Rune" /> value to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentException">The formatted value exceeds the supported field length. This exception is stored into the returned task.</exception>
    /// <exception cref="FormatException">The value cannot be formatted into a character sequence. This exception is stored into the returned task.</exception>
    /// <exception cref="InvalidOperationException">A previous write operation failed, or a converter for <see cref="Rune" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled. This exception is stored into the returned task.</exception>
    public ValueTask WriteRuneAsync(Rune value, CancellationToken cancellationToken = default)
    {
        ThrowIfNotActive();

        return WriteValueCoreAsync(value, _converters.Rune, cancellationToken);
    }
}
