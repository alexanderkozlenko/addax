// (c) Oleksandr Kozlenko. Licensed under the MIT license.

#pragma warning disable CA1815

namespace Addax.Formats.Tabular;

/// <summary>Represents a container that may or may not have a stored object.</summary>
/// <typeparam name="T">The type of object stored in the container.</typeparam>
public readonly struct TabularRecord<T>
    where T : notnull
{
    internal readonly T? Value;
    internal readonly bool HasValue;

    /// <summary>Initializes a new instance of the <see cref="TabularRecord{T}" /> struct with the specified object.</summary>
    /// <param name="value">The object to initialize the container with.</param>
    public TabularRecord(T value)
    {
        Value = value;
        HasValue = true;
    }
}
