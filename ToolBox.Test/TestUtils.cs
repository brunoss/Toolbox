using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using ToolBox.Utils;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestUtils
    {
        [Test]
        public void TestReverse()
        {
            var strings = new[] {"", "a", "ab", "abc", "abcd", "abcdefg"};
            foreach (string s in strings)
            {
                var aux = new string(s.ToCharArray().Reverse().ToArray());
                Assert.AreEqual(aux, s.Reverse());
            }
        }

        [Test]
        public void TestNextWeekDay()
        {
            Assert.AreEqual(DateTime.Today, DateTime.Today.NextWeekDay(DateTime.Today.DayOfWeek, true));
            Assert.AreEqual(DateTime.Today.AddDays(7), DateTime.Today.NextWeekDay(DateTime.Today.DayOfWeek));
        }

        [Test]
        public void TestAge()
        {
            var elapsed = DateTime.Today - DateTime.Today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(0, elapsed.Months());

            elapsed = DateTime.Today.AddDays(35) - DateTime.Today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(1, elapsed.Months());

            elapsed = DateTime.Today.AddDays(69) - DateTime.Today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(2, elapsed.Months());

            elapsed = DateTime.Today.AddDays(369) - DateTime.Today;
            Assert.AreEqual(1, elapsed.Years());
            Assert.AreEqual(0, elapsed.Months());
        }
    }
}
