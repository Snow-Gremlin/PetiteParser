﻿using System.Collections.Generic;

namespace Examples.CodeColoring {

    /// <summary>The interface for a text colorizer.</summary>
    public interface IColorer {

        /// <summary>Returns the color formatting for the given input text.</summary>
        /// <param name="input">The input text to colorize.</param>
        /// <returns>The formatting to color the input with.</returns>
        public IEnumerable<Formatting> Colorize(params string[] input);

        /// <summary>Returns an example text which this will colorize.</summary>
        public string ExampleCode { get; }
    }
}