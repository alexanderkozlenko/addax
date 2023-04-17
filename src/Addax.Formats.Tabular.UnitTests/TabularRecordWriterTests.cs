#pragma warning disable IDE1006

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularRecordWriterTests
{
    [TestMethod]
    public async Task WriteRecordWhenConverterNotDefined()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'));

        var record = new Version();

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => writer.WriteRecordAsync(record, CancellationToken).AsTask());
    }

    [TestMethod]
    public async Task WriteRecordWhenConverterNotImplemented()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'), new(recordConverters: new[] { new MyRecordConverter() }));

        var record = new MyRecord();

        await Assert.ThrowsExceptionAsync<NotSupportedException>(
            () => writer.WriteRecordAsync(record, CancellationToken).AsTask());
    }

    [TestMethod]
    public async Task WriteRecord()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'));

        var record = new[] { "v1" };

        await writer.WriteRecordAsync(record, CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        CollectionAssert.AreEqual("v1"u8.ToArray(), stream.ToArray());
    }

    [TestMethod]
    public async Task WriteRecords()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'));

        var records = new[]
        {
            new[] { "v1" },
            new[] { "v2" },
        };

        await writer.WriteRecordsAsync(records, CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        CollectionAssert.AreEqual("v1\nv2"u8.ToArray(), stream.ToArray());
    }

    [TestMethod]
    public async Task WriteRecordsEmpty()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'));

        var records = Array.Empty<string[]>();

        await writer.WriteRecordsAsync(records, CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        CollectionAssert.AreEqual(""u8.ToArray(), stream.ToArray());
    }

    [TestMethod]
    public async Task WriteRecordsWithNull()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'));

        var records = new[]
        {
            default(string[]),
        };

        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => writer.WriteRecordsAsync(records, CancellationToken).AsTask());
    }

    [TestMethod]
    public async Task WriteAsyncRecords()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'));

        var records = new[]
        {
            new[] { "v1" },
            new[] { "v2" },
        };

        await writer.WriteRecordsAsync(AsAsyncEnumerable(records), CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        CollectionAssert.AreEqual("v1\nv2"u8.ToArray(), stream.ToArray());
    }

    [TestMethod]
    public async Task WriteAsyncRecordsWithNull()
    {
        await using var stream = new MemoryStream();
        await using var writer = new TabularRecordWriter(stream, new("\n", ',', '"'));

        var records = new[]
        {
            default(string[]),
        };

        await Assert.ThrowsExceptionAsync<ArgumentException>(
            () => writer.WriteRecordsAsync(AsAsyncEnumerable(records), CancellationToken).AsTask());
    }

    private static async IAsyncEnumerable<T> AsAsyncEnumerable<T>(IEnumerable<T> source)
    {
        foreach (var item in source)
        {
            await Task.Yield();

            yield return item;
        }
    }

    private CancellationToken CancellationToken
    {
        get
        {
            return TestContext?.CancellationTokenSource?.Token ?? default;
        }
    }

    public TestContext? TestContext
    {
        get;
        set;
    }

    private sealed class MyRecord
    {
    }

    private sealed class MyRecordConverter : TabularRecordConverter<MyRecord>
    {
    }
}
