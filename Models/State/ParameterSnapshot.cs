namespace ESLViewer.Models.State;

/// <summary>
/// Plain data-only snapshot of a single panel parameter's persistable state.
/// </summary>
[Serializable]
public class ParameterSnapshot
{
    public ParameterSnapshot() { }

    public string Name { get; set; } = "Param";
    public double Min { get; set; } = 0;
    public double Max { get; set; } = 100;
    public double Value { get; set; } = 50;
}

