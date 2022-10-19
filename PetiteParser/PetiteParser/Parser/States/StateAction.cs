using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Parser.Table;
using System.Text;

namespace PetiteParser.Parser.States;

// TODO: Comment
internal readonly record struct StateAction(IAction Action, TokenItem[] Lookaheads, State? NextState = null) {

    public StateAction Join(StateAction other) {


        // TODO: Implement

        return this;
    }

    override public string ToString() {
        StringBuilder result = new();
        result.Append(this.Action);
        //if (this.NextState is not null)
        //    result.Append(" => " + this.NextState.Number);
        if (Lookaheads is not null && Lookaheads.Length > 0)
            result.Append(" @ "+this.Lookaheads.Join(", "));
        return result.ToString();
    }
}
