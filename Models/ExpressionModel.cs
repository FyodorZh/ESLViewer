using ESLViewer.Models.State;

namespace ESLViewer.Models;

public class ExpressionModel
{
    public string Expression { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Color { get; set; } = "#1b6ec2";
    public bool IsEnabled { get; set; } = true;
    public List<PanelPoint> Points { get; set; } = new();

    /// <summary>Creates a snapshot of the current persistable state. Does not include Points.</summary>
    public ExpressionSnapshot ToSnapshot() => new()
    {
        Expression = Expression,
        Label = Label,
        Color = Color,
        IsEnabled = IsEnabled,
    };

    /// <summary>
    /// Overwrites this instance's properties from the given snapshot.
    /// Points are cleared — data will be re-fetched after restore.
    /// </summary>
    public void ApplySnapshot(ExpressionSnapshot snapshot)
    {
        Expression = snapshot.Expression;
        Label = snapshot.Label;
        Color = snapshot.Color;
        IsEnabled = snapshot.IsEnabled;
        Points = new List<PanelPoint>();
    }
}
