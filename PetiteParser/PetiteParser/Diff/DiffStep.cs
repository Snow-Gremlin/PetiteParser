﻿using System.Collections.Generic;

namespace PetiteParser.Diff;

/// <summary>This is a step in the Levenshtein path.</summary>
/// <param name="Type">The type of the step being performed.</param>
/// <param name="Count">The number of steps of this type to perform.</param>
public readonly record struct DiffStep(StepType Type, int Count) {

    /// <summary>Gets the step type as a string.</summary>
    /// <param name="type">The type to get the string of.</param>
    /// <returns>The string of the given type.</returns>
    static public string ToString(StepType type) =>
         type switch {
             StepType.Equal   => "equal",
             StepType.Added   => "added",
             StepType.Removed => "removed",
             _                => "unknown",
         };

    /// <summary>This will simplify the output steps into the smallest set of steps.</summary>
    /// <param name="steps">The steps to simplify.</param>
    /// <returns>The simplified steps.</returns>
    static internal IEnumerable<DiffStep> Simplify(IEnumerable<DiffStep> steps) {
        int addedRun = 0, removedRun = 0, equalRun = 0;
        foreach (DiffStep step in steps) {
            if (step.Count > 0) {
                switch (step.Type) {

                    case StepType.Added:
                        if (equalRun > 0) {
                            yield return Equal(equalRun);
                            equalRun = 0;
                        }
                        addedRun += step.Count;
                        break;

                    case StepType.Removed:
                        if (equalRun > 0) {
                            yield return Equal(equalRun);
                            equalRun = 0;
                        }
                        removedRun += step.Count;
                        break;

                    case StepType.Equal:
                        if (removedRun > 0) {
                            yield return Removed(removedRun);
                            removedRun = 0;
                        }
                        if (addedRun > 0) {
                            yield return Added(addedRun);
                            addedRun = 0;
                        }
                        equalRun += step.Count;
                        break;
                }
            }
        }

        if (removedRun > 0) yield return Removed(removedRun);
        if (addedRun   > 0) yield return Added(addedRun);
        if (equalRun   > 0) yield return Equal(equalRun);
    }

    /// <summary>Creates a step which indicates that the first and second entries are equal.</summary>
    /// <param name="count">The number of steps to take.</param>
    /// <returns>The new equal step.</returns>
    static public DiffStep Equal(int count) => new(StepType.Equal, count);

    /// <summary>Creates a step which indicates the first entry was added.</summary>
    /// <param name="count">The number of steps to take.</param>
    /// <returns>The new added step.</returns>
    static public DiffStep Added(int count) => new(StepType.Added, count);

    /// <summary>Creates a step which indicates the first entry was removed.</summary>
    /// <param name="count">The number of steps to take.</param>
    /// <returns>The new removed step.</returns>
    static public DiffStep Removed(int count) => new(StepType.Removed, count);

    /// <summary>Indicates that the first and second entries are equal.</summary>
    public bool IsEqual => this.Type == StepType.Equal;

    /// <summary>Indicates the first entry was added.</summary>
    public bool IsAdded => this.Type == StepType.Added;

    /// <summary>Indicates the first entry was removed.</summary>
    public bool IsRemoved => this.Type == StepType.Removed;

    /// <summary>Gets a human readable string for this step.</summary>
    /// <returns>The debug string for this step.</returns>
    public override string ToString() => ToString(this.Type) + " " + this.Count;
}
