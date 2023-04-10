using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularFieldReaderOptionsTests
{
    [TestMethod]
    public void BufferSizeLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularFieldReaderOptions(bufferSize: -1));
    }

    [TestMethod]
    public void BufferSizeGreaterThanArrayMaxLength()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularFieldReaderOptions(bufferSize: Array.MaxLength + 1));
    }
}
