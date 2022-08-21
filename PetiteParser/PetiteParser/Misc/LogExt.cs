using PetiteParser.Log;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Misc {

    /// <summary>A collection of extension methods for logs.</summary>
    static public class LogExt {

        /// <summary>Gets the messages for the given entries.</summary>
        /// <param name="entries">The entries to get the messages from.</param>
        /// <returns>The messages from the given messages.</returns>
        static public IEnumerable<string> Messages(this IEnumerable<Entry> entries) =>
            entries.Select(e => e.Message);

        /// <summary>Gets the entries from the given entries which are at the given level.</summary>
        /// <param name="entries">The entries to filter by level.</param>
        /// <param name="level">The level to get the entries for.</param>
        /// <returns>The entries at the given level.</returns>
        static public IEnumerable<Entry> AtLevel(this IEnumerable<Entry> entries, Level level) =>
            entries.Where(e => e.Level == level);
    }
}
