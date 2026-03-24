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

    /// <param name="token">Optional token to cancel the HTTP request (e.g. when auto-refresh is stopped).</param>
    public async Task<List<PanelPoint>> EvaluateAsync(string expression, PanelType panelType,
        CancellationToken token = default)
    {
        var encoded = Uri.EscapeDataString(expression);
        try
        {
            var response = await _http.GetStringAsync($"invoke?script={encoded}", token);
            return _parsers[panelType].Parse(response);
        }
        catch
        {
            return [];
        }
    }

    /// <summary>
    /// Calls the backend and returns the raw response string unchanged.
    /// Returns an empty string on any HTTP or network error.
    /// </summary>
    public async Task<string> EvaluateRawAsync(string command)
    {
        var encoded = Uri.EscapeDataString(command);
        try
        {
            return await _http.GetStringAsync($"invoke?script={encoded}");
        }
        catch
        {
            return string.Empty;
        }
    }
}