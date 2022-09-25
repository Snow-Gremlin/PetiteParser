using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc;

/// <summary>A collection of extension methods for strings.</summary>
static public class StringExt {

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
    static public string Join<T>(this IEnumerable<T> values, string? separator = null) =>
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
        value.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

    /// <summary>This trims each string and returns it.</summary>
    /// <param name="values">The values to trim.</param>
    /// <returns>The trimmed strings.</returns>
    static public IEnumerable<string> Trim(this IEnumerable<string> values) =>
        values.Select(v => v.Trim());

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
