using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Compatibility;
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
                var aux = new string(Enumerable.Reverse(s.ToCharArray()).ToArray());
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
            Assert.AreEqual(0, elapsed.Age().Years);
            Assert.AreEqual(0, elapsed.Age().Months);

            elapsed = _today.AddDays(35) - _today;
            Assert.AreEqual(0, elapsed.Age().Years);
            Assert.AreEqual(1, elapsed.Age().Months);

            elapsed = _today.AddDays(69) - _today;
            Assert.AreEqual(0, elapsed.Age().Years);
            Assert.AreEqual(2, elapsed.Age().Months);

            elapsed = _today.AddDays(369) - _today;
            Assert.AreEqual(1, elapsed.Age().Years);
            Assert.AreEqual(0, elapsed.Age().Months);
        }
    }
}
