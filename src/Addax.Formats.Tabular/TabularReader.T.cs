// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular;

public partial class TabularReader
{
    /// <summary>Tries to parse the current field as a <see cref="char" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="char" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="char" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be represented as one UTF-16 code unit.</remarks>
    public bool TryGetChar(out char value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Char, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="bool" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="bool" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="bool" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be represented as one of the literals: <c>"false"</c>, <c>"true"</c>, <c>"0"</c>, <c>"1"</c>.</remarks>
    public bool TryGetBoolean(out bool value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Boolean, out value);
    }

    /// <summary>Tries to parse the current field as an <see cref="sbyte" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an <see cref="sbyte" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="sbyte" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public bool TryGetSByte(out sbyte value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.SByte, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="byte" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="byte" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="byte" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetByte(out byte value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Byte, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="short" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="short" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="short" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetInt16(out short value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Int16, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="ushort" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="ushort" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="ushort" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public bool TryGetUInt16(out ushort value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.UInt16, out value);
    }

    /// <summary>Tries to parse the current field as an <see cref="int" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an <see cref="int" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="int" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetInt32(out int value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Int32, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="uint" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="uint" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="uint" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public bool TryGetUInt32(out uint value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.UInt32, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="long" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="long" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="long" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetInt64(out long value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Int64, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="ulong" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="ulong" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="ulong" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public bool TryGetUInt64(out ulong value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.UInt64, out value);
    }

    /// <summary>Tries to parse the current field as an <see cref="Int128" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an <see cref="Int128" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Int128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetInt128(out Int128 value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Int128, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="UInt128" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="UInt128" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="UInt128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public bool TryGetUInt128(out UInt128 value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.UInt128, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="BigInteger" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="BigInteger" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="BigInteger" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetBigInteger(out BigInteger value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.BigInteger, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="Half" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Half" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Half" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetHalf(out Half value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Half, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="BFloat16" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="BFloat16" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="BFloat16" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetBFloat16(out BFloat16 value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.BFloat16, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="float" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="float" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="float" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetSingle(out float value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Single, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="double" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="double" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="double" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetDouble(out double value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Double, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="decimal" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="decimal" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="decimal" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetDecimal(out decimal value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Decimal, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="Decimal32" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Decimal32" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Decimal32" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetDecimal32(out Decimal32 value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Decimal32, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="Decimal64" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Decimal64" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Decimal64" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetDecimal64(out Decimal64 value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Decimal64, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="Decimal128" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Decimal128" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Decimal128" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public bool TryGetDecimal128(out Decimal128 value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Decimal128, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="TimeSpan" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="TimeSpan" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="TimeSpan" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to W3C and include only <c>"D"</c>, <c>"H"</c>, <c>"M"</c>, or <c>"S"</c> designators.</remarks>
    public bool TryGetTimeSpan(out TimeSpan value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.TimeSpan, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="TimeOnly" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="TimeOnly" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="TimeOnly" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset, which is ignored.</remarks>
    public bool TryGetTimeOnly(out TimeOnly value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.TimeOnly, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="DateOnly" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="DateOnly" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="DateOnly" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset, which is ignored.</remarks>
    public bool TryGetDateOnly(out DateOnly value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.DateOnly, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="DateTime" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="DateTime" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="DateTime" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset. A value with an offset is converted to Coordinated Universal Time (UTC) and has the <see cref="DateTimeKind.Utc" /> kind; a value without an offset has the <see cref="DateTimeKind.Unspecified" /> kind.</remarks>
    public bool TryGetDateTime(out DateTime value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.DateTime, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="DateTimeOffset" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="DateTimeOffset" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="DateTimeOffset" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset. A value without an offset is interpreted as Coordinated Universal Time (UTC).</remarks>
    public bool TryGetDateTimeOffset(out DateTimeOffset value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.DateTimeOffset, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="Guid" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Guid" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Guid" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to RFC 9562.</remarks>
    public bool TryGetGuid(out Guid value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Guid, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="Uri" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Uri" /> instance that represents the current field, or <see langword="null" /> on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Uri" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to RFC 3987.</remarks>
    public bool TryGetUri([NotNullWhen(true)] out Uri? value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Uri, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base16" encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an array of bytes that represents the current field, or <see langword="null" /> on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public bool TryGetBase16Array([NotNullWhen(true)] out byte[]? value)
    {
        ThrowIfNotActive();

        return TryParse(TabularBase16ArrayConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base64" encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an array of bytes that represents the current field, or <see langword="null" /> on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public bool TryGetBase64Array([NotNullWhen(true)] out byte[]? value)
    {
        ThrowIfNotActive();

        return TryParse(TabularBase64ArrayConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base16" encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a memory region of bytes that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public bool TryGetBase16Memory(out Memory<byte> value)
    {
        ThrowIfNotActive();

        return TryParse(TabularBase16MemoryConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base64" encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a memory region of bytes that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public bool TryGetBase64Memory(out Memory<byte> value)
    {
        ThrowIfNotActive();

        return TryParse(TabularBase64MemoryConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base16" encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a read-only memory region of bytes that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public bool TryGetBase16ReadOnlyMemory(out ReadOnlyMemory<byte> value)
    {
        ThrowIfNotActive();

        return TryParse(TabularBase16ReadOnlyMemoryConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base64" encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a read-only memory region of bytes that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public bool TryGetBase64ReadOnlyMemory(out ReadOnlyMemory<byte> value)
    {
        ThrowIfNotActive();

        return TryParse(TabularBase64ReadOnlyMemoryConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as a <see cref="Rune" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Rune" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <exception cref="InvalidOperationException">A previous read operation failed, or a converter for <see cref="Rune" /> cannot be resolved.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be represented as one or two UTF-16 code units.</remarks>
    public bool TryGetRune(out Rune value)
    {
        ThrowIfNotActive();

        return TryParse(_converters.Rune, out value);
    }

    /// <summary>Parses the current field as a <see cref="char" />.</summary>
    /// <returns>A <see cref="char" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="char" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be represented as one UTF-16 code unit.</remarks>
    public char GetChar()
    {
        if (!TryGetChar(out var result))
        {
            ThrowFieldFormatException<char>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="bool" />.</summary>
    /// <returns>A <see cref="bool" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="bool" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be represented as one of the literals: <c>"false"</c>, <c>"true"</c>, <c>"0"</c>, <c>"1"</c>.</remarks>
    public bool GetBoolean()
    {
        if (!TryGetBoolean(out var result))
        {
            ThrowFieldFormatException<bool>();
        }

        return result;
    }

    /// <summary>Parses the current field as an <see cref="sbyte" />.</summary>
    /// <returns>An <see cref="sbyte" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="sbyte" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public sbyte GetSByte()
    {
        if (!TryGetSByte(out var result))
        {
            ThrowFieldFormatException<sbyte>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="byte" />.</summary>
    /// <returns>A <see cref="byte" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="byte" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public byte GetByte()
    {
        if (!TryGetByte(out var result))
        {
            ThrowFieldFormatException<byte>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="short" />.</summary>
    /// <returns>A <see cref="short" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="short" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public short GetInt16()
    {
        if (!TryGetInt16(out var result))
        {
            ThrowFieldFormatException<short>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="ushort" />.</summary>
    /// <returns>A <see cref="ushort" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="ushort" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public ushort GetUInt16()
    {
        if (!TryGetUInt16(out var result))
        {
            ThrowFieldFormatException<ushort>();
        }

        return result;
    }

    /// <summary>Parses the current field as an <see cref="int" />.</summary>
    /// <returns>An <see cref="int" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="int" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public int GetInt32()
    {
        if (!TryGetInt32(out var result))
        {
            ThrowFieldFormatException<int>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="uint" />.</summary>
    /// <returns>A <see cref="uint" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="uint" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public uint GetUInt32()
    {
        if (!TryGetUInt32(out var result))
        {
            ThrowFieldFormatException<uint>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="long" />.</summary>
    /// <returns>A <see cref="long" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="long" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public long GetInt64()
    {
        if (!TryGetInt64(out var result))
        {
            ThrowFieldFormatException<long>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="ulong" />.</summary>
    /// <returns>A <see cref="ulong" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="ulong" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public ulong GetUInt64()
    {
        if (!TryGetUInt64(out var result))
        {
            ThrowFieldFormatException<ulong>();
        }

        return result;
    }

    /// <summary>Parses the current field as an <see cref="Int128" />.</summary>
    /// <returns>An <see cref="Int128" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="Int128" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public Int128 GetInt128()
    {
        if (!TryGetInt128(out var result))
        {
            ThrowFieldFormatException<Int128>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="UInt128" />.</summary>
    /// <returns>A <see cref="UInt128" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="UInt128" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    [CLSCompliant(false)]
    public UInt128 GetUInt128()
    {
        if (!TryGetUInt128(out var result))
        {
            ThrowFieldFormatException<UInt128>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="BigInteger" />.</summary>
    /// <returns>A <see cref="BigInteger" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="BigInteger" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public BigInteger GetBigInteger()
    {
        if (!TryGetBigInteger(out var result))
        {
            ThrowFieldFormatException<BigInteger>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="Half" />.</summary>
    /// <returns>A <see cref="Half" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Half" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public Half GetHalf()
    {
        if (!TryGetHalf(out var result))
        {
            ThrowFieldFormatException<Half>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="BFloat16" />.</summary>
    /// <returns>A <see cref="BFloat16" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="BFloat16" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public BFloat16 GetBFloat16()
    {
        if (!TryGetBFloat16(out var result))
        {
            ThrowFieldFormatException<BFloat16>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="float" />.</summary>
    /// <returns>A <see cref="float" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="float" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public float GetSingle()
    {
        if (!TryGetSingle(out var result))
        {
            ThrowFieldFormatException<float>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="double" />.</summary>
    /// <returns>A <see cref="double" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="double" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public double GetDouble()
    {
        if (!TryGetDouble(out var result))
        {
            ThrowFieldFormatException<double>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="decimal" />.</summary>
    /// <returns>A <see cref="decimal" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="decimal" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public decimal GetDecimal()
    {
        if (!TryGetDecimal(out var result))
        {
            ThrowFieldFormatException<decimal>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="Decimal32" />.</summary>
    /// <returns>A <see cref="Decimal32" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Decimal32" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public Decimal32 GetDecimal32()
    {
        if (!TryGetDecimal32(out var result))
        {
            ThrowFieldFormatException<Decimal32>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="Decimal64" />.</summary>
    /// <returns>A <see cref="Decimal64" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Decimal64" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public Decimal64 GetDecimal64()
    {
        if (!TryGetDecimal64(out var result))
        {
            ThrowFieldFormatException<Decimal64>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="Decimal128" />.</summary>
    /// <returns>A <see cref="Decimal128" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Decimal128" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    public Decimal128 GetDecimal128()
    {
        if (!TryGetDecimal128(out var result))
        {
            ThrowFieldFormatException<Decimal128>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="TimeSpan" />.</summary>
    /// <returns>A <see cref="TimeSpan" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="TimeSpan" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to W3C and include only <c>"D"</c>, <c>"H"</c>, <c>"M"</c>, or <c>"S"</c> designators.</remarks>
    public TimeSpan GetTimeSpan()
    {
        if (!TryGetTimeSpan(out var result))
        {
            ThrowFieldFormatException<TimeSpan>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="TimeOnly" />.</summary>
    /// <returns>A <see cref="TimeOnly" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="TimeOnly" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset, which is ignored.</remarks>
    public TimeOnly GetTimeOnly()
    {
        if (!TryGetTimeOnly(out var result))
        {
            ThrowFieldFormatException<TimeOnly>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="DateOnly" />.</summary>
    /// <returns>A <see cref="DateOnly" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="DateOnly" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset, which is ignored.</remarks>
    public DateOnly GetDateOnly()
    {
        if (!TryGetDateOnly(out var result))
        {
            ThrowFieldFormatException<DateOnly>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="DateTime" />.</summary>
    /// <returns>A <see cref="DateTime" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="DateTime" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset. A value with an offset is converted to Coordinated Universal Time (UTC) and has the <see cref="DateTimeKind.Utc" /> kind; a value without an offset has the <see cref="DateTimeKind.Unspecified" /> kind.</remarks>
    public DateTime GetDateTime()
    {
        if (!TryGetDateTime(out var result))
        {
            ThrowFieldFormatException<DateTime>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="DateTimeOffset" />.</summary>
    /// <returns>A <see cref="DateTimeOffset" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="DateTimeOffset" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to ISO 8601-1:2019, with an optional time zone offset. A value without an offset is interpreted as Coordinated Universal Time (UTC).</remarks>
    public DateTimeOffset GetDateTimeOffset()
    {
        if (!TryGetDateTimeOffset(out var result))
        {
            ThrowFieldFormatException<DateTimeOffset>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="Guid" />.</summary>
    /// <returns>A <see cref="Guid" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Guid" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to RFC 9562.</remarks>
    public Guid GetGuid()
    {
        if (!TryGetGuid(out var result))
        {
            ThrowFieldFormatException<Guid>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="Uri" />.</summary>
    /// <returns>A <see cref="Uri" /> instance.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Uri" /> instance.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be formatted according to RFC 3987.</remarks>
    public Uri GetUri()
    {
        if (!TryGetUri(out var result))
        {
            ThrowFieldFormatException<Uri>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base16" encoding.</summary>
    /// <returns>An array of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an array of bytes using "base16" encoding.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public byte[] GetBase16Array()
    {
        if (!TryGetBase16Array(out var result))
        {
            ThrowFieldFormatException<byte[]>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base64" encoding.</summary>
    /// <returns>An array of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an array of bytes using "base64" encoding.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public byte[] GetBase64Array()
    {
        if (!TryGetBase64Array(out var result))
        {
            ThrowFieldFormatException<byte[]>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base16" encoding.</summary>
    /// <returns>A memory region of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a memory region of bytes using "base16" encoding.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public Memory<byte> GetBase16Memory()
    {
        if (!TryGetBase16Memory(out var result))
        {
            ThrowFieldFormatException<Memory<byte>>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base64" encoding.</summary>
    /// <returns>A memory region of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a memory region of bytes using "base64" encoding.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public Memory<byte> GetBase64Memory()
    {
        if (!TryGetBase64Memory(out var result))
        {
            ThrowFieldFormatException<Memory<byte>>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base16" encoding.</summary>
    /// <returns>A read-only memory region of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a read-only memory region of bytes using "base16" encoding.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public ReadOnlyMemory<byte> GetBase16ReadOnlyMemory()
    {
        if (!TryGetBase16ReadOnlyMemory(out var result))
        {
            ThrowFieldFormatException<ReadOnlyMemory<byte>>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base64" encoding.</summary>
    /// <returns>A read-only memory region of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a read-only memory region of bytes using "base64" encoding.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>The field must be formatted according to RFC 4648.</remarks>
    public ReadOnlyMemory<byte> GetBase64ReadOnlyMemory()
    {
        if (!TryGetBase64ReadOnlyMemory(out var result))
        {
            ThrowFieldFormatException<ReadOnlyMemory<byte>>();
        }

        return result;
    }

    /// <summary>Parses the current field as a <see cref="Rune" />.</summary>
    /// <returns>A <see cref="Rune" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Rune" /> value.</exception>
    /// <exception cref="InvalidOperationException">A previous read operation failed.</exception>
    /// <exception cref="ObjectDisposedException">This instance has been disposed.</exception>
    /// <remarks>By default, the field must be represented as one or two UTF-16 code units.</remarks>
    public Rune GetRune()
    {
        if (!TryGetRune(out var result))
        {
            ThrowFieldFormatException<Rune>();
        }

        return result;
    }
}
