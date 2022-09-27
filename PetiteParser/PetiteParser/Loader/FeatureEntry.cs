using PetiteParser.Misc;
using System;
using System.Reflection;

namespace PetiteParser.Loader;

/// <summary>This handles a field or property member for a feature.</summary>
sealed internal class FeatureEntry {

    /// <summary>Finds the feature member with the given name in the given target value.</summary>
    /// <param name="target">The target object to look for features within.</param>
    /// <param name="name">The name of the feature to find.</param>
    /// <returns>The feature entry which was found.</returns>
    static public FeatureEntry FindFeature(Features features, string name) {
        foreach (MemberInfo member in features.GetType().GetMembers()) {
            NameAttribute? attr = member.GetCustomAttribute<NameAttribute>();
            if (attr is not null && attr.Name == name) {
                if (member is FieldInfo field) return new FeatureEntry(features, name, field);
                if (member is PropertyInfo property) return new FeatureEntry(features, name, property);

                // Currently this only supports Fields and Properties because they can both be read from and written to.
                throw new LoaderException("Unexpected feature member type, " + member.MemberType + " for \"" + name + "\".");
            }
        }
        throw new LoaderException("Unable to find the feature with the name, \"" + name + "\".");
    }

    /// <summary>The features this entry came from.</summary>
    public readonly Features Features;

    /// <summary>The name of the feature which was found.</summary>
    public readonly string Name;

    /// <summary>The type of the value for this feature entry.</summary>
    public readonly Type ValueType;

    /// <summary>The method to get the value as an object from this feature entry.</summary>
    public readonly Func<object?> GetValue;

    /// <summary>The method to set the value with an object to this feature entry.</summary>
    public readonly Action<object?> SetValue;

    /// <summary>Creates a new feature entry for the given field member.</summary>
    /// <param name="features">The features this field came from.</param>
    /// <param name="name">The name of the feature.</param>
    /// <param name="field">The field for the feature.</param>
    public FeatureEntry(Features features, string name, FieldInfo field) {
        this.Features  = features;
        this.Name      = name;
        this.ValueType = field.FieldType;
        this.GetValue  = () => field.GetValue(this.Features);
        this.SetValue  = (object? value) => field.SetValue(this.Features, value);
    }

    /// <summary>Creates a new feature entry for the given property member.</summary>
    /// <param name="features">The features this property came from.</param>
    /// <param name="name">The name of the feature.</param>
    /// <param name="property">The property for the feature.</param>
    public FeatureEntry(Features features, string name, PropertyInfo property) {
        this.Features  = features;
        this.Name      = name;
        this.ValueType = property.PropertyType;
        this.GetValue  = () => property.GetValue(this.Features);
        this.SetValue  = (object? value) => property.SetValue(this.Features, value);
    }
}
