using System.Collections.Generic;

namespace PetiteParser.Diff {

    /// <summary>
    /// This is an interface for all diff algorithm configurations which can be used multiple times for different input.
    /// This can help reduce memory pressure by reusing already allocated buffers.
    /// </summary>
    internal interface IAlgorithm {

        /// <summary>
        /// NoResizeNeeded determines if the diff algorithm can handle a container with
        /// the amount of data inside of the given container.
        /// </summary>
        /// <param name="comp">The comparator containing the source data to check the size of.</param>
        /// <returns>False a larger matrix, cache, vector, or whatever would be created to perform the diff.</returns>
        public bool NoResizeNeeded(Subcomparator comp);

        /// <summary>Performs a diff and returns all the steps to traverse those steps.</summary>
        /// <param name="comp">The comparator containing the source data to diff.</param>
        /// <returns>The steps to take for the diff in reverse order.</returns>
        public IEnumerable<Step> Diff(Subcomparator comp);
    }
}
