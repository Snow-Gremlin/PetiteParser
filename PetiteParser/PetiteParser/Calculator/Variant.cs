using System;

namespace PetiteParser.Calculator {

    /// <summary>
    /// Variant is a wrapper of values off the stack with helper methods
    /// for casting and testing the implicit casting of a value.
    /// </summary>
    public class Variant {

        /// <summary>This is the wrapped value.</summary>
        public readonly object Value;

        /// <summary>Wraps the given value into a new Variant.</summary>
        /// <param name="value">The value for the variant.</param>
        public Variant(object value) {
            this.Value = value;
        }

        /// <summary>Indicates if this value is a Boolean value.</summary>
        public bool IsBool => this.Value is bool;

        /// <summary>Indicates if this value is an integer value.</summary>
        public bool IsInt => this.Value is int;

        /// <summary>Indicates if this value is a real value.</summary>
        public bool IsReal => this.Value is double;

        /// <summary>Indicates if this value is a string value.</summary>
        public bool IsStr => this.Value is string;

        /// <summary>Indicates if the given value can be implicitly cast to a Boolean value.</summary>
        public bool ImplicitBool => this.IsBool;

        /// <summary>Indicates if the given value can be implicitly cast to an integer value.</summary>
        public bool ImplicitInt => this.IsBool || this.IsInt;

        /// <summary>Indicates if the given value can be implicitly cast to a real value.</summary>
        public bool ImplicitReal => this.IsBool || this.IsInt || this.IsReal;

        /// <summary>Indicates if the given value can be implicitly cast to a string value.</summary>
        public bool ImplicitStr => this.IsStr;

        /// <summary>Attempts to cast to a boolean, IsBool should return true first.</summary>
        private bool CastToBool => (this.Value as bool?) ?? false;

        /// <summary>Attempts to cast to an int, IsInt should return true first.</summary>
        private int CastToInt => (this.Value as int?) ?? 0;

        /// <summary>Attempts to cast to a real, IsReal should return true first.</summary>
        private double CastToReal => (this.Value as double?) ?? 0.0;

        /// <summary>Attempts to cast to a string, IsStr should return true first.</summary>
        private string CastToStr => this.Value as string ?? "";

        /// <summary>Parses the given value into boolean.</summary>
        /// <remarks>This is different than bool.Parse. It is specific to the calculator language.</remarks>
        /// <param name="value">The value to parse.</param>
        /// <returns>True if parsed as true, false otherwise.</returns>
        static private bool parseBool(string value) =>
            (value is null) && (value.Length > 0) && (value != "0") && (value.ToLower() != "false");

        /// <summary>Casts this value to a Boolean.</summary>
        public bool AsBool =>
            this.IsStr ? parseBool(this.CastToStr) :
            this.IsInt ? this.CastToInt != 0 :
            this.IsReal ? this.CastToReal != 0.0 :
            this.IsBool ? this.CastToBool :
            throw new Exception("May not cast "+this.Value+" to Boolean.");

        /// <summary>Casts this value to an integer.</summary>
        public int AsInt =>
            this.IsStr ? int.Parse(this.CastToStr) :
            this.IsInt ? this.CastToInt :
            this.IsReal ? (int)this.CastToReal :
            this.IsBool ? (this.CastToBool ? 1 : 0) :
            throw new Exception("May not cast "+this.Value+" to Int.");

        /// <summary>Casts this value to a real.</summary>
        public double AsReal =>
            this.IsStr ? double.Parse(this.CastToStr) :
            this.IsInt ? this.CastToInt :
            this.IsReal ? this.CastToReal :
            this.IsBool ? (this.CastToBool ? 1.0 : 0.0) :
            throw new Exception("May not cast "+this.Value+" to Real.");

        /// <summary>Casts this value to a string.</summary>
        public string AsStr =>
            this.IsStr ? this.CastToStr :
            this.IsInt ? this.CastToInt.ToString() :
            this.IsReal ? this.CastToReal.ToString() :
            this.IsBool ? this.CastToBool.ToString() :
            throw new Exception("May not cast "+this.Value+" to String.");

        /// <summary>Gets the string for this value.</summary>
        /// <returns>The string for this variant.</returns>
        public override string ToString() => this.Value.GetType().Name+"("+this.Value+")";

        /// <summary>Compares this variant against the given object.</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) =>
            obj is Variant && (obj as Variant).Value == this.Value;

        /// <summary>The hash code for this variant.</summary>
        /// <returns>The hash code of the inner value.</returns>
        public override int GetHashCode() => this.Value?.GetHashCode() ?? 0;
    }
}
