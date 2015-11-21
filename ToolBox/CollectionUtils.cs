using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ToolBox
{
    public static class CollectionUtils
    {
        public static bool ContainsOneOf<T>(this IEnumerable<T> col, params T[] values)
        {
            foreach (T t in col)
            {
                if (values.Contains(t))
                    return true;
            }
            return false;
        }

        public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                col.Add(value);
            }
        }

        public static Tuple<int, int> BinarySearch<T>(this IList<T> list, T elem) where T : IComparable
        {
            return BinarySearch(list, elem, 0, list.Count - 1);
        }

        public static Tuple<int, int> BinarySearch<T>(this IList<T> list, T elem, int left, int right) where T : IComparable
        {
            int idxFirst = -1;
            int auxRight = right;
            int auxLeft = -1;
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (list[mid].CompareTo(elem) > 0)
                {
                    right = mid - 1;
                }
                else if (list[mid].CompareTo(elem) < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    if (list[right].CompareTo(elem) > 0)
                    {
                        auxRight = right + 1;
                    }
                    right = mid - 1;
                    idxFirst = mid;
                    auxLeft = auxLeft < 0 ? mid : auxLeft;
                }
            }
            int idxLast = BinarySearchLast(list, elem, auxLeft, auxRight);
            return Tuple.Create(idxFirst, idxLast);
        }

        private static int BinarySearchLast<T>(IList<T> list, T elem, int left, int right) where T : IComparable
        {
            int idxLast = -1;
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (list[mid].CompareTo(elem) > 0)
                {
                    right = mid - 1;
                }
                else if (list[mid].CompareTo(elem) < 0)
                {
                    left = mid + 1;
                }
                else
                {
                    left = mid + 1;
                    idxLast = mid;
                }
            }
            return idxLast;
        }

        public class RetrieveElement<T> : IEnumerable<T>
        {
            public bool Exists { get; set; }
            public T Element { get; set; }
            public IEnumerator<T> Remaining { get; set; }
            public IEnumerator<T> GetEnumerator()
            {
                using (Remaining)
                {
                    while (Remaining.MoveNext())
                    {
                        yield return Remaining.Current;
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public static RetrieveElement<T> FirstAndRemaining<T>(this IEnumerable<T> values)
        {
            using (var enumerator = values.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    return new RetrieveElement<T>() { Exists = false, Remaining = enumerator };
                }
                return new RetrieveElement<T>() { Exists = true, Element = enumerator.Current, Remaining = enumerator };
            }
        }

        public static IEnumerable<IEnumerable<T>> OrderedGroup<T>(this IEnumerable<T> values, Func<T, IComparable> idSelector)
        {
            using (var it = values.GetEnumerator())
            {
                if (it.MoveNext())
                {
                    IComparable field = idSelector(it.Current);
                    var list = new List<T>();
                    while (it.MoveNext())
                    {
                        if (idSelector(it.Current).CompareTo(field) == 0)
                        {
                            list.Add(it.Current);
                        }
                        else
                        {
                            yield return list;
                            list.Clear();
                            list.Add(it.Current);
                            field = idSelector(it.Current);
                        }
                    }
                    yield return list;
                }
            }
        }

        public static RetrieveElement<T> AllAndLast<T>(this IEnumerable<T> values)
        {
            var list = values as IList<T> ?? values.ToList();
            if (list.Count > 0)
            {
                return new RetrieveElement<T>
                {
                    Exists = true,
                    Remaining = list.Take(list.Count - 1).GetEnumerator(),
                    Element = list[list.Count - 1]
                };

            }
            return new RetrieveElement<T>
            {
                Exists = false,
                Remaining = list.GetEnumerator()
            };
        }

        static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(()=>new Random());
        public static IList<T> Shuffle<T>(this IEnumerable<T> col)
        {
            var list = col as IList<T> ?? col.ToList();
            Random random = Random.Value;
            for (int i = 0; i < list.Count; ++i)
            {
                //get a chance of staying in same place
                list.Swap(i, random.Next(i, list.Count));
            }
            return list;
        }

        public static void Swap<T>(this IEnumerable<T> col, int idxSrc, int idxDest)
        {
            var list = col as IList<T> ?? col.ToList();
            T aux = list[idxSrc];
            list[idxSrc] = list[idxDest];
            list[idxDest] = aux;
        }

        public class Statistics
        {
            public decimal Sum { get; set; }
            public decimal Min { get; set; }
            public decimal Max { get; set; }
            public decimal Mean { get; set; }
            public int Count { get; set; }
            public decimal Variance { get; set; }
            public double StandardDeviaton { get; set; }
        }

        public static T GetStatistics<T>(this IEnumerable<decimal> values)
            where T:Statistics
        {
            decimal min = new decimal(double.PositiveInfinity);
            decimal max = new decimal(double.NegativeInfinity);
            int count = 0;
            decimal acc = 0;
            foreach (var value in values)
            {
                min = Math.Min(min, value);
                max = Math.Max(max, value);
                acc += value;
                ++count;
            }
            decimal mean = acc/count;
            decimal variance = 0;
            Statistics stats = new Statistics()
            {
                Count = count,
                Max = max,
                Min = min,
                Sum = acc,
                Mean = mean,
            };
            foreach (var value in values)
            {
                variance = (value - mean)*(value - mean) + variance;
            }
            stats.Variance = variance;
            stats.StandardDeviaton = Math.Sqrt((double) variance);
            return (T)stats;
        }
    }

}
