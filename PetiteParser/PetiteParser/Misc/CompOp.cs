using System;

namespace PetiteParser.Misc;

/// <summary>Predefined null-aware methods for operator overloading.</summary>
static internal class CompOp {

    /// <summary>Determines if two objects are equal.</summary>
    /// <param name="left">The left object in the comparison.</param>
    /// <param name="right">The right object in the comparison.</param>
    /// <returns>True if the two objects are equal, false otherwise.</returns>
    public static bool Equal<T>(T? left, T? right)
        where T : class =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Determines if two objects are not equal.</summary>
    /// <param name="left">The left object in the comparison.</param>
    /// <param name="right">The right object in the comparison.</param>
    /// <returns>True if the two objects are not equal, false otherwise.</returns>
    public static bool NotEqual<T>(T? left, T? right)
        where T : class =>
        !Equal(left, right);

    /// <summary>Determines if the left object is less than the right object.</summary>
    /// <param name="left">The left object in the comparison.</param>
    /// <param name="right">The right object in the comparison.</param>
    /// <returns>True if the left object is less than the right object, false otherwise.</returns>
    public static bool LessThan<T>(T? left, T? right)
        where T : IComparable<T> =>
        left is null ? right is not null : left.CompareTo(right) < 0;

    /// <summary>Determines if the left object is less than or equal to the right object.</summary>
    /// <param name="left">The left object in the comparison.</param>
    /// <param name="right">The right object in the comparison.</param>
    /// <returns>True if the left object is less than or equal to the right object, false otherwise.</returns>
    public static bool LessThanEqual<T>(T? left, T? right)
        where T : IComparable<T> =>
        left is null || left.CompareTo(right) <= 0;

    /// <summary>Determines if the left object is greater than the right object.</summary>
    /// <param name="left">The left object in the comparison.</param>
    /// <param name="right">The right object in the comparison.</param>
    /// <returns>True if the left object is greater than the right object, false otherwise.</returns>
    public static bool GreaterThan<T>(T? left, T? right)
        where T : IComparable<T> =>
        left is not null && left.CompareTo(right) > 0;

    /// <summary>Determines if the left object is greater than or equal to the right object.</summary>
    /// <param name="left">The left object in the comparison.</param>
    /// <param name="right">The right object in the comparison.</param>
    /// <returns>True if the left object is greater than or equal to the right object, false otherwise.</returns>
    public static bool GreaterThanEqual<T>(T? left, T? right)
        where T : IComparable<T> =>
        left is null ? right is null : left.CompareTo(right) >= 0;
}
