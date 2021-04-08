namespace PetiteParser.Grammar {

    /// <summary>
    /// A prompt is an optional item which can be added to a parse that is carried
    /// through to the parse results. The prompts can used when compiling or interpreting.
    /// </summary>
    public class Prompt: Item {

        /// <summary>Creates a new prompt.</summary>
        /// <param name="name">The name of this prompt.</param>
        internal Prompt(string name) : base(name) { }

        /// <summary>Gets the string for this prompt.</summary>
        /// <returns>The name of this prompt.</returns>
        public override string ToString() => "{"+base.ToString()+"}";
    }
}
