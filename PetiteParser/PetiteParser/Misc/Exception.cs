namespace PetiteParser.Misc {

    /// <summary>The exceptions for petite parser.</summary>
    public class Exception: System.Exception {

        /// <summary>Creates a new exception.</summary>
        /// <param name="message">The message for this exception.</param>
        public Exception(string message) : base(message) { }

        /// <summary>Creates a new exception.</summary>
        /// <param name="message">The message for this exception.</param>
        /// <param name="inner">The inner exception to this exception.</param>
        public Exception(string message, System.Exception inner) : base(message, inner) { }

        /// <summary>Adds additional key value pair of data to this exception.</summary>
        /// <param name="key">The key for the additional data.</param>
        /// <param name="value">The value for the additional data.</param>
        /// <returns>This exception so that these calls can be chained.</returns>
        public Exception With(string key, object value) {
            this.Data.Add(key, value);
            return this;
        }
    }
}
