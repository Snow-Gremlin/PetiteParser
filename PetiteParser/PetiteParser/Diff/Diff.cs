using PetiteParser.Formatting;
using System;
using System.Collections.Generic;

namespace PetiteParser.Diff;

/// <summary>A collection of methods to determine the differences between two sources.</summary>
sealed public class Diff {

    /// <summary>
    /// This is the default point at which the Hybrid algorithm can switch from Hirschberg to Wagner-Fischer.
    /// When both length of the comparable are smaller than this value Wagner-Fischer is used.
    /// The Wagner matrix will never be larger than this value of entries.
    /// </summary>
    public const int DefaultWagnerThreshold = 500;

    /// <summary>Creates a diff which only uses Hirschberg.</summary>
    /// <remarks>This is not the fastest algorithm but can be used if memory pressures are high.</remarks>
    /// <param name="length">
    /// The given length is the initial score vector size used by the Hirschberg algorithm.
    /// If the vector is too small it will be reallocated to the larger size.
    /// This should be set to the length of the second source plus one.
    /// Use negative to not preallocate the vectors.
    /// </param>
    /// <returns>The diff using the Hirschberg algorithm.</returns>
    static public Diff Hirschberg(int length = 0) => new(new Hirschberg(length));

    /// <summary>Creates a new Wagner-Fischer algorithm instance for performing a diff.</summary>
    /// <param name="size">
    /// The amount of matrix space, width * height, to preallocate for the Wagner-Fischer algorithm.
    /// Use zero or less to not preallocate any matrix.
    /// </param>
    /// <returns>The diff using the Wagner-Fischer algorithm.</returns>
    static public Diff Wagner(int size) => new(new Wagner(size));

    /// <summary>
    /// Creates a new hybrid Hirschberg with Wagner-Fischer cutoff for performing a diff.
    /// </summary>
    /// <param name="length"> 
    /// This is the initial score vector size of the Hirschberg algorithm. If the vector
    /// is too small it will be reallocated to the larger size. Use -1 to not preallocate the vectors.
    /// The useReduce flag indicates if the equal padding edges should be checked
    /// at each step of the algorithm or not.
    /// </param>
    /// <param name="size">
    /// The given size is the amount of matrix space, width * height, to use for the Wagner-Fischer.
    /// This must be greater than 4 to use the cutoff. The larger the size, the more memory is used
    /// creating the matrix but the earlier the Wagner-Fischer algorithm can take over.
    /// </param>
    /// <returns>The diff using the Hybrid algorithm.</returns>
    static public Diff Hybrid(int length, int size) => new(new Hirschberg(length, new Wagner(size)));

    /// <summary>
    /// Creates the default diff algorithm with default configuration.
    /// The default is a hybrid Hirschberg with Wagner-Fischer using
    /// a reduction at each step and the default Wagner threshold.
    /// </summary>
    /// <returns>The diff using the default Hybrid algorithm.</returns>
    static public Diff Default() => Hybrid(-1, DefaultWagnerThreshold);

    /// <summary>The amount of digits used to determine progress has changed.</summary>
    private const int progressDigits = 4;

    /// <summary>The diff algorithm to use.</summary>
    readonly private IAlgorithm algorithm;

    /// <summary>Indicates that the diff should cancel.</summary>
    private bool cancel;

    /// <summary>Creates a new diff using the given algorithm.</summary>
    /// <param name="algorithm">The algorithm that will perform the diff.</param>
    private Diff(IAlgorithm algorithm) => this.algorithm = algorithm;

    /// <summary>Creates a new default hybrid diff.</summary>
    public Diff() : this(Default().algorithm) { }

    /// <summary>This is emitted when a diff has started.</summary>
    public event EventHandler? Started;

    /// <summary>This is emitted when a diff has finished.</summary>
    public event EventHandler? Finished;

    /// <summary>This is emitted periodically while a diff is being worked on.</summary>
    /// <remarks>It will emit the progress between zero, just started, and one, finished.</remarks>
    public event EventHandler<ProgressEventArgs>? ProgressUpdated;

    /// <summary>Cancels running a diff.</summary>
    /// <remarks>
    /// Since the returned steps could just stop being iterated through to stop,
    /// this is to help when using something like ToArray or ToList.
    /// </remarks>
    public void Cancel() => this.cancel = true;

