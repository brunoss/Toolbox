﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace ToolBox
{
    public static class CollectionUtils
    {
        public static T ContainsOneOf<T>(this IEnumerable<T> col, params T[] values)
        {
            foreach (var t in col)
            {
                if (values.Contains(t))
                {
                    return t;
                }
            }
            return default(T);
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

        public static LinkedListNode<T> Find<T>(this LinkedList<T> list, Predicate<T> pred)
        {
            var node = list.First;
            while (node != null)
            {
                if (pred(node.Value))
                {
                    return node;
                }
                node = node.Next;
            }
            return null;
        } 

        public static IEnumerable<IEnumerable<T>> OrderedGroup<T>(this IEnumerable<T> values, Func<T, IComparable> idSelector)
        {
            using (var it = values.GetEnumerator())
            {
                if (it.MoveNext())
                {
                    var field = idSelector(it.Current);
                    var list = new List<T>();
                    list.Add(it.Current);
                    while (true)
                    {
                        if (!it.MoveNext())
                        {
                            yield return list;
                            yield break;
                        }
                        if (idSelector(it.Current).CompareTo(field) == 0)
                        {
                            list.Add(it.Current);
                        }
                        else
                        {
                            yield return list;
                            list = new List<T>();
                            list.Add(it.Current);
                            field = idSelector(it.Current);
                        }
                    }
                }
            }
        }

        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(()=>new Random());
        public static IList<T> Shuffle<T>(this IEnumerable<T> col)
        {
            var list = col as IList<T> ?? col.ToList();
            var random = Random.Value;
            for (int i = 0; i < list.Count; ++i)
            {
                //get a chance of staying in same place
                list.Swap(i, random.Next(i, list.Count));
            }
            return list;
        }

        public static void Swap<T>(this IList<T> col, int idxSrc, int idxDest)
        {
            var aux = col[idxSrc];
            col[idxSrc] = col[idxDest];
            col[idxDest] = aux;
        }

        public static void Reverse<T>(this IList<T> col)
        {
            for (int i = 0; i < col.Count / 2; ++i)
            {
                col.Swap(i, col.Count -i - 1);
            }
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

        public static Statistics GetStatistics(this ICollection<decimal> values)
        {
            var min = new decimal(double.PositiveInfinity);
            var max = new decimal(double.NegativeInfinity);
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
            var stats = new Statistics
            {
                Count = count,
                Max = max,
                Min = min,
                Sum = acc,
                Mean = mean
            };
            foreach (var value in values)
            {
                variance = (value - mean)*(value - mean) + variance;
            }
            stats.Variance = variance;
            stats.StandardDeviaton = Math.Sqrt((double) variance);
            return stats;
        }
    }

}
