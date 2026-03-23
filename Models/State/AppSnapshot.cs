namespace ESLViewer.Models.State;

/// <summary>
/// Represents the complete serializable application state.
/// The <see cref="Version"/> field is the primary hook for forward/backward compatibility.
/// Increment it when making breaking changes to the format.
/// Unknown extra fields in JSON are silently ignored (see StateService.Deserialize).
/// </summary>
[Serializable]
public class AppSnapshot
{
    public AppSnapshot() { }

    /// <summary>Format version. Current: "1.0".</summary>
    public string Version { get; set; } = "1.0";

    public int Columns { get; set; } = 2;
    public int PlotSize { get; set; } = 500;

    public List<PlotSnapshot> Plots { get; set; } = new();
}

