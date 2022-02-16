using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopIt.Data;

namespace PopIt.Test;

[TestClass]
public class RectangleTests
{
    [TestMethod]
    public void Right_Bottom_Test()
    {
        var rect = new Rectangle(3, 4, 2, 8);
        var res1 = rect.Bottom == 12;
        var res2 = rect.Right == 5;
        Assert.IsTrue(res1);
        Assert.IsTrue(res2);
    }
}

