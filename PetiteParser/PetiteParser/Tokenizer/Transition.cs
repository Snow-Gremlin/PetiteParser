namespace PetiteParser.Tokenizer {

    /// <summary>
    /// A transition is a matcher group which connects two states together.
    /// When at one state this transition should be taken to the next if
    /// the next character in the input is a match.
    /// </summary>
    sealed public class Transition: Matcher.Group {

        /// <summary>Gets the state to goto if a character matches this transition.</summary>
        public readonly State Target;

        /// <summary>Creates a new transition.</summary>
        /// <param name="target">The state to target.</param>
        /// <param name="consume">Indicates if this consumes the character.</param>
        public Transition(State target, bool consume = false) {
            this.Target = target;
            this.Consume = consume;
        }

        /// <summary>
        /// Indicates if the character should be consumed (true)
        /// or appended (false) to the resulting string.
        /// </summary>
        public bool Consume;

        /// <summary>This sets the consume flag for this transition.</summary>
        /// <param name="consume">True to consume token, false otherwise.</param>
        /// <returns>This transition so that it can be used in a chain.</returns>
        public Transition SetConsume(bool consume) {
            this.Consume = consume;
            return this;
        }

        /// <summary>Gets the string for the transition.</summary>
        /// <returns>The transition's string.</returns>
        public override string ToString() => this.Target.Name+": "+base.ToString()+
            (this.Consume ? " (consume)" : "");
    }
}
