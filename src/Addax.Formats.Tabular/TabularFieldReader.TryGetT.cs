// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Text;
using Addax.Formats.Tabular.Internal;

namespace Addax.Formats.Tabular;

public partial class TabularFieldReader
{
    /// <summary>Tries to retrieve the current reader value as <see cref="char" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="char" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    public bool TryGetChar(out char result)
    {
        return TryGet(_converterChar, out result);
    }

    /// <summary>Tries to retrieve the current reader value as <see cref="Rune" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="Rune" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    public bool TryGetRune(out Rune result)
    {
        return TryGet(_converterRune, out result);
    }

    /// <summary>Tries to retrieve the current reader value as <see cref="string" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="string" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully retrieved; <see langword="false" /> otherwise.</returns>
    /// <remarks>The result is a new <see cref="string" /> instance or <see cref="string.Empty" />.</remarks>
    public bool TryGetString([NotNullWhen(true)] out string? result)
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);

        if (_bufferKind is BufferKind.None)
        {
            ThrowInvalidOperationException();
        }

        return _stringFactory.TryCreateString(_value, out result);

        [DoesNotReturn]
        [StackTraceHidden]
        static void ThrowInvalidOperationException()
        {
            throw new InvalidOperationException("The current reader value can only be accessed following a read operation.");
        }
    }

    /// <summary>Tries to parse the current reader value as <see cref="bool" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="bool" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be represented as one of the supported literals: <c>"true"</c>, <c>"false"</c>, <c>"1"</c>, <c>"0"</c>.</remarks>
    public bool TryGetBoolean(out bool result)
    {
        return TryGet(_converterBoolean, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="sbyte" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="sbyte" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public bool TryGetSByte(out sbyte result)
    {
        return TryGet(_converterSByte, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="byte" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="byte" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    public bool TryGetByte(out byte result)
    {
        return TryGet(_converterByte, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="short" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="short" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    public bool TryGetInt16(out short result)
    {
        return TryGet(_converterInt16, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="ushort" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="ushort" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt16(out ushort result)
    {
        return TryGet(_converterUInt16, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="int" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="int" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    public bool TryGetInt32(out int result)
    {
        return TryGet(_converterInt32, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="uint" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="uint" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt32(out uint result)
    {
        return TryGet(_converterUInt32, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="long" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="long" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    public bool TryGetInt64(out long result)
    {
        return TryGet(_converterInt64, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="ulong" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="ulong" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt64(out ulong result)
    {
        return TryGet(_converterUInt64, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="Int128" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="Int128" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    public bool TryGetInt128(out Int128 result)
    {
        return TryGet(_converterInt128, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="UInt128" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="UInt128" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    [CLSCompliant(false)]
    public bool TryGetUInt128(out UInt128 result)
    {
        return TryGet(_converterUInt128, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="BigInteger" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="BigInteger" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted using invariant culture.</remarks>
    public bool TryGetBigInteger(out BigInteger result)
    {
        return TryGet(_converterBigInteger, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="Half" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="Half" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public bool TryGetHalf(out Half result)
    {
        return TryGet(_converterHalf, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="float" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="float" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public bool TryGetSingle(out float result)
    {
        return TryGet(_converterSingle, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="double" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="double" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public bool TryGetDouble(out double result)
    {
        return TryGet(_converterDouble, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="decimal" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="decimal" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in the fixed-point or scientific notation using invariant culture.</remarks>
    public bool TryGetDecimal(out decimal result)
    {
        return TryGet(_converterDecimal, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="Complex" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="Complex" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in a ISO 80000-2:2019 compliant format with real numbers in the fixed-point or scientific notation using invariant culture (e.g., <c>"0.54+0.84i"</c>).</remarks>
    public bool TryGetComplex(out Complex result)
    {
        return TryGet(_converterComplex, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="TimeSpan" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="TimeSpan" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"PT13H13M12S"</c> for <c>13:12:12.0000000</c>). The value should include only D, H, M, and S designators; the precision should be lower than or equal to 100 ns.</remarks>
    public bool TryGetTimeSpan(out TimeSpan result)
    {
        return TryGet(_converterTimeSpan, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="TimeOnly" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="TimeOnly" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"13:12:12"</c>). The precision should be lower than or equal to 100 ns.</remarks>
    public bool TryGetTimeOnly(out TimeOnly result)
    {
        return TryGet(_converterTimeOnly, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="DateOnly" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="DateOnly" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27"</c>).</remarks>
    public bool TryGetDateOnly(out DateOnly result)
    {
        return TryGet(_converterDateOnly, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="DateTime" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="DateTime" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27T13:12:12"</c>). The precision should be lower than or equal to 100 ns.</remarks>
    public bool TryGetDateTime(out DateTime result)
    {
        return TryGet(_converterDateTime, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="DateTimeOffset" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="DateTimeOffset" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in an ISO 8601-1:2019 compliant format (e.g., <c>"2002-01-27T13:12:12-07:00"</c>). The precision should be lower than or equal to 100 ns.</remarks>
    public bool TryGetDateTimeOffset(out DateTimeOffset result)
    {
        return TryGet(_converterDateTimeOffset, out result);
    }

    /// <summary>Tries to parse the current reader value as <see cref="Guid" /> and returns a value that indicates whether the operation succeeded.</summary>
    /// <param name="result">When this method returns, contains the <see cref="Guid" /> equivalent of the current reader value if the operation succeeded.</param>
    /// <returns><see langword="true" /> if the entire value can be successfully parsed; <see langword="false" /> otherwise.</returns>
    /// <remarks>The value should be formatted in the RFC 4122 format (e.g., <c>"fae04ec0-301f-11d3-bf4b-00c04f79efbc"</c>).</remarks>
    public bool TryGetGuid(out Guid result)
    {
        return TryGet(_converterGuid, out result);
    }
}
