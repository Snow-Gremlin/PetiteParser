using System.Collections.Generic;
using System;

namespace PetiteParser.Diff;

/// <summary>
/// This will perform a Hirschberg with an optional hybrid diff on a given comparable.
/// The base algorithm is a Hirschberg's algorithm used to divide the problem space until
/// the threshold is reached to switch to the hybrid (usually Wagner).
/// </summary>
/// <see cref="https://en.wikipedia.org/wiki/Hirschberg%27s_algorithm"/>
sealed internal class Hirschberg: IAlgorithm {
    private readonly HirschbergScores scores;
    private readonly IAlgorithm hybrid;

    /// <summary>This creates a new Hirschberg diff algorithm.</summary>
    /// <param name="length">
    /// The given length is the initial score vector size. If the vector is too small it will be
    /// reallocated to the larger size. Use zero or less to not preallocate the vectors.
    /// </param>
    /// <param name="hybrid">
    /// This allows for an optional diff to use when possible to hybrid the algorithm, to not use
    /// the optional diff pass in null. The hybrid is used if it has enough memory preallocated,
    /// (i.e. NoResizeNeeded returns true), otherwise Hirschberg will continue to divide the space
    /// until the hybrid can be used without causing it to reallocate memory.
    /// </param>
    public Hirschberg(int length, IAlgorithm hybrid = null) {
        this.scores = new HirschbergScores(length);
        this.hybrid = hybrid;
    }

    /// <summary>
    /// NoResizeNeeded determines if the diff algorithm can handle a container with
    /// the amount of data inside of the given container.
    /// </summary>
    /// <param name="comp">The comparator containing the source data to check the size of.</param>
    /// <returns>False a larger vector will be created to perform the diff.</returns>
    public bool NoResizeNeeded(Subcomparator comp) =>
        this.scores.Length >= comp.BLength + 1;

    /// <summary>Performs a diff and returns all the steps to traverse those steps.</summary>
    /// <param name="comp">The comparator containing the source data to diff.</param>
    /// <returns>The steps to take for the diff in reverse order.</returns>
    public IEnumerable<Step> Diff(Subcomparator comp) {
        Stack<Tuple<Subcomparator, int>> stack = new();
        stack.Push(new Tuple<Subcomparator, int>(comp, 0));

	    while (stack.Count > 0) {
		    Tuple<Subcomparator, int> pair = stack.Pop();
            Subcomparator cur = pair.Item1;
            int remainder = pair.Item2;

            if (remainder > 0) yield return Step.Equal(remainder);
            if (cur is null) continue;

            int before, after;
            (cur, before, after) = cur.Reduce();
            if (after > 0) yield return Step.Equal(after);
            stack.Push(new Tuple<Subcomparator, int>(null, before));

		    if (cur.IsEndCase) {
                foreach (Step step in cur.EndCase())
                    yield return step;
                continue;
		    }

		    if (this.hybrid is not null && this.hybrid.NoResizeNeeded(cur)) {
                foreach (Step step in this.hybrid.Diff(cur))
                    yield return step;
                continue;
		    }

            int aLen = cur.ALength;
            int bLen = cur.BLength;
            int aMid, bMid;
		    (aMid, bMid) = this.scores.Split(cur);
            stack.Push(new Tuple<Subcomparator, int>(cur.Sub(   0, aMid,    0, bMid), 0));
            stack.Push(new Tuple<Subcomparator, int>(cur.Sub(aMid, aLen, bMid, bLen), 0));
	    }
    }
}
