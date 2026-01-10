# Lab Data Acquisition System V3

**Version 3.0 - Modular Component Architecture**

## Overview

V3 is a complete refactoring of the Lab Data Acquisition System with a modern, modular architecture. Each UI component is separated into its own UserControl, making the codebase easier to maintain, extend, and collaborate on.

## What's New in V3

### Modular Component Architecture
- **7 Separate UserControls** in the `Views/` directory
- **Event-driven communication** between components
- **80% reduction** in MainWindow.xaml size (337 → 66 lines)
- **Independent editing** - work on one component without affecting others

### Components

1. **MenuBarControl** - Complete application menu system
   - File, Edit, View, Tools, Help menus
   - 20+ menu actions

2. **ToolBarControl** - Quick-access toolbar
   - Session management
   - View navigation
   - Acquisition controls

3. **DashboardControl** - Main control panel
   - Pressure transducer configuration
   - 4 thermocouple channels
   - 4 valve controls

4. **GraphsControl** - Real-time visualization
   - Pressure vs Time graph
   - Temperature vs Time graph

5. **DataTableControl** - Tabular data display
   - Export to CSV
   - Clear data

6. **CalibrationsControl** - Calibration management
   - New/Load/Save calibrations
   - Active calibrations display

7. **CsvLoggingControl** - CSV logging configuration
   - File path selection
   - Start/Stop logging

## Technical Details

### Framework
- **.NET 8.0** (Windows)
- **WPF** (Windows Presentation Foundation)
- **Visual Studio 2022** compatible

### Architecture Benefits
- ✅ **Modular** - Each component is self-contained
- ✅ **Maintainable** - Easier to locate and fix issues
- ✅ **Extensible** - Simple to add new features
- ✅ **Testable** - Components can be tested independently
- ✅ **Collaborative** - Multiple developers can work simultaneously

## How to Load and Run

### In Visual Studio
1. Open `PressureDataLoggerV3.sln` in Visual Studio 2022
2. Press **F5** to build and run
3. The application window will open

### From Command Line
```bash
cd PressureDataLoggerV3
dotnet build
dotnet run
```

## Project Structure

```
PressureDataLoggerV3/
├── Views/                           # Modular UI Components
│   ├── MenuBarControl.xaml/cs
│   ├── ToolBarControl.xaml/cs
│   ├── DashboardControl.xaml/cs
│   ├── GraphsControl.xaml/cs
│   ├── DataTableControl.xaml/cs
│   ├── CalibrationsControl.xaml/cs
│   └── CsvLoggingControl.xaml/cs
├── Models/                          # Data models
│   ├── AppConfiguration.cs
│   ├── PressureSample.cs
│   ├── TemperatureSample.cs
│   ├── ValveState.cs
│   └── CalibrationData.cs
├── Services/                        # Business logic
│   ├── DAQManager.cs
│   └── CSVLogger.cs
├── Interfaces/                      # Service interfaces
│   ├── IDAQManager.cs
│   └── ICSVLogger.cs
├── MainWindow.xaml                  # Main window (simplified)
├── MainWindow.xaml.cs               # Event coordination
└── App.xaml                         # Application entry point
```

## Editing Components

Each component can be edited independently:

### To Edit Menu Bar:
```
Open: Views/MenuBarControl.xaml
```

### To Edit Dashboard:
```
Open: Views/DashboardControl.xaml
```

### To Edit Graphs:
```
Open: Views/GraphsControl.xaml
```

All component files follow the same pattern:
- `.xaml` file for UI layout
- `.xaml.cs` file for event handling and logic

## Event-Driven Architecture

Components communicate with MainWindow through C# events:

```csharp
// Component raises event
public event EventHandler? PressureStartRequested;
private void BtnStart_Click(object sender, RoutedEventArgs e) 
    => PressureStartRequested?.Invoke(this, EventArgs.Empty);

// MainWindow handles event
dashboardControl.PressureStartRequested += (s, e) => BtnStart_Click(s, e);
```

This ensures:
- **Loose coupling** - Components don't depend on MainWindow
- **Easy testing** - Mock events for unit tests
- **Clear separation** - UI logic stays in components, business logic in MainWindow

## Comparison: V2 vs V3

| Feature | V2 | V3 |
|---------|----|----|
| MainWindow.xaml | 337 lines | 66 lines |
| Architecture | Monolithic | Modular |
| Component Separation | None | 7 UserControls |
| Framework | .NET 6.0 | .NET 8.0 |
| Editability | Hard to focus | Easy to focus |
| Maintainability | Difficult | Easy |
| Extensibility | Limited | High |

## Requirements

- Windows 10 or Windows 11
- Visual Studio 2022 (or .NET 8.0 SDK)
- No hardware required (runs in simulation mode)

## Features

All features from previous versions are preserved:
- ✅ Pressure transducer data acquisition
- ✅ 4-channel thermocouple temperature monitoring
- ✅ 4 valve controls
- ✅ Real-time data visualization
- ✅ CSV data logging
- ✅ Calibration management
- ✅ Session save/load
- ✅ Multiple export formats (CSV, JSON)

## Documentation

For detailed usage instructions, see:
- `USER_GUIDE.md` - Comprehensive user manual
- `HOW_TO_RUN.md` - Quick start guide
- `ARCHITECTURE.md` - System architecture details

## Version History

- **V3.0** (Current) - Modular component architecture
- **V2.0** - Multi-sensor support with dashboard
- **V1.0** - Initial pressure logger

## License

See LICENSE file for details.

---

**V3 is production-ready and fully functional. All components are independently editable and follow modern WPF best practices.**
