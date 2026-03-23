# AGENTS.md — ESLViewer

## Architecture

**Component hierarchy:** `Home.razor` → `PlotGrid` → `PlotPanel` → `ExpressionEditor`

- **PlotGrid** owns the `List<PlotModel>` and grid layout (columns, size slider).
- **PlotPanel** owns chart state, refresh logic, and auto-refresh timer. Passes `Plot.Expressions` (an `ObservableCollection<ExpressionModel>`) down to `ExpressionEditor`.
- **ExpressionEditor** is purely UI — it mutates the shared `ObservableCollection` directly and fires `OnRefresh`/`OnColorChanged` callbacks up to `PlotPanel`.

All state is in-memory; there is no persistence layer.

## Backend API

- Endpoint: `GET /invoke?script={url-encoded-expression}` (configured via `"ServerUrl"` in `wwwroot/appsettings.json`)
- Numeric response: `[(-2, 4),(-1, 1),(0, 0)]`
- DateTime response: `[('dd.MM.yyyy HH:mm:ss', <float>), ...]`

`MathServerService.EvaluateAsync` silently returns `[]` on any HTTP or parse error — callers never need to handle exceptions from it.

## PlotType / Parser Pattern

Two plot types: `PlotType.Numeric` and `PlotType.DateTime`. Each has a dedicated `IPointParser` implementation registered in `MathServerService._parsers`. To add a new type:
1. Add enum value to `Models/PlotType.cs`
2. Implement `IPointParser` in `Services/`
3. Register in the `_parsers` dictionary in `MathServerService`

**DateTime specifics:** `DateTimePointParser` converts parsed `DateTime` to Unix milliseconds stored as `double` in `PlotPoint.X`. The `ApexPointSeries` for DateTime plots casts `XValue` as `(long)p.X`; Numeric plots cast as `(decimal)p.X`. Keep these casts consistent when modifying chart series markup.

## Chart Rendering Gotchas

- **Resize** is handled by incrementing `_chartKey` (forces full ApexChart remount), not by calling a resize API.
- **`_chartReady` flag** prevents `RenderAsync()` from firing before ApexCharts finishes its own JS initialization. Set to `true` only after the first `OnAfterRenderAsync`.
- **`_needsChartRender` + immediate `RenderAsync()`**: `UpdateChart()` sets the flag for the `OnAfterRenderAsync` cycle *and* calls `_chart.RenderAsync()` immediately to handle the series-hide case. Both paths are intentional.
- **`_enabledExpressions`** filters to expressions where `IsEnabled == true && Points.Count > 0`; series with no data are never passed to ApexChart.

## Key Conventions

- `ExpressionModel.Label` is the series display name; falls back to `Expression` string if blank.
- `DefaultColors` in `ExpressionEditor` cycles via `Expressions.Count % DefaultColors.Length` — always append, never reshuffle.
- Pressing **Enter** (without Shift) in the textarea triggers a single-expression refresh; the **Refresh** button calls `OnRefresh(null)` which refreshes all expressions.
- `PlotPanel` implements `IAsyncDisposable` to cancel the auto-refresh `CancellationTokenSource`. Any new background task added to `PlotPanel` must be cancelled in `DisposeAsync`.

## Build & Config

```bash
dotnet build
dotnet run        # http://localhost:5241
```

Set the backend URL in `wwwroot/appsettings.json`:
```json
{ "ServerUrl": "http://localhost:3456/" }
```

There are no automated tests in this project.

