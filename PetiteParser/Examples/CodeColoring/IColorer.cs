using System.Collections.Generic;

namespace Examples.CodeColoring {
    public interface IColorer {

        public IEnumerable<Formatted> Colorize(params string[] input);


        public string ExampleCode { get; }
    }
}
