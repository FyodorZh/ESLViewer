using System.Globalization;
using System.Text.RegularExpressions;
using ESLViewer.Models;

namespace ESLViewer.Services;

public class DateTimePointParser : IPointParser
{
    // Matches: ('dd.MM.yyyy HH:mm:ss', <float>)
    private static readonly Regex Regex = new(
        @"\('(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})'\s*,\s*([+-]?[0-9]*\.?[0-9]+(?:[eE][+-]?[0-9]+)?)\s*\)",
        RegexOptions.Compiled);

    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";

    public List<PanelPoint> Parse(string raw)
    {
        var result = new List<PanelPoint>();
        foreach (Match m in Regex.Matches(raw))
        {
            if (DateTime.TryParseExact(m.Groups[1].Value, DateFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt) &&
                double.TryParse(m.Groups[2].Value, NumberStyles.Float,
                    CultureInfo.InvariantCulture, out var y))
            {
                // Store Unix milliseconds as double so PanelPoint.X stays a single type
                var unixMs = (double)new DateTimeOffset(dt, TimeSpan.Zero).ToUnixTimeMilliseconds();
                result.Add(new PanelPoint(unixMs, y));
            }
        }
        return result;
    }
}
