using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Grammar;

/// <summary>
/// A grammar is a definition of a language.
/// It is made up of a set of terms and the rules for how each term is used.
/// </summary>
/// <remarks>
/// Formally a Grammar is defined as `G = (V, E, R, S)`:
/// - `V` is the set of `v` (non-terminal characters / variables).
///   These are referred to as the terms of the grammar in this implementation.
/// - `E` (normally shown as an epsilon) is the set of `t` (terminals / tokens).
///   These are referred to as the tokens of this grammar in this implementation.
///   `V` and `E` are disjoint, meaning no `v` exists in `E` and no `t` exists in `V`.
/// - `R` is the relationship of `V` to `(V union E)*`, where here the asterisk is
///   the Kleene star operation. Each `r` in `R` is a rule (rewrite rules / productions)
///   of the grammar as represented by `v → [v or t]*` where `[v or t]` is an item in
///   in the rule. Each term must be the start of one or more rules with, at least
///   one rule must contain a single item. Each term may include a rule with no items
///   (`v → ε`). There should be no duplicate rules for a term.
/// - `S` is the start term where `S` must exist in `V`.
///
/// For the LR1 parser, used by Petite Parser, the grammar must be a Context-free
/// Language (CFL) where `L(G) = {w in E*: S => w}`, meaning that all non-terminals can be
/// reached (`=>` means reachable) from the start term following the rules of the grammar.
///
/// To be a _proper_ CFG there my be no unreachable terms (for all `N` in `V` there
/// exists an `a` and `b` in `(V union U)*` such that `S => a N b`), no unproductive
/// symbols (for all `N` in `V` there exists a `w` in `E*` such that `N => w`),
/// no ε-rules (there does not exist an `N` in `V` such that `N → ε` exist in `R`), and
/// there are no cycles (there does not exist an `N` in `V` such that `N => ... => N`).
/// </remarks>
/// <see cref="https://en.wikipedia.org/wiki/Context-free_grammar"/>
sealed public class Grammar {

    /// <summary> This will trim an item name and check if the name is empty.</summary>
    /// <param name="name">The name to sanitize.</param>
    /// <param name="itemName">The name of the type of item being sanitized.</param>
    /// <returns>The sanitized name.</returns>
    static private string sanitizedName(string name, string itemName) =>
        string.IsNullOrWhiteSpace(name) ?
        throw new Exception("May not have an all whitespace or empty "+itemName+" name.") :
        name.Trim();

    private readonly HashSet<Term>      terms;
    private readonly HashSet<TokenItem> tokens;
    private readonly HashSet<Prompt>    prompts;

    /// <summary>Creates a new empty grammar.</summary>
    public Grammar() {
        this.terms   = new HashSet<Term>();
        this.tokens  = new HashSet<TokenItem>();
        this.prompts = new HashSet<Prompt>();
        this.StartTerm  = null;
        this.ErrorToken = null;
    }

    /// <summary>Creates a copy of this grammar.</summary>
    /// <returns>The copied grammar.</returns>
    public Grammar Copy() {
        Grammar grammar = new();
        foreach (Term term in this.terms)
            grammar.add(term.Name);

        if (this.StartTerm is not null)
            grammar.StartTerm = grammar.findTerm(this.StartTerm.Name);

        if (this.ErrorToken is not null)
            grammar.ErrorToken = grammar.Token(this.ErrorToken.Name);

        foreach (Term term in this.terms) {
            Term termCopy = grammar.findTerm(term.Name);
            foreach (Rule rule in term.Rules) {
                Rule ruleCopy = new(grammar, termCopy);
                foreach (Item item in rule.Items) {
                    Item itemCopy =
                            item is Term      ? grammar.Term(item.Name) :
                            item is TokenItem ? grammar.Token(item.Name) :
                            item is Prompt    ? grammar.Prompt(item.Name) :
                            throw new Exception("Unknown item type: "+item);
                    ruleCopy.Items.Add(itemCopy);
                }
                termCopy.Rules.Add(ruleCopy);
            }
        }
        return grammar;
    }

    /// <summary>
    /// Creates or adds a term for a set of rules and
    /// sets it as the starting term for the grammar.
    /// </summary>
    /// <param name="termName">The name of the term to start from.</param>
    /// <returns>The new start term.</returns>
    public Term Start(string termName) =>
        this.StartTerm = this.Term(termName);

    /// <summary>Gets the start term for this grammar.</summary>
    public Term StartTerm { get; private set; }

    /// <summary>
    /// This sets the token name for errors from the tokenizer.
    /// Anytime a token with this name is received an error will be
    /// created and added to the results, instead of the token being used in the parse.
    /// </summary>
    /// <param name="tokenName">The name of the token to create errors for.</param>
    /// <returns>The token item for the given token name.</returns>
    public TokenItem Error(string tokenName) =>
        this.ErrorToken = this.Token(tokenName);

