using System;

namespace PetiteParser.Loader {


    internal class Features {

        private string currentMode;

        public Features() {}

        public void SetMode(string mode) => this.currentMode = mode;

        public void Set(string key) =>
            throw new Exception("May not set feature " + key + " with " + this.currentMode + ".");

        public void Set(string key, string value) =>
            throw new Exception("May not set feature " + key + " to " + value + " with " + this.currentMode + ".");
    }
}
