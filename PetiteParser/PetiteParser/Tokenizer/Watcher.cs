using System.Text;
using System.IO;

namespace PetiteParser.Tokenizer {

    /// <summary>This is a tool for debugging a tokenizer configuration.</summary>
    /// <remarks>This can be extended and overridden to change how this watches the tokenizer.</remarks>
    public class Watcher {

        /// <summary>Creates a new watcher.</summary>
        /// <param name="tout">The writer to output to or null to not output.</param>
        public Watcher(TextWriter tout = null) {
            this.Output = tout;
        }

        /// <summary>The writer to output to.</summary>
        protected TextWriter Output;

        /// <summary>Indicates that the tokenizer has been started.</summary>
        virtual public void StartTokenization() =>
            this.Output?.WriteLine("Start");

        /// <summary>Indicates that a tokenizable location was found but this token may not be returned.</summary>
        /// <remarks>By default this will not output anything.</remarks>
        /// <param name="state">This is the current state when the tokenizer is set..</param>
        /// <param name="token">The token which is pending and may be returned or replaced.</param>
        virtual public void SetToken(State state, Token token) { }

        /// <summary>Indicates a tokenization step.</summary>
        /// <remarks>This may output the same character twice if it needed to be retokenized.</remarks>
        /// <param name="state">The current state at this step.</param>
        /// <param name="c">The rune which is being tokenized.</param>
        /// <param name="loc">The location of this rune in the scannet.</param>
        /// <param name="trans">The transition which this rule will take from the current state.</param>
        virtual public void Step(State state, Rune c, Scanner.Location loc, Transition trans) =>
            this.Output?.WriteLine("Step(state:"+state.Name+", rune:"+c+", loc:"+loc+", "+
                (trans is null ? "trarget:-" : "target:"+trans.Target.Name+", consume:"+trans.Consume)+")");

        /// <summary>Indicates a token has been found and the state machine is resetting.</summary>
        /// <param name="count">This is the number of characters that need to be retokenized.</param>
        /// <param name="token">The token that was found and maybe returned.</param>
        /// <param name="consume">True if the token is being consumed, false if the token is returned.</param>
        virtual public void YieldAndReset(int count, Token token, bool consume) =>
            this.Output?.WriteLine("Yield(retoken:"+count+", token:"+token.Name+", text:"+token.Text+", loc:["+token.Start+".."+token.End+"]"+consume+")");

        /// <summary>Indicate the tokenizer has finished with one last token found.</summary>
        /// <param name="token">The token that was found and maybe returned.</param>
        /// <param name="consume">True if the token is being consumed, false if the token is returned.</param>
        virtual public void FinishTokenization(Token token, bool consume) =>
            this.Output?.WriteLine("Finished(token:"+token.Name+", text:"+token.Text+", loc:["+token.Start+".."+token.End+"]"+consume+")");

        /// <summary>Indicates the tokenizer has finished.</summary>
        virtual public void FinishTokenization() =>
            this.Output?.WriteLine("Finished");
    }
}
