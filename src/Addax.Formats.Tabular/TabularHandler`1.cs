// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Defines the core behavior of reading an object from or writing an object to a tabular record and provides a base for derived classes.</summary>
/// <typeparam name="T">The type of object handled by the record handler.</typeparam>
public abstract class TabularHandler<T> : TabularHandler
    where T : notnull
{
    /// <summary>Initializes a new instance of the <see cref="TabularHandler{T}" /> class.</summary>
    protected TabularHandler()
    {
    }

    /// <summary>When overridden in a derived class, reads the next record using the specified reader.</summary>
    /// <param name="reader">The reader to read with.</param>
    /// <returns>The object read from the record.</returns>
    /// <remarks>The default implementation throws a <see cref="NotSupportedException" />.</remarks>
    public virtual T Read(TabularReader reader)
    {
        throw new NotSupportedException();
    }

    /// <summary>When overridden in a derived class, asynchronously reads the next record using the specified reader.</summary>
    /// <param name="reader">The reader to read with.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous read operation. The value of its <see cref="ValueTask{TResult}.Result" /> property contains the object read from the record.</returns>
    /// <remarks>The default implementation throws a <see cref="NotSupportedException" />.</remarks>
    public virtual ValueTask<T> ReadAsync(TabularReader reader, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>When overridden in a derived class, writes the next record using the specified writer.</summary>
    /// <param name="writer">The writer to write with.</param>
    /// <param name="record">The object to write.</param>
    /// <remarks>The default implementation throws a <see cref="NotSupportedException" />.</remarks>
    public virtual void Write(TabularWriter writer, T record)
    {
        throw new NotSupportedException();
    }

    /// <summary>When overridden in a derived class, asynchronously writes the next record using the specified writer.</summary>
    /// <param name="writer">The writer to write with.</param>
    /// <param name="record">The object to write.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <remarks>The default implementation throws a <see cref="NotSupportedException" />.</remarks>
    public virtual ValueTask WriteAsync(TabularWriter writer, T record, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>Gets the names of the record fields.</summary>
    /// <value>A collection of field names, or <see langword="null" /> if the records have no header.</value>
    public virtual IEnumerable<string?>? Header
    {
        get
        {
            return null;
        }
    }
}
