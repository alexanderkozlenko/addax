// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Numerics;
using System.Text;

namespace Addax.Formats.Tabular;

public partial class TabularFieldWriter
{
    /// <summary>Writes the specified <see cref="char" /> value to the internal buffer.</summary>
    /// <param name="value">The value to be written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void WriteChar(char value)
    {
        Write(value, SelectConverter<char>());
    }

    /// <summary>Writes the specified <see cref="Rune" /> value to the internal buffer.</summary>
    /// <param name="value">The value to be written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void WriteRune(Rune value)
    {
        Write(value, SelectConverter<Rune>());
    }

    /// <summary>Writes the specified <see cref="string" /> value to the internal buffer.</summary>
    /// <param name="value">The value to be written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    public void WriteString(string? value)
    {
        Write(value.AsSpan());
    }

    /// <summary>Formats and writes the specified <see cref="bool" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be represented as one of the supported literals: <c>"true"</c>, <c>"false"</c>.</remarks>
    public void WriteBoolean(bool value)
    {
        Write(value, SelectConverter<bool>());
    }

    /// <summary>Formats and writes the specified <see cref="sbyte" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public void WriteSByte(sbyte value)
    {
        Write(value, SelectConverter<sbyte>());
    }

    /// <summary>Formats and writes the specified <see cref="byte" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    public void WriteByte(byte value)
    {
        Write(value, SelectConverter<byte>());
    }

    /// <summary>Formats and writes the specified <see cref="short" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    public void WriteInt16(short value)
    {
        Write(value, SelectConverter<short>());
    }

    /// <summary>Formats and writes the specified <see cref="ushort" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public void WriteUInt16(ushort value)
    {
        Write(value, SelectConverter<ushort>());
    }

    /// <summary>Formats and writes the specified <see cref="int" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    public void WriteInt32(int value)
    {
        Write(value, SelectConverter<int>());
    }

    /// <summary>Formats and writes the specified <see cref="uint" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public void WriteUInt32(uint value)
    {
        Write(value, SelectConverter<uint>());
    }

    /// <summary>Formats and writes the specified <see cref="long" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    public void WriteInt64(long value)
    {
        Write(value, SelectConverter<long>());
    }

    /// <summary>Formats and writes the specified <see cref="ulong" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public void WriteUInt64(ulong value)
    {
        Write(value, SelectConverter<ulong>());
    }

    /// <summary>Formats and writes the specified <see cref="Int128" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    public void WriteInt128(Int128 value)
    {
        Write(value, SelectConverter<Int128>());
    }

    /// <summary>Formats and writes the specified <see cref="UInt128" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public void WriteUInt128(UInt128 value)
    {
        Write(value, SelectConverter<UInt128>());
    }

    /// <summary>Formats and writes the specified <see cref="BigInteger" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="FormatException">The specified value cannot be formatted as a sequence of characters.</exception>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted using invariant culture.</remarks>
    public void WriteBigInteger(BigInteger value)
    {
        Write(value, SelectConverter<BigInteger>());
    }

    /// <summary>Formats and writes the specified <see cref="Half" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted to the more compact of either fixed-point or scientific notation using invariant culture.</remarks>
    public void WriteHalf(Half value)
    {
        Write(value, SelectConverter<Half>());
    }

    /// <summary>Formats and writes the specified <see cref="float" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted to the more compact of either fixed-point or scientific notation using invariant culture.</remarks>
    public void WriteSingle(float value)
    {
        Write(value, SelectConverter<float>());
    }

    /// <summary>Formats and writes the specified <see cref="double" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted to the more compact of either fixed-point or scientific notation using invariant culture.</remarks>
    public void WriteDouble(double value)
    {
        Write(value, SelectConverter<double>());
    }

    /// <summary>Formats and writes the specified <see cref="decimal" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted to the more compact of either fixed-point or scientific notation using invariant culture.</remarks>
    public void WriteDecimal(decimal value)
    {
        Write(value, SelectConverter<decimal>());
    }

    /// <summary>Formats and writes the specified <see cref="Complex" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted in a ISO 80000-2:2019 compliant format with real numbers formatted to the more compact of either fixed-point or scientific notation using invariant culture (e.g., <c>"0.54+0.84i"</c>).</remarks>
    public void WriteComplex(Complex value)
    {
        Write(value, SelectConverter<Complex>());
    }

    /// <summary>Formats and writes the specified <see cref="TimeSpan" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"PT13H13M12S"</c> for <c>13:12:12.0000000</c>). The value can include only D, H, M, and S designators; the precision is lower than or equal to 100 ns.</remarks>
    public void WriteTimeSpan(TimeSpan value)
    {
        Write(value, SelectConverter<TimeSpan>());
    }

    /// <summary>Formats and writes the specified <see cref="TimeOnly" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"13:12:12"</c>). The precision is lower than or equal to 100 ns.</remarks>
    public void WriteTimeOnly(TimeOnly value)
    {
        Write(value, SelectConverter<TimeOnly>());
    }

    /// <summary>Formats and writes the specified <see cref="DateOnly" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27"</c>).</remarks>
    public void WriteDateOnly(DateOnly value)
    {
        Write(value, SelectConverter<DateOnly>());
    }

    /// <summary>Formats and writes the specified <see cref="DateTime" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27T13:12:12"</c>). The precision is lower than or equal to 100 ns.</remarks>
    public void WriteDateTime(DateTime value)
    {
        Write(value, SelectConverter<DateTime>());
    }

    /// <summary>Formats and writes the specified <see cref="DateTimeOffset" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27T13:12:12-07:00"</c>). The precision is lower than or equal to 100 ns.</remarks>
    public void WriteDateTimeOffset(DateTimeOffset value)
    {
        Write(value, SelectConverter<DateTimeOffset>());
    }

    /// <summary>Formats and writes the specified <see cref="Guid" /> value as a sequence of characters to the internal buffer.</summary>
    /// <param name="value">The value to be formatted and written.</param>
    /// <exception cref="InvalidOperationException">A record is not explicitly started or contains a comment.</exception>
    /// <remarks>The value will be formatted in the RFC 4122 format (e.g., <c>"fae04ec0-301f-11d3-bf4b-00c04f79efbc"</c>).</remarks>
    public void WriteGuid(Guid value)
    {
        Write(value, SelectConverter<Guid>());
    }
}
