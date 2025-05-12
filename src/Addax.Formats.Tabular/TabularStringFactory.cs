// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Provides a creation method for creating <see cref="string" /> instances from character sequences.</summary>
public class TabularStringFactory : IDisposable
{
    internal static readonly TabularStringFactory Default = new();

    private readonly HashSet<string> _stringTable;
    private readonly HashSet<string>.AlternateLookup<ReadOnlySpan<char>> _stringLookup;
    private readonly int _maxLength;

    private SpinLock _lock;

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class.</summary>
    public TabularStringFactory()
        : this(0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class with the specified maximum string length.</summary>
    /// <param name="maxLength">The maximum length of a string to participate in pooling. Must be greater than or equal to zero and less than or equal to <see cref="Array.MaxLength" />.</param>
    /// <remarks>This constructor enables a thread-safe string pool based on hash code and length.</remarks>
    public TabularStringFactory(int maxLength)
        : this(maxLength, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class with the specified maximum string length.</summary>
    /// <param name="maxLength">The maximum length of a string to participate in pooling. Must be greater than or equal to zero and less than or equal to <see cref="Array.MaxLength" />.</param>
    /// <param name="poolCapacity">The initial number of elements that the string pool can contain. Must be greater than or equal to zero.</param>
    /// <remarks>This constructor enables a thread-safe string pool based on hash code and length.</remarks>
    public TabularStringFactory(int maxLength, int poolCapacity)
    {
        _maxLength = Math.Max(0, Math.Min(maxLength, Array.MaxLength));
        _stringTable = new(Math.Max(0, poolCapacity), StringComparer.Ordinal);
        _stringLookup = _stringTable.GetAlternateLookup<ReadOnlySpan<char>>();
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
            var locked = false;

            try
            {
                _lock.Enter(ref locked);
                _stringTable.Clear();
            }
            finally
            {
                if (locked)
                {
                    _lock.Exit();
                }
            }
        }
    }

    /// <summary>Create a <see cref="string" /> instance from a character sequence.</summary>
    /// <param name="source">The read-only character sequence to create from.</param>
    /// <returns>A new or an existing <see cref="string" /> instance.</returns>
    public virtual string Create(ReadOnlySpan<char> source)
    {
        return source.IsEmpty ? string.Empty : (source.Length > _maxLength ? new(source) : Lookup(source));
    }

    private string Lookup(ReadOnlySpan<char> source)
    {
        var locked = false;

        try
        {
            _lock.Enter(ref locked);

            if (!_stringLookup.TryGetValue(source, out var value))
            {
                value = new(source);

                _stringTable.Add(value);
            }

            return value;
        }
        finally
        {
            if (locked)
            {
                _lock.Exit();
            }
        }
    }
}
