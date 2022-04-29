namespace PetiteParser.Diff {

    /// <summary>The event arguments to indicate the progress of a diff.</summary>
    sealed public class ProgressEventArgs: System.EventArgs {

        /// <summary>The progress of a diff between zero and one where zero is just started and one is finished.</summary>
        readonly public double Progress;

        /// <summary>Creates a new progress event arguments.</summary>
        /// <param name="progress">The progress of a diff between zero and one.</param>
        public ProgressEventArgs(double progress) => this.Progress = progress;
    }
}
