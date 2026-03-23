namespace ESLViewer.Models.State;

/// <summary>
/// Plain data-only snapshot of a single panel's persistable state.
/// This is the canonical extensibility point for per-panel persisted state.
/// Add new per-panel fields here as the save/restore format evolves and
/// increment <see cref="AppSnapshot.Version"/> for any breaking changes.
/// </summary>
[Serializable]
public class PanelSnapshot
{
    public PanelSnapshot() { }

    public string Title { get; set; } = string.Empty;

    /// <summary>Serialized name of <see cref="ESLViewer.Models.PanelType"/>.</summary>
    public string PanelType { get; set; } = "Numeric";

    public List<ExpressionSnapshot> Expressions { get; set; } = new();

    /// <summary>
    /// Whether the close button is visible on an EmptyPanel. Defaults to true.
    /// </summary>
    public bool CanBeClosed { get; set; } = true;

    /// <summary>
    /// Last known visible axis range. Null means "auto" / not yet captured.
    /// </summary>
    public ViewRect? ViewRect { get; set; }

    // ── Grid-panel specific ──────────────────────────────────────────────────

    /// <summary>Number of columns in a Grid panel. Ignored for non-Grid panels.</summary>
    public int XSize { get; set; } = 1;

    /// <summary>Number of rows in a Grid panel. Ignored for non-Grid panels.</summary>
    public int YSize { get; set; } = 1;

    /// <summary>Snapshots of child cells (row-major order). Only used for Grid panels.</summary>
    public List<PanelSnapshot> GridCells { get; set; } = new();

    // ── Dashboard-panel specific ─────────────────────────────────────────────

    public string? DashboardCommand { get; set; }
    public string? DashboardTextColor { get; set; }
    public string? DashboardBackgroundColor { get; set; }
    public bool DashboardAutoRefreshEnabled { get; set; }
    public string? DashboardAutoRefreshInterval { get; set; }
}

