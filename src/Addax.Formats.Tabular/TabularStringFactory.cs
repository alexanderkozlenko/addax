// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Numerics;

namespace Addax.Formats.Tabular;

/// <summary>Provides a method for creating <see cref="string" /> instances from character sequences.</summary>
public class TabularStringFactory : IDisposable
{
    internal static readonly TabularStringFactory Default = new();

    private string?[] _pool;
    private int _poolMask;
    private int _maxLength;

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class.</summary>
    public TabularStringFactory()
        : this(0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class with the specified maximum string length.</summary>
    /// <param name="maxLength">The maximum length of a string to participate in pooling. A negative value is treated as zero, and a value greater than <see cref="Array.MaxLength" /> as <see cref="Array.MaxLength" />.</param>
    /// <remarks>This constructor enables a string pool of 4,096 strings when <paramref name="maxLength" /> is greater than zero.</remarks>
    public TabularStringFactory(int maxLength)
        : this(maxLength, 0)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class with the specified maximum string length and pool capacity.</summary>
    /// <param name="maxLength">The maximum length of a string to participate in pooling. A negative value is treated as zero, and a value greater than <see cref="Array.MaxLength" /> as <see cref="Array.MaxLength" />.</param>
    /// <param name="poolCapacity">The number of strings the pool can contain, rounded up to a power of two. A value less than one is treated as 4,096, and a value greater than 1,073,741,824 as 1,073,741,824.</param>
    /// <remarks>This constructor enables a string pool of a fixed capacity when <paramref name="maxLength" /> is greater than zero, where a string replaces the pooled string it collides with.</remarks>
    public TabularStringFactory(int maxLength, int poolCapacity)
    {
        _maxLength = Math.Max(0, Math.Min(maxLength, Array.MaxLength));
        _pool = [];

        if (_maxLength > 0)
        {
            var poolLength = (int)BitOperations.RoundUpToPowerOf2((uint)(poolCapacity < 1 ? 4096 : Math.Min(poolCapacity, 1 << 30)));

            _pool = ArrayPool<string?>.Shared.Rent(poolLength);
            _poolMask = poolLength - 1;

            Array.Clear(_pool, 0, poolLength);
        }
    }

    /// <summary>Releases all resources used by the current instance of the <see cref="TabularStringFactory" /> class.</summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>Releases the unmanaged resources used by the <see cref="TabularStringFactory" /> and optionally releases the managed resources.</summary>
    /// <param name="disposing"><see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && (_maxLength > 0))
        {
            ArrayPool<string?>.Shared.Return(_pool, true);

            _pool = [];
            _poolMask = 0;
            _maxLength = 0;
        }
    }

    /// <summary>Creates a <see cref="string" /> instance from a character sequence.</summary>
    /// <param name="source">The read-only character sequence to create from.</param>
    /// <returns>A new or an existing <see cref="string" /> instance.</returns>
    public virtual string Create(ReadOnlySpan<char> source)
    {
        if (source.IsEmpty)
        {
            return string.Empty;
        }

        if (source.Length > _maxLength)
        {
            return new(source);
        }

        ref var slot = ref _pool[string.GetHashCode(source) & _poolMask];

        var value = slot;

        if ((value is null) || !source.SequenceEqual(value))
        {
            value = new(source);
            slot = value;
        }

        return value;
    }
}
