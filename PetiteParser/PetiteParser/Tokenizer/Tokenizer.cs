using System;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Tokenizer {

    /// <summary>A tokenizer for breaking a string into tokens.</summary>
    public class Tokenizer {
        static internal string EscapeText(string text) =>
            text.Replace("\\", "\\\\").Replace("\n", "\\n").Replace("\t", "\\t").Replace("\"", "\\\"");

        private Dictionary<string, State> states;
        private Dictionary<string, TokenState> token;
        private HashSet<string> consume;
        private State start;

        /// <summary>Creates a new tokenizer.</summary>
        public Tokenizer() {
            this.states = new Dictionary<string, State>();
            this.token = new Dictionary<string, TokenState>();
            this.consume = new HashSet<string>();
            this.start = null;
        }

        /// <summary>
        /// Sets the start state for the tokenizer to a state with the name state name.
        /// If that state doesn't exist it will be created.
        /// </summary>
        /// <param name="stateName">The name of the state to set to the start.</param>
        /// <returns>The new start node.</returns>
        public State Start(string stateName) {
            this.start = this.State(stateName);
            return this.start;
        }

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
        public void Consume(IEnumerable<string> tokens) {
            foreach (string token in tokens) this.consume.Add(token);
        }

        /// <summary>
        /// Tokenizes the given input string with the current configured
        /// tokenizer and returns the iterator of tokens for the input.
        /// This will throw an exception if the input is not tokenizable.
        /// </summary>
        /// <param name="input">The input string to tokenize.</param>
        /// <returns>The resulting tokens.</returns>
        public IEnumerable<Token> Tokenize(string input) =>
            this.Tokenize(input.EnumerateRunes());

        /// <summary>
        /// Tokenizes the given iterator of characters with the current configured
        /// tokenizer and returns the iterator of tokens for the input.
        /// This will throw an exception if the input is not tokenizable.
        /// </summary>
        /// <param name="runes">The input runes to tokenize.</param>
        /// <returns>The resulting tokens.</returns>
        public IEnumerable<Token> Tokenize(IEnumerable<Rune> runes) {
            IEnumerator<Rune> iterator = runes.GetEnumerator();
            Token lastToken = null;
            State state = this.start;
            int index = 0;
            int lastIndex = 0;
            int lastLength = 0;
            List<Rune> outText  = new List<Rune>();
            List<Rune> allInput = new List<Rune>();
            List<Rune> retoken  = new List<Rune>();

            while (true) {
                Rune c;
                if (retoken.Count > 0) {
                    c = retoken[0];
                    retoken.RemoveAt(0);
                } else {
                    if (!iterator.MoveNext()) break;
                    c = iterator.Current;
                }
                allInput.Add(c);
                index++;

                // Transition to the next state with the current character.
                Transition trans = state.FindTransition(c);
                if (trans is null) {
                    // No transition found.
                    if (lastToken is null) {
                        // No previous found token state, therefore this part
                        // of the input isn't tokenizable with this tokenizer.
                        string text = string.Concat(allInput);
                        throw new Exception("Untokenizable string [state: "+state.Name+
                            ", index "+index+"]: \""+EscapeText(text)+"\"");
                    }

                    // Reset to previous found token's state.
                    Token resultToken = lastToken;
                    index = lastIndex;
                    allInput.RemoveRange(0, lastLength);
                    retoken.AddRange(allInput);
                    allInput.Clear();
                    outText.Clear();
                    lastToken = null;
                    lastLength = 0;
                    state = this.start;

                    if (!this.consume.Contains(resultToken.Name))
                        yield return resultToken;

                } else {
                    // Transition to the next state and check if it is an acceptance state.
                    // Store acceptance state to return to if needed.
                    if (!trans.Consume) outText.Add(c);
                    state = trans.Target;
                    if (state.Token != null) {
                        string text = string.Concat(outText);
                        lastToken = state.Token.GetToken(text, index);
                        lastLength = allInput.Count;
                        lastIndex = index;
                    }
                }
            }

            if ((lastToken != null) && (!this.consume.Contains(lastToken.Name)))
                yield return lastToken;
        }

        /// <summary>Gets the human readable debug string.</summary>
        /// <returns></returns>
        public override string ToString() {
            StringBuilder buf = new StringBuilder();
            if (this.start != null) buf.AppendLine(this.start.ToDebugString());
            foreach (State state in this.states.Values) {
              if (state != this.start) buf.AppendLine(state.ToDebugString());
            }
            return buf.ToString();
          }
        }
    }
}
