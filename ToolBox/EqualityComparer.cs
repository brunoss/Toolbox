using System;
using System.Collections.Generic;
using System.Linq;

namespace ToolBox
{
    public class EqualityComparer<T> : IEqualityComparer<T> where T:class
    {
        private readonly Func<T, IList<IComparable>> _selector;
        public EqualityComparer(Func<T, IList<IComparable>> selector)
        {
            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
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
            if (ReferenceEquals(x, y))
            {
                return true;
            }
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
            return !xProps.Where((t, i) => t.CompareTo(yProps[i]) != 0).Any();
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
