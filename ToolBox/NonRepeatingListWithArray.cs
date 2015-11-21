using System.Collections;
using System.Collections.Generic;

namespace ToolBox
{
    public class NonRepeatingListFactory
    {
        public static INonRepeatingList GetNonRepeatingList(int min, int max, int count)
        {
            long length = max - min + 1;
            if (length / 8 > count * 2 * sizeof(int))
            {
                //if the amount of bytes occupied by the array is greater then the dictionary
                return new NonRepeatingListWithSet();
            }
            return new NonRepeatingListWithArray(min, max);
        }
    }

    public interface INonRepeatingList : IEnumerable<int>
    {
        bool Add(int value);
    }

    internal class NonRepeatingListWithArray : List<int>, INonRepeatingList
    {
        private readonly BitArray inList;
        private readonly int min;

        public NonRepeatingListWithArray(int min, int max)
        {
            this.inList = new BitArray(max-min+1);
            this.min = min;
        }

        bool INonRepeatingList.Add(int value)
        {
            if (!this.inList[value - this.min])
            {
                this.Add(value);
                this.inList[value - this.min] = true;
                return true;
            }
            return false;
        }
    }

    internal class NonRepeatingListWithSet : List<int>, INonRepeatingList
    {
        private readonly HashSet<int> mapValue = new HashSet<int>();
        private int currentIndex = 0;

        bool INonRepeatingList.Add(int value)
        {
            if (mapValue.Add(value))
            {
                base.Add(value);
                return true;
            }
            return false;
        }
    }
}