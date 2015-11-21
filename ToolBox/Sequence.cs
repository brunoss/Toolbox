using System;
using System.Collections.Generic;

namespace ToolBox
{
    public static class Sequence
    {
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
            var values = NonRepeatingListFactory.GetNonRepeatingList(min, max, count);
            for (int i = (int)(length - count); i < length; ++i)
            {
                if (!values.Add(random.Next(min, i+min+1)))
                {
                    values.Add(i+min);
                }
            }
            return values;
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
