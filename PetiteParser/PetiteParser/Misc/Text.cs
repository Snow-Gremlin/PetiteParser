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
                    case '\\':
                        buf.Append("\\\\");
                        break;
                    case '\n':
                        buf.Append("\\n");
                        break;
                    case '\t':
                        buf.Append("\\t");
                        break;
                    case '\r':
                        buf.Append("\\r");
                        break;
                    case '\"':
                        buf.Append("\\\"");
                        break;
                    default:
                        buf.Append(c);
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
                //  "\\", "\n", "\"", "\'", "\t", "\r", "\xFF", "\uFFFF"
                string hex;
                Rune charCode;
                switch (value[stop+1]) {
                    case '\\':
                        buf.Append('\\');
                        break;
                    case 'n':
                        buf.Append('\n');
                        break;
                    case 't':
                        buf.Append('\t');
                        break;
                    case 'r':
                        buf.Append('\r');
                        break;
                    case '\'':
                        buf.Append('\'');
                        break;
                    case '"':
                        buf.Append('"');
                        break;
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
    }
}