    /// <summary>Runs the diff algorithm.</summary>
    /// <param name="comp">The comparator with the data to diff.</param>
    /// <returns>The steps to take for the diff in reverse order and needing simplified.</returns>
    private IEnumerable<DiffStep> runAlgorithm(IComparator comp) {
        if (comp is null) yield break;
        SubComparator cont = new(new ReverseComparator(comp));

        int before, after;
        (cont, before, after) = cont.Reduce();
        if (after > 0) yield return DiffStep.Equal(after);
            
        foreach (DiffStep step in cont.IsEndCase ? cont.EndCase() : this.algorithm.Diff(cont))
            yield return step;

        if (before > 0) yield return DiffStep.Equal(before);
    }

    /// <summary>Watches the progress of the steps passing through and emits events.</summary>
    /// <param name="steps">The steps to watch the progress of.</param>
    /// <param name="comp">The comparator with the data that the steps are coming from.</param>
    /// <returns>The steps which have passed into this method.</returns>
    private IEnumerable<DiffStep> watchProgress(IEnumerable<DiffStep> steps, IComparator comp) {
        int total = comp.ALength+comp.BLength;
        int current = 0;
        double progress = 0.0;
        this.cancel = false;

        this.Started?.Invoke(this, EventArgs.Empty);

        if (!this.cancel)
            this.ProgressUpdated?.Invoke(this, new ProgressEventArgs(0.0));

        foreach (DiffStep step in steps) {
            if (this.cancel) break;
            if (step.IsEqual) current += step.Count*2;
            else              current += step.Count;

            double newProgress = Math.Round(current/(double)total, progressDigits);
            if (newProgress > progress) {
                progress = newProgress;
                this.ProgressUpdated?.Invoke(this, new ProgressEventArgs(progress));
            }

            yield return step;
        }

        if (!this.cancel)
            this.Finished?.Invoke(this, EventArgs.Empty);
    }

    #region Path

    /// <summary>Determines the difference path for the sources as defined by the given comparable.</summary>
    /// <param name="comp">The comparator to read the data from.</param>
    /// <returns>All the steps for the best path defining the difference.</returns>
    public IEnumerable<DiffStep> Path(IComparator comp) =>
        DiffStep.Simplify(this.watchProgress(this.runAlgorithm(comp), comp));

    /// <summary>Determines the difference path for the two given string lists.</summary>
    /// <typeparam name="T">This is the type of the elements to compare in the lists.</typeparam>
    /// <param name="aSource">The first list (added).</param>
    /// <param name="bSource">The second list (removed).</param>
    /// <param name="comparer">
    /// The custom comparer for comparing source entries or
    /// null to used the default comparer.
    /// </param>
    /// <returns>All the steps for the best path defining the difference.</returns>
    public IEnumerable<DiffStep> Path<T>(IReadOnlyList<T> aSource, IReadOnlyList<T> bSource, IEqualityComparer<T>? comparer = null) =>
        this.Path(new Comparator<T>(aSource, bSource, comparer));

    /// <summary>Gets the difference path for the lines in the given strings.</summary>
    /// <param name="aSource">The first multi-line string (added).</param>
    /// <param name="bSource">The second multi-line string (removed).</param>
    /// <returns>All the steps for the best path defining the difference.</returns>
    public IEnumerable<DiffStep> Path(string aSource, string bSource) =>
        this.Path(aSource.SplitLines(), bSource.SplitLines());

    #endregion
    #region PlusMinus

    /// <summary>Gets the labeled difference between the two list of lines.</summary>
    /// <param name="aSource">The first multi-line string (added).</param>
    /// <param name="bSource">The second multi-line string (removed).</param>
    /// <returns>
    /// The joined lines pre-pending a "+" to new lines in `bSource`,
    /// a "-" for any to removed strings from `aSource`, and space if the strings are the same.
    /// </returns>
    public string PlusMinus(string aSource, string bSource) =>
        this.PlusMinus(aSource.SplitLines(), bSource.SplitLines()).JoinLines();

