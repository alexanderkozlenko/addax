// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>The exception that is thrown when tabular data is invalid.</summary>
public class TabularContentException : TabularException
{
    /// <summary>Initializes a new instance of the <see cref="TabularContentException" /> class.</summary>
    public TabularContentException()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularContentException" /> class with the specified error message.</summary>
    /// <param name="message">The message that describes the error.</param>
    public TabularContentException(string? message)
        : base(message)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularContentException" /> class with the specified error message and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a <see langword="null" /> reference if no inner exception is specified.</param>
    public TabularContentException(string? message, Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularContentException" /> class with the specified error message and the position in the stream at which the error was detected.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="position">The zero-based character position in the stream at which the error was detected, or <see langword="null" /> if the position is unknown.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="position" /> is negative.</exception>
    public TabularContentException(string? message, long? position)
        : this(message, position, null)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularContentException" /> class with the specified error message, the position in the stream at which the error was detected, and a reference to the inner exception that is the cause of this exception.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="position">The zero-based character position in the stream at which the error was detected, or <see langword="null" /> if the position is unknown.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a <see langword="null" /> reference if no inner exception is specified.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="position" /> is negative.</exception>
    public TabularContentException(string? message, long? position, Exception? innerException)
        : base(message, innerException)
    {
        if (position.HasValue)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(position.Value, nameof(position));
        }

        Position = position;
    }

    /// <summary>Gets the zero-based character position in the stream at which the error was detected.</summary>
    /// <value>A non-negative 64-bit signed integer, or <see langword="null" /> if the position is unknown.</value>
    /// <remarks>The position counts decoded characters, not bytes, and excludes the byte order mark.</remarks>
    public long? Position
    {
        get;
    }
}
