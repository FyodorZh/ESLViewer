using System.Collections.ObjectModel;
using ESLViewer.Models.State;

namespace ESLViewer.Models;

public class PanelModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "New Panel";
    public PanelType PanelType { get; set; } = PanelType.Numeric;
    public ObservableCollection<ExpressionModel> Expressions { get; set; } = new();

    /// <summary>
    /// Controls whether the close (×) button is shown on an EmptyPanel.
    /// Has no effect on other panel types (they always show the close button).
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool CanBeClosed { get; set; } = true;

    /// <summary>
    /// Creates a snapshot of this panel's persistable state.
    /// ViewRect is left null here — it is populated asynchronously by
    /// Panel.CaptureSnapshotAsync() via JS interop.
    /// </summary>
    public PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = PanelType.ToString(),
        Expressions = Expressions.Select(e => e.ToSnapshot()).ToList(),
        CanBeClosed = CanBeClosed,
        ViewRect = null,
    };

    /// <summary>
    /// Constructs a new PanelModel from a snapshot.
    /// All expressions are restored with empty Points lists — data is re-fetched after restore.
    /// </summary>
    public static PanelModel FromSnapshot(PanelSnapshot snapshot)
    {
        var model = new PanelModel
        {
            Title = snapshot.Title,
            PanelType = Enum.TryParse<PanelType>(snapshot.PanelType, out var pt) ? pt : PanelType.Numeric,
            CanBeClosed = snapshot.CanBeClosed,
        };
        foreach (var exprSnap in snapshot.Expressions)
        {
            var expr = new ExpressionModel();
            expr.ApplySnapshot(exprSnap);
            model.Expressions.Add(expr);
        }
        return model;
    }
}
