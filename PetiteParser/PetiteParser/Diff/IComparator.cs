using System.Linq;

namespace PetiteParser.Diff;

/// <summary>A simple interface for generic difference determination.</summary>
public interface IComparator {

    /// <summary>Gets the minimum value of the given values.</summary>
    /// <param name="values">The values to find the minimum within.</param>
    /// <returns>The minimum value from all the inputs.</returns>
    static internal int Min(params int[] values) => values.Min();

    /// <summary>The length of the first source being compared.</summary>
    public int ALength { get; }

    /// <summary>The length of the second source being compared.</summary>
    public int BLength { get; }

    /// <summary>Determines the weight of the entries in the two given indices.</summary>
    /// <param name="aIndex">The index into the first source.</param>
    /// <param name="bIndex">The index into the second source.</param>
    /// <returns>The weight of the comparison between the two entries.</returns>
    public bool Equals(int aIndex, int bIndex);

    /// <summary>Determines the cost to remove an entry from the first source at the given index.</summary>
    /// <param name="aIndex">The index in the first source of the removed entry.</param>
    /// <returns>The value greater than zero indicating how must cost removing this entry will incur.</returns>
    public int RemoveCost(int aIndex) => 1;

    /// <summary>Determines the cost to add an entry from the second source at the given index.</summary>
    /// <param name="bIndex">The index in the second source of the add entry.</param>
    /// <returns>The value greater than zero indicating how must cost adding this entry will incur.</returns>
    public int AddCost(int bIndex) => 1;

    /// <summary>
    /// Determines the cost of replacing an entry from the first source
    /// with an entry from the second source at the given indices.
    /// </summary>
    /// <remarks>Typically this will be the sum of the costs for the removal and adding.</remarks>
    /// <param name="aIndex">The index in the first source to remove via replacement.</param>
    /// <param name="bIndex">The index in the second source to add via replacement.</param>
    /// <returns>
    /// Zero if the entries at the indices are equal. If the entries are not equal then a value
    /// greater than zero indicating how much cost this replacement will incur.
    /// </returns>
    public int SubstitutionCost(int aIndex, int bIndex) => this.Equals(aIndex, bIndex) ? 0 : 2;
}
