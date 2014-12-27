using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medallion.Parse
{
    internal static class Throw
    {
        public static void If(bool condition, string paramName)
        {
            if (condition)
            {
                throw new ArgumentException(paramName);
            }
        }

        public static void IfNull<T>(T value, string paramName)
        {
            if (value == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void IfNullOrEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException(value, paramName);
            }
        }
    }

    internal static class Helpers
    {
        public static T As<T>(this T @this)
        {
            return @this;
        }
    }

    internal static class Empty<T>
    {
        private static T[] array;
        public static T[] Array { get { return array ?? (array = new T[0]); } }

        private static Set<T> set;
        public static IReadOnlySet<T> Set { get { return set ?? (set = new Set<T>()); } }
    }

    internal static class Hash
    {
        public static int Combine(int a, int b)
        {
            return unchecked((3 * a) + b);
        }
    }

    internal static class CollectionHelpers
    {
        public static bool AddRange<T>(this ISet<T> @this, IEnumerable<T> elements)
        {
            Throw.IfNull(@this, "this");
            Throw.IfNull(elements, "elements");

            var changed = false;
            foreach (var element in elements)
            {
                changed |= @this.Add(element);
            }

            return changed;
        } 

        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            Throw.IfNull(@this, "this");
            Throw.IfNull(action, "action");

            foreach (var item in @this)
            {
                action(item);
            }
        }

        public static Set<T> ToSet<T>(this IEnumerable<T> @this, IEqualityComparer<T> comparer = null)
        {
            Throw.IfNull(@this, "this");

            return new Set<T>(@this, comparer ?? EqualityComparer<T>.Default);
        }

        public static IEnumerable<T> Append<T>(this IEnumerable<T> @this, T value)
        {
            foreach (var item in @this)
            {
                yield return item;
            }
            yield return value;
        }

        public static string ToDelimitedString<T>(this IEnumerable<T> @this, string separator = ", ")
        {
            return string.Join(separator, @this);
        }

        public static bool StartsWith<T>(this IEnumerable<T> @this, IEnumerable<T> prefix, IEqualityComparer<T> comparer = null)
        {
            Throw.IfNull(@this, "this");
            Throw.IfNull(prefix, "prefix");

            var comparerToUse = comparer ?? EqualityComparer<T>.Default;
            using (var thisEnumerator = @this.GetEnumerator())
            using (var prefixEnumerator = prefix.GetEnumerator())
            {
                while (prefixEnumerator.MoveNext())
                {
                    if (!thisEnumerator.MoveNext()
                        || !comparerToUse.Equals(thisEnumerator.Current, prefixEnumerator.Current))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public static T Last<T>(this IReadOnlyList<T> @this)
        {
            Throw.IfNull(@this, "this");
            Throw.If(@this.Count == 0, "this: list contained no elements");

            return @this[@this.Count - 1];
        }
    }

    internal sealed class SequenceComparer<T> : IEqualityComparer<IEnumerable<T>>
    {
        private static IEqualityComparer<IEnumerable<T>> @default;
        public static IEqualityComparer<IEnumerable<T>> Default
        {
            get { return @default ?? (@default = new SequenceComparer<T>(EqualityComparer<T>.Default)); }
        }

        private readonly IEqualityComparer<T> elementComparer;
        public SequenceComparer(IEqualityComparer<T> elementComparer)
        {
            Throw.IfNull(elementComparer, "elementComparer");

            this.elementComparer = elementComparer;
        }

        public bool Equals(IEnumerable<T> x, IEnumerable<T> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }

            return x.SequenceEqual(y, this.elementComparer);
        }

        public int GetHashCode(IEnumerable<T> sequence)
        {
            return sequence == null ? -1 : sequence.Select(this.elementComparer.GetHashCode).Aggregate(Hash.Combine); 
        }
    }
}
