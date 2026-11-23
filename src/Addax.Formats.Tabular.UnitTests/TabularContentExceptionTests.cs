namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularContentExceptionTests
{
    [TestMethod]
    [DataRow(long.MinValue)]
    [DataRow(-1L)]
    public void ConstructorWithPositionThrowsForNegativePosition(long position)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new TabularContentException("message", position));

        Assert.AreEqual("position", exception.ParamName);
    }

    [TestMethod]
    [DataRow(long.MinValue)]
    [DataRow(-1L)]
    public void ConstructorWithPositionAndInnerExceptionThrowsForNegativePosition(long position)
    {
        var exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new TabularContentException("message", position, new InvalidOperationException()));

        Assert.AreEqual("position", exception.ParamName);
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(long.MaxValue)]
    [DataRow(null)]
    public void ConstructorWithPositionSetsPosition(long? position)
    {
        var exception = new TabularContentException("message", position);

        Assert.AreEqual(position, exception.Position);
    }

    [TestMethod]
    [DataRow(0L)]
    [DataRow(long.MaxValue)]
    [DataRow(null)]
    public void ConstructorWithPositionAndInnerExceptionSetsPosition(long? position)
    {
        var exception = new TabularContentException("message", position, new InvalidOperationException());

        Assert.AreEqual(position, exception.Position);
    }

    [TestMethod]
    public void ConstructorLeavesPositionUnknown()
    {
        var exception = new TabularContentException();

        Assert.IsNull(exception.Position);
    }

    [TestMethod]
    public void ConstructorWithMessageLeavesPositionUnknown()
    {
        var exception = new TabularContentException("message");

        Assert.IsNull(exception.Position);
    }

    [TestMethod]
    public void ConstructorWithMessageAndInnerExceptionLeavesPositionUnknown()
    {
        var exception = new TabularContentException("message", new InvalidOperationException());

        Assert.IsNull(exception.Position);
    }
}
