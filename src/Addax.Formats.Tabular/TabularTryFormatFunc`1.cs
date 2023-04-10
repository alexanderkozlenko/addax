// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Encapsulates a method that tries to format a value into a sequence of characters.</summary>
/// <typeparam name="T">The type of a value handled by the method.</typeparam>
/// <param name="value">The value to be formatted.</param>
/// <param name="buffer">The destination for a sequence of characters.</param>
/// <param name="provider">An object that provides culture-specific formatting information.</param>
/// <param name="charsWritten">When this method returns, contains the number of characters that were written.</param>
/// <returns><see langword="true" /> if the value was successfully formatted; <see langword="false" /> otherwise.</returns>
public delegate bool TabularTryFormatFunc<T>(T? value, Span<char> buffer, IFormatProvider provider, out int charsWritten);
