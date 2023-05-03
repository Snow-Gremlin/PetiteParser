using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Loader;
using PetiteParser.Misc;
using System;
using TestPetiteParser.Tools;

namespace TestPetiteParser.PetiteParserTests.LoaderTests;

[TestClass]
sealed public class FeatureTests {

    sealed private class TestFeatures : Features {
        private int property1Value;

        [Name("field_one")]
        public string? Field1;

        [Name("field_two")]
        public bool Field2;

        [Name("field_three")]
        public int Field3;

        [Name("field_four")]
        public double Field4;

        [Name("property_one")]
        public int Property1 {
            get => this.property1Value;
            set => this.property1Value = Math.Clamp(value, 0, 10);
        }
    }

    [TestMethod]
    public void FeatureTest1() {
        TestFeatures features = new();
        Loader loader = new(features);

        features.Field1 = "";
        loader.Load("$set field_one \"cat\";");
        Assert.AreEqual("cat", features.Field1);

        features.Field1 = "";
        loader.Load("$set field_one \"42\";");
        Assert.AreEqual("42", features.Field1);

        features.Field1 = "";
        loader.Load("$set field_one \"true\";");
        Assert.AreEqual("true", features.Field1);

        TestTools.ThrowsException(() =>
            loader.Load("$enable field_one;"),
            "Error enabling a feature field_one: May not enable or disable a flag unless it is boolean.");
    }

    [TestMethod]
    public void FeatureTest2() {
        TestFeatures features = new();
        Loader loader = new(features);

        features.Field2 = false;
        loader.Load("$enable field_two;");
        Assert.IsTrue(features.Field2);

        features.Field2 = true;
        loader.Load("$disable field_two;");
        Assert.IsFalse(features.Field2);

        features.Field2 = false;
        loader.Load("$set field_two \"true\";");
        Assert.IsTrue(features.Field2);

        features.Field2 = true;
        loader.Load("$set field_two \"false\";");
        Assert.IsFalse(features.Field2);

        TestTools.ThrowsException(() =>
            loader.Load("$set field_two \"42\";"),
            "Error setting feature field_two: Unable to parse \"42\" into bool.");

        TestTools.ThrowsException(() =>
            loader.Load("$enable apple, orange, banana;"),
            "Unable to find the feature with the name, \"apple\".");
    }

    [TestMethod]
    public void FeatureTest3() {
        TestFeatures features = new();
        Loader loader = new(features);

        features.Field3 = 12;
        loader.Load("$set field_three \"42\";");
        Assert.AreEqual(42, features.Field3);

        TestTools.ThrowsException(() =>
            loader.Load("$set field_three \"cat\";"),
            "Error setting feature field_three: Unable to parse \"cat\" into int.");

        TestTools.ThrowsException(() =>
            loader.Load("$set apple \"42\";"),
            "Unable to find the feature with the name, \"apple\".");

        TestTools.ThrowsException(() =>
            loader.Load("$enable field_three;"),
            "Error enabling a feature field_three: May not enable or disable a flag unless it is boolean.");
    }

    [TestMethod]
    public void FeatureTest4() {
        TestFeatures features = new();
        Loader loader = new(features);

        features.Field4 = 10.0;
        loader.Load("$set field_four \"0\";");
        Assert.AreEqual(0.0, features.Field4);

        features.Field4 = 0.0;
        loader.Load("$set field_four \"3.14\";");
        Assert.AreEqual(3.14, features.Field4);

        features.Field4 = 0.0;
        loader.Load("$set field_four \"1.0e-9\";");
        Assert.AreEqual(1.0e-9, features.Field4);

        TestTools.ThrowsException(() =>
            loader.Load("$set field_four \"cat\";"),
            "Error setting feature field_four: Unable to parse \"cat\" into double.");

        TestTools.ThrowsException(() =>
            loader.Load("$enable field_four;"),
            "Error enabling a feature field_four: May not enable or disable a flag unless it is boolean.");
    }

    [TestMethod]
    public void FeatureTest5() {
        TestFeatures features = new();
        Loader loader = new(features);

        features.Property1 = 5;
        loader.Load("$set property_one \"42\";");
        Assert.AreEqual(10, features.Property1);

        features.Property1 = 5;
        loader.Load("$set property_one \"-10\";");
        Assert.AreEqual(0, features.Property1);

        features.Property1 = 7;
        loader.Load("$set property_one \"4\";");
        Assert.AreEqual(4, features.Property1);

        TestTools.ThrowsException(() =>
            loader.Load("$set property_one \"cat\";"),
            "Error setting feature property_one: Unable to parse \"cat\" into int.");

        TestTools.ThrowsException(() =>
            loader.Load("$enable property_one;"),
            "Error enabling a feature property_one: May not enable or disable a flag unless it is boolean.");
    }
}
