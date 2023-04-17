// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;

namespace Addax.Formats.Tabular;

public partial class TabularFieldReader
{
    /// <summary>Retrieves the current reader value as <see cref="char" />.</summary>
    /// <returns>The <see cref="char" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="char" />.</exception>
    public char GetChar()
    {
        return Get(_converterChar);
    }

    /// <summary>Retrieves the current reader value as <see cref="Rune" />.</summary>
    /// <returns>The <see cref="Rune" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="Rune" />.</exception>
    public Rune GetRune()
    {
        return Get(_converterRune);
    }

    /// <summary>Retrieves the current reader value as <see cref="string" />.</summary>
    /// <returns>The <see cref="string" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="string" />.</exception>
    /// <remarks>The result is a new <see cref="string" /> instance or <see cref="string.Empty" />.</remarks>
    public string GetString()
    {
        if (!TryGetString(out var result))
        {
            ThrowFormatException();
        }

        return result;

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowFormatException()
        {
            throw new FormatException($"The current reader value cannot be represented as '{typeof(string)}'.");
        }
    }

    /// <summary>Parses and returns the current reader value as <see cref="bool" />.</summary>
    /// <returns>The <see cref="bool" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="bool" />.</exception>
    /// <remarks>The value must be represented as one of the supported literals: <c>"true"</c>, <c>"false"</c>, <c>"1"</c>, <c>"0"</c>.</remarks>
    public bool GetBoolean()
    {
        return Get(_converterBoolean);
    }

    /// <summary>Parses and returns the current reader value as <see cref="sbyte" />.</summary>
    /// <returns>The <see cref="sbyte" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="sbyte" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public sbyte GetSByte()
    {
        return Get(_converterSByte);
    }

    /// <summary>Parses and returns the current reader value as <see cref="byte" />.</summary>
    /// <returns>The <see cref="byte" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="byte" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    public byte GetByte()
    {
        return Get(_converterByte);
    }

    /// <summary>Parses and returns the current reader value as <see cref="short" />.</summary>
    /// <returns>The <see cref="short" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="short" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    public short GetInt16()
    {
        return Get(_converterInt16);
    }

    /// <summary>Parses and returns the current reader value as <see cref="ushort" />.</summary>
    /// <returns>The <see cref="ushort" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="ushort" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public ushort GetUInt16()
    {
        return Get(_converterUInt16);
    }

    /// <summary>Parses and returns the current reader value as <see cref="int" />.</summary>
    /// <returns>The <see cref="int" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="int" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    public int GetInt32()
    {
        return Get(_converterInt32);
    }

    /// <summary>Parses and returns the current reader value as <see cref="uint" />.</summary>
    /// <returns>The <see cref="uint" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="uint" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public uint GetUInt32()
    {
        return Get(_converterUInt32);
    }

    /// <summary>Parses and returns the current reader value as <see cref="long" />.</summary>
    /// <returns>The <see cref="long" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="long" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    public long GetInt64()
    {
        return Get(_converterInt64);
    }

    /// <summary>Parses and returns the current reader value as <see cref="ulong" />.</summary>
    /// <returns>The <see cref="ulong" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="ulong" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public ulong GetUInt64()
    {
        return Get(_converterUInt64);
    }

    /// <summary>Parses and returns the current reader value as <see cref="Int128" />.</summary>
    /// <returns>The <see cref="Int128" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="Int128" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    public Int128 GetInt128()
    {
        return Get(_converterInt128);
    }

    /// <summary>Parses and returns the current reader value as <see cref="UInt128" />.</summary>
    /// <returns>The <see cref="UInt128" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="UInt128" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public UInt128 GetUInt128()
    {
        return Get(_converterUInt128);
    }

    /// <summary>Parses and returns the current reader value as <see cref="BigInteger" />.</summary>
    /// <returns>The <see cref="BigInteger" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="BigInteger" />.</exception>
    /// <remarks>The value must be formatted using invariant culture.</remarks>
    public BigInteger GetBigInteger()
    {
        return Get(_converterBigInteger);
    }

