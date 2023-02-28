using System;

namespace PetiteParser.Grammar;

/// <summary>An item is part of a term rule.</summary>
public abstract class Item: IComparable<Item> {

    /// <summary>Creates a new item.</summary>
    /// <param name="name">The name of the item.</param>
    protected Item(string name) => this.Name = name;

    /// <summary>The name of the item.</summary>
    public readonly string Name;

    /// <summary>Gets the string for this item.</summary>
    /// <returns>The name of the item.</returns>
    public override string ToString() => this.Name;

    /// <summary>Gets the hash code for this item.</summary>
    /// <returns>The item's name's hash code.</returns>
    public override int GetHashCode() => this.ToString().GetHashCode();

    /// <summary>Determines if this item is equal to the given object.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equivalent, false otherwise.</returns>
    public override bool Equals(object obj) =>
        (obj is Item) && ((obj as Item).ToString() == this.ToString());

    /// <summary>Gets a value for the item type to use when comparing items.</summary>
    /// <param name="item">The item to get the comparable value from.</param>
    /// <returns>A value for the type of the given item.</returns>
    static private int typeOrderValue(Item item) =>
        item switch {
            Term      => 0,
            TokenItem => 1,
            Prompt    => 2,
            _         => throw new Exception("Unexpected item type, "+item.GetType()),
        };

    /// <summary>Compares this item against the given item.</summary>
    /// <param name="other">The other item to compare against.</param>
    /// <returns>
    /// Negative if this item is smaller than the given other,
    /// 0 if equal, 1 if this item is larger.
    /// </returns>
    public int CompareTo(Item other) {
        if (other is null) return 1;
        int cmp = typeOrderValue(this) - typeOrderValue(other);
        return cmp != 0 ? cmp : this.Name.CompareTo(other.Name);
    }
}
