using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Matcher {

    /// <summary>A collection of group matchers.</summary>
    public class Group: IMatcher {

        /// <summary>Create a new group of matchers.</summary>
        /// <param name="matchers">The matchers for the group.</param>
        public Group(params IMatcher[] matchers) :
            this(matchers as IEnumerable<IMatcher>) { }

        /// <summary>Create a new group of matchers.</summary>
        /// <param name="matchers">The matchers for the group.</param>
        public Group(IEnumerable<IMatcher> matchers) {
            this.Matchers = new List<IMatcher>(matchers);
        }

        /// <summary>Gets the list of all matchers in the order they will be checked.</summary>
        public List<IMatcher> Matchers { get; }

        /// <summary>Determines if this matcher matches the given character.</summary>
        /// <param name="c">The character to match.</param>
        /// <returns>True if any of the contained matchers match, false otherwise.</returns>
        public virtual bool Match(Rune c) {
            foreach (IMatcher matcher in this.Matchers) {
                if (matcher.Match(c)) return true;
            }
            return false;
        }

        /// <summary>Adds a given matcher.</summary>
        /// <typeparam name="T">The matcher type that is being added.</typeparam>
        /// <param name="matcher">The matcher to add.</param>
        /// <returns>The newly added matcher.</returns>
        private T addMatcher<T>(T matcher) where T : IMatcher {
            this.Matchers.Add(matcher);
            return matcher;
        }

        /// <summary>Adds an all matcher.</summary>
        /// <returns>The newly added matcher.</returns>
        public All AddAll() => this.addMatcher(new All());

        /// <summary>Adds a new not matcher.</summary>
        /// <param name="matchers">The initial matchers.</param>
        /// <returns>The newly added matcher.</returns>
        public Not AddNot(params IMatcher[] matchers) => this.addMatcher(new Not(matchers));

        /// <summary>Adds a new not matcher.</summary>
        /// <param name="matchers">The initial matchers.</param>
        /// <returns>The newly added matcher.</returns>
        public Not AddNot(IEnumerable<IMatcher> matchers) => this.addMatcher(new Not(matchers));

        /// <summary>Adds a new range matcher.</summary>
        /// <param name="low">The lower rune inclusively in the range.</param>
        /// <param name="high">The higher rune inclusively in the range.</param>
        /// <returns>The newly added matcher.</returns>
        public Range AddRange(string low, string high) => this.addMatcher(new Range(low, high));

        /// <summary>Adds a new range matcher.</summary>
        /// <param name="low">The lower rune inclusively in the range.</param>
        /// <param name="high">The higher rune inclusively in the range.</param>
        /// <returns>The newly added matcher.</returns>
        public Range AddRange(char low, char high) => this.addMatcher(new Range(low, high));

        /// <summary>Adds a new range matcher.</summary>
        /// <param name="low">The lower rune inclusively in the range.</param>
        /// <param name="high">The higher rune inclusively in the range.</param>
        /// <returns>The newly added matcher.</returns>
        public Range AddRange(Rune low, Rune high) => this.addMatcher(new Range(low, high));

        /// <summary>Adds a set matcher for all the characters in the given string.</summary>
        /// <param name="set">The string containing all the runes to match.</param>
        /// <returns>The newly added matcher.</returns>
        public Set AddSet(string set) => this.addMatcher(new Set(set));

        /// <summary>Adds a set matcher for all the characters in the given characters.</summary>
        /// <param name="set">The set of characters to match.</param>
        /// <returns>The newly added matcher.</returns>
        public Set AddSet(params char[] set) => this.addMatcher(new Set(set));

        /// <summary>Adds a set matcher for all the characters in the given characters.</summary>
        /// <param name="set">The set of characters to match.</param>
        /// <returns>The newly added matcher.</returns>
        public Set AddSet(IEnumerable<char> set) => this.addMatcher(new Set(set));

        /// <summary>Adds a set matcher for all the characters in the given runes.</summary>
        /// <param name="set">The set of runes to match.</param>
        /// <returns>The newly added matcher.</returns>
        public Set AddSet(params Rune[] set) => this.addMatcher(new Set(set));

        /// <summary>Adds a set matcher for all the characters in the given runes.</summary>
        /// <param name="set">The set of runes to match.</param>
        /// <returns>The newly added matcher.</returns>
        public Set AddSet(IEnumerable<Rune> set) => this.addMatcher(new Set(set));

        /// <summary>Returns the string for this matcher.</summary>
        /// <returns>The string for this matcher.</returns>
        public override string ToString() =>
            string.Join(", ", this.Matchers.Select((IMatcher m) => m.ToString()));
    }
}
