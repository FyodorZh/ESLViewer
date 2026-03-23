namespace ESLViewer.Models.State;

/// <summary>
/// Plain data-only snapshot of a single plot's persistable state.
/// This is the canonical extensibility point for per-plot persisted state.
/// Add new per-plot fields here as the save/restore format evolves and
/// increment <see cref="AppSnapshot.Version"/> for any breaking changes.
/// </summary>
[Serializable]
public class PlotSnapshot
{
    public PlotSnapshot() { }

    public string Title { get; set; } = string.Empty;

    /// <summary>Serialized name of <see cref="ESLViewer.Models.PlotType"/>.</summary>
    public string PlotType { get; set; } = "Numeric";

    public List<ExpressionSnapshot> Expressions { get; set; } = new();

    /// <summary>
    /// Last known visible axis range. Null means "auto" / not yet captured.
    /// </summary>
    public ViewRect? ViewRect { get; set; }
}

