using System.Numerics;
using System.Text;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularRegistryTests
{
    [TestMethod]
    [DataRow(typeof(bool[]))]
    [DataRow(typeof(bool?[]))]
    [DataRow(typeof(byte[]))]
    [DataRow(typeof(byte?[]))]
    [DataRow(typeof(char[]))]
    [DataRow(typeof(char?[]))]
    [DataRow(typeof(DateOnly[]))]
    [DataRow(typeof(DateOnly?[]))]
    [DataRow(typeof(DateTime[]))]
    [DataRow(typeof(DateTime?[]))]
    [DataRow(typeof(DateTimeOffset[]))]
    [DataRow(typeof(DateTimeOffset?[]))]
    [DataRow(typeof(decimal[]))]
    [DataRow(typeof(decimal?[]))]
    [DataRow(typeof(double[]))]
    [DataRow(typeof(double?[]))]
    [DataRow(typeof(Guid[]))]
    [DataRow(typeof(Guid?[]))]
    [DataRow(typeof(Half[]))]
    [DataRow(typeof(Half?[]))]
    [DataRow(typeof(short[]))]
    [DataRow(typeof(short?[]))]
    [DataRow(typeof(int[]))]
    [DataRow(typeof(int?[]))]
    [DataRow(typeof(long[]))]
    [DataRow(typeof(long?[]))]
    [DataRow(typeof(Int128[]))]
    [DataRow(typeof(Int128?[]))]
    [DataRow(typeof(sbyte[]))]
    [DataRow(typeof(sbyte?[]))]
    [DataRow(typeof(float[]))]
    [DataRow(typeof(float?[]))]
    [DataRow(typeof(string[]))]
    [DataRow(typeof(TimeOnly[]))]
    [DataRow(typeof(TimeOnly?[]))]
    [DataRow(typeof(TimeSpan[]))]
    [DataRow(typeof(TimeSpan?[]))]
    [DataRow(typeof(ushort[]))]
    [DataRow(typeof(ushort?[]))]
    [DataRow(typeof(uint[]))]
    [DataRow(typeof(uint?[]))]
    [DataRow(typeof(ulong[]))]
    [DataRow(typeof(ulong?[]))]
    [DataRow(typeof(UInt128[]))]
    [DataRow(typeof(UInt128?[]))]
    [DataRow(typeof(Uri[]))]
    [DataRow(typeof(BFloat16[]))]
    [DataRow(typeof(BFloat16?[]))]
    [DataRow(typeof(BigInteger[]))]
    [DataRow(typeof(BigInteger?[]))]
    [DataRow(typeof(Decimal32[]))]
    [DataRow(typeof(Decimal32?[]))]
    [DataRow(typeof(Decimal64[]))]
    [DataRow(typeof(Decimal64?[]))]
    [DataRow(typeof(Decimal128[]))]
    [DataRow(typeof(Decimal128?[]))]
    [DataRow(typeof(Rune[]))]
    [DataRow(typeof(Rune?[]))]
    public void HandlersContainsBuiltInArrayHandler(Type recordType)
    {
        Assert.IsTrue(TabularRegistry.Handlers.TryGetValue(recordType, out var handler));
        Assert.IsInstanceOfType(handler, typeof(TabularHandler<>).MakeGenericType(recordType));
    }

    [TestMethod]
    [DataRow(typeof(bool))]
    [DataRow(typeof(byte))]
    [DataRow(typeof(char))]
    [DataRow(typeof(DateOnly))]
    [DataRow(typeof(DateTime))]
    [DataRow(typeof(DateTimeOffset))]
    [DataRow(typeof(decimal))]
    [DataRow(typeof(double))]
    [DataRow(typeof(Guid))]
    [DataRow(typeof(Half))]
    [DataRow(typeof(short))]
    [DataRow(typeof(int))]
    [DataRow(typeof(long))]
    [DataRow(typeof(Int128))]
    [DataRow(typeof(sbyte))]
    [DataRow(typeof(float))]
    [DataRow(typeof(TimeOnly))]
    [DataRow(typeof(TimeSpan))]
    [DataRow(typeof(ushort))]
    [DataRow(typeof(uint))]
    [DataRow(typeof(ulong))]
    [DataRow(typeof(UInt128))]
    [DataRow(typeof(Uri))]
    [DataRow(typeof(BFloat16))]
    [DataRow(typeof(BigInteger))]
    [DataRow(typeof(Decimal32))]
    [DataRow(typeof(Decimal64))]
    [DataRow(typeof(Decimal128))]
    [DataRow(typeof(Rune))]
    public void ConvertersContainsBuiltInConverter(Type valueType)
    {
        Assert.IsTrue(TabularRegistry.Converters.TryGetValue(valueType, out var converter));
        Assert.IsInstanceOfType(converter, typeof(TabularConverter<>).MakeGenericType(valueType));
    }

    [TestMethod]
    [DataRow(typeof(byte[]))]
    [DataRow(typeof(Memory<byte>))]
    [DataRow(typeof(ReadOnlyMemory<byte>))]
    public void ConvertersDoesNotContainBinaryConverter(Type valueType)
    {
        Assert.IsFalse(TabularRegistry.Converters.ContainsKey(valueType));
    }

    [TestMethod]
    public void ReaderUsesHandlerFromOptionsWithoutRegistration()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(UnregisteredRecord)] = new UnregisteredRecordHandler();

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^w"));
        using var reader = new TabularReader<UnregisteredRecord>(stream, dialect, options);

        Assert.IsTrue(reader.TryReadRecord());
        Assert.AreEqual("v|w", reader.CurrentRecord.Value);
    }

    [TestMethod]
    public async Task ReaderUsesHandlerFromOptionsWithoutRegistrationAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(UnregisteredRecord)] = new UnregisteredRecordHandler();

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes("v^w"));
        await using var reader = new TabularReader<UnregisteredRecord>(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadRecordAsync());
        Assert.AreEqual("v|w", reader.CurrentRecord.Value);
    }

    [TestMethod]
    public void WriterUsesHandlerFromOptionsWithoutRegistration()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(UnregisteredRecord)] = new UnregisteredRecordHandler();

        using var stream = new MemoryStream();
        using var writer = new TabularWriter<UnregisteredRecord>(stream, dialect, options);

        writer.WriteRecord(new("v|w"));
        writer.Flush();

        Assert.AreEqual("v^w$", Encoding.UTF8.GetString(stream.ToArray()));
    }

    [TestMethod]
    public async Task WriterUsesHandlerFromOptionsWithoutRegistrationAsync()
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions();

        options.Handlers[typeof(UnregisteredRecord)] = new UnregisteredRecordHandler();

        await using var stream = new MemoryStream();
        await using var writer = new TabularWriter<UnregisteredRecord>(stream, dialect, options);

        await writer.WriteRecordAsync(new("v|w"));
        await writer.FlushAsync();

        Assert.AreEqual("v^w$", Encoding.UTF8.GetString(stream.ToArray()));
    }

    private static TabularDialect CreateDialect(string lineTerminator, char escapeChar)
    {
        return new(lineTerminator, '^', '\'', escapeChar);
    }
}
