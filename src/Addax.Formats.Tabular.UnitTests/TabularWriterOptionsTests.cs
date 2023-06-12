using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularWriterOptionsTests
{
    [TestMethod]
    public void BufferSizeLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularWriterOptions(bufferSize: -1));
    }

    [TestMethod]
    public void BufferSizeGreaterThanArrayMaxLength()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularWriterOptions(bufferSize: Array.MaxLength + 1));
    }
}
