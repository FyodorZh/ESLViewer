using System.Collections.ObjectModel;

namespace ESLViewer.Models;

public class PlotModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "New Plot";
    public PlotType PlotType { get; set; } = PlotType.Numeric;
    public ObservableCollection<ExpressionModel> Expressions { get; set; } = new();
}
