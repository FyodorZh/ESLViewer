# ESLPlotter

## Project Overview

Interactive mathematical function plotter built with Blazor WebAssembly. Users can create multiple plot graphs, enter mathematical expressions, and visualize them interactively. Expressions are evaluated by a backend math server.

## Tech Stack

- **Blazor WebAssembly** (.NET 10.0)
- **ApexCharts** (Blazor-ApexCharts 6.1.0) for interactive charting
- **Bootstrap 5** for UI layout
- **C#** with nullable reference types enabled

## Architecture

### Component Hierarchy

```
PlotGrid → PlotPanel → ExpressionEditor
```

- **PlotGrid** (`Components/PlotGrid.razor`): manages the collection of plots and grid layout
- **PlotPanel** (`Components/PlotPanel.razor`): single plot with its ApexChart and expression list
- **ExpressionEditor** (`Components/ExpressionEditor.razor`): input row for a single expression

### Data Flow

1. User enters an expression in `ExpressionEditor`
2. `PlotPanel` calls `MathServerService.GetPointsAsync(expr)`
3. Service sends `GET /calculate?expr={expression}` to the backend
4. Response (format: `[(-2, 4),(-1,1),(0,0)]`) is parsed via regex into `List<PlotPoint>`
5. ApexChart renders the points

### Key Files

| File | Purpose |
|------|---------|
| `Components/PlotPanel.razor` | Main plotting component with chart |
| `Components/ExpressionEditor.razor` | Expression input UI |
| `Components/PlotGrid.razor` | Grid layout container |
| `Services/MathServerService.cs` | HTTP client for math API |
| `Models/ExpressionModel.cs` | Expression with color/label |
| `Models/PlotModel.cs` | Plot container |
| `Models/PlotPoint.cs` | X,Y coordinate record |
| `wwwroot/appsettings.json` | Backend server URL config |

## Build & Run

```bash
dotnet build       # Build
dotnet run         # Run dev server (http://localhost:5241)
dotnet publish -c Release  # Production build
```

The app requires a backend math server running at `http://localhost:3456/` (configurable in `wwwroot/appsettings.json` under `"ServerUrl"`).

Backend API endpoint: `GET /calculate?expr={expression}`
Expected response format: `[(-2, 4),(-1, 1),(0, 0),(1, 1),(2, 4)]`

## Features

- Multiple independent plot panels in a configurable grid (1–4 columns)
- Multiple expressions per plot, each with custom color and label
- Toggle expression visibility
- Editable plot titles
- Adjustable plot size (280–900px)
- Interactive charts (zoom, pan via ApexCharts)
