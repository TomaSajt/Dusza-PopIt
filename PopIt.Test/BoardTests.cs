using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace PopIt.Test;

[TestClass]
public class BoardTests
{
    [TestMethod]
    public void SanityCheck()
    {
        Assert.AreEqual(1, 1, "Math has crumbled");
    }
}
