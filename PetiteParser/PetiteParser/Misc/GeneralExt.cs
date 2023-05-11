using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc;

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
    static public T2? Foreach<T1, T2>(this IEnumerable<T1> values, Func<T1, T2?> handle, Func<T2?, T2?, T2?>? combiner = null) =>
        values.Select(handle).Aggregate(default, combiner ?? ((a, b) => b));

    /// <summary>This performs the given action on each element of this collection.</summary>
    /// <typeparam name="T1">The type of values in the collection.</typeparam>
    /// <param name="values">The collection of values to apply the action to.</param>
    /// <param name="handle">The action to perform on each of the elements.</param>
    /// <returns>True if any handle returned true, false if all returned false or empty.</returns>
    static public bool ForeachAny<T1>(this IEnumerable<T1> values, Func<T1, bool> handle) =>
        values.Foreach(handle, (a, b) => a || b);

    /// <summary>This filters out any null values from the given collection.</summary>
    /// <typeparam name="T">The type of values to check.</typeparam>
    /// <param name="values">The collection to check for nulls within.</param>
    /// <returns>The collection without null values.</returns>
    static public IEnumerable<T> NotNull<T>(this IEnumerable<T> values) =>
        values.Where(value => value is not null);

    /// <summary>This determines if the given values are in sorted order from lowest to highest.</summary>
    /// <typeparam name="T">The type of the values to check.</typeparam>
    /// <param name="values">The collection to check the sort order of.</param>
    /// <returns>True if sorted, false otherwise.</returns>
    static public bool IsSorted<T>(this IEnumerable<T> values) where T : IComparable<T> {
        T? prev = default;
        bool first = true;
        foreach (T value in values) {
            if (first) first = false;
            else if (value.CompareTo(prev) < 0) return false;
            prev = value;
        }
        return true;
    }

    /// <summary>
    /// This gets the only value from the given values.
    /// If there are zero or more than one value then the "else" value is returned.
    /// </summary>
    /// <typeparam name="T">the type of the values to check.</typeparam>
    /// <param name="values">The values to get the only value from.</param>
    /// <param name="elseValue">The value to return if there were zero or more than one value.</param>
    /// <returns>The only value or the given "else value.</returns>
    static public T? OnlyOne<T>(this IEnumerable<T> values, T? elseValue = default) {
        T? only = elseValue;
        bool hasOne = false;
        foreach (T value in values) {
            if (hasOne) return elseValue;
            hasOne = true;
            only = value;
        }
        return only;
    }

    /// <summary>This gets a copy of the range in a list.</summary>
    /// <typeparam name="T">The type of the values in the list.</typeparam>
    /// <param name="list">The list to get a copy of a range from.</param>
    /// <param name="range">The range of the list to copy.</param>
    /// <returns>The copy of the range in the list.</returns>    
    public static List<T> GetRange<T>(this List<T> list, Range range) {
        (int start, int length) = range.GetOffsetAndLength(list.Count);
        return list.GetRange(start, length);
    }

    /// <summary>This removes of the range from this list.</summary>
    /// <typeparam name="T">The type of the values in the list.</typeparam>
    /// <param name="list">The list to remove a range from.</param>
    /// <param name="range">The range of the list to remove.</param>
    public static void RemoveRange<T>(this List<T> list, Range range) {
        (int start, int length) = range.GetOffsetAndLength(list.Count);
        list.RemoveRange(start, length);
    }
}