    /// <summary>Gets the start term for this grammar.</summary>
    public TokenItem ErrorToken { get; private set; }

    /// <summary>Gets the terms for this grammar.</summary>
    public IEnumerable<Term> Terms => this.terms;

    /// <summary>Gets the tokens for this grammar.</summary>
    public IEnumerable<TokenItem> Tokens => this.tokens;

    /// <summary>Gets the prompts for this grammar.</summary>
    public IEnumerable<Prompt> Prompts => this.prompts;

    /// <summary>Finds a term in this grammar by the given name.</summary>
    /// <param name="termName">The name of the term to find.</param>
    /// <returns>The term by the given name or null if no term by that name if found.</returns>
    private Term findTerm(string termName) =>
        this.terms.FindItemByName(termName);

    /// <summary>Finds all the terms which have the given term prefix.</summary>
    /// <param name="termPrefix">The prefix the name must start with.</param>
    /// <returns>The terms which start with the given prefix.</returns>
    private IEnumerable<Term> findTermsStartingWith(string termPrefix) =>
        this.terms.FindItemsStartingWith(termPrefix);

    /// <summary>
    /// Adds a new term to this grammar.
    /// If the start term isn't set, it will be set to this term.
    /// </summary>
    /// <param name="termName">The name of the term to create.</param>
    /// <returns>The newly created term.</returns>
    private Term add(string termName) {
        Term nt = new(this, termName);
        this.terms.Add(nt);
        this.StartTerm ??= nt;
        return nt;
    }

    /// <summary>Find the existing token in this grammar or add it if not found.</summary>
    /// <param name="tokenName">The token name to find or add.</param>
    /// <returns>The new or found token.</returns>
    public TokenItem Token(string tokenName) {
        tokenName = sanitizedName(tokenName, "token");
        TokenItem token = this.tokens.FindItemByName(tokenName);
        if (token is null) {
            token = new(tokenName);
            this.tokens.Add(token);
        }
        return token;
    }

    /// <summary>Find the existing prompt in this grammar or add it if not found.</summary>
    /// <param name="promptName">The prompt name to find or add.</param>
    /// <returns>The new or found prompt.</returns>
    public Prompt Prompt(string promptName) {
        promptName = sanitizedName(promptName, "prompt");
        Prompt prompt = this.prompts.FindItemByName(promptName);
        if (prompt is null) {
            prompt = new(promptName);
            this.prompts.Add(prompt);
        }
        return prompt;
    }

    /// <summary>
    /// Gets or adds a term for a set of rules to this grammar.
    /// If the start term isn't set, it will be set to this term.
    /// </summary>
    /// <param name="termName">The term name to find or add.</param>
    /// <returns>The new or found term.</returns>
    public Term Term(string termName) {
        termName = sanitizedName(termName, "term");
        Term nt = this.findTerm(termName);
        nt ??= this.add(termName);
        return nt;
    }

    /// <summary>Removes the given term from the list of terms.</summary>
    /// <remarks>
    /// This will not remove the term from any other places it is used meaning
    /// unless the term isn't used in any rule, this will cause the validator to fail.
    /// </remarks>
    /// <param name="term">The term to remove.</param>
    /// <returns>True if the term was removed, false otherwise.</returns>
    internal bool RemoveTerm(Term term) =>
        this.terms.Remove(term);

    /// <summary>
    /// Gets or adds a term for and starts a new rule for that term.
    /// If the start term isn't set, it will be set to this rule's term.
    /// </summary>
    /// <param name="termName">The term name for the new rule.</param>
    /// <returns>The new rule.</returns>
    public Rule NewRule(string termName) => this.Term(termName).NewRule();

    /// <summary>
    /// Adds a new term for a set of rules to this grammar.
    /// The name will be uniquely generated automatically.
    /// </summary>
    /// <param name="termNamePrefix">The prefix part to the name to generate.</param>
    /// <returns>The new term.</returns>
    internal Term AddRandomTerm(string termNamePrefix = null) {
        string prefix = (termNamePrefix?.Trim() ?? "") + "'";
        int maxValue = 0;
        foreach (Term term in this.findTermsStartingWith(prefix)) {
            if (int.TryParse(term.Name[prefix.Length..], out int value) && value > maxValue)
                maxValue = value;
        }
        return this.Term(prefix+maxValue);
    }

    /// <summary>Gets a string showing the whole language.</summary>
    /// <returns>The string for this grammar.</returns>
    public override string ToString() {
        StringBuilder buf = new();
        if (this.StartTerm is not null)
            buf.AppendLine("> "+this.StartTerm);
        foreach (Term term in this.Terms)
            buf.AppendLine(term.ToStringWithRules());
        return buf.ToString();
    }
}
