using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ToolBox.Test
{
    [TestFixture]
    public class TestNonRepeatingGenerator
    {
        [Test]
        public void TestLastCountElementsAreNotInIndex0()
        {
            for (int i = 0; i < 1000000; ++i)
            {
                var list = Sequence.BobFloydNonRepeatingSequence(0, 15, 4).ToList();
                Assert.IsFalse(new[] {13, 14, 15 }.Contains(list[0]));
                Assert.AreEqual(4, list.Count);
            }
        }

        [Test]
        public void TestBobFloydBecomesLinear()
        {
            var list = Sequence.BobFloydNonRepeatingSequence(0, 15, 16).ToList();
            for (int i = 0; i < list.Count; ++i)
            {
                Assert.AreEqual(i, list[i]);
            }
        }

        [Test]
        public void TestBobFloydRandomness()
        {
            TestRandomness(() => Sequence.BobFloydNonRepeatingSequence(8, 15, 4).ToList(), 8, 4);
        }


        [Test]
        public void TestBobFloydWithShuffleRandomness()
        {
            TestRandomness(() => Sequence.NonRepeatingRandomSequence(0, 15, 4).ToList(), 16, 4);
        }

        [Test]
        public void TestShuffleRandomness()
        {
            TestRandomness(() => Enumerable.Range(0, 16).ToList().Shuffle().Take(4).ToList(), 16, 4);    
        }
        

        public void TestRandomness(Func<List<int>> getRandomList, int rangeLength, int count)
        {
            var combinations = rangeLength.Factorial(rangeLength - count + 1);
            var iterations = combinations * 100;
            var ocurrences = new ConcurrentDictionary<long, int>(Environment.ProcessorCount, (int)combinations);

            var partitioner = Partitioner.Create(0, (long)iterations);
            Parallel.ForEach(partitioner, new ParallelOptions() {MaxDegreeOfParallelism = Environment.ProcessorCount},
                range =>
                {
                    //hopefully having a private dictionary will help concurrency
                    var privateOcurrences = new Dictionary<long, int>();
                    for (long i = range.Item1; i < range.Item2; ++i)
                    {
                        var list = getRandomList();
                        //this will only work when numbers are between 0 and 88
                        long acc = list.Aggregate<int, long>(0, (current, value) => (value + 11) + current*100);
                        privateOcurrences.AddOrUpdate(acc, 1, v => v + 1);
                    }
                    foreach (var privateOcurrence in privateOcurrences)
                    {
                        ocurrences.AddOrUpdate(privateOcurrence.Key, 
                            privateOcurrence.Value,
                            (k, v) => v + privateOcurrence.Value);
                    }
                });

            var averageOcurrences = iterations / (combinations * 1.0m);
            var currentAverage = ocurrences.Values.Average();
            Debug.WriteLine("The average ocurrences of this implementation is {0} comparing to {1} in the 'perfect' scenario", currentAverage, averageOcurrences);
            Assert.Less(currentAverage, averageOcurrences * 1.05m);
            Assert.Greater(currentAverage, averageOcurrences * 0.95m);
        }
    }
}
