using PetiteParser.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc {

    /// <summary>A collection of extesion methods.</summary>
    static public class Extensions {

        /// <summary>This performd the given action on each element of this collection.</summary>
        /// <remarks>The given handle will called on null values in the collection.</remarks>
        /// <typeparam name="T">The type of values in the collection.</typeparam>
        /// <param name="values">The values collection of values to apply the action to.</param>
        /// <param name="handle">The action to perform on each of the elements.</param>
        static public void Foreach<T>(this IEnumerable<T> values, Action<T> handle) {
            foreach (T value in values) handle(value);
        }

        /// <summary>This performd the given action on each element of this collection.</summary>
        /// <remarks>
        /// The given handle will called on null values in the collection.
        /// The returned values from the function calls will be discarded.
        /// </remarks>
        /// <typeparam name="T1">The type of values in the collection.</typeparam>
        /// <typeparam name="T2">The return type of the handle.</typeparam>
        /// <param name="values">The values collection of values to apply the action to.</param>
        /// <param name="handle">The action to perform on each of the elements.</param>
        static public void Foreach<T1, T2>(this IEnumerable<T1> values, Func<T1, T2> handle) {
            foreach (T1 value in values) handle(value);
        }

        /// <summary>This filters out any null values from the given collection.</summary>
        /// <typeparam name="T">The type of values to check.</typeparam>
        /// <param name="values">The collection to check for nulls within.</param>
        /// <returns>The collection without null values.</returns>
        static public IEnumerable<T> NotNull<T>(this IEnumerable<T> values) =>
            values.Where(value => value is not null);

        /// <summary>This findd the item in this collection by the given name.</summary>
        /// <typeparam name="T">The type of item to search for the names in.</typeparam>
        /// <param name="items">The collection of items to search within.</param>
        /// <param name="name">The name of the item to try to find.</param>
        /// <returns>The first item found with the given name or null if not found.</returns>
        static public T FindItemByName<T>(this IEnumerable<T> items, string name) where T: Item =>
            items.NotNull().FirstOrDefault(item => item.Name == name);

        /// <summary>This gets all the names from the given items.</summary>
        /// <typeparam name="T">The type of items to get the names for.</typeparam>
        /// <param name="items">The items to get the names from.</param>
        /// <returns>The </returns>
        static public IEnumerable<string> ToNames<T>(this IEnumerable<T> items) where T : Item =>
            items.NotNull().Select(item => item.Name);

        /// <summary>This gets a string for all the objects' strings joined by the given seperator.</summary>
        /// <typeparam name="T">The value type to get the string from.</typeparam>
        /// <param name="values">The values to get the strings from.</param>
        /// <param name="seperator">The seperator to put between the strings.</param>
        /// <returns>The join of the strings for the given values.</returns>
        static public string Join<T>(this IEnumerable<T> values, string seperator = null) =>
            string.Join(seperator, values);
    }
}
