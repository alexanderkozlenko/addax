// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular;

public partial class TabularReader
{
    /// <summary>Tries to parse the current field as <see cref="char" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="char" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be represented as one UTF-16 code unit.</remarks>
    public bool TryGetChar(out char value)
    {
        return TryGet(_currentField.AsSpan(), TabularCharConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="bool" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="bool" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be represented as one of the literals: <c>"false"</c>, <c>"true"</c>, <c>"0"</c>, <c>"1"</c>. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetBoolean(out bool value)
    {
        return TryGet(_currentField.AsSpan(), TabularBooleanConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="sbyte" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="sbyte" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, percent sign are allowed.</remarks>
    [CLSCompliant(false)]
    public bool TryGetSByte(out sbyte value)
    {
        return TryGet(_currentField.AsSpan(), TabularSByteConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="byte" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="byte" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, percent sign are allowed.</remarks>
    public bool TryGetByte(out byte value)
    {
        return TryGet(_currentField.AsSpan(), TabularByteConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="short" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="short" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetInt16(out short value)
    {
        return TryGet(_currentField.AsSpan(), TabularInt16Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="ushort" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="ushort" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt16(out ushort value)
    {
        return TryGet(_currentField.AsSpan(), TabularUInt16Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="int" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="int" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetInt32(out int value)
    {
        return TryGet(_currentField.AsSpan(), TabularInt32Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="uint" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="uint" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt32(out uint value)
    {
        return TryGet(_currentField.AsSpan(), TabularUInt32Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="long" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="long" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetInt64(out long value)
    {
        return TryGet(_currentField.AsSpan(), TabularInt64Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="ulong" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="ulong" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt64(out ulong value)
    {
        return TryGet(_currentField.AsSpan(), TabularUInt64Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="Int128" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Int128" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetInt128(out Int128 value)
    {
        return TryGet(_currentField.AsSpan(), TabularInt128Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="UInt128" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="UInt128" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt128(out UInt128 value)
    {
        return TryGet(_currentField.AsSpan(), TabularUInt128Converter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="BigInteger" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="BigInteger" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetBigInteger(out BigInteger value)
    {
        return TryGet(_currentField.AsSpan(), TabularBigIntegerConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="Half" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Half" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetHalf(out Half value)
    {
        return TryGet(_currentField.AsSpan(), TabularHalfConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="float" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="float" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetSingle(out float value)
    {
        return TryGet(_currentField.AsSpan(), TabularSingleConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="double" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="double" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetDouble(out double value)
    {
        return TryGet(_currentField.AsSpan(), TabularDoubleConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="decimal" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="decimal" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public bool TryGetDecimal(out decimal value)
    {
        return TryGet(_currentField.AsSpan(), TabularDecimalConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="TimeSpan" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="TimeSpan" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 3339 and include only <c>"D"</c>, <c>"H"</c>, <c>"M"</c>, or <c>"S"</c> designators. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetTimeSpan(out TimeSpan value)
    {
        return TryGet(_currentField.AsSpan(), TabularTimeSpanConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="TimeOnly" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="TimeOnly" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetTimeOnly(out TimeOnly value)
    {
        return TryGet(_currentField.AsSpan(), TabularTimeOnlyConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="DateOnly" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="DateOnly" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetDateOnly(out DateOnly value)
    {
        return TryGet(_currentField.AsSpan(), TabularDateOnlyConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="DateTime" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="DateTime" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetDateTime(out DateTime value)
    {
        return TryGet(_currentField.AsSpan(), TabularDateTimeConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="DateTimeOffset" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="DateTimeOffset" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetDateTimeOffset(out DateTimeOffset value)
    {
        return TryGet(_currentField.AsSpan(), TabularDateTimeOffsetConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="Guid" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains a <see cref="Guid" /> value that represents the current field, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 4122. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetGuid(out Guid value)
    {
        return TryGet(_currentField.AsSpan(), TabularGuidConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as <see cref="Uri" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an <see cref="Uri" /> instance that represents the current field, or <see langword="null" /> on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 3986. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetUri([NotNullWhen(true)] out Uri? value)
    {
        return TryGet(_currentField.AsSpan(), TabularUriConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base16" ("hex") encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an array of bytes that represents the current field, or <see langword="null" /> on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 4648. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetBase16Binary([NotNullWhen(true)] out byte[]? value)
    {
        return TryGet(_currentField.AsSpan(), TabularBase16BinaryConverter.Instance, out value);
    }

    /// <summary>Tries to parse the current field as binary data encoded with "base64" encoding and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="value">When this method returns, contains an array of bytes that represents the current field, or <see langword="null" /> on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if the field was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The value should be formatted according to the RFC 4648. Leading and trailing white-space characters are allowed.</remarks>
    public bool TryGetBase64Binary([NotNullWhen(true)] out byte[]? value)
    {
        return TryGet(_currentField.AsSpan(), TabularBase64BinaryConverter.Instance, out value);
    }

    /// <summary>Parses the current field as <see cref="char" />.</summary>
    /// <returns>A <see cref="char" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="char" /> value.</exception>
    /// <remarks>The field must be represented as one UTF-16 code unit.</remarks>
    public char GetChar()
    {
        if (!TryGetChar(out var result))
        {
            ThrowFieldFormatException<char>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="bool" />.</summary>
    /// <returns>A <see cref="bool" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="bool" /> value.</exception>
    /// <remarks>The field must be represented as one of the literals: <c>"false"</c>, <c>"true"</c>, <c>"0"</c>, <c>"1"</c>. Leading and trailing white-space characters are allowed.</remarks>
    public bool GetBoolean()
    {
        if (!TryGetBoolean(out var result))
        {
            ThrowFieldFormatException<bool>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="sbyte" />.</summary>
    /// <returns>A <see cref="sbyte" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="sbyte" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, percent sign are allowed.</remarks>
    [CLSCompliant(false)]
    public sbyte GetSByte()
    {
        if (!TryGetSByte(out var result))
        {
            ThrowFieldFormatException<sbyte>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="byte" />.</summary>
    /// <returns>A <see cref="byte" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="byte" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, percent sign are allowed.</remarks>
    public byte GetByte()
    {
        if (!TryGetByte(out var result))
        {
            ThrowFieldFormatException<byte>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="short" />.</summary>
    /// <returns>A <see cref="short" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="short" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public short GetInt16()
    {
        if (!TryGetInt16(out var result))
        {
            ThrowFieldFormatException<short>();
        }

        return result;
    }

    /// <summary>Parses the current field as an <see cref="ushort" />.</summary>
    /// <returns>A <see cref="ushort" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="ushort" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
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
    /// <returns>A <see cref="int" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="int" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public int GetInt32()
    {
        if (!TryGetInt32(out var result))
        {
            ThrowFieldFormatException<int>();
        }

        return result;
    }

    /// <summary>Parses the current field as an <see cref="uint" />.</summary>
    /// <returns>A <see cref="uint" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="uint" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    [CLSCompliant(false)]
    public uint GetUInt32()
    {
        if (!TryGetUInt32(out var result))
        {
            ThrowFieldFormatException<uint>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="long" />.</summary>
    /// <returns>A <see cref="long" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="long" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public long GetInt64()
    {
        if (!TryGetInt64(out var result))
        {
            ThrowFieldFormatException<long>();
        }

        return result;
    }

    /// <summary>Parses the current field as an <see cref="ulong" />.</summary>
    /// <returns>A <see cref="ulong" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="ulong" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
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
    /// <returns>A <see cref="Int128" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="Int128" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public Int128 GetInt128()
    {
        if (!TryGetInt128(out var result))
        {
            ThrowFieldFormatException<Int128>();
        }

        return result;
    }

    /// <summary>Parses the current field as an <see cref="UInt128" />.</summary>
    /// <returns>A <see cref="UInt128" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="UInt128" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    [CLSCompliant(false)]
    public UInt128 GetUInt128()
    {
        if (!TryGetUInt128(out var result))
        {
            ThrowFieldFormatException<UInt128>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="BigInteger" />.</summary>
    /// <returns>A <see cref="BigInteger" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="BigInteger" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public BigInteger GetBigInteger()
    {
        if (!TryGetBigInteger(out var result))
        {
            ThrowFieldFormatException<BigInteger>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="Half" />.</summary>
    /// <returns>A <see cref="Half" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Half" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public Half GetHalf()
    {
        if (!TryGetHalf(out var result))
        {
            ThrowFieldFormatException<Half>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="float" />.</summary>
    /// <returns>A <see cref="float" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="float" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public float GetSingle()
    {
        if (!TryGetSingle(out var result))
        {
            ThrowFieldFormatException<float>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="double" />.</summary>
    /// <returns>A <see cref="double" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="double" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public double GetDouble()
    {
        if (!TryGetDouble(out var result))
        {
            ThrowFieldFormatException<double>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="decimal" />.</summary>
    /// <returns>A <see cref="decimal" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="decimal" /> value.</exception>
    /// <remarks>Leading and trailing white-space characters, group separators, percent and per mille signs are allowed.</remarks>
    public decimal GetDecimal()
    {
        if (!TryGetDecimal(out var result))
        {
            ThrowFieldFormatException<decimal>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="TimeSpan" />.</summary>
    /// <returns>A <see cref="TimeSpan" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="TimeSpan" /> value.</exception>
    /// <remarks>The field must be formatted according to the RFC 3339 and include only <c>"D"</c>, <c>"H"</c>, <c>"M"</c>, or <c>"S"</c> designators. Leading and trailing white-space characters are allowed.</remarks>
    public TimeSpan GetTimeSpan()
    {
        if (!TryGetTimeSpan(out var result))
        {
            ThrowFieldFormatException<TimeSpan>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="TimeOnly" />.</summary>
    /// <returns>A <see cref="TimeOnly" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="TimeOnly" /> value.</exception>
    /// <remarks>The field must be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public TimeOnly GetTimeOnly()
    {
        if (!TryGetTimeOnly(out var result))
        {
            ThrowFieldFormatException<TimeOnly>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="DateOnly" />.</summary>
    /// <returns>A <see cref="DateOnly" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="DateOnly" /> value.</exception>
    /// <remarks>The field must be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public DateOnly GetDateOnly()
    {
        if (!TryGetDateOnly(out var result))
        {
            ThrowFieldFormatException<DateOnly>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="DateTime" />.</summary>
    /// <returns>A <see cref="DateTime" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="DateTime" /> value.</exception>
    /// <remarks>The field must be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public DateTime GetDateTime()
    {
        if (!TryGetDateTime(out var result))
        {
            ThrowFieldFormatException<DateTime>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="DateTimeOffset" />.</summary>
    /// <returns>A <see cref="DateTimeOffset" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="DateTimeOffset" /> value.</exception>
    /// <remarks>The field must be formatted according to the RFC 3339. Leading and trailing white-space characters are allowed.</remarks>
    public DateTimeOffset GetDateTimeOffset()
    {
        if (!TryGetDateTimeOffset(out var result))
        {
            ThrowFieldFormatException<DateTimeOffset>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="Guid" />.</summary>
    /// <returns>A <see cref="Guid" /> value.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as a <see cref="Guid" /> value.</exception>
    /// <remarks>The field must be formatted according to the RFC 4122. Leading and trailing white-space characters are allowed.</remarks>
    public Guid GetGuid()
    {
        if (!TryGetGuid(out var result))
        {
            ThrowFieldFormatException<Guid>();
        }

        return result;
    }

    /// <summary>Parses the current field as <see cref="Uri" />.</summary>
    /// <returns>An <see cref="Uri" /> instance.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an <see cref="Uri" /> instance.</exception>
    /// <remarks>The field must be formatted according to the RFC 3986. Leading and trailing white-space characters are allowed.</remarks>
    public Uri GetUri()
    {
        if (!TryGetUri(out var result))
        {
            ThrowFieldFormatException<Uri>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base16" ("hex") encoding.</summary>
    /// <returns>An array of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an array of bytes using "base16" ("hex") encoding.</exception>
    /// <remarks>The field must be formatted according to the RFC 4648. Leading and trailing white-space characters are allowed.</remarks>
    public byte[] GetBase16Binary()
    {
        if (!TryGetBase16Binary(out var result))
        {
            ThrowFieldFormatException<byte[]>();
        }

        return result;
    }

    /// <summary>Parses the current field as binary data encoded with "base64" encoding.</summary>
    /// <returns>An array of bytes.</returns>
    /// <exception cref="FormatException">The current field cannot be parsed as an array of bytes using "base64" encoding.</exception>
    /// <remarks>The field must be formatted according to the RFC 4648. Leading and trailing white-space characters are allowed.</remarks>
    public byte[] GetBase64Binary()
    {
        if (!TryGetBase64Binary(out var result))
        {
            ThrowFieldFormatException<byte[]>();
        }

        return result;
    }
}
