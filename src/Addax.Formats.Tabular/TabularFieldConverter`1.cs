// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Converts an object or value to or from tabular field.</summary>
/// <typeparam name="T">The type of object or value handled by the converter.</typeparam>
public abstract class TabularFieldConverter<T> : TabularFieldConverter
{
    /// <summary>Initializes a new instance of the <see cref="TabularFieldConverter{T}" /> class.</summary>
    protected TabularFieldConverter()
    {
    }

    /// <summary>Tries to get the maximum buffer length for formatting an object or value to a sequence of characters.</summary>
    /// <param name="value">The object or value to be formatted.</param>
    /// <param name="result">When this method returns, contains the buffer length, or an undefined value on failure.</param>
    /// <returns><see langword="true" /> if the buffer length can be calculated; <see langword="false" /> otherwise.</returns>
    public virtual bool TryGetFormatBufferLength(T? value, out int result)
    {
        result = TabularDataInfo.StackBufferLength;

        return true;
    }

    /// <summary>Tries to get the maximum buffer length for parsing an object or value from a sequence of characters.</summary>
    /// <param name="result">When this method returns, contains the buffer length, or an undefined value on failure.</param>
    /// <returns><see langword="true" /> if the buffer length can be calculated; <see langword="false" /> otherwise.</returns>
    public virtual bool TryGetParseBufferLength(out int result)
    {
        result = Array.MaxLength;

        return true;
    }

    /// <summary>Tries to format an object or value to a sequence of characters.</summary>
    /// <param name="value">The object or value to be formatted.</param>
    /// <param name="buffer">The destination for a sequence of characters.</param>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <param name="charsWritten">When this method returns, contains the number of characters that were written.</param>
    /// <returns><see langword="true" /> if the object or value was successfully formatted; <see langword="false" /> otherwise.</returns>
    public virtual bool TryFormat(T? value, Span<char> buffer, IFormatProvider provider, out int charsWritten)
    {
        throw new NotSupportedException();
    }

    /// <summary>Tries to parse a sequence of characters to an object or value.</summary>
    /// <param name="buffer">The sequence of characters to parse.</param>
    /// <param name="provider">An object that provides culture-specific formatting information.</param>
    /// <param name="result">When this method returns, contains the result of successful parsing, or an undefined value on failure.</param>
    /// <returns><see langword="true" /> if the sequence of characters was successfully parsed; <see langword="false" /> otherwise.</returns>
    public virtual bool TryParse(ReadOnlySpan<char> buffer, IFormatProvider provider, out T? result)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public sealed override Type FieldType
    {
        get
        {
            return typeof(T);
        }
    }
}
