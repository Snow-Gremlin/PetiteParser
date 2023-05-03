using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PetiteParser.Formatting;

/// <summary>Tools for processing text.</summary>
/// <see cref="https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/strings/"/>
static public class Text {
    #region Escape

    /// <summary>This converts a character rune into an escaped string for printing.</summary>
    /// <param name="r">The character rune to escape.</param>
    /// <returns>The escaped string.</returns>
    static public string Escape(Rune r) {
        if (r.IsAscii) {
            char c = (char)r.Value;
            switch (c) {
                case '\\': return "\\\\";
                case '\'': return "\\\'";
                case '\"': return "\\\"";
                case '\0': return "\\0";
                case '\b': return "\\b";
                case '\f': return "\\f";
                case '\n': return "\\n";
                case '\r': return "\\r";
                case '\t': return "\\t";
                case '\v': return "\\v";
            }
            if (!Rune.IsControl(r)) return c.ToString();
        }
        int value = r.Value;
        return value <= 0xFF   ? string.Format(CultureInfo.InvariantCulture, "\\x{0:X2}", value) :
               value <= 0xFFFF ? string.Format(CultureInfo.InvariantCulture, "\\u{0:X4}", value) :
                                 string.Format(CultureInfo.InvariantCulture, "\\U{0:X8}", value);
    }

    /// <summary>This converts a character into an escaped string for printing.</summary>
    /// <param name="c">The character to escape.</param>
    /// <returns>The escaped string.</returns>
    static public string Escape(char c) => Escape(new Rune(c));

    /// <summary>This converts unescaped characters into escaped string for printing.</summary>
    /// <param name="values">The characters to escape.</param>
    /// <returns>The escaped strings.</returns>
    static public IEnumerable<string> Escape(IEnumerable<char> values) => values.Select(Escape);

    /// <summary>This converts unescaped character runes into escaped string for printing.</summary>
    /// <param name="values">The characters to escape.</param>
    /// <returns>The escaped strings.</returns>
    static public IEnumerable<string> Escape(IEnumerable<Rune> values) => values.Select(Escape);

    /// <summary>This converts an unescaped string into an escaped string for printing.</summary>
    /// <param name="value">The string to escape.</param>
    /// <returns>The escaped string.</returns>
    static public string Escape(string value) => Escape(value.EnumerateRunes()).Join();

    #endregion
    #region Unescape

    /// <summary>This is a helper to unescape a simple single character.</summary>
    /// <param name="part">the simple single character.</param>
    /// <returns>The number of additional characters read and the string from the escaped value.</returns>
    static private (int size, string part) unescapeSingle(string part) => (0, part);

    /// <summary>This is a helper to unescape a hex encoded sequence.</summary>
    /// <remarks>This does not support the variable length (like C#'s \xH[H][H][H]).</remarks>
    /// <param name="value">The string being unescaped.</param>
    /// <param name="index">The index of the escaped character.</param>
    /// <param name="size">The number of characters to read.</param>
    /// <returns>The number of additional characters read and the string from the escaped value.</returns>
    static private (int size, string part) unescapeHex(string value, int index, int size) {
        int low = index + 1;
        int high = low + size;
        if (value.Length < high)
            throw new FormatException("Not enough values after escape sequence " +
                "[value: " + value[index] + ", index: " + index + ", size: " + size + "]");
        Rune charCode = new(int.Parse(value[low..high], NumberStyles.HexNumber, CultureInfo.InvariantCulture));
        return (size, charCode.ToString());
    }

    /// <summary>This is a helper to unescape a single sequence.</summary>
    /// <param name="value">The string being unescaped.</param>
    /// <param name="index">The index of the escaped character.</param>
    /// <returns>The number of additional characters read and the string from the escaped value.</returns>
    static private (int size, string part) unescape(string value, int index) =>
        value[index] switch {
            '\\' => unescapeSingle("\\"),
            '\'' => unescapeSingle("\'"),
            '\"' => unescapeSingle("\""),
            '0'  => unescapeSingle("\0"),
            'b'  => unescapeSingle("\b"),
            'f'  => unescapeSingle("\f"),
            'n'  => unescapeSingle("\n"),
            'r'  => unescapeSingle("\r"),
            't'  => unescapeSingle("\t"),
            'v'  => unescapeSingle("\v"),
            'x'  => unescapeHex(value, index, 2),
            'u'  => unescapeHex(value, index, 4),
            'U'  => unescapeHex(value, index, 8),
            _    => throw new FormatException("Unknown escape sequence [value: " + value[index] + ", index: " + index + "]")
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
        int count = value.Length;
        while (start < count) {
            int stop = value.IndexOf('\\', start);
            if (stop < 0) {
                buf.Append(value[start..]);
                break;
            }
            buf.Append(value[start..stop]);
            (int size, string part) = unescape(value, stop + 1);
            buf.Append(part);
            start = stop + 2 + size;
        }
        return buf.ToString();
    }

    #endregion
    #region Format

    /// <summary>Formats the given boolean value.</summary>
    /// <remarks>Uses lower case true and false instead of the default C# title case.</remarks>
    /// <param name="value">The boolean value to convert to a string.</param>
    /// <returns>The formatted boolean.</returns>
    static private string format(bool value) => value ? "true" : "false";

    /// <summary>Formats the given double value and ensures it doesn't look like an int.</summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted double.</returns>
    static private string format(double value) {
        string str = value.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture);
        return !double.IsFinite(value) ? str :
            str.Contains('.') || str.Contains('e') ? str :
            str + ".0";
    }
    
    /// <summary>Formats the given float value and ensures it doesn't look like an int.</summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The formatted float.</returns>
    static private string format(float value) {
        string str = value.ToString(CultureInfo.InvariantCulture).ToLower(CultureInfo.InvariantCulture);
        return !float.IsFinite(value) ? str :
            str.Contains('.') || str.Contains('e') ? str :
            str + ".0";
    }

    /// <summary>Used to format the resulting values from the calculator.</summary>
    /// <param name="value">The value to format.</param>
    /// <returns>The string for the format.</returns>
    static public string ValueToString(object? value) =>
        value switch {
            null           => "null",
            bool      bVal => format(bVal),
            Exception eVal => eVal.Message,
            double    dVal => format(dVal),
            float     fVal => format(fVal),
            char      cVal => Escape(cVal),
            Rune      rVal => Escape(rVal),
            string    sVal => Escape(sVal),
            _              => Escape(value.ToString() ?? "null")
        };

    #endregion
}
