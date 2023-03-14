using PetiteParser.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Diff;

/// <summary>A container for the comparator used to determine subset of the data in the comparisons.</summary>
sealed internal class Subcomparator : IComparator {

    /// <summary>The comparator to get the sources from.</summary>
    private readonly IComparator comp;

    /// <summary>The index offset to the first sources.</summary>
    private readonly int aOffset;

    /// <summary>The index offset to the second sources.</summary>
    private readonly int bOffset;

    /// <summary>Creates a new sub-comparator.</summary>
    /// <param name="comp">The comparator to contain.</param>
    /// <param name="aOffset">The index offset for the first source.</param>
    /// <param name="aLength">The first source length.</param>
    /// <param name="bOffset">The index offset for the second source.</param>
    /// <param name="bLength">The second source length.</param>
    private Subcomparator(IComparator comp, int aOffset, int aLength, int bOffset, int bLength) {
        this.comp    = comp;
        this.aOffset = aOffset;
        this.ALength = aLength;
        this.bOffset = bOffset;
        this.BLength = bLength;
    }

    /// <summary>Creates a new container for all of the full lengths of the comparator sources.</summary>
    /// <param name="comp">The comparator to contain.</param>
    public Subcomparator(IComparator comp) :
        this(comp, 0, comp.ALength, 0,  comp.BLength) { }

    /// <summary>Creates a new sub-comparator for a subset relative to this container's settings.</summary>
    /// <param name="aLow">The lower of the first source index offsets relative to this container's settings.</param>
    /// <param name="aHigh">The higher of the first source index offsets relative to this container's settings.</param>
    /// <param name="bLow">The lower of the second source index offsets relative to this container's settings.</param>
    /// <param name="bHigh">The higher of the second source index offsets relative to this container's settings.</param>
    /// <returns>The new sub-comparator relative to this container's settings.</returns>
    public Subcomparator Sub(int aLow, int aHigh, int bLow, int bHigh) =>
        new(this.comp, this.aOffset+aLow, aHigh-aLow, this.bOffset+bLow, bHigh-bLow);

    /// <summary>Gets the reversed version of this subset comparator.</summary>
    public ReverseComparator Reversed => new(this);

    /// <summary>The part of the length of the first source being compared.</summary>
    public int ALength { get; }

    /// <summary>The part of the length of the second source being compared.</summary>
    public int BLength { get; }

    /// <summary>Determines the weight of the entries in the two given indices.</summary>
    /// <param name="aIndex">The index into the first source.</param>
    /// <param name="bIndex">The index into the second source.</param>
    /// <returns>The weight of the comparison between the two entries.</returns>
    public bool Equals(int aIndex, int bIndex) =>
        this.comp.Equals(this.aOffset + aIndex, this.bOffset + bIndex);

    /// <summary>Determines the cost to remove an entry from the first source at the given index.</summary>
    /// <param name="aIndex">The index in the first source of the removed entry.</param>
    /// <returns>The value from the contained comparator at the adjusted index.</returns>
    public int RemoveCost(int aIndex) =>
        this.comp.RemoveCost(this.aOffset + aIndex);

    /// <summary>Determines the cost to add an entry from the second source at the given index.</summary>
    /// <param name="bIndex">The index in the second source of the add entry.</param>
    /// <returns>The value from the contained comparator at the adjusted index.</returns>
    public int AddCost(int bIndex) =>
        this.comp.AddCost(this.bOffset + bIndex);

    /// <summary>
    /// Determines the cost of replacing an entry from the first source
    /// with an entry from the second source at the given indices.
    /// </summary>
    /// <param name="aIndex">The index in the first source to remove via replacement.</param>
    /// <param name="bIndex">The index in the second source to add via replacement.</param>
    /// <returns>The value from the contained comparator at the adjusted indices.</returns>
    public int SubstitionCost(int aIndex, int bIndex) =>
        this.comp.SubstitionCost(this.aOffset + aIndex, this.bOffset + bIndex);

    /// <summary>This ranges across all the values in the first range in order.</summary>
    /// <returns>The enumerable for the first range.</returns>
    private IEnumerable<int> aRange() {
        for (int i = 0, j = this.aOffset; i < this.ALength; i++, j++)
            yield return j;
    }

    /// <summary>This ranges across all the values in the second range in order.</summary>
    /// <returns>The enumerable for the second range.</returns>
    private IEnumerable<int> bRange() {
        for (int i = 0, j = this.bOffset; i < this.BLength; i++, j++)
            yield return j;
    }

