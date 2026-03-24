namespace ESLViewer.Models;

/// <summary>Parsed result of a table-format server response.</summary>
public class TableData
{
    public List<string> Columns { get; set; } = new();
    public List<TableRow> Rows { get; set; } = new();
    public bool IsEmpty => Columns.Count == 0 && Rows.Count == 0;
}

public class TableRow
{
    public string Name { get; set; } = string.Empty;
    /// <summary>Pre-formatted cell values (already trimmed strings).</summary>
    public List<string> Values { get; set; } = new();
}

