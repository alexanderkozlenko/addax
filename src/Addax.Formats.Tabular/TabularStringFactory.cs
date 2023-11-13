// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Collections.Concurrent;
using System.Diagnostics;

namespace Addax.Formats.Tabular;

/// <summary>Provides a creation method for creating <see cref="string" /> instances from character sequences.</summary>
public class TabularStringFactory : IDisposable
{
    internal static readonly TabularStringFactory Default = new();

    private readonly ConcurrentDictionary<int, string>? _stringTable;
    private readonly int _maxLength;

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class.</summary>
    public TabularStringFactory()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class with the specified maximum string length.</summary>
    /// <param name="maxLength">The maximum length of a string to participate in pooling. Must be greater than zero and less than or equal to <see cref="Array.MaxLength" />.</param>
    /// <remarks>This constructor enables a thread-safe string pool based on hash code and length.</remarks>
    public TabularStringFactory(int maxLength)
    {
        _maxLength = Math.Max(1, Math.Min(maxLength, Array.MaxLength));
        _stringTable = new();
    }

    /// <summary>Releases the resources used by the current instance of the <see cref="TabularStringFactory" /> class.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases the managed and unmanaged resources used by the current instance of the <see cref="TabularStringFactory" /> class.</summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stringTable?.Clear();
        }
    }

    /// <summary>Create a <see cref="string" /> instance from a character sequence.</summary>
    /// <param name="source">The read-only character sequence to create from.</param>
    /// <returns>A new or an existing <see cref="string" /> instance.</returns>
    public virtual string Create(ReadOnlySpan<char> source)
    {
        return source.IsEmpty ? string.Empty : (source.Length > _maxLength ? new(source) : GetOrAdd(source));
    }

    private string GetOrAdd(ReadOnlySpan<char> source)
    {
        Debug.Assert(_stringTable is not null);

        var hashCode = string.GetHashCode(source, StringComparison.Ordinal);

        if (!_stringTable.TryGetValue(hashCode, out var value))
        {
            value = new(source);

            try
            {
                _stringTable.TryAdd(hashCode, value);
            }
            catch (OverflowException)
            {
                _stringTable.Clear();
            }
        }

        return value;
    }
}
