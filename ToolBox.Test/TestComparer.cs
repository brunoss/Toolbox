using System;
using NUnit.Framework;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestComparer
    {
        [Test]
        public void Compare()
        {
            var comparer1 = new EqualityComparer<Tuple<int, int>>(a => new IComparable[]{a.Item1, a.Item2});
            var comparer2 = new EqualityComparer<Tuple<int, int>>(a => a.Item1, a => a.Item2);
            for (int i = 0; i < 10; i++)
            {
                Assert.True(comparer1.Equals(Tuple.Create(i, i), Tuple.Create(i, i)));
                Assert.True(comparer2.Equals(Tuple.Create(i, i), Tuple.Create(i, i)));
                Assert.False(comparer1.Equals(Tuple.Create(i, i), Tuple.Create(i + 1, i)));
                Assert.False(comparer2.Equals(Tuple.Create(i, i), Tuple.Create(i + 1, i)));
                Assert.False(comparer1.Equals(Tuple.Create(i, i), Tuple.Create(i, i + 1)));
                Assert.False(comparer2.Equals(Tuple.Create(i, i), Tuple.Create(i, i + 1)));
            }
        }
    }
}
