namespace ESLViewer.Models.State;

/// <summary>
/// Represents the complete serializable application state.
/// The <see cref="Version"/> field is the primary hook for forward/backward compatibility.
/// Increment it when making breaking changes to the format.
/// Unknown extra fields in JSON (e.g. legacy "Columns"/"PanelSize") are silently ignored.
/// </summary>
[Serializable]
public class AppSnapshot
{
    public AppSnapshot() { }

    /// <summary>Format version. Current: "1.2".</summary>
    public string Version { get; set; } = "1.2";

    public List<PanelSnapshot> Panels { get; set; } = new();

    /// <summary>User preferences (added in v1.2). Null means use defaults.</summary>
    public AppPreferences? Preferences { get; set; }
}
