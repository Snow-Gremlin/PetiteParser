using System;
using System.Collections.Generic;

namespace PetiteParser.ParseTree {

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
        /// <returns>The new exception to throw.</returns>
        static private Exception failedToFindException(string prompt) =>
            new("Failed to find the handle for the prompt \"" + prompt + "\".");

        /// <summary>Creates a new exception for when a custom type prompt argument is null.</summary>
        /// <returns>The new exception to throw.</returns>
        static private Exception nullTypeArgsException() =>
            new("Must provide a non-null instance of a custom type prompt arguments.");

        /// <summary>Processes this tree node with the given handles for the prompts to call.</summary>
        /// <typeparam name="T">The type of prompt arguments that is being used.</typeparam>
        /// <param name="handles">The set of handles for the prompt to call.</param>
        /// <param name="args">The argument of the given type to use when processing.</param>
        void Process<T>(Dictionary<string, PromptHandle<T>> handles, T args) where T : PromptArgs {
            if (args is null) throw nullTypeArgsException();
            void innerHandle(PromptArgs args) {
                if (!handles.TryGetValue(args.Prompt, out PromptHandle<T> hndl))
                    throw failedToFindException(args.Prompt);
                hndl(args as T);
            }
            this.Process((PromptHandle)innerHandle, args);
        }

        /// <summary>Processes this tree node with the given handles for the prompts to call.</summary>
        /// <param name="handles">The set of handles for the prompt to call.</param>
        /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
        void Process(Dictionary<string, PromptHandle> handles, PromptArgs args = null) {
            void innerHandle(PromptArgs args) {
                if (!handles.TryGetValue(args.Prompt, out PromptHandle hndl))
                    throw failedToFindException(args.Prompt);
                hndl(args);
            }
            this.Process((PromptHandle)innerHandle, args);
        }

        /// <summary>Processes this tree node with the given handle for the prompts to call.</summary>
        /// <param name="handle">The handler to call on each prompt.</param>
        /// <param name="args">The argument of the given type to use when processing.</param>
        void Process<T>(PromptHandle<T> handle, T args) where T : PromptArgs {
            if (args is null) throw nullTypeArgsException();
            void innerHandle(PromptArgs args) => handle(args as T);
            this.Process((PromptHandle)innerHandle, args);
        }

        /// <summary>Processes this tree node with the given handle for the prompts to call.</summary>
        /// <param name="handle">The handler to call on each prompt.</param>
        /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
        void Process(PromptHandle handle, PromptArgs args = null);

        /// <summary>This returns this node and all inner items as an enumerable.</summary>
        IEnumerable<ITreeNode> Nodes { get; }
    }
}
