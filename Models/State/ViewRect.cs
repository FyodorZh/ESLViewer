namespace ESLViewer.Models.State;

/// <summary>
/// Represents the visible axis range of a chart.
/// All fields are nullable — null means "auto" / not yet captured.
/// </summary>
[Serializable]
public class ViewRect
{
    public ViewRect() { }

    public double? XMin { get; set; }
    public double? XMax { get; set; }
    public double? YMin { get; set; }
    public double? YMax { get; set; }
}

