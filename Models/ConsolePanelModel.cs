using ESLViewer.Models.State;

namespace ESLViewer.Models;

/// <summary>
/// A panel that behaves like a Linux console — user types commands, server processes them,
/// and results are displayed in a scrollable output area.
/// </summary>
public class ConsolePanelModel : PanelModel
{
    public ConsolePanelModel()
    {
        PanelType = PanelType.Console;
        Title = "Console";
    }

    /// <summary>Full console output log (input + output lines).</summary>
    public List<ConsoleLine> ConsoleLog { get; set; } = new();

    /// <summary>Command history entries (oldest first, newest at the end).</summary>
    public List<string> CommandHistory { get; set; } = new();

    /// <summary>The current in-progress input text.</summary>
    public string CurrentInput { get; set; } = string.Empty;

    /// <summary>Text appended to each command before sending (plus a newline separator).</summary>
    public string CommandPrefix { get; set; } = string.Empty;

    /// <summary>Whether the command-prefix editor strip is visible.</summary>
    public bool ShowCommandPrefix { get; set; }

    public override PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = "Console",
        CanBeClosed = CanBeClosed,
        MinWidth = MinWidth,
        MinHeight = MinHeight,
        ConsoleLog = ConsoleLog.Select(l => new ConsoleLineSnapshot(l.Text, l.IsInput)).ToList(),
        ConsoleHistory = CommandHistory.ToList(),
        ConsoleCurrentInput = CurrentInput,
        ConsoleCommandPrefix = CommandPrefix,
        ConsoleShowCommandPrefix = ShowCommandPrefix,
    };

    public static ConsolePanelModel FromConsoleSnapshot(PanelSnapshot snapshot) => new()
    {
        Title = snapshot.Title,
        CanBeClosed = snapshot.CanBeClosed,
        MinWidth = snapshot.MinWidth,
        MinHeight = snapshot.MinHeight,
        ConsoleLog = snapshot.ConsoleLog.Select(l => new ConsoleLine(l.Text, l.IsInput)).ToList(),
        CommandHistory = snapshot.ConsoleHistory.ToList(),
        CurrentInput = snapshot.ConsoleCurrentInput ?? string.Empty,
        CommandPrefix = snapshot.ConsoleCommandPrefix ?? string.Empty,
        ShowCommandPrefix = snapshot.ConsoleShowCommandPrefix,
    };
}