    /// <summary>Gets the labeled difference between the two list of values.</summary>
    /// <param name="aSource">The first list of values (added).</param>
    /// <param name="bSource">The second list of values (removed).</param>
    /// <param name="comparer">
    /// The custom comparer for comparing source entries or
    /// null to used the default comparer.
    /// </param>
    /// <param name="equalPrefix">The string to prefix the equal values.</param>
    /// <param name="addedPrefix">The string to prefix the added values to bSoruce.</param>
    /// <param name="removedPrefix">The string to prefix the removed values to aSource.</param>
    /// <returns>
    /// The values as strings pre-pending the added prefix to new lines in `bSource`,
    /// the removed prefix for any to removed strings from `aSource`, and the equal prefix if the strings are the same.
    /// </returns>
    public IEnumerable<string> PlusMinus<T>(IReadOnlyList<T> aSource, IReadOnlyList<T> bSource,
        IEqualityComparer<T>? comparer = null,
        string equalPrefix   = " ",
        string addedPrefix   = "+",
        string removedPrefix = "-") {
        int aIndex = 0, bIndex = 0;
        foreach (DiffStep step in this.Path(aSource, bSource, comparer)) {
            switch (step.Type) {
                case StepType.Equal:
                    for (int i = step.Count-1; i >= 0; i--) {
                        yield return equalPrefix + aSource[aIndex];
                        aIndex++;
                        bIndex++;
                    }
                    break;

                case StepType.Added:
                    for (int i = step.Count-1; i >= 0; i--) {
                        yield return addedPrefix + bSource[bIndex];
                        bIndex++;
                    }
                    break;

                case StepType.Removed:
                    for (int i = step.Count-1; i >= 0; i--) {
                        yield return removedPrefix + aSource[aIndex];
                        aIndex++;
                    }
                    break;
            }
        }
    }

    #endregion
    #region Merge

    /// <summary>
    /// Merge gets the labeled difference between the two strings diff-ed by line
    /// using a similar output to the git merge conflict differences output.
    /// </summary>
    /// <param name="aSource">The first multi-line string (added).</param>
    /// <param name="bSource">The second multi-line string (removed).</param>
    /// <returns>
    /// The joined lines with added and removed ranges
    /// separated by lines with symbols similar to git's diff.
    /// </returns>
    public string Merge(string aSource, string bSource) =>
        this.Merge(aSource.SplitLines(), bSource.SplitLines()).JoinLines();

    /// <summary>
    /// Merge gets the labeled difference between the two strings diff-ed by line
    /// using a similar output to the git merge conflict differences output.
    /// </summary>
    /// <param name="aSource">The first list of values (added).</param>
    /// <param name="bSource">The second list of values (removed).</param>
    /// <param name="comparer">
    /// The custom comparer for comparing source entries or
    /// null to used the default comparer.
    /// </param>
    /// <param name="startChange">The line to put before a change starts.</param>
    /// <param name="middleChange">The line to put between added and removed changes.</param>
    /// <param name="endChange">The line to put after a change ends.</param>
    /// <returns>
    /// The joined lines with added and removed ranges
    /// separated by lines with symbols similar to git's diff.
    /// </returns>
    public IEnumerable<string> Merge<T>(IReadOnlyList<T> aSource, IReadOnlyList<T> bSource,
        IEqualityComparer<T>? comparer = null,
        string startChange  = "<<<<<<<<",
        string middleChange = "========",
        string endChange    = ">>>>>>>>") {
        int aIndex = 0, bIndex = 0;
        StepType prevState = StepType.Equal;
        foreach (DiffStep step in this.Path(aSource, bSource, comparer)) {
            switch (step.Type) {
                case StepType.Equal:
                    switch (prevState) {
                        case StepType.Added:
                            yield return endChange;
                            break;
                        case StepType.Removed:
                            yield return middleChange;
                            yield return endChange;
                            break;
                    }
                    for (int i = step.Count - 1; i >= 0; i--) {
                        yield return aSource[aIndex]?.ToString() ?? "";
                        aIndex++;
                        bIndex++;
                    }
                    break;

                case StepType.Added:
                    switch (prevState) {
                        case StepType.Equal:
                            yield return startChange;
                            yield return middleChange;
                            break;
                        case StepType.Removed:
                            yield return middleChange;
                            break;
                    }
                    for (int i = step.Count - 1; i >= 0; i--) {
                        yield return bSource[bIndex]?.ToString() ?? "";
                        bIndex++;
                    }
                    break;

                case StepType.Removed:
                    switch (prevState) {
                        case StepType.Equal:
                            yield return startChange;
                            break;
                        case StepType.Added:
                            yield return endChange;
                            yield return startChange;
                            break;
                    }
                    for (int i = step.Count - 1; i >= 0; i--) {
                        yield return aSource[aIndex]?.ToString() ?? "";
                        aIndex++;
                    }
                    break;
            }
            prevState = step.Type;
        }

        switch (prevState) {
            case StepType.Added:
                yield return endChange;
                break;
            case StepType.Removed:
                yield return middleChange;
                yield return endChange;
                break;
        }
    }

    #endregion
}
