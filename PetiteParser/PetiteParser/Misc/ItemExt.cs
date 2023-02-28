using PetiteParser.Grammar;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc;

/// <summary>A collection of extension methods for items.</summary>
static public class ItemExt {

    /// <summary>This finds the item in this collection by the given name.</summary>
    /// <typeparam name="T">The type of item to search for the names in.</typeparam>
    /// <param name="items">The collection of items to search within.</param>
    /// <param name="name">The name of the item to try to find.</param>
    /// <returns>The first item found with the given name or null if not found.</returns>
    static public T FindItemByName<T>(this IEnumerable<T> items, string name) where T : Item =>
        items.NotNull().FirstOrDefault(item => item.Name == name);

    /// <summary>This finds the items in this collection which start with a given name prefix.</summary>
    /// <typeparam name="T">The type of item to search for the names in.</typeparam>
    /// <param name="items">The collection of items to search within.</param>
    /// <param name="prefix">The name prefix of the items to select.</param>
    /// <returns>The items which have the given prefix in there name.</returns>
    static public IEnumerable<T> FindItemsStartingWith<T>(this IEnumerable<T> items, string prefix) where T : Item =>
        items.NotNull().Where(item => item.Name.StartsWith(prefix));

    /// <summary>This gets all the names from the given items.</summary>
    /// <typeparam name="T">The type of items to get the names for.</typeparam>
    /// <param name="items">The items to get the names from.</param>
    /// <returns>The names from the given items.</returns>
    static public IEnumerable<string> ToNames<T>(this IEnumerable<T> items) where T : Item =>
        items.NotNull().Select(item => item.Name);
}
