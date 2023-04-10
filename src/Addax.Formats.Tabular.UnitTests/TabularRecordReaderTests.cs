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
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'), new(converters: new[] { new MyRecordConverter() }));

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
    public async Task ReadRecordsWithPredicate()
    {
        await using var stream = new MemoryStream("c1\nv1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(Predicate, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("v1", result[0].FirstOrDefault());
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        await reader.ReadRecordAsync<string[]>(CancellationToken);

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(Predicate, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkip()
    {
        await using var stream = new MemoryStream("c1\nv1\nc2\nv2"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(Predicate, skip: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("v2", result[0].FirstOrDefault());
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkipWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        await reader.ReadRecordAsync<string[]>(CancellationToken);

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(Predicate, skip: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkipWhenSkipLessThanZero()
    {
        await using var stream = new MemoryStream("c1\nv1\nc2\nv2"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => reader.ReadRecordsAsync<string[]>(Predicate, skip: -1, cancellationToken: CancellationToken));
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkipAndTake()
    {
        await using var stream = new MemoryStream("c1\nv1\nc2\nv2\nc3\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(Predicate, skip: 1, take: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("v2", result[0].FirstOrDefault());
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkipAndTakeWhenTakeZero()
    {
        await using var stream = new MemoryStream("c1\nv1\nc2\nv2\nc3\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(Predicate, skip: 1, take: 0, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkipAndTakeWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        await reader.ReadRecordAsync<string[]>(CancellationToken);

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(Predicate, skip: 1, take: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkipAndTakeWhenSkipLessThanZero()
    {
        await using var stream = new MemoryStream("c1\nv1\nc2\nv2\nc3\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => reader.ReadRecordsAsync<string[]>(Predicate, skip: -1, take: 1, cancellationToken: CancellationToken));
    }

    [TestMethod]
    public async Task ReadRecordsWithPredicateAndSkipAndTakeWhenTakeLessThanZero()
    {
        await using var stream = new MemoryStream("c1\nv1\nc2\nv2\nc3\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        static bool Predicate(TabularRecord<string[]> record)
        {
            return record.Content is [ ['v', ..], ..];
        }

        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => reader.ReadRecordsAsync<string[]>(Predicate, skip: 1, take: -1, cancellationToken: CancellationToken));
    }

    [TestMethod]
    public async Task ReadRecordsWithSkip()
    {
        await using var stream = new MemoryStream("v1\nv2"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(skip: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("v2", result[0].FirstOrDefault());
    }

    [TestMethod]
    public async Task ReadRecordsWithSkipWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        await reader.ReadRecordAsync<string[]>(CancellationToken);

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(skip: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWithSkipWhenSkipLessThanZero()
    {
        await using var stream = new MemoryStream("v1\nv2"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => reader.ReadRecordsAsync<string[]>(skip: -1, cancellationToken: CancellationToken));
    }

    [TestMethod]
    public async Task ReadRecordsWithSkipAndTake()
    {
        await using var stream = new MemoryStream("v1\nv2\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(skip: 1, take: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(1, result.Count);
        Assert.AreEqual("v2", result[0].FirstOrDefault());
    }

    [TestMethod]
    public async Task ReadRecordsWithSkipAndTakeWhenTakeZero()
    {
        await using var stream = new MemoryStream("v1\nv2\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(skip: 1, take: 0, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWithSkipAndTakeWhenNoMoreRecords()
    {
        await using var stream = new MemoryStream("v1"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        await reader.ReadRecordAsync<string[]>(CancellationToken);

        var result = new List<string[]>();

        await foreach (var record in reader.ReadRecordsAsync<string[]>(skip: 1, take: 1, cancellationToken: CancellationToken))
        {
            Assert.IsTrue(record.HasContent);

            result.Add(record.Content);
        }

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public async Task ReadRecordsWithSkipAndTakeWhenSkipLessThanZero()
    {
        await using var stream = new MemoryStream("v1\nv2\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => reader.ReadRecordsAsync<string[]>(skip: -1, take: 1, cancellationToken: CancellationToken));
    }

    [TestMethod]
    public async Task ReadRecordsWithSkipAndTakeWhenTakeLessThanZero()
    {
        await using var stream = new MemoryStream("v1\nv2\nv3"u8.ToArray());
        await using var reader = new TabularRecordReader(stream, new("\n", ',', '"'));

        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => reader.ReadRecordsAsync<string[]>(skip: 1, take: -1, cancellationToken: CancellationToken));
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
