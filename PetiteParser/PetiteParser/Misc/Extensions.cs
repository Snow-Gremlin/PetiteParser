using System.Collections.Generic;

namespace PetiteParser.Misc {

    /// <summary>Extensions to simplify lists and enumerable values.</summary>
    static public class Extensions {

        /// <summary>Combines a set of sets into a single set.</summary>
        /// <param name="input">The input combine together.</param>
        /// <returns>The combined result of the input sets.</returns>
        static public IEnumerable<T> Combine<T>(this IEnumerable<IEnumerable<T>> input) {
            foreach (IEnumerable<T> part in input)
                foreach (T value in part)
                    yield return value;
        }
    }
}
