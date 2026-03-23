using ESLViewer.Models;

namespace ESLViewer.Services;

public interface IPointParser
{
    List<PanelPoint> Parse(string raw);
}
