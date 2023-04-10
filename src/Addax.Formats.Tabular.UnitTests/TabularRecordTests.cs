using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularRecordTests
{
    [TestMethod]
    public void AsContent()
    {
        var record = TabularRecord<string>.AsContent("v");

        Assert.IsTrue(record.HasContent);
        Assert.AreEqual("v", record.Content);
        Assert.IsNull(record.Comment);
    }

    [TestMethod]
    public void AsComment()
    {
        var record = TabularRecord<string>.AsComment("v");

        Assert.IsFalse(record.HasContent);
        Assert.IsNull(record.Content);
        Assert.AreEqual("v", record.Comment);
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void OperatorEqualityAsComment(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsComment(value1);
        var record2 = TabularRecord<string>.AsComment(value2);

        Assert.AreEqual(result, record1 == record2);
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void OperatorEqualityAsContent(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsContent(value1);
        var record2 = TabularRecord<string>.AsContent(value2);

        Assert.AreEqual(result, record1 == record2);
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void OperatorInequalityAsComment(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsComment(value1);
        var record2 = TabularRecord<string>.AsComment(value2);

        Assert.AreEqual(result, !(record1 != record2));
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void OperatorInequalityAsContent(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsContent(value1);
        var record2 = TabularRecord<string>.AsContent(value2);

        Assert.AreEqual(result, !(record1 != record2));
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void EqualityEqualsAsComment(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsComment(value1);
        var record2 = TabularRecord<string>.AsComment(value2);

        Assert.AreEqual(result, record1.Equals(record2));
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void EqualityEqualsAsContent(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsContent(value1);
        var record2 = TabularRecord<string>.AsContent(value2);

        Assert.AreEqual(result, record1.Equals(record2));
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void ObjectEqualsAsComment(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsComment(value1);
        var record2 = TabularRecord<string>.AsComment(value2);

        Assert.AreEqual(result, record1.Equals((object)record2));
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    [DataRow("v2", "v1", false)]
    public void ObjectEqualsAsContent(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsContent(value1);
        var record2 = TabularRecord<string>.AsContent(value2);

        Assert.AreEqual(result, record1.Equals((object)record2));
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    public void GetHashCodeAsComment(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsComment(value1);
        var record2 = TabularRecord<string>.AsComment(value2);

        Assert.AreEqual(result, record1.GetHashCode() == record2.GetHashCode());
    }

    [DataTestMethod]
    [DataRow("v1", "v1", true)]
    [DataRow("v1", "v2", false)]
    public void GetHashCodeAsContent(string value1, string value2, bool result)
    {
        var record1 = TabularRecord<string>.AsContent(value1);
        var record2 = TabularRecord<string>.AsContent(value2);

        Assert.AreEqual(result, record1.GetHashCode() == record2.GetHashCode());
    }
}
