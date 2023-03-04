namespace PetiteParser.Diff;

/// <summary>The types of steps which can be taken.</summary>
public enum StepType {

    /// <summary>This indicate to step forward both sources because the values are equal.</summary>
    Equal,

    /// <summary>
    /// This steps forward the second source and
    /// indicates that the second source has something added to it not in the first source.
    /// </summary>
    Added,

    /// <summary>
    /// This steps forward the first source and
    /// indicates that the second source has something removed from the first source.
    /// </summary>
    Removed
}
