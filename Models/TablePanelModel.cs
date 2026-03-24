using ESLViewer.Models.State;

namespace ESLViewer.Models;

/// <summary>
/// A panel that displays server data as an HTML table.
/// Response format: [("col1","col2",...), ("row1_name", v1, v2,...), ...]
/// </summary>
public class TablePanelModel : PanelModel
{
    public string Command { get; set; } = string.Empty;
    public bool AutoRefreshEnabled { get; set; }
    public AutoRefreshInterval AutoRefreshInterval { get; set; } = AutoRefreshInterval.Seconds5;

    public TablePanelModel()
    {
        PanelType = PanelType.Table;
        Title = "Table";
    }

    public override PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = "Table",
        CanBeClosed = CanBeClosed,
        TableCommand = Command,
        TableAutoRefreshEnabled = AutoRefreshEnabled,
        TableAutoRefreshInterval = AutoRefreshInterval.ToString(),
    };

    public static TablePanelModel FromTableSnapshot(PanelSnapshot snapshot) => new()
    {
        Title = snapshot.Title,
        CanBeClosed = snapshot.CanBeClosed,
        Command = snapshot.TableCommand ?? string.Empty,
        AutoRefreshEnabled = snapshot.TableAutoRefreshEnabled,
        AutoRefreshInterval = Enum.TryParse<AutoRefreshInterval>(snapshot.TableAutoRefreshInterval, out var iv)
            ? iv : AutoRefreshInterval.Seconds5,
    };
}

