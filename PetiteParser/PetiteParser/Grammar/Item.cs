using PetiteParser.Misc;
using System;

namespace PetiteParser.Grammar;

/// <summary>An item is part of a term rule.</summary>
public abstract class Item : IComparable<Item> {

    /// <summary>Determines if two items are equal.</summary>
    /// <param name="left">The left item in the comparison.</param>
    /// <param name="right">The right item in the comparison.</param>
    /// <returns>True if the two items are equal, false otherwise.</returns>
    public static bool operator ==(Item? left, Item? right) => CompOp.Equal(left, right);

    /// <summary>Determines if two items are not equal.</summary>
    /// <param name="left">The left item in the comparison.</param>
    /// <param name="right">The right item in the comparison.</param>
    /// <returns>True if the two items are not equal, false otherwise.</returns>
    public static bool operator !=(Item? left, Item? right) => CompOp.NotEqual(left, right);

    /// <summary>Determines if the left item is less than the right item.</summary>
    /// <param name="left">The left item in the comparison.</param>
    /// <param name="right">The right item in the comparison.</param>
    /// <returns>True if the left item is less than the right item, false otherwise.</returns>
    public static bool operator <(Item? left, Item? right) => CompOp.LessThan(left, right);

    /// <summary>Determines if the left item is less than or equal to the right item.</summary>
    /// <param name="left">The left item in the comparison.</param>
    /// <param name="right">The right item in the comparison.</param>
    /// <returns>True if the left item is less than or equal to the right item, false otherwise.</returns>
    public static bool operator <=(Item? left, Item? right) => CompOp.LessThanEqual(left, right);

    /// <summary>Determines if the left item is greater than the right item.</summary>
    /// <param name="left">The left item in the comparison.</param>
    /// <param name="right">The right item in the comparison.</param>
    /// <returns>True if the left item is greater than the right item, false otherwise.</returns>
    public static bool operator >(Item? left, Item? right) => CompOp.GreaterThan(left, right);

    /// <summary>Determines if the left item is greater than or equal to the right item.</summary>
    /// <param name="left">The left item in the comparison.</param>
    /// <param name="right">The right item in the comparison.</param>
    /// <returns>True if the left item is greater than or equal to the right item, false otherwise.</returns>
    public static bool operator >=(Item? left, Item? right) => CompOp.GreaterThanEqual(left, right);

    /// <summary>Creates a new item.</summary>
    /// <param name="name">The name of the item.</param>
    protected Item(string name) => this.Name = name;

    /// <summary>The name of the item.</summary>
    public string Name { get; }

    /// <summary>Gets the string for this item.</summary>
    /// <returns>The name of the item.</returns>
    public override string ToString() => this.Name;

    /// <summary>Gets the hash code for this item.</summary>
    /// <returns>The item's name's hash code.</returns>
    public override int GetHashCode() => this.ToString().GetHashCode();

    /// <summary>Determines if this item is equal to the given object.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equivalent, false otherwise.</returns>
    public override bool Equals(object? obj) =>
        obj is Item item && item.ToString() == this.ToString();

    /// <summary>Gets a value for the item type to use when comparing items.</summary>
    /// <param name="item">The item to get the comparable value from.</param>
    /// <returns>A value for the type of the given item.</returns>
    static private int typeOrderValue(Item item) =>
        item switch {
            Term      => 0,
            TokenItem => 1,
            Prompt    => 2,
            _         => throw new GrammarException("Unexpected item type, "+item.GetType()),
        };

    /// <summary>Compares this item against the given item.</summary>
    /// <param name="other">The other item to compare against.</param>
    /// <returns>
    /// Negative if this item is smaller than the given other,
    /// 0 if equal, 1 if this item is larger.
    /// </returns>
    public int CompareTo(Item? other) {
        if (other is null) return 1;
        int cmp = typeOrderValue(this) - typeOrderValue(other);
        return cmp != 0 ? cmp : string.CompareOrdinal(this.Name, other.Name);
    }
}
