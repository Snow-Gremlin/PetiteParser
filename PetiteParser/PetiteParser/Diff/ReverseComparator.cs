namespace PetiteParser.Diff;

/// <summary>A comparator which reverses the indices.</summary>
sealed internal class ReverseComparator : IComparator {

    /// <summary>The comparator to reverse.</summary>
    private readonly IComparator comp;

    /// <summary>Creates a new comparator which reverses the indices.</summary>
    /// <param name="comp">The comparator to reverse.</param>
    public ReverseComparator(IComparator comp) => this.comp = comp;

    /// <summary>The length of the first source being compared.</summary>
    public int ALength => this.comp.ALength;

    /// <summary>The length of the second source being compared.</summary>
    public int BLength => this.comp.BLength;

    /// <summary>Determines the weight of the entries in the two given indices.</summary>
    /// <param name="aIndex">The index into the first source.</param>
    /// <param name="bIndex">The index into the second source.</param>
    /// <returns>The weight of the comparison between the two entries.</returns>
    public bool Equals(int aIndex, int bIndex) =>
        this.comp.Equals(this.ALength - 1 - aIndex, this.BLength - 1 - bIndex);

    /// <summary>Determines the cost to remove an entry from the first source at the given index.</summary>
    /// <param name="aIndex">The index in the first source of the removed entry.</param>
    /// <returns>The value greater than zero indicating how must cost removing this entry will incur.</returns>
    public int RemoveCost(int aIndex) =>
        this.comp.RemoveCost(this.ALength - 1 - aIndex);

    /// <summary>Determines the cost to add an entry from the second source at the given index.</summary>
    /// <param name="bIndex">The index in the second source of the add entry.</param>
    /// <returns>The value greater than zero indicating how must cost adding this entry will incur.</returns>
    public int AddCost(int bIndex) =>
        this.comp.AddCost(this.BLength - 1 - bIndex);

    /// <summary>
    /// Determines the cost of replacing an entry from the first source
    /// with an entry from the second source at the given indices.
    /// </summary>
    /// <param name="aIndex">The index in the first source to remove via replacement.</param>
    /// <param name="bIndex">The index in the second source to add via replacement.</param>
    /// <returns>This is the cost this replacement will incur.</returns>
    public int SubstitutionCost(int aIndex, int bIndex) =>
        this.comp.SubstitutionCost(this.ALength - 1 - aIndex, this.BLength - 1 - bIndex);
}
