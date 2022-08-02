using PetiteParser.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc {

    /// <summary>A collection of extension methods.</summary>
    static public class Extensions {

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
        /// <returns>
        /// The result of the last handle which was called.
        /// If you need to aggregate use an aggregator, do it within the handle, or use something else.
        /// </returns>
        static public T2 Foreach<T1, T2>(this IEnumerable<T1> values, Func<T1, T2> handle) {
            T2 result = default;
            foreach (T1 value in values) result = handle(value);
            return result;
        }

        /// <summary>This filters out any null values from the given collection.</summary>
        /// <typeparam name="T">The type of values to check.</typeparam>
        /// <param name="values">The collection to check for nulls within.</param>
        /// <returns>The collection without null values.</returns>
        static public IEnumerable<T> NotNull<T>(this IEnumerable<T> values) =>
            values.Where(value => value is not null);
    }
}
