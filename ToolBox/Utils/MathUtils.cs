using System;
using System.Numerics;

namespace ToolBox
{
    public static class MathUtils
    {
        public static decimal Factorial(this int n)
        {
            return n.Factorial(2);
        }

        public static decimal Factorial(this int n, int min)
        {
            decimal acc = 1;
            while (n >= min)
            {
                acc *= n;
                --n;
            }
            return acc;
        }

        public static decimal GetCombinations(int elements, int length)
        {
            return (length.Factorial()/(elements.Factorial()*Factorial(length - elements)));
        }

        private static bool IsSqrt(BigInteger n, BigInteger root)
        {
            BigInteger lowerBound = root * root;
            BigInteger upperBound = (root + 1) * (root + 1);

            return (n >= lowerBound && n < upperBound);
        }

        public static BigInteger Sqrt(this BigInteger n)
        {
            if (n < ulong.MaxValue)
            {
                return (ulong)Math.Sqrt((ulong)n);
            }

            if (n > 0)
            {
                int bitLength = (int)Math.Ceiling(BigInteger.Log(n, 2));
                BigInteger root = BigInteger.One << (bitLength / 2);

                while (!IsSqrt(n, root))
                {
                    root += n / root;
                    root /= 2;
                }

                return root;
            }

            throw new ArithmeticException("NaN");
        }
    }
}
