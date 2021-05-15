using System.Collections.Generic;

namespace Examples.CodeColoring {
    public interface IColorer {

        public IEnumerable<Formatting> Colorize(params string[] input);


        public string ExampleCode { get; }
    }
}
