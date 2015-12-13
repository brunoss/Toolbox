using System;
using System.Collections.Generic;

namespace ToolBox
{
    public static class Sequence
    {
        public static IEnumerable<int> NonRepeatingRandomSequence(int min, int max, int count, int? seed = null)
        {
            var random = seed != null ? new Random(seed.Value) : new Random();
            var length = max - min + 1;
            var values = NonRepeatingListFactory.GetNonRepeatingList(min, max, count);
            for (int i = length - count; i < length; ++i)
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
