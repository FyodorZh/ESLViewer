using ESLViewer.Models.State;

namespace ESLViewer.Models;

/// <summary>
/// A panel that displays raw server text with configurable colors and auto-refresh.
/// </summary>
public class DashboardPanelModel : PanelModel
{
    public string Command { get; set; } = string.Empty;
    public string TextColor { get; set; } = "#1a1a2e";
    public string BackgroundColor { get; set; } = "#ffffff";
    public bool AutoRefreshEnabled { get; set; }
    public AutoRefreshInterval AutoRefreshInterval { get; set; } = AutoRefreshInterval.Seconds5;

    public DashboardPanelModel()
    {
        PanelType = PanelType.Dashboard;
        Title = "Dashboard";
    }

    public override PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = "Dashboard",
        CanBeClosed = CanBeClosed,
        DashboardCommand = Command,
        DashboardTextColor = TextColor,
        DashboardBackgroundColor = BackgroundColor,
        DashboardAutoRefreshEnabled = AutoRefreshEnabled,
        DashboardAutoRefreshInterval = AutoRefreshInterval.ToString(),
    };

    public static DashboardPanelModel FromDashboardSnapshot(PanelSnapshot snapshot) => new()
    {
        Title = snapshot.Title,
        CanBeClosed = snapshot.CanBeClosed,
        Command = snapshot.DashboardCommand ?? string.Empty,
        TextColor = snapshot.DashboardTextColor ?? "#1a1a2e",
        BackgroundColor = snapshot.DashboardBackgroundColor ?? "#ffffff",
        AutoRefreshEnabled = snapshot.DashboardAutoRefreshEnabled,
        AutoRefreshInterval = Enum.TryParse<AutoRefreshInterval>(snapshot.DashboardAutoRefreshInterval, out var iv)
            ? iv : AutoRefreshInterval.Seconds5,
    };
}

