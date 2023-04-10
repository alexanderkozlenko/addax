using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Addax.Formats.Tabular.UnitTests;

[TestClass]
public sealed class TabularRecordReaderOptionsTests
{
    [TestMethod]
    public void BufferSizeLessThanZero()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularRecordReaderOptions(bufferSize: -1));
    }

    [TestMethod]
    public void BufferSizeGreaterThanArrayMaxLength()
    {
        Assert.ThrowsException<ArgumentOutOfRangeException>(
            () => new TabularRecordReaderOptions(bufferSize: Array.MaxLength + 1));
    }
}
