using System.Collections.Generic;

namespace PetiteParser.Grammar {

    /// <summary>
    /// A token is an item to represent a group of text to the parser so it can match tokens to determine
    /// the different rules to take while parsing. This mirrors the `Tokenizer.Token` result object.
    /// </summary>
    public class TokenItem: Item {

        /// <summary>Creates a new token item.</summary>
        /// <param name="name">The name of this item.</param>
        internal TokenItem(string name) : base(name) { }

        /// <summary>Gets the string for this token.</summary>
        /// <returns>The name of this item.</returns>
        public override string ToString() => "["+base.ToString()+"]";
    }
}
