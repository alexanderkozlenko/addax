// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Defines the core behavior of converting a value to or from a character sequence and provides a base for derived classes.</summary>
/// <typeparam name="T">The type of value handled by the converter.</typeparam>
public abstract class TabularConverter<T> : TabularConverter
{
    /// <summary>Initializes a new instance of the <see cref="TabularConverter{T}" /> class.</summary>
    protected TabularConverter()
    {
    }

    /// <summary>When overridden in a derived class, tries to format a value into the provided span of characters.</summary>
    /// <param name="value">The value to format.</param>
    /// <param name="destination">The span in which to write the value formatted as a span of characters.</param>
    /// <param name="provider">An optional object that supplies culture-specific formatting information for <paramref name="destination" />.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters that were written in <paramref name="destination" />.</param>
    /// <returns><see langword="true" /> if the formatting was successful; otherwise, <see langword="false" />.</returns>
    /// <remarks>The method should return <see langword="false" /> only if there is not enough space in the destination buffer. Any other failures should throw an exception. The default implementation throws a <see cref="NotSupportedException" />.</remarks>
    public virtual bool TryFormat(T? value, Span<char> destination, IFormatProvider? provider, out int charsWritten)
    {
        throw new NotSupportedException();
    }

    /// <summary>When overridden in a derived class, tries to parse a span of characters into a value.</summary>
    /// <param name="source">The span of characters to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information about <paramref name="source" />.</param>
    /// <param name="value">When this method returns, contains the result of successfully parsing <paramref name="source" />, or an undefined value on failure.</param>
    /// <returns><see langword="true" /> if <paramref name="source" /> was successfully parsed; otherwise, <see langword="false" />.</returns>
    /// <remarks>The default implementation throws a <see cref="NotSupportedException" />.</remarks>
    public virtual bool TryParse(ReadOnlySpan<char> source, IFormatProvider? provider, out T? value)
    {
        throw new NotSupportedException();
    }

    internal sealed override Type ValueType
    {
        get
        {
            return typeof(T);
        }
    }
}
