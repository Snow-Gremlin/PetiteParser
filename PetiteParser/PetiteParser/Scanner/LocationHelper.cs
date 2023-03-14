using System.Text;

namespace PetiteParser.Scanner;

/// <summary>This tool is a helper object for scanners to helper keep track of locations.</summary>
sealed public class LocationHelper {

    /// <summary>The character used as line separators.</summary>
    static readonly public Rune NewLine = new('\n');

    /// <summary>The current name for the input data.</summary>
    /// <remarks>This can be set to a file path to set the name in the location of tokens.</remarks>
    public string Name { get; set; }

    /// <summary>The number of line separators from the beginning of the input.</summary>
    /// <remarks>This starts count with 1 for the first line.</remarks>
    public int LineNumber { get; set; }

    /// <summary>The offset since the last line separator.</summary>
    public int Column { get; set; }

    /// <summary>The offset from the beginning of the input.</summary>
    public int Index{ get; set; }

    /// <summary>Creates a new location helper.</summary>
    public LocationHelper() {
        this.Name = "";
        this.LineNumber = 1;
        this.Column = 0;
        this.Index = 0;
    }

    /// <summary>Creates the current location.</summary>
    public Location Location => new(this.Name, this.LineNumber, this.Column, this.Index);
    
    /// <summary>Steps location with the given rune.</summary>
    /// <param name="rune">The rune to step with.</param>
    public void Step(Rune rune) {
        this.Index++;
        this.Column++;
        if (rune == NewLine) {
            this.LineNumber++;
            this.Column = 0;
        }
    }

    /// <summary>Resets the location to the initial values.</summary>
    public void Reset() {
        this.LineNumber = 1;
        this.Column = 0;
        this.Index = 0;
    }
}
