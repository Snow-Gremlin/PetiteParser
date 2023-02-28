using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Grammar;

/// <summary>
/// A term is a group of rules and part of a rule which defines part of the grammar language.
/// </summary>
/// <remarks>
/// For example the term `<T>` with the rules `<T> => "(" <E> ")"`,
/// `<T> => <E> * <E>`, and `<T> => <E> + <E>`.
/// </remarks>
public class Term: Item {

    /// <summary>The grammar this term belongs to.</summary>
    private readonly Grammar grammar;

    /// <summary>Creates a new rule for the given grammar and term.</summary>
    /// <param name="grammar">The grammar for this term.</param>
    /// <param name="name">The name of this term.</param>
    internal Term(Grammar grammar, string name) : base(name) {
        this.grammar = grammar;
        this.Rules = new List<Rule>();
    }

    /// <summary>Gets the list of rules starting with this term.</summary>
    public List<Rule> Rules { get; }

    /// <summary>Adds a new rule to this term.</summary>
    /// <returns>The newly added rule.</returns>
    public Rule NewRule() {
        Rule rule = new(this.grammar, this);
        this.Rules.Add(rule);
        return rule;
    }

    /// <summary>Gets the string for this term.</summary>
    /// <returns>This is the name of the term.</returns>
    public override string ToString() => "<"+base.ToString()+">";

    /// <summary>Gets the string for the term with all of it's rules shown too.</summary>
    /// <returns>The string for this term and its rules.</returns>
    public string ToStringWithRules() {
        if (this.Rules.Count <= 0) return this.ToString();
        StringBuilder buf = new();
        string head = this.ToString() + " →";
        foreach (Rule rule in this.Rules) {
            buf.Append(head);
            buf.Append(rule.ToString(-1, false));
            head = System.Environment.NewLine + "   |";
        }
        return buf.ToString();
    }
}
