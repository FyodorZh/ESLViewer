using ESLViewer.Models;

namespace ESLViewer.Services;

public class MathServerService
{
    private readonly HttpClient _http;

    private static readonly Dictionary<PanelType, IPointParser> _parsers = new()
    {
        [PanelType.Numeric]  = new NumericPointParser(),
        [PanelType.DateTime] = new DateTimePointParser(),
    };

    public MathServerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<PanelPoint>> EvaluateAsync(string expression, PanelType panelType)
    {
        var encoded = Uri.EscapeDataString(expression);
        try
        {
            var response = await _http.GetStringAsync($"invoke?script={encoded}");
            return _parsers[panelType].Parse(response);
        }
        catch
        {
            return [];
        }
    }
}