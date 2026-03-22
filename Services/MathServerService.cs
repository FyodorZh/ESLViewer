using System.Text.RegularExpressions;
using ESLPlotter.Models;

namespace ESLPlotter.Services;

public class MathServerService
{
    private readonly HttpClient _http;

    public MathServerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PlotPoint>> EvaluateAsync(string expression)
    {
        var encoded = Uri.EscapeDataString(expression);
        try
        {
            var response = await _http.GetStringAsync($"calculate?expr={encoded}");
            return ParsePoints(response);
        }
        catch
        {
            return [];
        }
    }

    // Parses server response like: [(-2, 4),(-1,1),(0,0),(1,1),(2,4)]
    internal static List<PlotPoint> ParsePoints(string raw)
    {
        var result = new List<PlotPoint>();
        var matches = Regex.Matches(raw, @"\(\s*([+-]?[0-9]*\.?[0-9]+(?:[eE][+-]?[0-9]+)?)\s*,\s*([+-]?[0-9]*\.?[0-9]+(?:[eE][+-]?[0-9]+)?)\s*\)");
        foreach (Match m in matches)
        {
            if (double.TryParse(m.Groups[1].Value, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var x) &&
                double.TryParse(m.Groups[2].Value, System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, out var y))
            {
                result.Add(new PlotPoint(x, y));
            }
        }
        return result;
    }
}
