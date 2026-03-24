using System.Text;
using ESLViewer.Models;

namespace ESLViewer.Services;

/// <summary>
/// Parses the table-format server response:
/// [("col1","col2",...), ("rowName", 1.0, 2.0,...), ...]
/// </summary>
public static class TableDataParser
{
    public static TableData Parse(string response)
    {
        var result = new TableData();
        if (string.IsNullOrWhiteSpace(response)) return result;

        var tuples = ExtractTuples(response);

        for (int i = 0; i < tuples.Count; i++)
        {
            var items = SplitTuple(tuples[i]);
            if (items.Count == 0) continue;

            if (i == 0)
            {
                // First tuple → column headers
                result.Columns.AddRange(items.Select(StripQuotes));
            }
            else
            {
                // Subsequent tuples → data rows
                var row = new TableRow
                {
                    Name = StripQuotes(items[0]),
                    Values = items.Skip(1).Select(v => v.Trim()).ToList()
                };
                result.Rows.Add(row);
            }
        }

        return result;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>Extract the content of each top-level (…) group.</summary>
    private static List<string> ExtractTuples(string input)
    {
        var tuples = new List<string>();
        int depth = 0;
        int start = -1;
        bool inStr = false;
        char strChar = '"';

        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (!inStr && (c == '"' || c == '\''))
            {
                inStr = true;
                strChar = c;
            }
            else if (inStr && c == strChar)
            {
                inStr = false;
            }
            else if (!inStr && c == '(')
            {
                if (depth == 0) start = i + 1;
                depth++;
            }
            else if (!inStr && c == ')')
            {
                depth--;
                if (depth == 0 && start >= 0)
                {
                    tuples.Add(input[start..i]);
                    start = -1;
                }
            }
        }

        return tuples;
    }

    /// <summary>Split a tuple's content by commas, respecting quoted strings.</summary>
    private static List<string> SplitTuple(string tuple)
    {
        var items = new List<string>();
        var current = new StringBuilder();
        bool inStr = false;
        char strChar = '"';

        foreach (var c in tuple)
        {
            if (!inStr && (c == '"' || c == '\''))
            {
                inStr = true;
                strChar = c;
                current.Append(c);
            }
            else if (inStr && c == strChar)
            {
                inStr = false;
                current.Append(c);
            }
            else if (!inStr && c == ',')
            {
                items.Add(current.ToString().Trim());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        if (current.Length > 0)
            items.Add(current.ToString().Trim());

        return items;
    }

    private static string StripQuotes(string s)
    {
        s = s.Trim();
        if (s.Length >= 2 &&
            ((s[0] == '"' && s[^1] == '"') || (s[0] == '\'' && s[^1] == '\'')))
            return s[1..^1];
        return s;
    }
}

