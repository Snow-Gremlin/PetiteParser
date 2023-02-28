using System;
using System.Windows.Forms;

namespace ExamplesRunner;

/// <summary>Entry point for the example runner.</summary>
static public class EntryPoint {

    /// <summary>The main entry point for the application.</summary>
    [STAThread]
    static public void Main() {
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
