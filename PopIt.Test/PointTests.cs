using Microsoft.VisualStudio.TestTools.UnitTesting;
using PopIt.Data;

namespace PopIt.Test;

[TestClass]
public class PointTests
{
    [TestMethod]
    public void Addition_Test()
    {
        Point p1 = new(3, 4), p2 = new(5, 1);
        var res1 = p1 + p2;
        Assert.AreEqual(res1, new(8, 5));
    }
    [TestMethod]
    public void Subtraction_Test()
    {
        Point p1 = new(3, 4), p2 = new(5, 1);
        var res1 = p1 - p2;
        Assert.AreEqual(res1, new(-2, 3));
    }
    [TestMethod]
    public void Negation_Test()
    {
        Point p1 = new(3, 4);
        var res1 = -p1;
        Assert.AreEqual(res1, new(-3, -4));
    }
}

