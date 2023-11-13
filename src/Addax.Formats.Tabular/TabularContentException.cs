// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>The exception that is thrown when tabular data is invalid.</summary>
public class TabularContentException : Exception
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
}
