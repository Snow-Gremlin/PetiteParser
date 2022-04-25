using System.IO;
using System.Text;

namespace PetiteParser.Tokenizer {

    /// <summary>This is a tool for debugging a tokenizer configuration.</summary>
    /// <remarks>This can be extended and overridden to change how this watches the tokenizer.</remarks>
    public class Watcher {

        /// <summary>Creates a Watcher which outputs to the console's standard out.</summary>
        static public Watcher Console => new(System.Console.Out);

        /// <summary>Creates a new watcher.</summary>
        /// <param name="tout">The writer to output to or null to not output.</param>
        public Watcher(TextWriter tout = null) =>
            this.Output = tout;

        /// <summary>The writer to output to.</summary>
        protected TextWriter Output;

        /// <summary>Indicates that the tokenizer has been started.</summary>
        virtual public void StartTokenization() =>
            this.Output?.WriteLine("Start");

        /// <summary>Indicates that a tokenizable location was found but this token may not be returned.</summary>
        /// <remarks>By default this will not output anything.</remarks>
        /// <param name="state">This is the current state when the tokenizer is set..</param>
        /// <param name="token">The token which is pending and may be returned or replaced.</param>
        virtual public void SetToken(State state, Token token) =>
            this.Output?.WriteLine("SetToken(state:"+state.Name+", token:"+token+")");

        /// <summary>Indicates a tokenization step.</summary>
        /// <remarks>This may output the same character twice if it needed to be re-tokenized.</remarks>
        /// <param name="state">The current state at this step.</param>
        /// <param name="c">The rune which is being tokenized.</param>
        /// <param name="loc">The location of this rune in the scanner.</param>
        /// <param name="trans">The transition which this rule will take from the current state.</param>
        virtual public void Step(State state, Rune c, Scanner.Location loc, Transition trans) =>
            this.Output?.WriteLine("Step(state:"+state.Name+", rune:"+c+", loc:"+loc+", "+
                (trans is null ? "target:-" : "target:"+trans.Target.Name+", consume:"+trans.Consume)+")");

        /// <summary>Indicates a token has been found and the state machine is resetting.</summary>
        /// <param name="count">This is the number of characters that need to be re-tokenized.</param>
        /// <param name="token">The token that was found and maybe returned.</param>
        /// <param name="consume">True if the token is being consumed, false if the token is returned.</param>
        virtual public void YieldAndRescan(int count, Token token, bool consume) =>
            this.Output?.WriteLine("YieldAndRescan(retoken:"+count+", token:"+token.Name+
                ", text:"+token.Text+", loc:["+token.Start+".."+token.End+"], consume:"+consume+")");

        /// <summary>Indicates that an error token has been created or sdded to.</summary>
        /// <param name="token">The error token which was created or added to.</param>
        virtual public void PushToError(Token token) =>
            this.Output?.WriteLine("PushToError(token:"+token+")");

        /// <summary>Indicates the tokenizer has finished.</summary>
        virtual public void FinishTokenization() =>
            this.Output?.WriteLine("Finished");
    }
}
