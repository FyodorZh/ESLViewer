namespace ESLViewer.Models.State;

/// <summary>Serializable snapshot of a single <see cref="Models.ConsoleLine"/>.</summary>
[Serializable]
public class ConsoleLineSnapshot
{
    public ConsoleLineSnapshot() { }

    public ConsoleLineSnapshot(string text, bool isInput)
    {
        Text = text;
        IsInput = isInput;
    }

    public string Text { get; set; } = string.Empty;
    public bool IsInput { get; set; }
}

