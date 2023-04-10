using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularRecordWriterOptionsTests
{
    [TestMethod]
    public void BufferSizeLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularRecordWriterOptions(bufferSize: -1));
    }

    [TestMethod]
    public void BufferSizeGreaterThanArrayMaxLength()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularRecordWriterOptions(bufferSize: Array.MaxLength + 1));
    }
}
