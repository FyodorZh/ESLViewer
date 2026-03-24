namespace ESLViewer.Models;

/// <summary>
/// User-configurable application preferences stored as part of the persistent state.
/// </summary>
public class AppPreferences
{
    /// <summary>
    /// Override for the math server base URL. When non-empty, takes precedence over
    /// the value from appsettings.json. Requires a page reload to take effect.
    /// </summary>
    public string ServerUrl { get; set; } = string.Empty;

    /// <summary>Default auto-refresh interval used when enabling auto-refresh on a new panel.</summary>
    public string DefaultAutoRefreshInterval { get; set; } = "Seconds10";

    /// <summary>Whether the expression editor is shown by default on new panels.</summary>
    public bool ShowExpressionEditorByDefault { get; set; } = true;
}

