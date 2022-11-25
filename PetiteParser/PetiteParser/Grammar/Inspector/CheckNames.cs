using PetiteParser.Grammar;

namespace PetiteParser.Grammar.Inspector;

/// <summary>An inspector to check the names of the terms.</summary>
sealed internal class CheckNames : IInspector
{

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar.Grammar grammar, Logger.ILogger log)
    {
        foreach (Term item in grammar.Terms)
            checkName(item, "term", log);
        foreach (TokenItem item in grammar.Tokens)
            checkName(item, "token", log);
        foreach (Prompt item in grammar.Prompts)
            checkName(item, "prompt", log);
    }

    /// <summary>Checks the name of the given item.</summary>
    /// <param name="item">The item to check the name of.</param>
    /// <param name="itemType">The name of the item type to use for any errors.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    static private void checkName(Item item, string itemType, Logger.ILogger log)
    {
        if (string.IsNullOrWhiteSpace(item.Name))
            log.AddErrorF("There exists a {0} which has a whitespace or empty name.", itemType);
    }
}
