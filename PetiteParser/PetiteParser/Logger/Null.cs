namespace PetiteParser.Logger {

    /// <summary>
    /// A null logger is a logger which doesn't actually log anything.
    /// However, it will keep track of if an error entry has been
    /// logged to indicate if there was a failure.
    /// </summary>
    sealed public class Null : BaseLog {

        /// <summary>Creates a new null logger.</summary>
        public Null() : base(null) { }
    }
}
