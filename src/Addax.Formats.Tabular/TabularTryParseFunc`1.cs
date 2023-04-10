// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Encapsulates a method that tries to parse a sequence of characters into a value.</summary>
/// <typeparam name="T">The type of a value handled by the method.</typeparam>
/// <param name="buffer">The sequence of characters to parse.</param>
/// <param name="provider">An object that provides culture-specific formatting information.</param>
/// <param name="result">When this method returns, contains the result of successful parsing, or an undefined value on failure.</param>
/// <returns><see langword="true" /> if the sequence of characters was successfully parsed; <see langword="false" /> otherwise.</returns>
public delegate bool TabularTryParseFunc<T>(ReadOnlySpan<char> buffer, IFormatProvider provider, out T? result);
