using System.Collections.ObjectModel;

namespace ESLViewer.Models;

public class PlotModel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; } = "New Plot";
    public ObservableCollection<ExpressionModel> Expressions { get; set; } = new();
}
