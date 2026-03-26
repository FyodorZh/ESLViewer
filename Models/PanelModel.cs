using ESLViewer.Models.State;

namespace ESLViewer.Models;

public abstract class PanelModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public PanelType PanelType { get; set; }

    /// <summary>
    /// Controls whether the close (×) button is shown on an EmptyPanel.
    /// Has no effect on other panel types (they always show the close button).
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool CanBeClosed { get; set; } = true;

    /// <summary>Minimum width in pixels for resize constraints. Default 100.</summary>
    public int MinWidth { get; set; } = 100;

    /// <summary>Minimum height in pixels for resize constraints. Default 100.</summary>
    public int MinHeight { get; set; } = 100;

    /// <summary>
    /// Creates a snapshot of this panel's persistable state.
    /// ViewRect is left null here — it is populated asynchronously by
    /// Panel.CaptureSnapshotAsync() via JS interop.
    /// </summary>
    public virtual PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = PanelType.ToString(),
        CanBeClosed = CanBeClosed,
        MinWidth = MinWidth,
        MinHeight = MinHeight,
        ViewRect = null,
    };

    /// <summary>
    /// Constructs a concrete PanelModel from a snapshot.
    /// All expressions are restored with empty Points lists — data is re-fetched after restore.
    /// </summary>
    public static PanelModel FromSnapshot(PanelSnapshot snapshot) => snapshot.PanelType switch
    {
        "Grid"      => GridPanelModel.FromGridSnapshot(snapshot),
        "Dashboard" => DashboardPanelModel.FromDashboardSnapshot(snapshot),
        "Table"     => TablePanelModel.FromTableSnapshot(snapshot),
        "Console"   => ConsolePanelModel.FromConsoleSnapshot(snapshot),
        "Numeric"   => NumericPanelModel.FromSnapshot(snapshot),
        "DateTime"  => DateTimePanelModel.FromSnapshot(snapshot),
        "Empty"     => new EmptyPanelModel { CanBeClosed = snapshot.CanBeClosed },
        _           => NumericPanelModel.FromSnapshot(snapshot), // legacy fallback
    };
}
