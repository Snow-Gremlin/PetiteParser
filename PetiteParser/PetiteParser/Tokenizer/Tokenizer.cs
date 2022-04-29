using PetiteParser.Misc;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Tokenizer {

    /// <summary>A tokenizer for breaking a string into tokens.</summary>
    sealed public class Tokenizer {

        /// <summary>The states organized by the state name.</summary>
        private readonly Dictionary<string, State> states;

        /// <summary>The token states organized by the token name.</summary>
        private readonly Dictionary<string, TokenState> token;

        /// <summary>The set of consuming tokens.</summary>
        private readonly HashSet<string> consume;

        /// <summary>The state to start a tokenization from.</summary>
        private State start;

        /// <summary>The token to use when an error occurs or nil to throw an exception.</summary>
        private TokenState errorToken;

        /// <summary>Creates a new tokenizer.</summary>
        public Tokenizer() {
            this.states     = new Dictionary<string, State>();
            this.token      = new Dictionary<string, TokenState>();
            this.consume    = new HashSet<string>();
            this.start      = null;
            this.errorToken = null;
        }

        /// <summary>
        /// Sets the start state for the tokenizer to a state with the name state name.
        /// If that state doesn't exist it will be created.
        /// </summary>
        /// <param name="stateName">The name of the state to set to the start.</param>
        /// <returns>The new start node.</returns>
        public State Start(string stateName) =>
            this.start = this.State(stateName);

        /// <summary>
        /// Creates and adds a state by the given name state name.
        /// If a state already exists it is returned, otherwise the new state is returned.
        /// </summary>
        /// <param name="stateName">The name of the state node to find or create.</param>
        /// <returns>The found or new state.</returns>
        public State State(string stateName) {
            if (!this.states.TryGetValue(stateName, out State state)) {
                state = new State(this, stateName);
                this.states.Add(stateName, state);
            }
            return state;
        }

        /// <summary>
        /// Creates and add an acceptance token with the given name tokenName.
        /// A new acceptance token is not connected to any state.
        /// If a token by that name already exists it will be returned, otherwise the new token is returned.
        /// </summary>
        /// <param name="tokenName">The name of the token state to find or create.</param>
        /// <returns>The found or new token state.</returns>
        public TokenState Token(string tokenName) {
            if (!this.token.TryGetValue(tokenName, out TokenState token)) {
                token = new TokenState(this, tokenName);
                this.token.Add(tokenName, token);
            }
            return token;
        }

        /// <summary>
        /// Joins the two given states and returns the new or already existing transition.
        /// </summary>
        /// <param name="startStateName">The name of the state to start at.</param>
        /// <param name="endStateName">The name of the state to end at.</param>
        /// <returns>The new or existing transition.</returns>
        public Transition Join(string startStateName, string endStateName) =>
            this.State(startStateName).Join(endStateName);

        /// <summary>
        /// This is short hand for a join and SetToken where the state name and token name are the same.
        /// </summary>
        /// <param name="startStateName">The name of the state to start at.</param>
        /// <param name="endStateName">The name of the state to end at.</param>
        /// <returns>The new or existing transition.</returns>
        public Transition JoinToToken(string startStateName, string endStateName) {
            this.State(endStateName).SetToken(endStateName);
            return this.State(startStateName).Join(endStateName);
        }

        /// <summary>
        /// Sets the token for the given state and returns the acceptance token.
        /// </summary>
        /// <param name="stateName">The name of the state.</param>
        /// <param name="tokenName">The name of the token.</param>
        /// <returns>The acceptance token.</returns>
        public TokenState SetToken(string stateName, string tokenName) =>
            this.State(stateName).SetToken(tokenName);

        /// <summary>Sets which tokens should be consumed and not emitted.</summary>
        /// <param name="tokens">The tokens to consume.</param>
        public void Consume(params string[] tokens) =>
            this.Consume(tokens as IEnumerable<string>);

        /// <summary>Sets which tokens should be consumed and not emitted.</summary>
        /// <param name="tokens">The tokens to consume.</param>
        public void Consume(IEnumerable<string> tokens) =>
            tokens.Foreach(this.consume.Add);

        /// <summary>
        /// Sets the error token to use if the tokenizer can not tokenize something.
        /// If that token doesn't exist it will be created.
        /// </summary>
        /// <param name="stateName">The name of the error token.</param>
        /// <returns>The new error token.</returns>
        public TokenState ErrorToken(string stateName) =>
            this.errorToken = this.Token(stateName);

        /// <summary>Indicates if errors will be tokenized or if they'll be thrown.</summary>
        public bool TokenizeError => this.errorToken is not null;

        /// <summary>
        /// Tokenizes the given input string with the current configured
        /// tokenizer and returns the iterator of tokens for the input.
        /// This will throw an exception if the input is not tokenizable.
        /// </summary>
        /// <param name="input">The input string to tokenize.</param>
        /// <returns>The resulting tokens.</returns>
        public IEnumerable<Token> Tokenize(params string[] input) =>
            this.Tokenize(new Scanner.Default(input));

        /// <summary>
        /// Tokenizes the given input string with the current configured
        /// tokenizer and returns the iterator of tokens for the input.
        /// This will throw an exception if the input is not tokenizable.
        /// </summary>
        /// <param name="watcher">This is a tool used to help debug a tokenizer configuration.</param>
        /// <param name="input">The input string to tokenize.</param>
        /// <returns>The resulting tokens.</returns>
        public IEnumerable<Token> Tokenize(Watcher watcher, params string[] input) =>
            this.Tokenize(new Scanner.Default(input), watcher);

        /// <summary>
        /// Tokenizes the given input string with the current configured
        /// tokenizer and returns the iterator of tokens for the input.
        /// This will throw an exception if the input is not tokenizable.
        /// </summary>
        /// <remarks>
        /// If the input comes from multiple locations, the input name may be changed
        /// during the enumeration of the input into the tokenizer.
        /// </remarks>
        /// <param name="input">The input strings to tokenize.</param>
        /// <param name="watcher">This is a tool used to help debug a tokenizer configuration.</param>
        /// <returns>The resulting tokens.</returns>
        public IEnumerable<Token> Tokenize(IEnumerable<string> input, Watcher watcher = null) =>
            this.Tokenize(new Scanner.Default(input), watcher);

        /// <summary>
        /// Tokenizes the given iterator of characters with the current configured
        /// tokenizer and returns the iterator of tokens for the input.
        /// This will throw an exception if the input is not tokenizable.
        /// </summary>
        /// <remarks>
        /// If the input comes from multiple locations, the input name may be changed
        /// during the enumeration of the input into the tokenizer.
        /// </remarks>
        /// <param name="input">The input runes to tokenize.</param>
        /// <param name="watcher">This is a tool used to help debug a tokenizer configuration.</param>
        /// <returns>The resulting tokens.</returns>
        public IEnumerable<Token> Tokenize(IEnumerable<Rune> input, Watcher watcher = null) =>
            this.Tokenize(new Scanner.Default(input), watcher);

        /// <summary>
        /// Tokenizes the given iterator of characters with the current configured
        /// tokenizer and returns the iterator of tokens for the input.
        /// This will throw an exception if the input is not tokenizable.
        /// </summary>
        /// <remarks>
        /// If the input comes from multiple locations, the input name may be changed
        /// during the enumeration of the input into the tokenizer.
        /// </remarks>
        /// <param name="scanner">The input to get the runes to tokenize.</param>
        /// <param name="watcher">This is a tool used to help debug a tokenizer configuration.</param>
        /// <returns>The resulting tokens.</returns>
        public IEnumerable<Token> Tokenize(Scanner.IScanner scanner, Watcher watcher = null) =>
            new Runner(scanner, watcher, this.start, this.errorToken, this.consume).Tokenize();

        /// <summary>Gets the human readable debug string.</summary>
        /// <returns>The tokenizer's string.</returns>
        public override string ToString() {
            StringBuilder buf = new();
            if (this.start is not null) this.start.AppendDebugString(buf, this.consume);
            foreach (State state in this.states.Values) {
                if (state != this.start) state.AppendDebugString(buf, this.consume);
            }
            if (this.TokenizeError) {
                buf.Append(this.errorToken.ToString());
            }
            return buf.ToString();
        }
    }
}
