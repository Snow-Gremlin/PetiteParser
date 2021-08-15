using System.Globalization;
using System.Text;

namespace PetiteParser.Misc {

    /// <summary>Tools for processing text.</summary>
    static public class Text {

        /// <summary>
        /// This will convert an unescaped string into an escaped string
        /// for printing. This will not escape all control characters.
        /// </summary>
        /// <param name="value">The value to escape.</param>
        /// <returns>The escaped string.</returns>
        static public string Escape(string value) {
            StringBuilder buf = new(value.Length);
            foreach (char c in value) {
                switch (c) {
                    case '\\': buf.Append("\\\\"); break;
                    case '\'': buf.Append("\\\'"); break;
                    case '\"': buf.Append("\\\""); break;
                    case '\b': buf.Append("\\b");  break;
                    case '\f': buf.Append("\\f");  break;
                    case '\n': buf.Append("\\n");  break;
                    case '\r': buf.Append("\\r");  break;
                    case '\t': buf.Append("\\t");  break;
                    case '\v': buf.Append("\\v");  break;
                    default:
                        Rune r = new(c);
                        if (r.IsAscii) buf.Append(c);
                        else buf.Append(string.Format("\\u{0:X4}", r.Value));
                        break;
                }
            }
            return buf.ToString();
        }

        /// <summary>
        /// This will convert an escaped strings from a tokenized language into
        /// the correct characters for the string.
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
                string hex;
                Rune charCode;
                switch (value[stop+1]) {
                    case '\\': buf.Append('\\'); break;
                    case '\'': buf.Append('\''); break;
                    case '\"': buf.Append('\"'); break;
                    case 'b':  buf.Append('\b'); break;
                    case 'f':  buf.Append('\f'); break;
                    case 'n':  buf.Append('\n'); break;
                    case 'r':  buf.Append('\r'); break;
                    case 't':  buf.Append('\t'); break;
                    case 'v':  buf.Append('\v'); break;
                    case 'x':
                        hex = value[(stop+2)..(stop+4)];
                        charCode = new Rune(int.Parse(hex, NumberStyles.HexNumber));
                        buf.Append(charCode);
                        stop += 2;
                        break;
                    case 'u':
                        hex = value[(stop+2)..(stop+6)];
                        charCode = new Rune(int.Parse(hex, NumberStyles.HexNumber));
                        buf.Append(charCode);
                        stop += 4;
                        break;
                }
                start = stop + 2;
            }
            return buf.ToString();
        }

        /// <summary>Formats the given double value.</summary>
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
            value is bool ? (((value as bool?) ?? false) ? "true" : "false") :
            value is Exception ? (value as Exception).Message :
            value is double ? formatDouble(value as double? ?? 0.0) :
            value is string ? Misc.Text.Escape(value as string) :
            value.ToString();
    }
}
