using System.Linq;
using System.Numerics;
using NUnit.Framework;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestPoteincialPrimes
    {
        [Test]
        public void TestPotencialPrimes()
        {
            var potentialPrimes = new BigInteger[] {2, 3, 5, 7, 9, 11, 13, 17, 19, 21, 23, 27, 29, 31, 33, 37};
            Assert.AreEqual(potentialPrimes, Sequence.PotentialPrimes().Take(potentialPrimes.Length));
        }

        [Test]
        public void TestPrimes()
        {
            var primes = Sequence.Primes().Take(1000000).ToList();
            Assert.AreEqual(int.MaxValue, primes.Count);
        }
    }
}
