using NUnit.Framework;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestMath
    {
        [Test]
        public void TestFactorial()
        {
            long currentFactorial = 1; //1!
            for (int i = 1; i < 16; ++i)
            {
                currentFactorial *= i;
                Assert.AreEqual(currentFactorial, i.Factorial());
            }
        }

        [Test]
        public void TestMaxFactorial()
        {
            Assert.AreEqual(13*12*11, 13.Factorial(11));
        }

        [Test]
        public void TestCombinations()
        {
            Assert.AreEqual(1820, MathUtils.GetCombinations(4, 16));
        }
    }
}
