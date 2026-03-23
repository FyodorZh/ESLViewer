# ESLViewer

## Project Overview

Interactive mathematical function plotter built with Blazor WebAssembly. Users can create multiple panels, enter mathematical expressions, and visualize them interactively. Expressions are evaluated by a backend math server.

## Tech Stack

- **Blazor WebAssembly** (.NET 10.0)
- **ApexCharts** (Blazor-ApexCharts 6.1.0) for interactive charting
- **Bootstrap 5** for UI layout
- **C#** with nullable reference types enabled

## Architecture

### Component Hierarchy

```
PanelGrid → Panel → ExpressionEditor
```

- **PanelGrid** (`Components/PanelGrid.razor`): manages the collection of panels and grid layout
- **Panel** (`Components/Panel.razor`): single panel with its ApexChart and expression list
- **ExpressionEditor** (`Components/ExpressionEditor.razor`): input row for a single expression

### Data Flow

1. User enters an expression in `ExpressionEditor`
2. `Panel` calls `MathServerService.EvaluateAsync(expr)`
3. Service sends `GET /invoke?script={expression}` to the backend
4. Response (format: `[(-2, 4),(-1,1),(0,0)]`) is parsed via regex into `List<PanelPoint>`
5. ApexChart renders the points

### Key Files

| File | Purpose |
|------|---------|
| `Components/Panel.razor` | Main panel component with chart |
| `Components/ExpressionEditor.razor` | Expression input UI |
| `Components/PanelGrid.razor` | Grid layout container |
| `Services/MathServerService.cs` | HTTP client for math API |
| `Models/ExpressionModel.cs` | Expression with color/label |
| `Models/PanelModel.cs` | Panel container |
| `Models/PanelPoint.cs` | X,Y coordinate record |
| `wwwroot/appsettings.json` | Backend server URL config |

## Build & Run

```bash
dotnet build       # Build
dotnet run         # Run dev server (http://localhost:5241)
dotnet publish -c Release  # Production build
```

The app requires a backend math server running at `http://localhost:3456/` (configurable in `wwwroot/appsettings.json` under `"ServerUrl"`).

Backend API endpoint: `GET /invoke?script={expression}`
Expected response format: `[(-2, 4),(-1, 1),(0, 0),(1, 1),(2, 4)]`

## Features

- Multiple independent panels in a configurable grid (1–4 columns)
- Multiple expressions per panel, each with custom color and label
- Toggle expression visibility
- Editable panel titles
- Adjustable panel size (280–900px)
- Interactive charts (zoom, pan via ApexCharts)
