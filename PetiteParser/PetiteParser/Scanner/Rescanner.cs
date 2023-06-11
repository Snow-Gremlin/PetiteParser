using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Scanner;

/// <summary>A scanner designed to be able to step backwards some number of steps.</summary>
/// <remarks>
/// This was designed for the tokenizer so that characters can be stepped through,
/// when a token is found it is kept and then when reaching no way to continue the token will be returned,
/// any characters which were not part of that token needs to be pushed back onto the scanner and rescanned.
/// </remarks>
sealed public class Rescanner : IScanner {
    private readonly IScanner inner;
    private readonly List<Rune> scanned;
    private readonly List<Rune> rescan;
    private readonly List<Location?> curLocs;
    private readonly List<Location?> reLocs;

    /// <summary>Creates a new scanner which can be pushed back to a prior state.</summary>
    /// <param name="inner">The input to get the runes to tokenize.</param>
    public Rescanner(IScanner inner) {
        if (inner is null)
            throw new ScannerException("Must provide a non-null scanner");
        this.inner    = inner;
        this.Current  = this.inner.Current;
        this.Location = this.inner.Location;

        this.scanned = new();
        this.rescan  = new();
        this.curLocs = new();
        this.reLocs  = new();
    }

    /// <summary>The current location being processed.</summary>
    public Location? Location { get; private set; }

    /// <summary>The current character being processed.</summary>
    public Rune Current { get; private set; }

    /// <summary>The current character being processed.</summary>
    object IEnumerator.Current => this.Current;

    /// <summary>The characters which have been scanned but not pushed back.</summary>
    public IReadOnlyList<Rune> ScannedRunes => this.scanned;

    /// <summary>The number of characters which have been scanned and can be pushed back.</summary>
    public int ScannedCount => this.scanned.Count;

    /// <summary>The locations for each character which has been scanned but not pushed back.</summary>
    public IReadOnlyList<Location?> ScannedLocations => this.curLocs;

    /// <summary>The first character scanned since the last push back or from the beginning.</summary>
    public Rune StartRune => this.scanned.Count < 0 ? this.Current : this.scanned[0];

    /// <summary>The location of the first character scanned since the last push back or from the beginning.</summary>
    public Location? StartLocation => this.curLocs.Count <= 0 ? this.Location : this.curLocs[0];

    /// <summary>The number of characters which have been pushed back and not processed again.</summary>
    public int RescanCount => this.rescan.Count;

    /// <summary>Disposes this scanner and inner scanner.</summary>
    public void Dispose() {
        GC.SuppressFinalize(this);
        this.inner.Dispose();
        this.scanned.Clear();
        this.rescan.Clear();
        this.curLocs.Clear();
        this.reLocs.Clear();
    }

    /// <summary>Resets this scanner back to the beginning of the scan.</summary>
    public void Reset() {
        this.inner.Reset();
        this.scanned.Clear();
        this.rescan.Clear();
        this.curLocs.Clear();
        this.reLocs.Clear();
    }

    /// <summary>Advances the tokenizer to the next character and location in the scan.</summary>
    /// <remarks>If there are any characters which needed to be rescanned, those are pulled from first.</remarks>
    /// <returns>True if there was more characters, false if done reading.</returns>
    public bool MoveNext() {
        if (this.rescan.Count > 0) {
            this.Current = this.rescan[0];
            this.rescan.RemoveAt(0);

            this.Location = this.reLocs[0];
            this.reLocs.RemoveAt(0);
        } else {
            if (!this.inner.MoveNext()) return false;
            this.Current = this.inner.Current;
            this.Location = this.inner.Location;
        }
        this.scanned.Add(this.Current);
        this.curLocs.Add(this.Location);
        return true;
    }

    /// <summary>
    /// This will push the scanned characters back into the characters to process so that they can be scanned
    /// while skipping over the given number of characters to not rescan.
    /// </summary>
    /// <param name="skip">The number of characters to not rescan.</param>
    public void Rescan(int skip) {
        if (skip < 0 || skip > this.ScannedCount)
            throw new ScannerException("May not skip more characters than have been read since last push back " +
                "[count: " + this.ScannedCount + ", skip: " + skip + "]");

        this.scanned.RemoveRange(0, skip);
        this.rescan.AddRange(this.scanned);
        this.scanned.Clear();

        this.curLocs.RemoveRange(0, skip);
        this.reLocs.AddRange(this.curLocs);
        this.curLocs.Clear();
    }
}
