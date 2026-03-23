using System.Globalization;
using System.Text.RegularExpressions;
using ESLViewer.Models;

namespace ESLViewer.Services;

public class NumericPointParser : IPointParser
{
    private static readonly Regex Regex = new(
        @"\(\s*([+-]?[0-9]*\.?[0-9]+(?:[eE][+-]?[0-9]+)?)\s*,\s*([+-]?[0-9]*\.?[0-9]+(?:[eE][+-]?[0-9]+)?)\s*\)",
        RegexOptions.Compiled);

    public List<PanelPoint> Parse(string raw)
    {
        var result = new List<PanelPoint>();
        foreach (Match m in Regex.Matches(raw))
        {
            if (double.TryParse(m.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var x) &&
                double.TryParse(m.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var y))
                result.Add(new PanelPoint(x, y));
        }
        return result;
    }
}
