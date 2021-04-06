using System;
using System.Text;

namespace PetiteParser.Matcher {

    /// <summary>A matcher for a predefined set of characters.</summary>
    public class Predef: IMatcher {

        /// <summary>Matches any rune that is categorized as a control character.</summary>
        static public Predef Control => new Predef("Control", Rune.IsControl);

        /// <summary>Matches any rune that is categorized as a decimal digit.</summary>
        static public Predef Digit => new Predef("Digit", Rune.IsDigit);

        /// <summary>Matches any rune that is categorized as a letter.</summary>
        static public Predef Letter => new Predef("Letter", Rune.IsLetter);

        /// <summary>Matches any rune that is categorized as a letter or a decimal digit.</summary>
        static public Predef LetterOrDigit => new Predef("LetterOrDigit", Rune.IsLetterOrDigit);

        /// <summary>Matches any rune that is categorized as a lowercase letter.</summary>
        static public Predef Lower => new Predef("Lower", Rune.IsLower);

        /// <summary>Matches any rune that is categorized as a number.</summary>
        static public Predef Number => new Predef("Number", Rune.IsNumber);

        /// <summary>Matches any rune that is categorized as a punctuation mark.</summary>
        static public Predef Punctuation => new Predef("Punctuation", Rune.IsPunctuation);

        /// <summary>Matches any rune that is categorized as a separator character.</summary>
        static public Predef Separator => new Predef("Separator", Rune.IsSeparator);

        /// <summary>Matches any rune that is categorized as a symbol character.</summary>
        static public Predef Symbol => new Predef("Symbol", Rune.IsSymbol);

        /// <summary>Matches any rune that is categorized as a uppercase letter.</summary>
        static public Predef Upper => new Predef("Upper", Rune.IsUpper);

        /// <summary>Matches any rune that is categorized as a white space character.</summary>
        static public Predef WhiteSpace => new Predef("WhiteSpace", Rune.IsWhiteSpace);

        /// <summary>The name of the predefinition.</summary>
        private readonly string name;

        /// <summary>The handler for checking the rune.</summary>
        private readonly Func<Rune, bool> handler;

        /// <summary>Creates a new predefined matcher.</summary>
        /// <param name="name">The name of the matcher.</param>
        /// <param name="handler">The handler for checking the rune.</param>
        public Predef(string name, Func<Rune, bool> handler) {
            this.name = name;
            this.handler = handler;
        }

        /// <summary>Determines if this matcher matches the given character.</summary>
        /// <param name="c">The character to match.</param>
        /// <returns>True if the character is matched by the predefined set, false otherwise.</returns>
        public bool Match(Rune c) => this.handler(c);

        /// <summary>Returns the string for this matcher.</summary>
        /// <returns>The string for this matcher.</returns>
        public override string ToString() => this.name;
    }
}
