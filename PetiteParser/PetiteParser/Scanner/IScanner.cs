using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Scanner {

    /// <summary>A scanner used to read input information for the scannar.</summary>
    public interface IScanner: IEnumerator<Rune> {

        /// <summary>Gets the location for the current character.</summary>
        Location Location { get; }
    }
}
