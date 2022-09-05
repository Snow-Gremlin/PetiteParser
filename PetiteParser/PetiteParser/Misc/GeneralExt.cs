using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc {

    /// <summary>A collection of extension methods.</summary>
    static public class GeneralExt {

        /// <summary>This returns all values which do not match the given predicate.</summary>
        /// <typeparam name="T">Te type of value in the collection.</typeparam>
        /// <param name="values">The collection of values to filter.</param>
        /// <param name="predicate">The predicate to filter with.</param>
        static public IEnumerable<T> WhereNot<T>(this IEnumerable<T> values, Func<T, bool> predicate) =>
            values.Where(v => !predicate(v));

        /// <summary>This performed the given action on each element of this collection.</summary>
        /// <remarks>The given handle be will called on null values in the collection.</remarks>
        /// <typeparam name="T">The type of values in the collection.</typeparam>
        /// <param name="values">The collection of values to apply the action to.</param>
        /// <param name="handle">The action to perform on each of the elements.</param>
        static public void Foreach<T>(this IEnumerable<T> values, Action<T> handle) {
            foreach (T value in values) handle(value);
        }

        /// <summary>This performed the given action on each element of this collection.</summary>
        /// <remarks>The given handle will be called on null values in the collection.</remarks>
        /// <typeparam name="T1">The type of values in the collection.</typeparam>
        /// <typeparam name="T2">The return type of the handle.</typeparam>
        /// <param name="values">The collection of values to apply the action to.</param>
        /// <param name="handle">The action to perform on each of the elements.</param>
        /// <param name="combiner">An optional function to combine the results, if null then the last result is returned.</param>
        /// <returns>The result of the combiner if not null, or the result of the last handle which was called.</returns>
        static public T2 Foreach<T1, T2>(this IEnumerable<T1> values, Func<T1, T2> handle, Func<T2, T2, T2> combiner = null) =>
            values.Select(handle).Aggregate(default, combiner ?? ((a, b) => b));

        /// <summary>This performs the given action on each element of this collection.</summary>
        /// <typeparam name="T1">The type of values in the collection.</typeparam>
        /// <param name="values">The collection of values to apply the action to.</param>
        /// <param name="handle">The action to perform on each of the elements.</param>
        /// <returns>True if any handle returned true, false if all returned false.</returns>
        static public bool ForeachAny<T1>(this IEnumerable<T1> values, Func<T1, bool> handle) =>
            values.Foreach(handle, (a, b) => a || b);

        /// <summary>This filters out any null values from the given collection.</summary>
        /// <typeparam name="T">The type of values to check.</typeparam>
        /// <param name="values">The collection to check for nulls within.</param>
        /// <returns>The collection without null values.</returns>
        static public IEnumerable<T> NotNull<T>(this IEnumerable<T> values) =>
            values.Where(value => value is not null);

        /// <summary>This pairs each value with it's previous value.</summary>
        /// <remarks>A list like [A, B, C, D] will output [(A, B), (B, C), (C, D)].</remarks>
        /// <typeparam name="T">The type of values to pair.</typeparam>
        /// <param name="values">The collection to pair with previous value.</param>
        /// <returns>The paired values from the given values.</returns>
        static public IEnumerable<(T, T)> PairWithPrevious<T>(this IEnumerable<T> values) {
            T prev = default;
            bool first = true;
            foreach (T value in values) {
                if (first) {
                    prev = value;
                    first = false;
                    continue;
                }
                yield return (prev, value);
                prev = value;
            }
        }

        /// <summary>This determines if the given values are in sorted order from lowest to highest.</summary>
        /// <typeparam name="T">The type of the values to check.</typeparam>
        /// <param name="values">The collection to check the sort order of.</param>
        /// <returns>True if sorted, false otherwise.</returns>
        static public bool IsSorted<T>(this IEnumerable<T> values) where T : IComparable<T> =>
            values.PairWithPrevious().All(p => p.Item1.CompareTo(p.Item2) <= 0);
    }
}
