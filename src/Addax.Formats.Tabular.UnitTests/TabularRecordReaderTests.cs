#pragma warning disable IDE1006

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularRecordReaderTests
{
    [TestMethod]
    public async Task SkipRecord()
    {
        await using var stream = new MemoryStream("v1\nv2"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        await reader.SkipRecordAsync(CancellationToken);

        var record = await reader.ReadRecordAsync<string[]>(CancellationToken);

        Assert.IsTrue(record.HasContent);
        Assert.AreEqual("v2", record.Content.FirstOrDefault());
    }

    [TestMethod]
    public async Task SkipRecordWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        await reader.SkipRecordAsync(CancellationToken);
        await reader.SkipRecordAsync(CancellationToken);
    }

    [TestMethod]
    public async Task ReadRecordWhenConverterNotDefined()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => reader.ReadRecordAsync<Version>(CancellationToken).AsTask());
    }

    [TestMethod]
    public async Task ReadRecordWhenConverterNotImplemented()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'), new(recordConverters: new[] { new MyRecordConverter() }));

        await Assert.ThrowsExceptionAsync<NotSupportedException>(
            () => reader.ReadRecordAsync<MyRecord>(CancellationToken).AsTask());
    }

    [TestMethod]
    public async Task ReadRecord()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        var record = await reader.ReadRecordAsync<string[]>(CancellationToken);

        Assert.IsTrue(record.HasContent);
        Assert.AreEqual("v1", record.Content.FirstOrDefault());
    }

    [TestMethod]
    public async Task ReadRecordWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        await reader.ReadRecordAsync<string[]>(CancellationToken);

        await Assert.ThrowsExceptionAsync<InvalidOperationException>(
            () => reader.ReadRecordAsync<string[]>(CancellationToken).AsTask());
    }

    [TestMethod]
    public async Task ReadRecordWithCommentIgnored()
    {
        await using var stream = new MemoryStream("#c"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"', '"', '#'));

        var record = await reader.ReadRecordAsync<string[]>(CancellationToken);

        Assert.IsFalse(record.HasContent);
        Assert.IsNull(record.Comment);
    }

    [TestMethod]
    public async Task ReadRecordWithCommentConsumed()
    {
        await using var stream = new MemoryStream("#c"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"', '"', '#'), new(consumeComments: true));

        var record = await reader.ReadRecordAsync<string[]>(CancellationToken);

        Assert.IsFalse(record.HasContent);
        Assert.AreEqual("c", record.Comment);
    }

    [TestMethod]
    public async Task ReadRecordsWhenConverterNotDefined()
    {
        await using var stream = new MemoryStream("v1\nv2"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        Assert.ThrowsException<InvalidOperationException>(
            () => reader.ReadRecordsAsync<Version>(cancellationToken: CancellationToken));
    }

    [TestMethod]
    public async Task ReadRecords()
    {
        await using var stream = new MemoryStream("v1\nv2"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("v1", result[0].FirstOrDefault());
        Assert.AreEqual("v2", result[1].FirstOrDefault());
    }

    [TestMethod]
    public async Task ReadRecordsWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        await reader.ReadRecordAsync<string[]>(CancellationToken);

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWhenTheSameEnumerable()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        var enumerable = reader.ReadRecordsAsync<string[]>(cancellationToken: CancellationToken);

        await foreach (var _ in enumerable)
        {
        }

        await foreach (var _ in enumerable)
        {
            Assert.Fail();
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
