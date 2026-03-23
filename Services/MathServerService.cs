using ESLViewer.Models;

namespace ESLViewer.Services;

public class MathServerService
{
    private readonly HttpClient _http;

    private static readonly Dictionary<PlotType, IPointParser> _parsers = new()
    {
        [PlotType.Numeric]  = new NumericPointParser(),
        [PlotType.DateTime] = new DateTimePointParser(),
    };

    public MathServerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PlotPoint>> EvaluateAsync(string expression, PlotType plotType)
    {
        var encoded = Uri.EscapeDataString(expression);
        try
        {
            var response = await _http.GetStringAsync($"invoke?script={encoded}");
            return _parsers[plotType].Parse(response);
        }
        catch
        {
            return [];
        }
    }
}