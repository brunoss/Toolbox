using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestComparer
    {
        [Test]
        public void Compare()
        {
            var comparer1 = new EqualityComparer<Tuple<string, int>>(a => new IComparable[]{a.Item1, a.Item2});
            var comparer2 = new EqualityComparer<Tuple<string, int>>(a => a.Item1, a => a.Item2);
        }
    }
}
