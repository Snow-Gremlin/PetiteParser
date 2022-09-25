using PetiteParser.Grammar;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Inspector;

/// <summary>An inspector to check that all the terms, tokens, and prompts, are reachable in the grammar.</summary>
sealed internal class CheckReachability : IInspector {

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar.Grammar grammar, Logger.ILogger log) {
        HashSet<string> termUnreached   = new(grammar.Terms.ToNames());
        HashSet<string> tokenUnreached  = new(grammar.Tokens.ToNames());
        HashSet<string> promptUnreached = new(grammar.Prompts.ToNames());

        touch(grammar.StartTerm, termUnreached, tokenUnreached, promptUnreached);

        if (grammar.ErrorToken is not null)
            tokenUnreached.Remove(grammar.ErrorToken.Name);

        if (termUnreached.Count > 0)
            log.AddErrorF("The following terms are unreachable: {0}", termUnreached.Join(", "));

        if (tokenUnreached.Count > 0)
            log.AddErrorF("The following tokens are unreachable: {0}", tokenUnreached.Join(", "));

        if (promptUnreached.Count > 0)
            log.AddErrorF("The following prompts are unreachable: {0}", promptUnreached.Join(", "));
    }

    /// <summary>This indicates that the given item has been reached and will recursively touch its own items.</summary>
    /// <param name="item">The item to mark as reachable.</param>
    /// <param name="termUnreached">The collection of term names which have not been reached yet.</param>
    /// <param name="tokenUnreached">The collection of token names which have not been reached yet.</param>
    /// <param name="promptUnreached">The collection of prompt names which have not been reached yet.</param>
    static private void touch(Item item, HashSet<string> termUnreached, HashSet<string> tokenUnreached, HashSet<string> promptUnreached) {
        if (item is Term term) {
            if (termUnreached.Contains(term.Name)) {
                termUnreached.Remove(term.Name);
                foreach (Rule r in term.Rules) {
                    foreach (Item innerItem in r.Items) {
                        touch(innerItem, termUnreached, tokenUnreached, promptUnreached);
                    }
                }
            }
        } else if (item is TokenItem) tokenUnreached.Remove(item.Name);
        else if (item is Prompt) promptUnreached.Remove(item.Name);
    }
}
