namespace ESLViewer.Models;

/// <summary>
/// Placeholder panel used in empty grid slots and top-level drop targets.
/// </summary>
public class EmptyPanelModel : PanelModel
{
    public EmptyPanelModel()
    {
        PanelType = PanelType.Empty;
    }
}

