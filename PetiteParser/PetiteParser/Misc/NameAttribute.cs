using System;

namespace PetiteParser.Misc;

/// <summary>An attribute to add a name to the property.</summary>
[AttributeUsage(AttributeTargets.All)]
sealed public class NameAttribute : Attribute {

    /// <summary>Creates a new name attribute.</summary>
    /// <param name="name">The name to assign to the property.</param>
    public NameAttribute(string name) => this.Name = name;

    /// <summary>The name set to the property.</summary>
    public string Name { get; }
}
