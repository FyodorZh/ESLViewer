namespace ESLPlotter.Models;

public class ExpressionModel
{
    public string Expression { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Color { get; set; } = "#1b6ec2";
    public bool IsEnabled { get; set; } = true;
    public List<(double X, double Y)> Points { get; set; } = new();
}
