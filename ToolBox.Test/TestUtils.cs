using System;
using System.Linq;
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

        private DateTime _today = DateTime.Today;
        [Test]
        public void TestNextWeekDay()
        {
            
            Assert.AreEqual(_today, DateTime.Today.NextWeekDay(_today.DayOfWeek, true));
            Assert.AreEqual(_today.AddDays(7), DateTime.Today.NextWeekDay(_today.DayOfWeek));
        }

        [Test]
        public void TestAge()
        {
            var elapsed = _today - _today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(0, elapsed.Months());

            elapsed = _today.AddDays(35) - _today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(1, elapsed.Months());

            elapsed = _today.AddDays(69) - _today;
            Assert.AreEqual(0, elapsed.Years());
            Assert.AreEqual(2, elapsed.Months());

            elapsed = _today.AddDays(369) - _today;
            Assert.AreEqual(1, elapsed.Years());
            Assert.AreEqual(0, elapsed.Months());
        }
    }
}
