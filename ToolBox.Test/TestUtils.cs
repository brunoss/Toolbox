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

        private DateTime Today = DateTime.Today;
        [Test]
        public void TestNextWeekDay()
        {
            
            Assert.AreEqual(Today, DateTime.Today.NextWeekDay(Today.DayOfWeek, true));
            Assert.AreEqual(Today.AddDays(7), DateTime.Today.NextWeekDay(Today.DayOfWeek));
        }

        [Test]
        public void TestAge()
        {
            var elapsed = Today - Today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(0, elapsed.Months());

            elapsed = Today.AddDays(35) - Today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(1, elapsed.Months());

            elapsed = Today.AddDays(69) - Today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(2, elapsed.Months());

            elapsed = Today.AddDays(369) - Today;
            Assert.AreEqual(1, elapsed.Years());
            Assert.AreEqual(0, elapsed.Months());
        }
    }
}
