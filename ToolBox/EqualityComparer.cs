using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ToolBox
{
    public class EqualityComparer<T> : IEqualityComparer<T>, IComparer<T> where T:class
    {
        private readonly Func<T, IList<IComparable>> _selector;
        private readonly IComparer<T> _comparer; 
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
            _selector = elem =>
            {
                return props.Select(p => p(elem)).ToArray();
            };
        }

        public EqualityComparer(IComparer<T> comparer)
        {
            _comparer = comparer;
        } 

        public int Compare(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }
            if (x == null || y == null) {
                throw new ArgumentNullException();
            }
            if (_comparer != null)
            {
                return _comparer.Compare(x, y);
            }
            var xProps = _selector(x);
            var yProps = _selector(y);
            return xProps.Select((t, i) => t.CompareTo(yProps[i])).FirstOrDefault(comp => comp != 0);
        }

        public bool Equals(T x, T y)
        {
            return Compare(x, y) == 0;
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