    /// <summary>Determines how much of the front of the sources are equal.</summary>
    /// <param name="width">The maximum possible amount of matches before having to stop.</param>
    /// <returns>The number of equal matches from the front of the sources.</returns>
    private int matchFront(int width) {
        int i = this.aOffset;
        int j = this.bOffset;
        for (int front = 0; front < width; front++) {
            if (!this.comp.Equals(i, j)) return front;
            i++;
            j++;
        }
        return width;
    }

    /// <summary>Determines how much of the back of the sources are equal.</summary>
    /// <param name="width">The maximum possible amount of matches before having to stop.</param>
    /// <returns>The number of equal matches from the back of the sources.</returns>
    private int matchBack(int width) {
        int i = this.ALength - 1 + this.aOffset;
        int j = this.BLength - 1 + this.bOffset;
        for (int back = 0; back < width; back++) {
            if (!this.comp.Equals(i, j)) return back;
            i--;
            j--;
        }
        return width;
    }

    /// <summary>Reduce determines how much of the edges of this container are equal.</summary>
    /// <returns>
    /// The reduced sub-container,
    /// the amount of the sources' front the sub-container which are equal,
    /// and the amount of the sources' back the sub-container which are equal.
    /// </returns>
    public (Subcomparator, int, int) Reduce() {
        int max   = Math.Min(this.ALength, this.BLength);
        int front = this.matchFront(max);
        int back  = this.matchBack(max-front);

        Subcomparator sub = new(this.comp,
            this.aOffset+front, this.ALength-front-back,
            this.bOffset+front, this.BLength-front-back);

        return (sub, front, back);
    }

    /// <summary>This handles when at the edge of the first source subset in the given container.</summary>
    /// <returns>The enumerable for the first source edge steps.</returns>
    private IEnumerable<DiffStep> aEdge() {
        if (this.ALength <= 0) {
            yield return DiffStep.Added(this.BLength);
            yield break;
        }

        int split = -1;
        foreach (int j in this.bRange()) {
            if (this.comp.Equals(0, j)) {
                split = j;
                break;
            }
        }

        if (split < 0) {
            yield return DiffStep.Added(this.BLength);
            yield return DiffStep.Removed(1);
        } else {
            yield return DiffStep.Added(this.BLength - split - 1);
            yield return DiffStep.Equal(1);
            yield return DiffStep.Added(split);
        }
    }

    /// <summary>This handles when at the edge of the second source subset in the given container.</summary>
    /// <returns>The enumerable for the second source edge steps.</returns>
    private IEnumerable<DiffStep> bEdge() {
        if (this.BLength <= 0) {
            yield return DiffStep.Removed(this.ALength);
            yield break;
        }

        int split = -1;
        foreach (int i in this.aRange()) {
            if (this.comp.Equals(i, 0)) {
                split = i;
                break;
            }
        }

        if (split < 0) {
            yield return DiffStep.Added(1);
            yield return DiffStep.Removed(this.ALength);
        } else {
            yield return DiffStep.Removed(this.ALength - split - 1);
            yield return DiffStep.Equal(1);
            yield return DiffStep.Removed(split);
        }
    }

    /// <summary>
    /// Determines if the given container is small enough
    /// to be simply returned without any diff algorithm.
    /// </summary>
    public bool IsEndCase => this.ALength <= 1 || this.BLength <= 1;

    /// <summary>
    /// Gets all the steps for the given container when small enough
    /// to be simply returned without any diff algorithm.
    /// </summary>
    /// <returns>Returns an enumerable if done, if not done then null will be returned.</returns>
    public IEnumerable<DiffStep> EndCase() =>
        this.ALength <= 1 ? this.aEdge() :
        this.BLength <= 1 ? this.bEdge() :
        Enumerable.Empty<DiffStep>();

    /// <summary>The string for debugging the comparator.</summary>
    /// <remarks>This should not be used if the sources are huge.</remarks>
    /// <returns>The human readable debug string.</returns>
    public override string ToString() {
        string aValues = "", bValues = "";
        if (this.comp is Comparator<string> strcmp) {
            aValues = this.aRange().Select(i => strcmp.SourceA[i]).Join("|");
            bValues = this.bRange().Select(j => strcmp.SourceB[j]).Join("|");
        }
        return "(" + this.aOffset + ", " + this.ALength + " [" + aValues + "], " +
                     this.bOffset + ", " + this.BLength + " [" + bValues + "])";
    }
}
