using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Analyzer;

/// <summary>
/// This is like a tiny fragment for indicating a location in a rule
/// with an offset that is an index into the rules items.
/// </summary>
public class RuleOffset {

    /// <summary>Creates a new rule offset.</summary>
    /// <param name="rule">The rule that has the index into.</param>
    /// <param name="index">The index offset into the rule.</param>
    public RuleOffset(Rule rule, int index) {
        this.Rule  = rule;
        this.Index = index;
    }

    /// <summary>The rule that has the index into.</summary>
    public Rule Rule { get; }

    /// <summary>The index offset into the rule.</summary>
    public int Index { get; }

    /// <summary>Indicates if the fragment is at the end of the rule.</summary>
    public bool AtEnd => this.Rule.BasicItems.Count() <= this.Index;

    /// <summary>The next item in the rule after this fragment's index or null if at the end.</summary>
    public Item? NextItem => this.Rule.BasicItems.ElementAtOrDefault(this.Index);

    /// <summary>This enumerates all the base items (no prompts) in this fragment's rules after the fragment's index.</summary>
    public IEnumerable<Item> FollowingItems => this.Rule.BasicItems.Skip(this.Index+1);

    /// <summary>This is the string for the rule offset.</summary>
    /// <returns>The rule as a string with the offset indicated in it.</returns>
    public override string ToString() => this.Rule.ToString(this.Index);
}
