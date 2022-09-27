using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Tokenizer;

/// <summary>
/// A state in the tokenizer used as a character point in the tokenizer state machine.
/// </summary>
sealed public class State {

    /// <summary>Creates a new state.</summary>
    /// <param name="tokenizer">The tokenizer this state is for.</param>
    /// <param name="name">This is the name for the tokenizer.</param>
    public State(Tokenizer tokenizer, string name) {
        this.Tokenizer = tokenizer;
        this.Trans     = new();
        this.Name      = name;
        this.Token     = null;
    }
    
    /// <summary>The tokenizer this state is for.</summary>
    public Tokenizer Tokenizer { get; }

    /// <summary>The name of the state.</summary>
    public string Name { get; }

    /// <summary>
    /// The acceptance token for this state if this state accepts the input at this point.
    /// If this isn't an accepting state this will return null.
    /// </summary>
    public TokenState? Token { get; private set; }

    /// <summary>The list of transactions from this state.</summary>
    public List<Transition> Trans { get; }

    /// <summary>
    /// Sets the acceptance token for this state to the token with the given token name.
    /// If no token by that name exists it will be created.
    /// </summary>
    /// <param name="tokenName">The name of the token to set.</param>
    /// <returns>The new token that was set.</returns>
    public TokenState SetToken(string tokenName) =>
        this.Token = this.Tokenizer.Token(tokenName);

    /// <summary>
    /// Joins this state to another state by the given [endStateName]
    /// with a new transition. If a transition already exists and it has the same consume
    /// flag between these two states that transition is returned,
    /// otherwise the new transition is returned.
    /// </summary>
    /// <param name="endStateName">The name of the state to end at.</param>
    /// <param name="consume">Indicates if this consumes the character.</param>
    /// <returns>The transition found or null.</returns>
    public Transition Join(string endStateName, bool consume = false) {
        foreach (Transition trans in this.Trans) {
            if ((trans.Target.Name == endStateName) && (trans.Consume == consume))
                return trans;
        }
        State target = this.Tokenizer.State(endStateName);
        Transition newTrans = new(target, consume);
        this.Trans.Add(newTrans);
        return newTrans;
    }

    /// <summary>
    /// Finds the matching transition given a character.
    /// If no transition matches null is returned.
    /// </summary>
    /// <param name="c">The character to find the transition for.</param>
    /// <returns>The transition found or null.</returns>
    public Transition? FindTransition(Rune c) =>
        this.Trans.FirstOrDefault(trans => trans.Match(c));

    /// <summary>Gets the name for this state.</summary>
    /// <returns>The state's name.</returns>
    public override string ToString() => this.Name;

    /// <summary>Gets the human readable debug string added to the given buffer.</summary>
    /// <param name="buf">The buffer to append to.</param>
    /// <param name="consume">The set of consumers.</param>
    internal void AppendDebugString(StringBuilder buf, HashSet<string> consume) {
        buf.Append("("+this.Name+")");
        if (this.Token is not null) {
            buf.Append(" => ["+this.Token.Name+"]");
            if (consume.Contains(this.Token.Name))
                buf.Append(" (consume)");
            this.Token.AppendDebugString(buf, consume);
        }
        foreach (Transition trans in this.Trans) {
            buf.AppendLine();
            buf.Append("  -- "+Text.Escape(trans.ToString()));
        }
        buf.AppendLine();
    }
}
