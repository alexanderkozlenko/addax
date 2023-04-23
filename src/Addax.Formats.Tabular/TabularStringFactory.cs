// (c) Oleksandr Kozlenko. Licensed under the MIT license.

using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace Addax.Formats.Tabular;

/// <summary>Provides creation of <see cref="string" /> instances.</summary>
public class TabularStringFactory : IDisposable
{
    private static readonly SpanAction<char, ReadOnlySequence<char>> _createAction = CreateString;

    /// <summary>Initializes a new instance of the <see cref="TabularStringFactory" /> class.</summary>
    public TabularStringFactory()
    {
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
    }

    /// <summary>Tries to create a <see cref="string" /> instance.</summary>
    /// <param name="value">The sequence of characters to create a <see cref="string" /> instance from.</param>
    /// <param name="result">When this method returns, contains the result of successful creation, or an undefined value on failure.</param>
    /// <returns><see langword="true" /> if the sequence of characters was successfully creation; <see langword="false" /> otherwise.</returns>
    public virtual bool TryCreateString(in ReadOnlySequence<char> value, [NotNullWhen(true)] out string? result)
    {
        var length = value.Length;

        if (length == 0)
        {
            result = string.Empty;

            return true;
        }
        else if (length <= Array.MaxLength)
        {
            result = value.IsSingleSegment ? new(value.FirstSpan) : string.Create((int)length, value, _createAction);

            return true;
        }
        else
        {
            result = null;

            return false;
        }
    }

    private static void CreateString(Span<char> target, ReadOnlySequence<char> source)
    {
        source.CopyTo(target);
    }
}
