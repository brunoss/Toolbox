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
        private readonly BitArray _inList;
        private readonly int _min;

        public NonRepeatingListWithArray(int min, int max)
        {
            _inList = new BitArray(max-min+1);
            _min = min;
        }

        bool INonRepeatingList.Add(int value)
        {
            if (!_inList[value - _min])
            {
                Add(value);
                _inList[value - _min] = true;
                return true;
            }
            return false;
        }
    }

    internal class NonRepeatingListWithSet : List<int>, INonRepeatingList
    {
        private readonly HashSet<int> _mapValue = new HashSet<int>();

        bool INonRepeatingList.Add(int value)
        {
            if (_mapValue.Add(value))
            {
                base.Add(value);
                return true;
            }
            return false;
        }
    }
}