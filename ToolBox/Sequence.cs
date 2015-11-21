using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;

namespace ToolBox
{
    public static class Sequence
    {
        private static readonly ISet<BigInteger> primes;

        static Sequence()
        {
            var formatter = new BinaryFormatter();
            var fileStream = new GZipStream(File.Open("primes.txt", FileMode.Open), CompressionMode.Decompress);
            var aux = (SortedSet<long>)formatter.Deserialize(fileStream);
            primes = new SortedSet<BigInteger>(aux.Select(l => new BigInteger(l)));
        }

        public static IEnumerable<int> BobFloydNonRepeatingSequence(int min, int max, int count, int? seed = null)
        {
            Random random;
            if (seed != null)
            {
                random = new Random(seed.Value);
            }
            else
            {
                random = new Random();
            }
            long length = max - min + 1;
            INonRepeatingList values = NonRepeatingListFactory.GetNonRepeatingList(min, max, count);
            for (int i = (int)(length - count); i < length; ++i)
            {
                if (!values.Add(random.Next(min, i+min+1)))
                {
                    values.Add(i+min);
                }
            }
            return values;
        }

        public static IEnumerable<BigInteger> PotentialPrimes()
        {
            yield return 2;
            yield return 3;
            BigInteger prime = 5;
            for (int i = 1; ; ++i)
            {
                yield return prime;
                if (i%5 == 0)
                {
                    prime += 4;
                    i = 1;
                }
                else
                {
                    prime += 2;
                }
            }
        }

        //Only compares with known primes
        private static bool IsPrime(BigInteger p)
        {
            var sqrt = p.Sqrt();
            foreach (var prime in primes)
            {
                if (prime < sqrt)
                {
                    if (p % prime == 0)
                    {
                        return false ;
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public static IEnumerable<BigInteger> Primes()
        {
            foreach (var p in primes)
            {
                yield return p;
            }
            foreach (var potentialPrime in PotentialPrimes().Where(p => p > primes.Last()))
            {
                if (IsPrime(potentialPrime))
                {
                    primes.Add(potentialPrime);
                    yield return potentialPrime;
                }
            }
        }

        public static IEnumerable<int> NonRepeatingRandomSequence(int min, int max, int count, int? seed = null)
        {
            Random random;
            if (seed != null)
            {
                random = new Random(seed.Value);
            }
            else
            {
                random = new Random();
            }
            long length = max - min + 1;
            INonRepeatingList values = NonRepeatingListFactory.GetNonRepeatingList(min, max, count);
            for (int i = (int)(length - count); i < length; ++i)
            {
                if (!values.Add(random.Next(min, i+min+1)))
                {
                    values.Add(i + min);
                }
            }
            values.Shuffle();
            return values;
        }
    }
}
