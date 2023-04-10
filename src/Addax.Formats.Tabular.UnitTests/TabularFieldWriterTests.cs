#pragma warning disable IDE0025
#pragma warning disable IDE1006

using System.Buffers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularFieldWriterTests
{
    [TestMethod]
    public async Task FlushOnDispose()
    {
        var dialect = new TabularDataDialect("\u000a", '\u001a', '\u001b', '\u001c');
        var stream = new MemoryStream();
        var writer = new TabularFieldWriter(stream, dialect, new(leaveOpen: true));

        writer.BeginRecord();
        writer.WriteString("v");

        await writer.DisposeAsync();

        stream.Seek(0, SeekOrigin.Begin);

        CollectionAssert.AreEqual("v"u8.ToArray(), stream.ToArray());
    }

    [TestMethod]
    public void WriteBeforeBeginRecord()
    {
        var dialect = new TabularDataDialect("\u000a", '\u001a', '\u001b', '\u001c');
        var stream = new MemoryStream();
        var writer = new TabularFieldWriter(stream, dialect, new(leaveOpen: true));

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteString("v"));
    }

    [TestMethod]
    public void WriteAfterComment()
    {
        var dialect = new TabularDataDialect("\u000a", '\u001a', '\u001b', '\u001c', '\u001d');
        var stream = new MemoryStream();
        var writer = new TabularFieldWriter(stream, dialect, new(leaveOpen: true));

        writer.BeginRecord();
        writer.WriteComment("v");

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteString("v"));
    }

    public static IEnumerable<object[]> WriteStringData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", "", "lf-1a-1b-1c" },
            new object[] { "", "", "lf-1a-1b-1b" },
            new object[] { "", "", "crlf-1a-1b-1c" },
            new object[] { "", "", "crlf-1a-1b-1b" },
            new object[] { "v", "v", "lf-1a-1b-1c" },
            new object[] { "v", "v", "lf-1a-1b-1b" },
            new object[] { "v", "v", "crlf-1a-1b-1c" },
            new object[] { "v", "v", "crlf-1a-1b-1b" },
            new object[] { "l", "qlq", "lf-1a-1b-1c" },
            new object[] { "l", "qlq", "lf-1a-1b-1b" },
            new object[] { "l", "l", "crlf-1a-1b-1c" },
            new object[] { "l", "l", "crlf-1a-1b-1b" },
            new object[] { "lt", "qltq", "crlf-1a-1b-1c" },
            new object[] { "lt", "qltq", "crlf-1a-1b-1b" },
            new object[] { "lv", "lv", "crlf-1a-1b-1c" },
            new object[] { "lv", "lv", "crlf-1a-1b-1b" },
            new object[] { "t", "t", "crlf-1a-1b-1c" },
            new object[] { "t", "t", "crlf-1a-1b-1b" },
            new object[] { "d", "qdq", "lf-1a-1b-1c" },
            new object[] { "d", "qdq", "lf-1a-1b-1b" },
            new object[] { "d", "qdq", "crlf-1a-1b-1c" },
            new object[] { "d", "qdq", "crlf-1a-1b-1b" },
            new object[] { "q", "qeqq", "lf-1a-1b-1c" },
            new object[] { "q", "qqqq", "lf-1a-1b-1b" },
            new object[] { "q", "qeqq", "crlf-1a-1b-1c" },
            new object[] { "q", "qqqq", "crlf-1a-1b-1b" },
            new object[] { "vq", "qveqq", "lf-1a-1b-1c" },
            new object[] { "vq", "qvqqq", "lf-1a-1b-1b" },
            new object[] { "vq", "qveqq", "crlf-1a-1b-1c" },
            new object[] { "vq", "qvqqq", "crlf-1a-1b-1b" },
            new object[] { "e", "e", "lf-1a-1b-1c" },
            new object[] { "e", "e", "crlf-1a-1b-1c" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    public static IEnumerable<object[]> WriteStringsData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", "d", "lf-1a-1b-1c" },
            new object[] { "", "d", "lf-1a-1b-1b" },
            new object[] { "", "d", "crlf-1a-1b-1c" },
            new object[] { "", "d", "crlf-1a-1b-1b" },
            new object[] { "v", "vdv", "lf-1a-1b-1c" },
            new object[] { "v", "vdv", "lf-1a-1b-1b" },
            new object[] { "v", "vdv", "crlf-1a-1b-1c" },
            new object[] { "v", "vdv", "crlf-1a-1b-1b" },
            new object[] { "l", "qlqdqlq", "lf-1a-1b-1c" },
            new object[] { "l", "qlqdqlq", "lf-1a-1b-1b" },
            new object[] { "l", "ldl", "crlf-1a-1b-1c" },
            new object[] { "l", "ldl", "crlf-1a-1b-1b" },
            new object[] { "lt", "qltqdqltq", "crlf-1a-1b-1c" },
            new object[] { "lt", "qltqdqltq", "crlf-1a-1b-1b" },
            new object[] { "t", "tdt", "crlf-1a-1b-1c" },
            new object[] { "t", "tdt", "crlf-1a-1b-1b" },
            new object[] { "lv", "lvdlv", "crlf-1a-1b-1c" },
            new object[] { "lv", "lvdlv", "crlf-1a-1b-1b" },
            new object[] { "d", "qdqdqdq", "lf-1a-1b-1c" },
            new object[] { "d", "qdqdqdq", "lf-1a-1b-1b" },
            new object[] { "d", "qdqdqdq", "crlf-1a-1b-1c" },
            new object[] { "d", "qdqdqdq", "crlf-1a-1b-1b" },
            new object[] { "q", "qeqqdqeqq", "lf-1a-1b-1c" },
            new object[] { "q", "qqqqdqqqq", "lf-1a-1b-1b" },
            new object[] { "q", "qeqqdqeqq", "crlf-1a-1b-1c" },
            new object[] { "q", "qqqqdqqqq", "crlf-1a-1b-1b" },
            new object[] { "vq", "qveqqdqveqq", "lf-1a-1b-1c" },
            new object[] { "vq", "qvqqqdqvqqq", "lf-1a-1b-1b" },
            new object[] { "vq", "qveqqdqveqq", "crlf-1a-1b-1c" },
            new object[] { "vq", "qvqqqdqvqqq", "crlf-1a-1b-1b" },
            new object[] { "e", "ede", "lf-1a-1b-1c" },
            new object[] { "e", "ede", "crlf-1a-1b-1c" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(WriteStringData))]
    public async Task WriteSpan(string inputScript, string expectedScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);
        var expectedContent = TabularTestingFactory.DecodeContentScript(dialect, expectedScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();
        writer.Write(inputContent.AsSpan());

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, encoding);
        var resultContent = await reader.ReadToEndAsync(CancellationToken);

        Assert.AreEqual(expectedContent, resultContent);
    }

    [DataTestMethod, DynamicData(nameof(WriteStringsData))]
    public async Task WriteSpans(string inputScript, string expectedScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);
        var expectedContent = TabularTestingFactory.DecodeContentScript(dialect, expectedScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();
        writer.Write(inputContent.AsSpan());
        writer.Write(inputContent.AsSpan());

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, encoding);
        var resultContent = await reader.ReadToEndAsync(CancellationToken);

        Assert.AreEqual(expectedContent, resultContent);
    }

    [DataTestMethod, DynamicData(nameof(WriteStringData))]
    public async Task WriteSequence(string inputScript, string expectedScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);
        var expectedContent = TabularTestingFactory.DecodeContentScript(dialect, expectedScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();
        writer.Write(new ReadOnlySequence<char>(inputContent.ToCharArray()));

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, encoding);
        var resultContent = await reader.ReadToEndAsync(CancellationToken);

        Assert.AreEqual(expectedContent, resultContent);
    }

    [DataTestMethod, DynamicData(nameof(WriteStringsData))]
    public async Task WriteSequences(string inputScript, string expectedScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);
        var expectedContent = TabularTestingFactory.DecodeContentScript(dialect, expectedScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();
        writer.Write(new ReadOnlySequence<char>(inputContent.ToCharArray()));
        writer.Write(new ReadOnlySequence<char>(inputContent.ToCharArray()));

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, encoding);
        var resultContent = await reader.ReadToEndAsync(CancellationToken);

        Assert.AreEqual(expectedContent, resultContent);
    }

    public static IEnumerable<object[]> WriteCommentData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", "c", "lf-1a-1b-1c-1d" },
            new object[] { "", "c", "crlf-1a-1b-1c-1d" },
            new object[] { "v", "cv", "lf-1a-1b-1c-1d" },
            new object[] { "v", "cv", "crlf-1a-1b-1c-1d" },
            new object[] { "q", "cq", "lf-1a-1b-1c-1d" },
            new object[] { "q", "cq", "crlf-1a-1b-1c-1d" },
            new object[] { "e", "ce", "lf-1a-1b-1c-1d" },
            new object[] { "e", "ce", "crlf-1a-1b-1c-1d" },
            new object[] { "c", "cc", "lf-1a-1b-1c-1d" },
            new object[] { "c", "cc", "crlf-1a-1b-1c-1d" },
            new object[] { "lv", "clv", "crlf-1a-1b-1c-1d" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(WriteCommentData))]
    public async Task WriteCommentSpan(string inputScript, string expectedScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);
        var expectedContent = TabularTestingFactory.DecodeContentScript(dialect, expectedScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();
        writer.WriteComment(inputContent.AsSpan());

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, encoding);
        var resultContent = await reader.ReadToEndAsync(CancellationToken);

        Assert.AreEqual(expectedContent, resultContent);
    }

    [DataTestMethod, DynamicData(nameof(WriteCommentData))]
    public async Task WriteCommentSequence(string inputScript, string expectedScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);
        var expectedContent = TabularTestingFactory.DecodeContentScript(dialect, expectedScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();
        writer.WriteComment(new ReadOnlySequence<char>(inputContent.ToCharArray()));

        await writer.FlushAsync(CancellationToken);

        stream.Seek(0, SeekOrigin.Begin);

        var reader = new StreamReader(stream, encoding);
        var resultContent = await reader.ReadToEndAsync(CancellationToken);

        Assert.AreEqual(expectedContent, resultContent);
    }

    public static IEnumerable<object[]> WriteUnsupportedCommentData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "l", "lf-1a-1b-1c-1d" },
            new object[] { "lt", "crlf-1a-1b-1c-1d" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(WriteUnsupportedCommentData))]
    public async Task WriteUnsupportedCommentSpan(string inputScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteComment(inputContent.AsSpan()));
    }

    [DataTestMethod, DynamicData(nameof(WriteUnsupportedCommentData))]
    public async Task WriteUnsupportedCommentSequence(string inputScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteComment(new ReadOnlySequence<char>(inputContent.ToCharArray())));
    }

    public static IEnumerable<object[]> WriteUnexpectedCommentData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "v", "lf-1a-1b-1c" },
            new object[] { "v", "crlf-1a-1b-1c" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(WriteUnexpectedCommentData))]
    public async Task WriteUnexpectedCommentSpan(string inputScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteComment(inputContent.AsSpan()));
    }

    [DataTestMethod, DynamicData(nameof(WriteUnexpectedCommentData))]
    public async Task WriteUnexpectedCommentSequence(string inputScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        writer.BeginRecord();

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteComment(new ReadOnlySequence<char>(inputContent.ToCharArray())));
    }

    public static IEnumerable<object[]> WriteCommentBeforeBeginRecordData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "v", "lf-1a-1b-1c-1d" },
            new object[] { "v", "crlf-1a-1b-1c-1d" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(WriteCommentBeforeBeginRecordData))]
    public async Task WriteCommentSpanBeforeBeginRecord(string inputScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteComment(inputContent.AsSpan()));
    }

    [DataTestMethod, DynamicData(nameof(WriteCommentBeforeBeginRecordData))]
    public async Task WriteCommentSequenceBeforeBeginRecord(string inputScript, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        var inputContent = TabularTestingFactory.DecodeContentScript(dialect, inputScript);

        await using var stream = new MemoryStream();
        await using var writer = new TabularFieldWriter(stream, dialect, new(encoding));

        Assert.ThrowsException<InvalidOperationException>(
            () => writer.WriteComment(new ReadOnlySequence<char>(inputContent.ToCharArray())));
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
}
