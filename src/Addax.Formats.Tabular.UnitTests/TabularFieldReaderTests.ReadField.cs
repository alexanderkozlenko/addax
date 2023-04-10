#pragma warning disable IDE0025
#pragma warning disable IDE1006

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

public partial class TabularFieldReaderTests
{
    public static IEnumerable<object[]> ReadFieldWhenBeginningOfStreamData => TabularTestingFactory.ExpandDynamicData(
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

    [DataTestMethod, DynamicData(nameof(ReadFieldWhenBeginningOfStreamData))]
    public async Task ReadFieldWhenBeginningOfStream(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        var result = await reader.ReadFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> ReadFieldWhenBeginningOfRecordData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "", true, "lf-1a-1b-1c" },
            new object[] { "", true, "lf-1a-1b-1b" },
            new object[] { "", true, "crlf-1a-1b-1c" },
            new object[] { "", true, "crlf-1a-1b-1b" },
            new object[] { "v", true, "lf-1a-1b-1c" },
            new object[] { "v", true, "lf-1a-1b-1b" },
            new object[] { "v", true, "crlf-1a-1b-1c" },
            new object[] { "v", true, "crlf-1a-1b-1b" },
            new object[] { "qvq", true, "lf-1a-1b-1c" },
            new object[] { "qvq", true, "lf-1a-1b-1b" },
            new object[] { "qvq", true, "crlf-1a-1b-1c" },
            new object[] { "qvq", true, "crlf-1a-1b-1b" },
            new object[] { "d", true, "lf-1a-1b-1c" },
            new object[] { "d", true, "lf-1a-1b-1b" },
            new object[] { "d", true, "crlf-1a-1b-1c" },
            new object[] { "d", true, "crlf-1a-1b-1b" },
            new object[] { "l", true, "lf-1a-1b-1c" },
            new object[] { "l", true, "lf-1a-1b-1b" },
            new object[] { "lt", true, "crlf-1a-1b-1c" },
            new object[] { "lt", true, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(ReadFieldWhenBeginningOfRecordData))]
    public async Task ReadFieldWhenBeginningOfRecord(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);

        var result = await reader.ReadFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> ReadFieldWhenFieldSeparationData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "d", true, "lf-1a-1b-1c" },
            new object[] { "d", true, "lf-1a-1b-1b" },
            new object[] { "d", true, "crlf-1a-1b-1c" },
            new object[] { "d", true, "crlf-1a-1b-1b" },
            new object[] { "dv", true, "lf-1a-1b-1c" },
            new object[] { "dv", true, "lf-1a-1b-1b" },
            new object[] { "dv", true, "crlf-1a-1b-1c" },
            new object[] { "dv", true, "crlf-1a-1b-1b" },
            new object[] { "dqvq", true, "lf-1a-1b-1c" },
            new object[] { "dqvq", true, "lf-1a-1b-1b" },
            new object[] { "dqvq", true, "crlf-1a-1b-1c" },
            new object[] { "dqvq", true, "crlf-1a-1b-1b" },
            new object[] { "dd", true, "lf-1a-1b-1c" },
            new object[] { "dd", true, "lf-1a-1b-1b" },
            new object[] { "dd", true, "crlf-1a-1b-1c" },
            new object[] { "dd", true, "crlf-1a-1b-1b" },
            new object[] { "dl", true, "lf-1a-1b-1c" },
            new object[] { "dl", true, "lf-1a-1b-1b" },
            new object[] { "dlt", true, "crlf-1a-1b-1c" },
            new object[] { "dlt", true, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(ReadFieldWhenFieldSeparationData))]
    public async Task ReadFieldWhenFieldSeparation(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);
        await reader.ReadFieldAsync(CancellationToken);

        var result = await reader.ReadFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> ReadFieldWhenEndOfRecordData => TabularTestingFactory.ExpandDynamicData(
        new[]
        {
            new object[] { "l", true, "lf-1a-1b-1c" },
            new object[] { "l", true, "lf-1a-1b-1b" },
            new object[] { "lt", true, "crlf-1a-1b-1c" },
            new object[] { "lt", true, "crlf-1a-1b-1b" },
            new object[] { "lv", true, "lf-1a-1b-1c" },
            new object[] { "lv", true, "lf-1a-1b-1b" },
            new object[] { "ltv", true, "crlf-1a-1b-1c" },
            new object[] { "ltv", true, "crlf-1a-1b-1b" },
            new object[] { "lqvq", true, "lf-1a-1b-1c" },
            new object[] { "lqvq", true, "lf-1a-1b-1b" },
            new object[] { "ltqvq", true, "crlf-1a-1b-1c" },
            new object[] { "ltqvq", true, "crlf-1a-1b-1b" },
            new object[] { "ld", true, "lf-1a-1b-1c" },
            new object[] { "ld", true, "lf-1a-1b-1b" },
            new object[] { "ltd", true, "crlf-1a-1b-1c" },
            new object[] { "ltd", true, "crlf-1a-1b-1b" },
            new object[] { "ll", true, "lf-1a-1b-1c" },
            new object[] { "ll", true, "lf-1a-1b-1b" },
            new object[] { "ltlt", true, "crlf-1a-1b-1c" },
            new object[] { "ltlt", true, "crlf-1a-1b-1b" },
        },
        new[]
        {
            "utf-8",
            "utf-8-bom",
            "utf-16",
            "utf-16-bom",
        });

    [DataTestMethod, DynamicData(nameof(ReadFieldWhenEndOfRecordData))]
    public async Task ReadFieldWhenEndOfRecord(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);
        await reader.MoveNextRecordAsync(CancellationToken);

        var result = await reader.ReadFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }

    public static IEnumerable<object[]> ReadFieldWhenEndOfStreamData => TabularTestingFactory.ExpandDynamicData(
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

    [DataTestMethod, DynamicData(nameof(ReadFieldWhenEndOfStreamData))]
    public async Task ReadFieldWhenEndOfStream(string streamScript, bool expected, string dialectScript, string encodingName)
    {
        var encoding = TabularTestingFactory.CreateEncoding(encodingName);
        var dialect = TabularTestingFactory.CreateDialect(dialectScript);

        await using var stream = new MemoryStream(encoding.GetBytes(TabularTestingFactory.DecodeContentScript(dialect, streamScript)));
        await using var reader = new TabularFieldReader(stream, dialect, new(encoding));

        await reader.MoveNextRecordAsync(CancellationToken);
        await reader.MoveNextRecordAsync(CancellationToken);

        var result = await reader.ReadFieldAsync(CancellationToken);

        Assert.AreEqual(expected, result);
    }
}
