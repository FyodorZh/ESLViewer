using ESLViewer.Models;

namespace ESLViewer.Services;

public interface IPointParser
{
    List<PlotPoint> Parse(string raw);
}
