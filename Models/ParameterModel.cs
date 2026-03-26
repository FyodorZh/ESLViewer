using System.Globalization;
using ESLViewer.Models.State;

namespace ESLViewer.Models;

/// <summary>
/// A named numeric parameter whose value is controlled by a slider in ExpressionEditor.
/// Expressions can reference it using the syntax {Name} (e.g. {Param1}).
/// </summary>
public class ParameterModel
{
    public string Name { get; set; } = "Param";
    public double Min { get; set; } = 0;
    public double Max { get; set; } = 100;
    public double Value { get; set; } = 50;

    public ParameterSnapshot ToSnapshot() => new()
    {
        Name = Name,
        Min = Min,
        Max = Max,
        Value = Value,
    };

    public void ApplySnapshot(ParameterSnapshot snapshot)
    {
        Name = snapshot.Name;
        Min = snapshot.Min;
        Max = snapshot.Max;
        Value = snapshot.Value;
    }

    /// <summary>Returns the value formatted for substitution into expression strings.</summary>
    public string FormattedValue =>
        Value.ToString("G", CultureInfo.InvariantCulture);
}

