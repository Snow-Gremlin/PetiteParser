using System.Collections.Generic;
using System;

namespace PetiteParser.Diff;

/// <summary> This will perform a Wagner–Fischer on a given comparable.</summary>
/// <see cref="https://en.wikipedia.org/wiki/Wagner%E2%80%93Fischer_algorithm"/>
sealed internal class Wagner : IAlgorithm {

    /// <summary>The array to uses as a matrix.</summary>
    private int[] costs;

    /// <summary>This creates a new Wagner–Fischer diff algorithm.</summary>
    /// <param name="size">
    /// The given size is the amount of matrix space, width * height, to preallocate
    /// for the Wagner-Fischer algorithm. Use zero or less to not preallocate any matrix.
    /// </param>
    public Wagner(int size) {
        this.costs = Array.Empty<int>();
        this.allocateMatrix(size);
    }

    /// <summary>This will create the array to used for the costs matrix.</summary>
    /// <param name="size">
    /// The given size is the amount of matrix space, width * height, to preallocate
    /// for the Wagner-Fischer algorithm. Use zero or less to not preallocate any matrix.
    /// </param>
    private void allocateMatrix(int size) {
        if (size > 0) this.costs = new int[size];
    }

    /// <summary>Gets the current size of the cost matrix.</summary>
    private int costSize => this.costs?.Length ?? 0;

    /// <summary>
    /// Determines if the diff algorithm can handle a container with the amount of data
    /// inside of the given container.
    /// This algorithm's cost matrix will be auto-resize if needed so this method
    /// only indicates if the current matrix are large enough to not need reallocation.
    /// </summary>
    /// <param name="comp">The comparator containing the source data to check the size of.</param>
    /// <returns>False a larger matrix will be created to perform the diff.</returns>
    public bool NoResizeNeeded(Subcomparator comp) =>
        this.costSize >= comp.ALength*comp.BLength;

    /// <summary>Performs a diff and returns all the steps to traverse those steps.</summary>
    /// <param name="comp">The comparator containing the source data to diff.</param>
    /// <returns>The steps to take for the diff in reverse order.</returns>
    public IEnumerable<DiffStep> Diff(Subcomparator comp) {
        int size = comp.ALength*comp.BLength;
        if (this.costSize < size) this.allocateMatrix(size);
        this.setCosts(comp);
        return this.walkPath(comp);
    }

    /// <summary> 
    /// This will populate the part of the cost matrix which is needed by the given container.
    /// The costs are based off of the equality of parts in the comparable in the given container.
    /// </summary>
    /// <param name="comp">The comparator to use to fill out the costs.</param>
    private void setCosts(IComparator comp) {
        int aLen = comp.ALength;
        int bLen = comp.BLength;

        int start = comp.SubstitionCost(0, 0);
        this.costs[0] = start;

        for (int i = 1, value = start; i < aLen; i++) {
            value = IComparator.Min(value+1,
                i+comp.SubstitionCost(i, 0));
            this.costs[i] = value;
        }

        for (int j = 1, k = aLen, value = start; j < bLen; j++, k+=aLen) {
            value = IComparator.Min(value+1,
                j+comp.SubstitionCost(0, j));
            this.costs[k] = value;
        }

        for (int j = 1, k = aLen+1, k2 = 1, k3 = 0; j < bLen; j++, k++, k2++, k3++) {
            for (int i = 1, value = this.costs[k-1]; i < aLen; i++, k++, k2++, k3++) {
                value = IComparator.Min(value+1,
                    this.costs[k2]+1,
                    this.costs[k3]+comp.SubstitionCost(i, j));
                this.costs[k] = value;
            }
        }
    }

    /// <summary>
    /// This gets the cost value at the given indices.
    /// If the indices are out-of-bounds the edge cost will be returned.
    /// </summary>
    /// <param name="i">The index into the first source.</param>
    /// <param name="j">The index into the second source.</param>
    /// <param name="aLen">The length of the first source.</param>
    /// <returns>The cost at the given indices.</returns>
    private int getCost(int i, int j, int aLen) =>
        i < 0 ? j + 1 :
        j < 0 ? i + 1 :
        this.costs[i + j*aLen];

    /// <summary>
    /// This will walk through the cost matrix backwards to find the minimum Levenshtein path.
    /// The steps for this path are added to the given collector.
    /// </summary>
    /// <param name="comp">The comparator to use during the walk.</param>
    /// <returns>The steps to take for this path in reverse order.</returns>
    private IEnumerable<DiffStep> walkPath(IComparator comp) {
        int aLen = comp.ALength;
        int i = comp.ALength - 1;
        int j = comp.BLength - 1;
        while (i >= 0 && j >= 0) {
            int aCost = this.getCost(i-1, j,   aLen);
            int bCost = this.getCost(i,   j-1, aLen);
            int cCost = this.getCost(i-1, j-1, aLen);
            int minCost = IComparator.Min(aCost, bCost, cCost);

            Func<DiffStep[]>? curMove = null;
            if (aCost == minCost) {
                curMove = () => {
                    i--;
                    return new DiffStep[] { DiffStep.Removed(1) };
                };
            }

            if (bCost == minCost) {
                curMove = () => {
                    j--;
                    return new DiffStep[] { DiffStep.Added(1) };
                };
            }

            if (cCost == minCost) {
                if (comp.Equals(i, j)) {
                    curMove = () => {
                        i--;
                        j--;
                        return new DiffStep[] { DiffStep.Equal(1) };
                    };
                } else
                    curMove ??= () => {
                        i--;
                        j--;
                        return new DiffStep[] {
                                DiffStep.Added(1),
                                DiffStep.Removed(1),
                            };
                    };
            }

            if (curMove is null)
                throw new MissingMethodException("Failed to set current move while walking path in Wagner's algorithm.");

            foreach (DiffStep step in curMove())
                yield return step;
        }

        yield return DiffStep.Removed(i + 1);
        yield return DiffStep.Added(j + 1);
    }
}
