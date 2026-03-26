using ESLViewer.Models.State;

namespace ESLViewer.Models;

public class NumericPanelModel : GraphPanelModel
{
    public NumericPanelModel()
    {
        PanelType = PanelType.Numeric;
        Title = "Numeric";
    }

    public static new NumericPanelModel FromSnapshot(PanelSnapshot snapshot)
    {
        var model = new NumericPanelModel();
        model.ApplyGraphSnapshot(snapshot);
        return model;
    }
}
