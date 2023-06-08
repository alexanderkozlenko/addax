// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace Addax.Formats.Tabular;

/// <summary>Provides creation of <see cref="string" /> instances.</summary>
public partial class TabularStringFactory : IDisposable
{
    internal static readonly TabularStringFactory Default = new();

    private static readonly SpanAction<char, ReadOnlySequence<char>> _createAction = CreateString;

    private readonly Dictionary<int, string>? _stringTable;
    private readonly int _maximumStringLength;

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class.</summary>
    public TabularStringFactory()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class using the specified string pool size and maximum string size.</summary>
    /// <param name="maximumStringLength"></param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="maximumStringLength" /> is less than zero or is greater than <see cref="Array.MaxLength" />.</exception>
    public TabularStringFactory(int maximumStringLength)
    {
        if ((maximumStringLength <= 0) || (maximumStringLength > Array.MaxLength))
        {
            throw new ArgumentOutOfRangeException(nameof(maximumStringLength), maximumStringLength, "The maximum string size must be greater than zero and less than or equal to 'System.Array.MaxLength'.");
        }

        _stringTable = new();
        _maximumStringLength = maximumStringLength;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc cref="IDisposable.Dispose" />
    /// <param name="disposing"><see langword="true" /> if managed and unmanaged resources should be disposed; <see langword="false" /> if unmanaged only.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _stringTable?.Clear();
        }
    }

    /// <summary>Tries to create a <see cref="string" /> instance.</summary>
    /// <param name="value">The sequence of characters to create a <see cref="string" /> instance from.</param>
    /// <param name="result">When this method returns, contains the result of successful creation, or an undefined value on failure.</param>
    /// <returns><see langword="true" /> if the sequence of characters was successfully creation; <see langword="false" /> otherwise.</returns>
    public virtual bool TryCreateString(in ReadOnlySequence<char> value, [NotNullWhen(true)] out string? result)
    {
        var length = value.Length;

        if (length is 0)
        {
            result = string.Empty;

            return true;
        }
        else if (length <= Array.MaxLength)
        {
            if ((_stringTable is null) || (length > _maximumStringLength))
            {
                result = value.IsSingleSegment ?
                    CreateString(value.First) :
                    CreateString(value);
            }
            else
            {
                var hashCode = value.IsSingleSegment ?
                    GetHashCode(value.First) :
                    GetHashCode(value);

                if (!_stringTable.TryGetValue(hashCode, out result))
                {
                    result = value.IsSingleSegment ?
                        CreateString(value.First) :
                        CreateString(value);

                    _stringTable[hashCode] = result;
                }
            }

            return true;
        }
        else
        {
            result = null;

            return false;
        }
    }

    private static string CreateString(ReadOnlyMemory<char> buffer)
    {
        return new(buffer.Span);
    }

    private static string CreateString(in ReadOnlySequence<char> buffer)
    {
        return string.Create((int)buffer.Length, buffer, _createAction);
    }

    private static void CreateString(Span<char> target, ReadOnlySequence<char> source)
    {
        source.CopyTo(target);
    }

    private static int GetHashCode(ReadOnlyMemory<char> buffer)
    {
        var hashCode = new HashCode();

        hashCode.AddBytes(MemoryMarshal.Cast<char, byte>(buffer.Span));

        return hashCode.ToHashCode();
    }

    private static int GetHashCode(in ReadOnlySequence<char> buffer)
    {
        var hashCode = new HashCode();

        foreach (var segment in buffer)
        {
            hashCode.AddBytes(MemoryMarshal.Cast<char, byte>(segment.Span));
        }

        return hashCode.ToHashCode();
    }
}
