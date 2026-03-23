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
    }

    public void AddRow()
    {
        for (int col = 0; col < XSize; col++)
            Cells.Add(new PanelModel { PanelType = PanelType.Empty, CanBeClosed = false });
        YSize++;
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
    }

    public void RemoveRow()
    {
        if (!CanRemoveRow()) return;
        Cells.RemoveRange(Cells.Count - XSize, XSize);
        YSize--;
    }

    // ── Snapshot ──────────────────────────────────────────────────────────────

    public override PanelSnapshot ToSnapshot() => new()
    {
        Title = Title,
        PanelType = "Grid",
        CanBeClosed = CanBeClosed,
        XSize = XSize,
        YSize = YSize,
        GridCells = Cells.Select(c => c.ToSnapshot()).ToList(),
        ViewRect = null,
    };

    public static GridPanelModel FromGridSnapshot(PanelSnapshot snapshot)
    {
        var model = new GridPanelModel
        {
            Title = snapshot.Title,
            CanBeClosed = snapshot.CanBeClosed,
        };
        model.Cells.Clear();
        model.XSize = Math.Max(1, snapshot.XSize);
        model.YSize = Math.Max(1, snapshot.YSize);

        foreach (var cellSnap in snapshot.GridCells)
            model.Cells.Add(PanelModel.FromSnapshot(cellSnap));

        // Pad with empty cells if snapshot is incomplete
        while (model.Cells.Count < model.XSize * model.YSize)
            model.Cells.Add(new PanelModel { PanelType = PanelType.Empty, CanBeClosed = false });

        return model;
    }
}

