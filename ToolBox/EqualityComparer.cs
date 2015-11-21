using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolBox
{
    public class EqualityComparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, IList<IComparable>> _selector;
        public EqualityComparer(Func<T, IList<IComparable>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }
            _selector = selector;
        }

        public EqualityComparer(params Func<T, IComparable>[] props)
        {
            _selector = (elem) =>
            {
                return props.Select(p => p(elem)).ToArray();
            };
        } 

        public bool Equals(T x, T y)
        {
            if (x == null)
            {
                return false;
            }
            if (y == null)
            {
                return false;
            }
            var xProps = _selector(x);
            var yProps = _selector(y);
            for (int i = 0; i < xProps.Count; ++i)
            {
                if (xProps[i].CompareTo(yProps[i]) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        public int GetHashCode(T obj)
        {
            int hash = 0;
            foreach (var prop in _selector(obj))
            {
                hash = hash ^ prop.GetHashCode();
            }
            return hash;
        }
    }
}
