﻿using PetiteParser.Parser;
using System.Collections.Generic;

namespace PetiteParser.ParseTree;

/// <summary>The handler signature for a method to call for a specific prompt.</summary>
/// <param name="args">The argument for handling a prompt in the node tree.</param>
public delegate void PromptHandle(PromptArgs args);

/// <summary>The handler signature for a method to call for a specific prompt.</summary>
/// <param name="args">The argument for handling a prompt in the node tree.</param>
public delegate void PromptHandle<in T>(T args) where T : PromptArgs;

/// <summary>
/// The tree node containing reduced rule of the grammar
/// filled out with tokens and other TreeNodes.
/// </summary>
public interface ITreeNode {

    /// <summary>Creates a new exception for when a custom type prompt argument is null.</summary>
    /// <param name="prompt">The prompt which was failed to be found.</param>
    /// <returns>The new exception to throw.</returns>
    static private ParserException failedToFindException(string prompt) =>
        new("Failed to find the handle for the prompt \"" + prompt + "\".");

    /// <summary>Creates a new exception for when a custom type prompt argument is null.</summary>
    /// <returns>The new exception to throw.</returns>
    static private ParserException nullTypeArgsException() =>
        new("Must provide a non-null instance of a custom type prompt arguments.");

    /// <summary>Processes this tree node with the given handles for the prompts to call.</summary>
    /// <typeparam name="T">The type of prompt arguments that is being used.</typeparam>
    /// <param name="promptHandles">The set of handles for the prompt to call.</param>
    /// <param name="args">The argument of the given type to use when processing.</param>
    void Process<T>(Dictionary<string, PromptHandle<T>> promptHandles, T args) where T : PromptArgs {
        if (args is null) throw nullTypeArgsException();

        void innerHandle(PromptArgs args) {
            if (!promptHandles.TryGetValue(args.Prompt, out PromptHandle<T>? handle))
                throw failedToFindException(args.Prompt);
            if (args is T tArgs) handle(tArgs);
        }

        this.Process((PromptHandle)innerHandle, args);
    }

    /// <summary>Processes this tree node with the given handles for the prompts to call.</summary>
    /// <param name="promptHandles">The set of handles for the prompt to call.</param>
    /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
    void Process(Dictionary<string, PromptHandle> promptHandles, PromptArgs? args = null) {
        void innerHandle(PromptArgs args) {
            if (!promptHandles.TryGetValue(args.Prompt, out PromptHandle? handle))
                throw failedToFindException(args.Prompt);
            handle(args);
        }

        this.Process((PromptHandle)innerHandle, args);
    }

    /// <summary>Processes this tree node with the given handle for the prompts to call.</summary>
    /// <param name="promptHandle">The handler to call on each prompt.</param>
    /// <param name="args">The argument of the given type to use when processing.</param>
    void Process<T>(PromptHandle<T> promptHandle, T args) where T : PromptArgs {
        if (args is null) throw nullTypeArgsException();

        void innerHandle(PromptArgs args) {
            if (args is T tArgs) promptHandle(tArgs);
        }

        this.Process((PromptHandle)innerHandle, args);
    }

    /// <summary>Processes this tree node with the given handle for the prompts to call.</summary>
    /// <param name="promptHandle">The handler to call on each prompt.</param>
    /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
    void Process(PromptHandle promptHandle, PromptArgs? args = null);

    /// <summary>This returns this node and all inner items as an enumerable.</summary>
    IEnumerable<ITreeNode> Nodes { get; }
}
