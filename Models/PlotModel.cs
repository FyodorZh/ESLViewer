using System.Collections.ObjectModel;
using ESLViewer.Models.State;

namespace ESLViewer.Models;

public class PlotModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "New Plot";
    public PlotType PlotType { get; set; } = PlotType.Numeric;
    public ObservableCollection<ExpressionModel> Expressions { get; set; } = new();

    /// <summary>
    /// Creates a snapshot of this plot's persistable state.
    /// ViewRect is left null here — it is populated asynchronously by
    /// PlotPanel.CaptureSnapshotAsync() via JS interop (Phase 3).
    /// </summary>
    public PlotSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PlotType = PlotType.ToString(),
        Expressions = Expressions.Select(e => e.ToSnapshot()).ToList(),
        ViewRect = null,
    };

    /// <summary>
    /// Constructs a new PlotModel from a snapshot.
    /// All expressions are restored with empty Points lists — data is re-fetched after restore.
    /// </summary>
    public static PlotModel FromSnapshot(PlotSnapshot snapshot)
    {
        var model = new PlotModel
        {
            Title = snapshot.Title,
            PlotType = Enum.TryParse<PlotType>(snapshot.PlotType, out var pt) ? pt : PlotType.Numeric,
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
