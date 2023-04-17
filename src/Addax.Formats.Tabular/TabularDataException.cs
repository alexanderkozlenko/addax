// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1032

namespace Addax.Formats.Tabular;

/// <summary>Represents an error that occurred during processing of tabular data.</summary>
public sealed class TabularDataException : Exception
{
    /// <summary>Initializes a new instance of the <see cref="TabularDataException" /> class with a specified error message and position.</summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="position">The total number of characters processed before the error occurred.</param>
    public TabularDataException(string message, long position)
        : base(message)
    {
        Debug.Assert(position >= 0);

        Position = position;
    }

    /// <summary>Gets the total number of characters processed before the error occurred.</summary>
    /// <value>A non-negative zero-based number.</value>
    public long Position
    {
        get;
    }
}
