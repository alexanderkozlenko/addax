// (c) Oleksandr Kozlenko. Licensed under the MIT license.

namespace Addax.Formats.Tabular;

/// <summary>Converts a sequence of characters to or from a tabular data record.</summary>
/// <typeparam name="T">The type of an object handled by the converter.</typeparam>
public abstract class TabularRecordConverter<T> : TabularRecordConverter
{
    /// <summary>Initializes a new instance of the <see cref="TabularRecordConverter{T}" /> class.</summary>
    protected TabularRecordConverter()
    {
    }

    /// <summary>Asynchronously reads the current record using the specified reader instance.</summary>
    /// <param name="reader">The instance of tabular field reader to read the current record with.</param>
    /// <param name="context">The current context for tabular records reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that will complete with a record as a <see cref="TabularRecord{T}" /> instance.</returns>
    public virtual ValueTask<TabularRecord<T>> ReadRecordAsync(TabularFieldReader reader, TabularReaderContext context, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>Reads the current record using the specified reader instance.</summary>
    /// <param name="reader">The instance of tabular field reader to read the current record with.</param>
    /// <param name="context">The current context for tabular records reading.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A record as a <see cref="TabularRecord{T}" /> instance.</returns>
    public virtual TabularRecord<T> ReadRecord(TabularFieldReader reader, TabularReaderContext context, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>Asynchronously writes the specified record using the specified writer instance.</summary>
    /// <param name="writer">The instance of tabular field writer to write the specified record with.</param>
    /// <param name="record">The tabular data record to write.</param>
    /// <param name="context">The current context for tabular records writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual ValueTask WriteRecordAsync(TabularFieldWriter writer, T record, TabularWriterContext context, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <summary>Writes the specified record using the specified writer instance.</summary>
    /// <param name="writer">The instance of tabular field writer to write the specified record with.</param>
    /// <param name="record">The tabular data record to write.</param>
    /// <param name="context">The current context for tabular records writing.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    public virtual void WriteRecord(TabularFieldWriter writer, T record, TabularWriterContext context, CancellationToken cancellationToken)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc />
    public sealed override Type RecordType
    {
        get
        {
            return typeof(T);
        }
    }
}
