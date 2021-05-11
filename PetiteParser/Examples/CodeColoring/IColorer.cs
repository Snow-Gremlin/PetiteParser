using System.Collections.Generic;

namespace Examples.CodeColoring {
    public interface IColorer {

        public string Name { get; }

        public IEnumerable<Formatted> Colorize(params string[] input);
    }
}
