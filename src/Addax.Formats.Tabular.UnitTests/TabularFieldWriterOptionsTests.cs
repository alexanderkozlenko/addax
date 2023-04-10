using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularFieldWriterOptionsTests
{
    [TestMethod]
    public void BufferSizeLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularFieldWriterOptions(bufferSize: -1));
    }

    [TestMethod]
    public void BufferSizeGreaterThanArrayMaxLength()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularFieldWriterOptions(bufferSize: Array.MaxLength + 1));
    }
}
