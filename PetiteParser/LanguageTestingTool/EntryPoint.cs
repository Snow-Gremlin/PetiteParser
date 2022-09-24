namespace LanguageTestingTool;

internal static class EntryPoint {

    /// <summary>The main entry point for the language testing tool.</summary>
    [STAThread]
    static void Main() {
        ApplicationConfiguration.Initialize();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
