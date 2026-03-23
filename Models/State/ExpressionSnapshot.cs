namespace ESLViewer.Models.State;

/// <summary>
/// Plain data-only snapshot of a single expression's persistable state.
/// No UI concerns, no computed/runtime data (Points are excluded).
/// System.Text.Json can round-trip this without custom converters.
/// </summary>
[Serializable]
public class ExpressionSnapshot
{
    public ExpressionSnapshot() { }

    public string Expression { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Color { get; set; } = "#1b6ec2";
    public bool IsEnabled { get; set; } = true;
}

