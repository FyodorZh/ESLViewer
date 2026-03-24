using ESLViewer.Models;

namespace ESLViewer.Services;

/// <summary>
/// Singleton (scoped) service that tracks the panel currently being dragged.
/// Coordinates drag-and-drop swapping between any panel and empty slots.
/// </summary>
public class DragDropService
{
    /// <summary>The panel currently being dragged, or null if no drag is active.</summary>
    public PanelModel? DraggedPanel { get; private set; }

    private Func<Task>? _removeFromSource;

    public bool IsDragging => DraggedPanel is not null;

    /// <summary>
    /// Starts a drag operation.
    /// </summary>
    /// <param name="panel">The panel being dragged.</param>
    /// <param name="removeFromSource">
    /// Callback that removes the panel from its current position, replacing it with an empty slot.
    /// </param>
    public void StartDrag(PanelModel panel, Func<Task> removeFromSource)
    {
        DraggedPanel = panel;
        _removeFromSource = removeFromSource;
    }

    /// <summary>
    /// Completes a drop: removes the panel from its source position and returns it.
    /// Returns null if no drag is active.
    /// </summary>
    public async Task<PanelModel?> CompleteDrop()
    {
        if (DraggedPanel is null || _removeFromSource is null) return null;
        var panel = DraggedPanel;
        var remove = _removeFromSource;
        DraggedPanel = null;
        _removeFromSource = null;
        await remove();
        return panel;
    }

    /// <summary>Cancels the current drag without performing any swap.</summary>
    public void Cancel()
    {
        DraggedPanel = null;
        _removeFromSource = null;
    }
}