    /// <summary>Parses and returns the current reader value as <see cref="Half" />.</summary>
    /// <returns>The <see cref="Half" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="Half" />.</exception>
    /// <remarks>The value must be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public Half GetHalf()
    {
        return Get(_converterHalf);
    }

    /// <summary>Parses and returns the current reader value as <see cref="float" />.</summary>
    /// <returns>The <see cref="float" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="float" />.</exception>
    /// <remarks>The value must be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public float GetSingle()
    {
        return Get(_converterSingle);
    }

    /// <summary>Parses and returns the current reader value as <see cref="double" />.</summary>
    /// <returns>The <see cref="double" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="double" />.</exception>
    /// <remarks>The value must be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public double GetDouble()
    {
        return Get(_converterDouble);
    }

    /// <summary>Parses and returns the current reader value as <see cref="decimal" />.</summary>
    /// <returns>The <see cref="decimal" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="decimal" />.</exception>
    /// <remarks>The value must be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public decimal GetDecimal()
    {
        return Get(_converterDecimal);
    }

    /// <summary>Parses and returns the current reader value as <see cref="Complex" />.</summary>
    /// <returns>The <see cref="Complex" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="Complex" />.</exception>
    /// <remarks>The value must be formatted in a ISO 80000-2:2019 compliant format with real numbers in the fixed-point or scientific notation using invariant culture (e.g., <c>"0.54+0.84i"</c>).</remarks>
    public Complex GetComplex()
    {
        return Get(_converterComplex);
    }

    /// <summary>Parses and returns the current reader value as <see cref="TimeSpan" />.</summary>
    /// <returns>The <see cref="TimeSpan" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="TimeSpan" />.</exception>
    /// <remarks>The value must be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"PT13H13M12S"</c> for <c>13:12:12.0000000</c>). The value must include only D, H, M, and S designators; the precision must be lower than or equal to 100 ns.</remarks>
    public TimeSpan GetTimeSpan()
    {
        return Get(_converterTimeSpan);
    }

    /// <summary>Parses and returns the current reader value as <see cref="TimeOnly" />.</summary>
    /// <returns>The <see cref="TimeOnly" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="TimeOnly" />.</exception>
    /// <remarks>The value must be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"13:12:12"</c>). The precision must be lower than or equal to 100 ns.</remarks>
    public TimeOnly GetTimeOnly()
    {
        return Get(_converterTimeOnly);
    }

    /// <summary>Parses and returns the current reader value as <see cref="DateOnly" />.</summary>
    /// <returns>The <see cref="DateOnly" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="DateOnly" />.</exception>
    /// <remarks>The value must be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27"</c>).</remarks>
    public DateOnly GetDateOnly()
    {
        return Get(_converterDateOnly);
    }

    /// <summary>Parses and returns the current reader value as <see cref="DateTime" />.</summary>
    /// <returns>The <see cref="DateTime" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="DateTime" />.</exception>
    /// <remarks>The value must be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27T13:12:12"</c>). The precision must be lower than or equal to 100 ns.</remarks>
    public DateTime GetDateTime()
    {
        return Get(_converterDateTime);
    }

    /// <summary>Parses and returns the current reader value as <see cref="DateTimeOffset" />.</summary>
    /// <returns>The <see cref="DateTimeOffset" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="DateTimeOffset" />.</exception>
    /// <remarks>The value must be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27T13:12:12-07:00"</c>). The precision must be lower than or equal to 100 ns.</remarks>
    public DateTimeOffset GetDateTimeOffset()
    {
        return Get(_converterDateTimeOffset);
    }

    /// <summary>Parses and returns the current reader value as <see cref="Guid" />.</summary>
    /// <returns>The <see cref="Guid" /> equivalent of the current reader value.</returns>
    /// <exception cref="FormatException">The current reader value cannot be represented as <see cref="Guid" />.</exception>
    /// <remarks>The value must be formatted in the RFC 4122 format (e.g., <c>"fae04ec0-301f-11d3-bf4b-00c04f79efbc"</c>).</remarks>
    public Guid GetGuid()
    {
        return Get(_converterGuid);
    }
}
