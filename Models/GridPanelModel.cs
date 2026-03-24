using ESLViewer.Models.State;

namespace ESLViewer.Models;

/// <summary>
/// A panel that arranges nested panels in an XSize × YSize grid.
/// Cells are stored in row-major order: cell at (row, col) is at index row*XSize+col.
/// </summary>
public class GridPanelModel : PanelModel
{
    public int XSize { get; set; } = 1;
    public int YSize { get; set; } = 1;

    /// <summary>Row-major flat list of child panels.</summary>
    public List<PanelModel> Cells { get; set; } = new();

    /// <summary>
    /// Column width fractions (length == XSize). Empty means uniform.
    /// Stored as raw ratios; do not need to sum to 1 — the CSS fr unit normalizes them.
    /// </summary>
    public double[] ColumnFractions { get; set; } = Array.Empty<double>();

    /// <summary>Row height fractions (length == YSize). Empty means uniform.</summary>
    public double[] RowFractions { get; set; } = Array.Empty<double>();

    /// <summary>Returns effective column fractions, falling back to uniform if sizes mismatch.</summary>
    public double[] GetColumnFractions() =>
        ColumnFractions.Length == XSize ? ColumnFractions : Enumerable.Repeat(1.0, XSize).ToArray();

    /// <summary>Returns effective row fractions, falling back to uniform if sizes mismatch.</summary>
    public double[] GetRowFractions() =>
        RowFractions.Length == YSize ? RowFractions : Enumerable.Repeat(1.0, YSize).ToArray();

    public GridPanelModel()
    {
        PanelType = PanelType.Grid;
        Title = "Grid Panel";
        // Start with one empty (non-closable) cell
        Cells.Add(new PanelModel { PanelType = PanelType.Empty, CanBeClosed = false });
    }

    public PanelModel GetCell(int row, int col) => Cells[row * XSize + col];

    // ── Grid mutations ────────────────────────────────────────────────────────

    public void AddColumn()
    {
        var newCells = new List<PanelModel>();
        for (int row = 0; row < YSize; row++)
        {
            for (int col = 0; col < XSize; col++)
                newCells.Add(Cells[row * XSize + col]);
            newCells.Add(new PanelModel { PanelType = PanelType.Empty, CanBeClosed = false });
        }
        Cells = newCells;
        XSize++;
        // Append a new uniform fraction
        var fracs = GetColumnFractions().ToList();
        fracs.Add(fracs.Count > 0 ? fracs.Average() : 1.0);
        ColumnFractions = fracs.ToArray();
    }

    public void AddRow()
    {
        for (int col = 0; col < XSize; col++)
            Cells.Add(new PanelModel { PanelType = PanelType.Empty, CanBeClosed = false });
        YSize++;
        var fracs = GetRowFractions().ToList();
        fracs.Add(fracs.Count > 0 ? fracs.Average() : 1.0);
        RowFractions = fracs.ToArray();
    }

    public bool CanRemoveColumn()
    {
        if (XSize <= 1) return false;
        for (int row = 0; row < YSize; row++)
            if (GetCell(row, XSize - 1).PanelType != PanelType.Empty) return false;
        return true;
    }

    public bool CanRemoveRow()
    {
        if (YSize <= 1) return false;
        for (int col = 0; col < XSize; col++)
            if (GetCell(YSize - 1, col).PanelType != PanelType.Empty) return false;
        return true;
    }

    public void RemoveColumn()
    {
        if (!CanRemoveColumn()) return;
        var newCells = new List<PanelModel>();
        for (int row = 0; row < YSize; row++)
            for (int col = 0; col < XSize - 1; col++)
                newCells.Add(Cells[row * XSize + col]);
        Cells = newCells;
        XSize--;
        if (ColumnFractions.Length > XSize)
            ColumnFractions = ColumnFractions.Take(XSize).ToArray();
    }

    public void RemoveRow()
    {
        if (!CanRemoveRow()) return;
        Cells.RemoveRange(Cells.Count - XSize, XSize);
        YSize--;
        if (RowFractions.Length > YSize)
            RowFractions = RowFractions.Take(YSize).ToArray();
    }

    // ── Snapshot ──────────────────────────────────────────────────────────────

    public override PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = "Grid",
        CanBeClosed = CanBeClosed,
        MinWidth = MinWidth,
        MinHeight = MinHeight,
        XSize = XSize,
        YSize = YSize,
        ColumnFractions = GetColumnFractions(),
        RowFractions = GetRowFractions(),
        GridCells = Cells.Select(c => c.ToSnapshot()).ToList(),
        ViewRect = null,
    };

    public static GridPanelModel FromGridSnapshot(PanelSnapshot snapshot)
    {
        var model = new GridPanelModel
        {
            Title = snapshot.Title,
            CanBeClosed = snapshot.CanBeClosed,
            MinWidth = snapshot.MinWidth,
            MinHeight = snapshot.MinHeight,
        };
        model.Cells.Clear();
        model.XSize = Math.Max(1, snapshot.XSize);
        model.YSize = Math.Max(1, snapshot.YSize);

        if (snapshot.ColumnFractions is { Length: > 0 })
            model.ColumnFractions = snapshot.ColumnFractions;
        if (snapshot.RowFractions is { Length: > 0 })
            model.RowFractions = snapshot.RowFractions;

        foreach (var cellSnap in snapshot.GridCells)
            model.Cells.Add(PanelModel.FromSnapshot(cellSnap));

        // Pad with empty cells if snapshot is incomplete
        while (model.Cells.Count < model.XSize * model.YSize)
            model.Cells.Add(new PanelModel { PanelType = PanelType.Empty, CanBeClosed = false });

        return model;
    }
}

