using System.Text;
using Addax.Formats.Tabular.Converters;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed partial class TabularStringFactoryTests
{
    [TestMethod]
    [DataRow("\u00e9\ud83d\ude00")]
    [DataRow("a")]
    [DataRow("abc")]
    public void CreateReturnsEqualString(string value)
    {
        using var factory = new TabularStringFactory();

        Assert.AreEqual(value, factory.Create(value.ToCharArray()));
    }

    [TestMethod]
    [DataRow(0, "a")]
    [DataRow(4, "a")]
    [DataRow(4, "abcd")]
    [DataRow(4, "abcde")]
    public void CreateWithMaxLengthReturnsEqualString(int maxLength, string value)
    {
        using var factory = new TabularStringFactory(maxLength);

        Assert.AreEqual(value, factory.Create(value.ToCharArray()));
    }

    [TestMethod]
    public void CreateReturnsEmptyStringForEmptySource()
    {
        using var factory = new TabularStringFactory();

        Assert.AreSame(string.Empty, factory.Create([]));
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(4)]
    public void CreateWithMaxLengthReturnsEmptyStringForEmptySource(int maxLength)
    {
        using var factory = new TabularStringFactory(maxLength);

        Assert.AreSame(string.Empty, factory.Create([]));
    }

    [TestMethod]
    [DataRow("a")]
    [DataRow("abc")]
    public void CreateWithoutPoolingReturnsNewInstance(string value)
    {
        using var factory = new TabularStringFactory();

        var value1 = factory.Create(value.ToCharArray());
        var value2 = factory.Create(value.ToCharArray());

        Assert.AreNotSame(value1, value2);
    }

    [TestMethod]
    [DataRow(1, "a")]
    [DataRow(4, "a")]
    [DataRow(4, "abcd")]
    [DataRow(int.MaxValue, "abcd")]
    public void CreateWithMaxLengthReturnsSameInstanceForEqualValues(int maxLength, string value)
    {
        using var factory = new TabularStringFactory(maxLength);

        var value1 = factory.Create(value.ToCharArray());
        var value2 = factory.Create(value.ToCharArray());

        Assert.AreSame(value1, value2);
    }

    [TestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    [DataRow(16)]
    public void CreateWithPoolCapacityReturnsSameInstanceForEqualValues(int poolCapacity)
    {
        using var factory = new TabularStringFactory(4, poolCapacity);

        var value1 = factory.Create("abcd".ToCharArray());
        var value2 = factory.Create("abcd".ToCharArray());

        Assert.AreSame(value1, value2);
    }

    [TestMethod]
    [DataRow("2020", "2021", "2022")]
    [DataRow("a", "b", "c")]
    [DataRow("item1", "item2", "item3")]
    public void CreateWithMaxLengthReturnsSameInstancesForDistinctValues(string value1, string value2, string value3)
    {
        using var factory = new TabularStringFactory(8);

        var instance1 = factory.Create(value1.ToCharArray());
        var instance2 = factory.Create(value2.ToCharArray());
        var instance3 = factory.Create(value3.ToCharArray());

        Assert.AreSame(instance1, factory.Create(value1.ToCharArray()));
        Assert.AreSame(instance2, factory.Create(value2.ToCharArray()));
        Assert.AreSame(instance3, factory.Create(value3.ToCharArray()));
    }

    [TestMethod]
    [DataRow("a", "b")]
    [DataRow("abcd", "dcba")]
    public void CreateWithPoolCapacityReplacesPooledString(string value1, string value2)
    {
        using var factory = new TabularStringFactory(4, 1);

        var instance1 = factory.Create(value1.ToCharArray());

        factory.Create(value2.ToCharArray());

        var instance2 = factory.Create(value1.ToCharArray());

        Assert.AreNotSame(instance1, instance2);
        Assert.AreSame(instance2, factory.Create(value1.ToCharArray()));
    }

    [TestMethod]
    [DataRow("a")]
    [DataRow("abcd")]
    public void CreateAfterDisposeReturnsNewInstance(string value)
    {
        var factory = new TabularStringFactory(4);

        factory.Create(value.ToCharArray());
        factory.Dispose();

        var value1 = factory.Create(value.ToCharArray());
        var value2 = factory.Create(value.ToCharArray());

        Assert.AreEqual(value, value1);
        Assert.AreNotSame(value1, value2);
    }

    [TestMethod]
    [DataRow(4, "a", "ab")]
    [DataRow(4, "ab", "ba")]
    public void CreateWithMaxLengthReturnsEqualStringAfterOtherValues(int maxLength, string value1, string value2)
    {
        using var factory = new TabularStringFactory(maxLength);

        Assert.AreEqual(value1, factory.Create(value1.ToCharArray()));
        Assert.AreEqual(value2, factory.Create(value2.ToCharArray()));
        Assert.AreEqual(value1, factory.Create(value1.ToCharArray()));
    }

    [TestMethod]
    [DataRow(int.MinValue, "a")]
    [DataRow(-1, "a")]
    [DataRow(0, "a")]
    [DataRow(4, "abcde")]
    public void CreateWithMaxLengthReturnsNewInstanceAboveMaxLength(int maxLength, string value)
    {
        using var factory = new TabularStringFactory(maxLength);

        var value1 = factory.Create(value.ToCharArray());
        var value2 = factory.Create(value.ToCharArray());

        Assert.AreNotSame(value1, value2);
    }

    [TestMethod]
    [DataRow("$", "<>")]
    [DataRow("'a'", "<a>")]
    [DataRow("a", "<a>")]
    public void ReaderGetStringUsesStringFactory(string content, string expected)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { StringFactory = new BracketingStringFactory() };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(reader.TryReadField());
        Assert.AreEqual(expected, reader.GetString());
    }

    [TestMethod]
    [DataRow("$", "<>")]
    [DataRow("'a'", "<a>")]
    [DataRow("a", "<a>")]
    public async Task ReaderGetStringUsesStringFactoryAsync(string content, string expected)
    {
        var dialect = CreateDialect("$", '\'');
        var options = new TabularOptions { StringFactory = new BracketingStringFactory() };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        Assert.IsTrue(await reader.TryReadFieldAsync());
        Assert.AreEqual(expected, reader.GetString());
    }

    [TestMethod]
    [DataRow("'a'^a^'a'")]
    [DataRow("a^a^a")]
    public void ReaderGetStringWithPoolingStringFactoryReturnsSameInstance(string content)
    {
        var dialect = CreateDialect("$", '\'');

        using var factory = new TabularStringFactory(4);

        var options = new TabularOptions { StringFactory = factory };

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var reader = new TabularReader(stream, dialect, options);

        var expected = factory.Create("a".ToCharArray());

        while (reader.TryReadField())
        {
            Assert.AreSame(expected, reader.GetString());
        }
    }

    [TestMethod]
    [DataRow("'a'^a^'a'")]
    [DataRow("a^a^a")]
    public async Task ReaderGetStringWithPoolingStringFactoryReturnsSameInstanceAsync(string content)
    {
        var dialect = CreateDialect("$", '\'');

        using var factory = new TabularStringFactory(4);

        var options = new TabularOptions { StringFactory = factory };

        await using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await using var reader = new TabularReader(stream, dialect, options);

        var expected = factory.Create("a".ToCharArray());

        while (await reader.TryReadFieldAsync())
        {
            Assert.AreSame(expected, reader.GetString());
        }
    }

    [TestMethod]
    public void UriConverterUsesStringFactory()
    {
        var converter = new TabularUriConverter(new PrefixingStringFactory());

        Assert.IsTrue(converter.TryParse("example", null, out var value));
        Assert.AreEqual(new Uri("urn:example"), value);
    }

    private static TabularDialect CreateDialect(string lineTerminator, char escapeChar)
    {
        return new(lineTerminator, '^', '\'', escapeChar);
    }
}
