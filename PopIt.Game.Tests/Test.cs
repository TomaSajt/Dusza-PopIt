using Xunit;

namespace PopIt.Game.Tests
{
    public class Test
    {
        [Fact]
        public void one_equals_one()
        {
            Assert.True(int.Parse("1")==1, "Int.Parse don't be workin");
        }
    }
}