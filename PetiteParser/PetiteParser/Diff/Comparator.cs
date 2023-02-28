using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Diff;

/// <summary>A list comparator to find the difference between them.</summary>
/// <typeparam name="T">This is the type of the elements to compare in the lists.</typeparam>
public class Comparator<T>: IComparator {

    /// <summary>The custom comparer to check equality of source entries.</summary>
    private readonly IEqualityComparer<T> comparer;

    /// <summary>Creates a new comparator for the two given lists.</summary>
    /// <param name="aSource">The first list (added).</param>
    /// <param name="bSource">The second list (removed).</param>
    /// <param name="comparer">
    /// The custom comparer for comparing source entries or
    /// null to used the default comparer.
    /// </param>
    public Comparator(IReadOnlyList<T> aSource, IReadOnlyList<T> bSource, IEqualityComparer<T> comparer = null) {
        this.SourceA  = aSource;
        this.SourceB  = bSource;
        this.comparer = comparer ?? EqualityComparer<T>.Default;
    }

    /// <summary>The length of the first list being compared.</summary>
    public int ALength => this.SourceA.Count;

    /// <summary>The length of the second list being compared.</summary>
    public int BLength => this.SourceB.Count;

    /// <summary>The first list (added).</summary>
    public readonly IReadOnlyList<T> SourceA;

    /// <summary>The second list (removed).</summary>
    public readonly IReadOnlyList<T> SourceB;

    /// <summary>Determines the weight of the entries in the two given indices.</summary>
    /// <param name="aIndex">The index into the first list.</param>
    /// <param name="bIndex">The index into the second list.</param>
    /// <returns>Zero if the values are equal and one (or the given scalar) if they are not equal</returns>
    public bool Equals(int aIndex, int bIndex) =>
        this.comparer.Equals(this.SourceA[aIndex], this.SourceB[bIndex]);

    /// <summary>The string for debugging the comparator.</summary>
    /// <remarks>This should not be used if the sources are huge.</remarks>
    /// <returns>The human readable debug string.</returns>
    public override string ToString() =>
        "(" + this.SourceA.Join("|") + ", " + this.SourceB.Join("|") + ")";
}
