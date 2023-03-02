namespace PetiteParser.Diff;

/// <summary>
/// This is the Hirschberg scores used for calculating the costs for two comparable sources
/// then finding the location to split the sources based on the cost.
/// </summary>
sealed internal class HirschbergScores {

    /// <summary>This is the score vector at the front of the score calculation.</summary>
    private int[] front;

    /// <summary>This is the score vector at the back of the score calculation.</summary>
    private int[] back;

    /// <summary>This is the score vector to store off a result vector to.</summary>
    private int[] other;

    /// <summary>This creates a new Hirschberg scores storage.</summary>
    /// <param name="length">
    /// This is the length to create the vectors. This must be one greater than
    /// the maximum second source length that will be passed into these scores.
    /// If zero or less then the vectors will have no size at first.
    /// </param>
    public HirschbergScores(int length) {
        this.front = System.Array.Empty<int>();
        this.back  = System.Array.Empty<int>();
        this.other = System.Array.Empty<int>();
        this.allocateVectors(length);
    }

    /// <summary>This will create the arrays used for the score vectors.</summary>
    /// <remarks>This has no effect if the length is zero or less.</remarks>
    /// <param name="length">The length to create the vectors.</param>
    private void allocateVectors(int length) {
        if (length < 0) length = 0;
        this.front = new int[length];
        this.back  = new int[length];
        this.other = new int[length];
    }

    /// <summary>The current length of the vectors.</summary>
    public int Length => this.back?.Length ?? 0;

    /// <summary>This swaps the front and back score vectors.</summary>
    private void swap() =>
        (this.front, this.back) = (this.back, this.front);

    /// <summary>This swaps the back and other score vectors.</summary>
    private void store() =>
        (this.other, this.back) = (this.back, this.other);

    /// <summary>
    /// This calculates the Needleman-Wunsch score.
    /// At the end of this calculation the score is in the back vector.
    /// </summary>
    /// <param name="cont">The comparator to calculate the scores with.</param>
    private void calculate(IComparator cont) {
	    int bLen = cont.BLength;
        if (this.Length < bLen+1)
            this.allocateVectors(bLen + 1);

        int aLen = cont.ALength;
        this.back[0] = 0;
        for (int j = 1; j <= bLen; j++)
            this.back[j] = this.back[j-1] + cont.AddCost(j-1);

	    for (int i = 1; i <= aLen; i++) {
            int removeCost = cont.RemoveCost(i-1);
            this.front[0] = this.back[0] + removeCost;
		    for (int j = 1; j <= bLen; j++) {
                this.front[j] = IComparator.Min(
                    this.back [j-1] + cont.SubstitionCost(i-1, j-1),
                    this.back [j]   + removeCost,
                    this.front[j-1] + cont.AddCost(j-1));
		    }

            this.swap();
	    }
    }

    /// <summary>
    /// This finds the pivot between the other score and the reverse of the back score.
    /// The pivot is the index of the maximum sum of each element in the two scores.
    /// </summary>
    /// <param name="bLength">The length of the second source and amount to find the pivot within.</param>
    /// <returns>The index of the pivot point.</returns>
    private int findPivot(int bLength) {
	    int index = 0;
	    int min = this.other[0] + this.back[bLength];
	    for (int j = 1; j <= bLength; j++) {
		    int value = this.other[j] + this.back[bLength-j];
		    if (value < min) {
			    min = value;
			    index = j;
		    }
	    }
	    return index;
    }

    /// <summary>This will find the first and second source mid points to split the container at.</summary>
    /// <param name="cont">The subset comparator to find the split mid points with.</param>
    /// <returns>The mid points to split the container at.</returns>
    public (int, int) Split(Subcomparator cont) {
        int aLen = cont.ALength;
        int bLen = cont.BLength;

	    int aMid = aLen / 2;
        this.calculate(cont.Sub(0, aMid, 0, bLen));
	    this.store();
        this.calculate(cont.Sub(aMid, aLen, 0, bLen).Reversed);
        int bMid = this.findPivot(bLen);

	    return (aMid, bMid);
    }
}
