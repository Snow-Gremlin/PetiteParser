namespace PetiteParser.Grammar {

    /// <summary>An item is part of a term rule.</summary>
    public abstract class Item {

        /// <summary>Creates a new item.</summary>
        /// <param name="name">The name of the item.</param>
        protected Item(string name) {
            this.Name = name;
        }

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
    }
}
