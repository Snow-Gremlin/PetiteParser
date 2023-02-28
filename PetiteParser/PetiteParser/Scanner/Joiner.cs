using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Scanner;

/// <summary>A scanner for scanning through several scanners.</summary>
public class Joiner: IScanner {

    /// <summary>The scanners to scan through.</summary>
    private readonly IEnumerator<IScanner> scanners;

    /// <summary>The current scanner.</summary>
    private IScanner current;

    /// <summary>Creates a new scanner for joining other scanners.</summary>
    /// <param name="scanners">The scanners to scan through.</param>
    public Joiner(params IScanner[] scanners):
        this(scanners as IEnumerable<IScanner>) { }

    /// <summary>Creates a new scanner for joining other scanners.</summary>
    /// <param name="scanners">The scanners to scan through.</param>
    public Joiner(IEnumerable<IScanner> scanners) {
        this.scanners = scanners.GetEnumerator();
        this.current = null;
    }

    /// <summary>Moves to the next rune.</summary>
    /// <returns>True if there is another rune, false if at the end.</returns>
    public bool MoveNext() {
        while (true) {
            if (this.current is not null && this.current.MoveNext()) return true;
            if (!this.scanners.MoveNext()) return false;
            this.current = this.scanners.Current;
            this.current?.Reset();
        }
    }

    /// <summary>Resets the scan.</summary>
    public void Reset() => this.scanners.Reset();

    /// <summary>This disposes the scanners.</summary>
    public void Dispose() {
        this.scanners.Dispose();
        this.current = null;
        GC.SuppressFinalize(this);
    }

    /// <summary>Gets the current rune.</summary>
    public Rune Current => this.current?.Current ?? new Rune();

    /// <summary>Gets the current rune.</summary>
    object IEnumerator.Current => this.current?.Current ?? new Rune();

    /// <summary>Get the current location.</summary>
    public Location Location => this.current?.Location;
}
