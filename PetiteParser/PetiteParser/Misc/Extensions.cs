using PetiteParser.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc {

    /// <summary>A collection of extension methods.</summary>
    static public class Extensions {

        /// <summary>This performed the given action on each element of this collection.</summary>
        /// <remarks>The given handle be will called on null values in the collection.</remarks>
        /// <typeparam name="T">The type of values in the collection.</typeparam>
        /// <param name="values">The values collection of values to apply the action to.</param>
        /// <param name="handle">The action to perform on each of the elements.</param>
        static public void Foreach<T>(this IEnumerable<T> values, Action<T> handle) {
            foreach (T value in values) handle(value);
        }

        /// <summary>This performed the given action on each element of this collection.</summary>
        /// <remarks>The given handle will be called on null values in the collection.</remarks>
        /// <typeparam name="T1">The type of values in the collection.</typeparam>
        /// <typeparam name="T2">The return type of the handle.</typeparam>
        /// <param name="values">The values collection of values to apply the action to.</param>
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

        /// <summary>This finds the item in this collection by the given name.</summary>
        /// <typeparam name="T">The type of item to search for the names in.</typeparam>
        /// <param name="items">The collection of items to search within.</param>
        /// <param name="name">The name of the item to try to find.</param>
        /// <returns>The first item found with the given name or null if not found.</returns>
        static public T FindItemByName<T>(this IEnumerable<T> items, string name) where T : Item =>
            items.NotNull().FirstOrDefault(item => item.Name == name);

        /// <summary>This gets all the names from the given items.</summary>
        /// <typeparam name="T">The type of items to get the names for.</typeparam>
        /// <param name="items">The items to get the names from.</param>
        /// <returns>The names from the given items.</returns>
        static public IEnumerable<string> ToNames<T>(this IEnumerable<T> items) where T : Item =>
            items.NotNull().Select(item => item.Name);

        /// <summary>Gets the every line, including the first line, in the given string.</summary>
        /// <param name="value">The string to indent each line.</param>
        /// <param name="indent">The indent to prepend to each line.</param>
        /// <returns>The indented strings.</returns>
        static public string IndentLines(this string value, string indent) =>
            value.SplitLines().Select(value => indent + value).JoinLines();

        /// <summary>This gets a string for all the objects' strings joined by the given separator.</summary>
        /// <typeparam name="T">The value type to get the strings from.</typeparam>
        /// <param name="values">The values to get the strings from.</param>
        /// <param name="separator">The separator to put between the strings.</param>
        /// <returns>The join of the strings for the given values.</returns>
        static public string Join<T>(this IEnumerable<T> values, string separator = null) =>
            string.Join(separator, values);

        /// <summary>This gets a string for all the objects' strings joined by the environment's new lines.</summary>
        /// <typeparam name="T">The value type to get the strings from.</typeparam>
        /// <param name="values">The values to get the string from.</param>
        /// <param name="indent">An optional indent to apply to all but the first line.</param>
        /// <returns>The join of the strings for the given values.</returns>
        static public string JoinLines<T>(this IEnumerable<T> values, string indent = "") =>
            values.Join(Environment.NewLine+indent);

        /// <summary>This splits this string on any kind of line separators.</summary>
        /// <param name="value">The string to split into lines.</param>
        /// <returns>The lines from the given value.</returns>
        static public string[] SplitLines(this string value) =>
            value.Split(new string[] { "\r\n", "\r", "\n"}, StringSplitOptions.None);

        /// <summary>This escapes the string's special characters.</summary>
        /// <param name="value">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        static public string Escape(this string value) =>
            Text.Escape(value);

        /// <summary>This escapes the strings' special characters.</summary>
        /// <param name="value">The strings to escape.</param>
        /// <returns>The escaped strings.</returns>
        static public IEnumerable<string> Escape(this IEnumerable<string> values) =>
            values.Select(Escape);

        /// <summary>This unescapes escaped characters in this string.</summary>
        /// <param name="values">The strings to unescape.</param>
        /// <returns>The unescaped string.</returns>
        static public string Unescape(this string value) =>
            Text.Unescape(value);

        /// <summary>This unescapes escaped characters in these strings.</summary>
        /// <param name="values">The strings to unescape.</param>
        /// <returns>The unescaped strings.</returns>
        static public IEnumerable<string> Unescape(this IEnumerable<string> values) =>
            values.Select(Unescape);
    }
}
