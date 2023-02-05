using PetiteParser.Logger;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>Removed any rules which are not reachable from the start term.</summary>
internal class RemoveUnusedTerms : IPrecept {

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log) {
        HashSet<Term> touched = new();
        this.addTerm(analyzer.Grammar.StartTerm, touched);

        List<Term> unreachable = analyzer.Grammar.Terms.WhereNot(touched.Contains).ToList();
        return unreachable.ForeachAny(analyzer.Grammar.RemoveTerm);
    }

    // TODO: Comment
    private void addTerm(Term? term, HashSet<Term> touched) {
        if (term is null || touched.Contains(term)) return;
        touched.Add(term);
        term.Rules.SelectMany(r => r.Items).OfType<Term>().Foreach(t => addTerm(t, touched));
    }
}
