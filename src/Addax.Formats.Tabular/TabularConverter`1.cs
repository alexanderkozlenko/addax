// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Defines the core behavior of converting a value from or to a sequence of characters and provides a base for derived classes.</summary>
/// <typeparam name="T">The type of value handled by the converter.</typeparam>
public abstract class TabularConverter<T>
{
    /// <summary>Initializes a new instance of the <see cref="TabularConverter{T}" /> class.</summary>
    protected TabularConverter()
    {
    }

    /// <summary>When overridden in a derived class, tries to format the value into a sequence of characters.</summary>
    /// <param name="value">The value to format.</param>
    /// <param name="destination">The buffer to write the value to.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="charsWritten">When this method returns, contains an integer that is the number of characters that were written in <paramref name="destination" />. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if <paramref name="value" /> was successfully formatted; otherwise, <see langword="false" />.</returns>
    /// <remarks>Throws a <see cref="NotSupportedException" /> exception by default.</remarks>
    public virtual bool TryFormat(T? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        throw new NotSupportedException();
    }

    /// <summary>When overridden in a derived class, tries to parse the sequence of characters into a value.</summary>
    /// <param name="source">The buffer to read a value from.</param>
    /// <param name="provider">An object that supplies culture-specific formatting information.</param>
    /// <param name="value">When this method returns, contains a value that is the result of successfully parsing <paramref name="source" />, or an undefined value on failure. This parameter is treated as uninitialized.</param>
    /// <returns><see langword="true" /> if <paramref name="source" /> was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>Throws a <see cref="NotSupportedException" /> exception by default.</remarks>
    public virtual bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out T? value)
    {
        throw new NotSupportedException();
    }
}
