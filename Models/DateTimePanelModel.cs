using ESLViewer.Models.State;

namespace ESLViewer.Models;

public class DateTimePanelModel : GraphPanelModel
{
    public DateTimePanelModel()
    {
        PanelType = PanelType.DateTime;
        Title = "DateTime";
    }

    public static new DateTimePanelModel FromSnapshot(PanelSnapshot snapshot)
    {
        var model = new DateTimePanelModel();
        model.ApplyGraphSnapshot(snapshot);
        return model;
    }
}
