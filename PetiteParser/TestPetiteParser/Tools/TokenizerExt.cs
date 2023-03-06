using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using PetiteParser.Logger;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestPetiteParser.Tools;

static internal class TokenizerExt {
    
    /// <summary>Checks the tokenizer will tokenize the given input.</summary>
    static public void Check(this Tokenizer tok, string input, params string[] expected) =>
        tok.Tokenize(input).CheckTokens(expected);

    /// <summary>Checks the tokens match the given input.</summary>
    static public void CheckTokens(this IEnumerable<Token> tokens, params string[] expected) =>
        Assert.AreEqual(expected.JoinLines(), tokens.JoinLines().Trim());

    /// <summary>Checks the tokenizer will fail with the given input.</summary>
    static public void CheckError(this Tokenizer tok, string input, params string[] expected) {
        StringBuilder resultBuf = new();
        try {
            foreach (Token token in tok.Tokenize(new Writer(), input))
                resultBuf.AppendLine(token.ToString());
            Assert.Fail("Expected an exception but didn't get one.");
        } catch (Exception ex) {
            resultBuf.AppendLine(ex.Message);
        }
        TestTools.AreEqual(expected.JoinLines(), resultBuf.ToString().Trim());
    }
}
