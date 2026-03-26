using ESLViewer.Models.State;

namespace ESLViewer.Models;

/// <summary>
/// Abstract base for graph-based panels (Numeric and DateTime).
/// Holds the expression list and auto-refresh settings shared by both types.
/// </summary>
public abstract class GraphPanelModel : PanelModel
{
    public List<ExpressionModel> Expressions { get; set; } = new();
    public bool AutoRefreshEnabled { get; set; }
    public AutoRefreshInterval AutoRefreshInterval { get; set; } = AutoRefreshInterval.Seconds5;

    public override PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = PanelType.ToString(),
        Expressions = Expressions.Select(e => e.ToSnapshot()).ToList(),
        CanBeClosed = CanBeClosed,
        MinWidth = MinWidth,
        MinHeight = MinHeight,
        GraphAutoRefreshEnabled = AutoRefreshEnabled,
        GraphAutoRefreshInterval = AutoRefreshInterval.ToString(),
        ViewRect = null,
    };

    /// <summary>Applies common graph snapshot fields to this instance.</summary>
    protected void ApplyGraphSnapshot(PanelSnapshot snapshot)
    {
        Title = snapshot.Title;
        CanBeClosed = snapshot.CanBeClosed;
        MinWidth = snapshot.MinWidth;
        MinHeight = snapshot.MinHeight;
        AutoRefreshEnabled = snapshot.GraphAutoRefreshEnabled;
        AutoRefreshInterval = Enum.TryParse<AutoRefreshInterval>(snapshot.GraphAutoRefreshInterval, out var iv)
            ? iv : AutoRefreshInterval.Seconds5;
        foreach (var exprSnap in snapshot.Expressions)
        {
            var expr = new ExpressionModel();
            expr.ApplySnapshot(exprSnap);
            Expressions.Add(expr);
        }
    }
}

