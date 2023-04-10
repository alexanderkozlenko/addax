#pragma warning disable IDE0025
#pragma warning disable IDE1006

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

public partial class TabularFieldReaderTests
{
    public static IEnumerable<object[]> MoveNextFieldWhenBeginningOfStreamData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", false, "lf-1a-1b-1c" },
            new object[] { "", false, "lf-1a-1b-1b" },
            new object[] { "", false, "crlf-1a-1b-1c" },
            new object[] { "", false, "crlf-1a-1b-1b" },
            new object[] { "v", false, "lf-1a-1b-1c" },
            new object[] { "v", false, "lf-1a-1b-1b" },
            new object[] { "v", false, "crlf-1a-1b-1c" },
            new object[] { "v", false, "crlf-1a-1b-1b" },
            new object[] { "qvq", false, "lf-1a-1b-1c" },
            new object[] { "qvq", false, "lf-1a-1b-1b" },
            new object[] { "qvq", false, "crlf-1a-1b-1c" },
            new object[] { "qvq", false, "crlf-1a-1b-1b" },
            new object[] { "d", false, "lf-1a-1b-1c" },
            new object[] { "d", false, "lf-1a-1b-1b" },
            new object[] { "d", false, "crlf-1a-1b-1c" },
            new object[] { "d", false, "crlf-1a-1b-1b" },
            new object[] { "l", false, "lf-1a-1b-1c" },
            new object[] { "l", false, "lf-1a-1b-1b" },
            new object[] { "lt", false, "crlf-1a-1b-1c" },
            new object[] { "lt", false, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(MoveNextFieldWhenBeginningOfStreamData))]
    public async Task MoveNextFieldWhenBeginningOfStream(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        var result = await reader.MoveNextFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> MoveNextFieldWhenBeginningOfRecordData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", false, "lf-1a-1b-1c" },
            new object[] { "", false, "lf-1a-1b-1b" },
            new object[] { "", false, "crlf-1a-1b-1c" },
            new object[] { "", false, "crlf-1a-1b-1b" },
            new object[] { "v", false, "lf-1a-1b-1c" },
            new object[] { "v", false, "lf-1a-1b-1b" },
            new object[] { "v", false, "crlf-1a-1b-1c" },
            new object[] { "v", false, "crlf-1a-1b-1b" },
            new object[] { "qvq", false, "lf-1a-1b-1c" },
            new object[] { "qvq", false, "lf-1a-1b-1b" },
            new object[] { "qvq", false, "crlf-1a-1b-1c" },
            new object[] { "qvq", false, "crlf-1a-1b-1b" },
            new object[] { "d", true, "lf-1a-1b-1c" },
            new object[] { "d", true, "lf-1a-1b-1b" },
            new object[] { "d", true, "crlf-1a-1b-1c" },
            new object[] { "d", true, "crlf-1a-1b-1b" },
            new object[] { "l", false, "lf-1a-1b-1c" },
            new object[] { "l", false, "lf-1a-1b-1b" },
            new object[] { "lt", false, "crlf-1a-1b-1c" },
            new object[] { "lt", false, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(MoveNextFieldWhenBeginningOfRecordData))]
    public async Task MoveNextFieldWhenBeginningOfRecord(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);

        var result = await reader.MoveNextFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> MoveNextFieldWhenFieldSeparationData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "d", false, "lf-1a-1b-1c" },
            new object[] { "d", false, "lf-1a-1b-1b" },
            new object[] { "d", false, "crlf-1a-1b-1c" },
            new object[] { "d", false, "crlf-1a-1b-1b" },
            new object[] { "dv", false, "lf-1a-1b-1c" },
            new object[] { "dv", false, "lf-1a-1b-1b" },
            new object[] { "dv", false, "crlf-1a-1b-1c" },
            new object[] { "dv", false, "crlf-1a-1b-1b" },
            new object[] { "dqvq", false, "lf-1a-1b-1c" },
            new object[] { "dqvq", false, "lf-1a-1b-1b" },
            new object[] { "dqvq", false, "crlf-1a-1b-1c" },
            new object[] { "dqvq", false, "crlf-1a-1b-1b" },
            new object[] { "dd", true, "lf-1a-1b-1c" },
            new object[] { "dd", true, "lf-1a-1b-1b" },
            new object[] { "dd", true, "crlf-1a-1b-1c" },
            new object[] { "dd", true, "crlf-1a-1b-1b" },
            new object[] { "dl", false, "lf-1a-1b-1c" },
            new object[] { "dl", false, "lf-1a-1b-1b" },
            new object[] { "dlt", false, "crlf-1a-1b-1c" },
            new object[] { "dlt", false, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(MoveNextFieldWhenFieldSeparationData))]
    public async Task MoveNextFieldWhenFieldSeparation(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);
        await reader.ReadFieldAsync(CancellationToken);

        var result = await reader.MoveNextFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> MoveNextFieldWhenEndOfRecordData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "l", false, "lf-1a-1b-1c" },
            new object[] { "l", false, "lf-1a-1b-1b" },
            new object[] { "lt", false, "crlf-1a-1b-1c" },
            new object[] { "lt", false, "crlf-1a-1b-1b" },
            new object[] { "lv", false, "lf-1a-1b-1c" },
            new object[] { "lv", false, "lf-1a-1b-1b" },
            new object[] { "ltv", false, "crlf-1a-1b-1c" },
            new object[] { "ltv", false, "crlf-1a-1b-1b" },
            new object[] { "lqvq", false, "lf-1a-1b-1c" },
            new object[] { "lqvq", false, "lf-1a-1b-1b" },
            new object[] { "ltqvq", false, "crlf-1a-1b-1c" },
            new object[] { "ltqvq", false, "crlf-1a-1b-1b" },
            new object[] { "ld", true, "lf-1a-1b-1c" },
            new object[] { "ld", true, "lf-1a-1b-1b" },
            new object[] { "ltd", true, "crlf-1a-1b-1c" },
            new object[] { "ltd", true, "crlf-1a-1b-1b" },
            new object[] { "ll", false, "lf-1a-1b-1c" },
            new object[] { "ll", false, "lf-1a-1b-1b" },
            new object[] { "ltlt", false, "crlf-1a-1b-1c" },
            new object[] { "ltlt", false, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(MoveNextFieldWhenEndOfRecordData))]
    public async Task MoveNextFieldWhenEndOfRecord(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);
        await reader.MoveNextRecordAsync(CancellationToken);

        var result = await reader.MoveNextFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> MoveNextFieldWhenEndOfStreamData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", false, "lf-1a-1b-1c" },
            new object[] { "", false, "lf-1a-1b-1b" },
            new object[] { "", false, "crlf-1a-1b-1c" },
            new object[] { "", false, "crlf-1a-1b-1b" },
            new object[] { "v", false, "lf-1a-1b-1c" },
            new object[] { "v", false, "lf-1a-1b-1b" },
            new object[] { "v", false, "crlf-1a-1b-1c" },
            new object[] { "v", false, "crlf-1a-1b-1b" },
            new object[] { "qvq", false, "lf-1a-1b-1c" },
            new object[] { "qvq", false, "lf-1a-1b-1b" },
            new object[] { "qvq", false, "crlf-1a-1b-1c" },
            new object[] { "qvq", false, "crlf-1a-1b-1b" },
            new object[] { "d", false, "lf-1a-1b-1c" },
            new object[] { "d", false, "lf-1a-1b-1b" },
            new object[] { "d", false, "crlf-1a-1b-1c" },
            new object[] { "d", false, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(MoveNextFieldWhenEndOfStreamData))]
    public async Task MoveNextFieldWhenEndOfStream(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);
        await reader.MoveNextRecordAsync(CancellationToken);

        var result = await reader.MoveNextFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }
}
