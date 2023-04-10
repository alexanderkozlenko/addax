// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Encapsulates a method that parses a sequence of characters into a value.</summary>
/// <typeparam name="T">The type of a value handled by the method.</typeparam>
/// <param name="buffer">The sequence of characters to parse.</param>
/// <param name="provider">An object that provides culture-specific formatting information.</param>
/// <returns>The result of parsing.</returns>
public delegate T? TabularParseFunc<T>(ReadOnlySpan<char> buffer, IFormatProvider provider);
