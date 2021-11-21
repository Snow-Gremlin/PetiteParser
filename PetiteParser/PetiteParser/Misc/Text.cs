using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PetiteParser.Misc {

    /// <summary>Tools for processing text.</summary>
    static public class Text {

        /// <summary>The converts any remaining character not yet escaped.</summary>
        /// <remarks>This is part of the Escape function.</remarks>
        /// <param name="c">The character to escape.</param>
        /// <returns>The escaped string.</returns>
        static private string escapeRest(char c) {
            Rune r = new(c);
            return r.IsAscii ? c.ToString() : string.Format("\\u{0:X4}", r.Value);
        }

        /// <summary>This converts a character into an escaped string for printing.</summary>
        /// <remarks>This will not escape all control characters.</remarks>
        /// <param name="c">The character to escape.</param>
        /// <returns>The escaped string.</returns>
        static public string Escape(char c) =>
            c switch {
                '\\' => "\\\\",
                '\'' => "\\\'",
                '\"' => "\\\"",
                '\0' => "\\0",
                '\b' => "\\b",
                '\f' => "\\f",
                '\n' => "\\n",
                '\r' => "\\r",
                '\t' => "\\t",
                '\v' => "\\v",
                _    => escapeRest(c),
            };

        /// <summary>This converts unescaped characters into escaped string sfor printing.</summary>
        /// <remarks>This will not escape all control characters.</remarks>
        /// <param name="values">The characters to escape.</param>
        /// <returns>The escaped strings.</returns>
        static public IEnumerable<string> Escape(IEnumerable<char> values) => values.Select(Escape);

        /// <summary>This converts an unescaped string into an escaped string for printing.</summary>
        /// <remarks>This will not escape all control characters.</remarks>
        /// <param name="value">The string to escape.</param>
        /// <returns>The escaped string.</returns>
        static public string Escape(string value) => Escape(value).Join();

        /// <summary>This is a helper to Unescape to unescape a hex encoded sequence.</summary>
        /// <param name="value">The string being unescaped.</param>
        /// <param name="index">The index of the escaped character.</param>
        /// <param name="size">The number of characters to read.</param>
        /// <returns>The string which was escaped.</returns>
        static private string unescapeHex(string value, int index, int size) {
            string hex = value[(index+1)..(index+1+size)];
            Rune charCode = new(int.Parse(hex, NumberStyles.HexNumber));
            return charCode.ToString();
        }

        /// <summary>This is a helper to Unescape to unescape a single sequence.</summary>
        /// <param name="value">The string being unescaped.</param>
        /// <param name="index">The index of the escaped character.</param>
        /// <returns>The number of additional characters read and the string from the escaped value.</returns>
        static private (int size, string part) unescape(string value, int index) =>
            value[index] switch {
                '\\' => (0, "\\"),
                '\'' => (0, "\'"),
                '\"' => (0, "\""),
                '0'  => (0, "\0"),
                'b'  => (0, "\b"),
                'f'  => (0, "\f"),
                'n'  => (0, "\n"),
                'r'  => (0, "\r"),
                't'  => (0, "\t"),
                'v'  => (0, "\v"),
                'x'  => (2, unescapeHex(value, index, 2)),
                'u'  => (4, unescapeHex(value, index, 4)),
                _    => throw new Exception("Unknown escape sequence").
                            With("value", value[index]).
                            With("index", index)
            };

        /// <summary>
        /// This converts an escaped strings from a tokenized language
        /// into the correct characters for the string.
        /// </summary>
        /// <param name="value">The value to unescape.</param>
        /// <returns>The unescaped string.</returns>
        static public string Unescape(string value) {
            StringBuilder buf = new();
            int start = 0;
            while (start < value.Length) {
                int stop = value.IndexOf('\\', start);
                if (stop < 0) {
                    buf.Append(value[start..]);
                    break;
                }
                buf.Append(value[start..stop]);
                (int size, string part) = unescape(value, stop+1);
                buf.Append(part);
                start = stop + 2 + size;
            }
            return buf.ToString();
        }

        /// <summary>Formats the given double value and ensures it doesn't look like an int.</summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The formatted double.</returns>
        static private string formatDouble(double value) {
            string str = value.ToString().ToLower();
            return str.Contains('.') || str.Contains('e') ? str : str+".0";
        }

        /// <summary>Used to format the resulting values from the calculator.</summary>
        /// <param name="value">The value to format.</param>
        /// <returns>The string for the format.</returns>
        static public string ValueToString(object value) =>
            value switch {
                null           => "null",
                bool      bVal => (bVal ? "true" : "false"),
                Exception eVal => eVal.Message,
                double    dVal => formatDouble(dVal),
                string    sVal => Escape(sVal),
                _              => value.ToString()
            };
    }
}
