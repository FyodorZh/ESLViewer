namespace ESLViewer.Models;

/// <summary>A single line in a ConsolePanel's output log.</summary>
/// <param name="Text">The displayed text.</param>
/// <param name="IsInput">True = typed command (shown with prompt); false = server response.</param>
public record ConsoleLine(string Text, bool IsInput);

