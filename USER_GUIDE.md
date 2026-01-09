# Lab Data Acquisition System - User Guide

## Table of Contents
1. [Getting Started](#getting-started)
2. [User Interface Overview](#user-interface-overview)
3. [Working with Pressure Transducers](#working-with-pressure-transducers)
4. [Working with Thermocouples](#working-with-thermocouples)
5. [Valve Control](#valve-control)
6. [Data Visualization](#data-visualization)
7. [Data Export and Logging](#data-export-and-logging)
8. [Calibration Management](#calibration-management)
9. [Session Management](#session-management)
10. [Tips and Best Practices](#tips-and-best-practices)

---

## Getting Started

### First Launch

When you first launch the Lab Data Acquisition System, you'll see:
- A professional menu bar at the top
- A toolbar with quick-access buttons
- The Dashboard tab showing all sensor controls
- A status bar at the bottom

### Quick Start - 3 Steps

1. **Configure Your Sensors**
   - Enter device/channel information in the Dashboard
   - Set appropriate sample rates

2. **Start Acquisition**
   - Click the green "Start" buttons for pressure or temperature
   - Or use the toolbar "Start Acquisition" button

3. **View Your Data**
   - Real-time values appear in colored display boxes
   - Switch to the "Data Table" tab to see all samples
   - Switch to "Graphs" tab for visual representation

---

## User Interface Overview

### Menu Bar

#### File Menu
- **New Session**: Start fresh, clearing all data
- **Open Session...**: Load a previously saved session
- **Save Session**: Save current configuration and data
- **Save Session As...**: Save with a new filename
- **Export Data**: 
  - Export to CSV...
  - Export to Excel... (coming soon)
  - Export to JSON...
- **Exit**: Close the application

#### Edit Menu
- **Configuration...**: Jump to configuration panel
- **Clear Data**: Remove all acquired data

#### View Menu
- **Dashboard**: Main control panel (default view)
- **Graphs**: Real-time chart visualizations
- **Data Table**: Tabular view of all data
- **Calibrations**: Calibration management
- **Show Toolbar**: Toggle toolbar visibility
- **Show Status Bar**: Toggle status bar visibility

#### Tools Menu
- **Device Configuration...**: Configure NI-DAQmx devices
- **Calibration Wizard...**: Step-by-step calibration
- **Valve Controls...**: Access valve control panel
- **Settings...**: Application preferences

#### Help Menu
- **User Guide**: Quick reference information
- **About...**: Version and feature information

### Toolbar

Quick-access buttons for common operations:
- **New**: Create new session
- **Open**: Open existing session
- **Save**: Save current session
- **Dashboard**: Switch to dashboard view
- **Graphs**: Switch to graphs view
- **Calibration**: Open calibration system
- **Start Acquisition**: Start all sensors (green)
- **Stop Acquisition**: Stop all sensors (red)

### Main Content Area

The main area uses tabs for different views:
- **Dashboard**: Control and monitor all devices
- **Graphs**: Real-time data visualization
- **Data Table**: List view of all samples
- **Calibrations**: Manage sensor calibrations
- **CSV Logging**: Configure data logging

### Status Bar

Located at the bottom, shows:
- Timestamp of last action
- Current status messages
- Error notifications
- Operation confirmations

---

## Working with Pressure Transducers

### Configuration

1. Navigate to the **Dashboard** tab
2. Locate the **Pressure Transducers** section
3. Configure the following settings:

   **Device/Channel**
   - Format: `Dev1/ai0` (device name and channel)
   - Use NI MAX to verify your device name
   - Common formats: `Dev1/ai0`, `Dev1/ai1`, etc.

   **Sample Rate (Hz)**
   - How many samples per second
   - Default: 1000 Hz
   - Range: 1 - 100,000 Hz (hardware dependent)
   - Higher rates = more data, more CPU usage

   **Samples Per Read**
   - Buffer size for each read operation
   - Default: 100 samples
   - Larger buffer = more efficient, slight delay

### Starting Acquisition

1. Verify your configuration settings
2. Click the green **Start** button
3. Observe:
   - Button becomes disabled
   - Stop button becomes enabled
   - Status bar shows "Acquisition started"
   - Latest pressure value appears in blue box

### Monitoring Data

**Latest Value Display**
- Large blue box shows most recent pressure reading
- Format: "X.XXXXXX V"
- Updates in real-time

**Status Bar**
- Shows acquisition status
- Displays error messages if any
- Shows sample count when stopped

### Stopping Acquisition

1. Click the red **Stop** button
2. Observe:
   - Start button becomes enabled
   - Stop button becomes disabled
   - Status bar shows total samples acquired
   - Configuration fields become editable again

### Simulation Mode

If NI-DAQmx hardware is not available:
- Application automatically uses simulation mode
- Generates realistic pressure data (~5V ± 0.5V)
- Perfect for testing and demonstration
- All features work the same way

---

## Working with Thermocouples

### Overview

The system supports 4 independent thermocouple channels:
- **TC1**: Thermocouple 1
- **TC2**: Thermocouple 2
- **TC3**: Thermocouple 3
- **TC4**: Thermocouple 4

### Starting Temperature Monitoring

1. Navigate to **Dashboard** tab
2. Locate **Thermocouples** section
3. Click the green **Start** button
4. All four channel displays update every second

### Reading Temperature Values

Each thermocouple display shows:
- Channel label (TC1, TC2, TC3, TC4)
- Current temperature in Celsius
- Format: "XX.X°C"
- Orange-themed display for easy identification

### Temperature Display Features

**Current Display: Celsius**
- Room temperature base (~25°C)
- Each channel simulates different temperature zones
- Updates every second

**Future Enhancements**
- Toggle between Celsius, Fahrenheit, Kelvin
- Temperature trend graphs
- Alarm thresholds
- Historical data export

### Stopping Temperature Monitoring

1. Click the red **Stop** button
2. Temperature displays freeze at last value
3. Start button becomes enabled again

### Simulation Mode

Without actual thermocouple hardware:
- Simulated temperature data is generated
- Realistic room temperature ranges
- Each channel has different base temperature
- Random variations simulate real conditions

---

## Valve Control

### Overview

Control up to 4 digital valves:
- **Valve 1**: Lab valve 1
- **Valve 2**: Lab valve 2
- **Valve 3**: Lab valve 3
- **Valve 4**: Lab valve 4

### Opening/Closing Valves

**To Toggle a Valve:**
1. Navigate to **Dashboard** tab
2. Locate **Valve Controls** section
3. Click the valve button

**Visual Feedback:**
- **Gray button** with "CLOSED" = Valve is closed
- **Green button** with "OPEN" = Valve is open
- Status bar shows "Valve X OPEN/CLOSED"

### Valve Status Monitoring

Each valve button displays:
- Valve label (Valve 1, Valve 2, etc.)
- Current status (OPEN or CLOSED)
- Color-coded for quick visual recognition

### Safety Notes

**Important Considerations:**
- Always verify valve positions before experiments
- Check status bar confirmations
- Monitor valve states during operation
- Be aware of system pressures when opening valves

**Future Enhancements:**
- Valve interlock logic
- Automated sequencing
- Time-based valve control
- Pressure-based automatic control

---

## Data Visualization

### Dashboard View

**Purpose**: Real-time monitoring and control

**Features:**
- Latest pressure value (large blue display)
- 4 thermocouple readings (orange displays)
- Valve status indicators
- Control buttons for all devices

**When to Use:**
- During active data acquisition
- For quick status checks
- When controlling valves
- For configuration changes

### Graphs View

**Purpose**: Visual trends and patterns

**Current Status:**
- Placeholders for pressure graph
- Placeholders for temperature graph
- Framework ready for charting library

**Planned Features:**
- Real-time line charts
- Multiple traces (4 temperatures)
- Zoom and pan controls
- Time window selection
- Export graph images

**Implementation Notes:**
- Will use OxyPlot or LiveCharts
- Coming in next update
- Data collection already supports graphing

### Data Table View

**Purpose**: Detailed sample inspection

**Features:**
- All acquired samples in chronological order
- Monospace font (Consolas) for alignment
- Timestamp and value for each sample
- Scrollable list (newest at top)
- Limited to last 100 samples in memory

**Export Options:**
- Click "Export to CSV" button
- Or use File → Export Data menu

**When to Use:**
- To verify specific readings
- To inspect data quality
- Before exporting data
- To check timestamp accuracy

---

## Data Export and Logging

### CSV Logging (Continuous)

**Purpose**: Real-time data logging during acquisition

**Setup:**
1. Navigate to **CSV Logging** tab
2. Click **Browse** to choose file location
3. Default: Documents/PressureData.csv
4. Click **Start Logging**

**While Logging:**
- Stop Logging button becomes enabled
- Browse button becomes disabled
- Status bar shows file path
- Data writes asynchronously (no performance impact)

**To Stop:**
- Click **Stop Logging** button
- File is closed and finalized
- Browse button becomes enabled

**File Format:**
```csv
Timestamp,Value
2024-01-09 14:30:15.123,5.234567
2024-01-09 14:30:15.124,5.189234
```

### Export to CSV (On-Demand)

**Purpose**: Export data after acquisition

**Steps:**
1. Go to **File** → **Export Data** → **Export to CSV...**
2. Choose location and filename
3. Click Save

**What's Exported:**
- All samples currently in memory
- Same format as CSV logging
- Limited to last 100 samples

### Export to JSON

**Purpose**: Structured data export

**Steps:**
1. Go to **File** → **Export Data** → **Export to JSON...**
2. Choose location and filename
3. Click Save

**Features:**
- Pretty-printed JSON
- Array of sample strings
- Easy to parse programmatically

**Example Output:**
```json
[
  "2024-01-09 14:30:15.123, 5.234567",
  "2024-01-09 14:30:15.124, 5.189234"
]
```

### Export to Excel

**Status**: Coming in future update

**Planned Features:**
- .xlsx file format
- Multiple worksheets
- Formatted columns
- Charts included

---

## Calibration Management

### Overview

Apply calibration curves to sensor readings for accurate measurements.

**Calibration Formula:**
```
Calibrated Value = (Raw Value × Gain) + Offset
```

### Calibration Tab

Navigate to **Calibrations** tab to access:
- **New Calibration**: Start calibration wizard
- **Load Calibration**: Open saved calibration
- **Save Calibration**: Save current calibration
- **Active Calibrations**: View loaded calibrations

### Creating a Calibration

**Steps:**
1. Click **New Calibration** or use Tools → Calibration Wizard
2. Select sensor to calibrate
3. Record calibration points (reference vs. measured)
4. System calculates gain and offset
5. Save calibration with descriptive name

**Calibration Points:**
- At least 2 points recommended
- More points = better accuracy
- Span the full measurement range
- Use certified reference standards

### Loading a Calibration

**Steps:**
1. Navigate to **Calibrations** tab
2. Click **Load Calibration**
3. Select calibration file (*.cal or *.json)
4. Calibration appears in Active Calibrations list

**File Types:**
- `.cal`: Native calibration format
- `.json`: JSON format for compatibility

### Saving a Calibration

**Steps:**
1. Navigate to **Calibrations** tab
2. Click **Save Calibration**
3. Choose location and filename
4. Calibration is saved for future use

### Applying Calibrations

**Automatic Application:**
- Once loaded, calibrations apply automatically
- Calibrated values displayed instead of raw
- Original raw data preserved in files

**Manual Application:**
- Use CalibrationData.ApplyCalibration() method
- Pass raw value, get calibrated value
- Useful for post-processing

---

## Session Management

### What is a Session?

A session includes:
- Current configuration (sample rates, channels)
- Acquired data samples
- Valve states
- Temperature readings
- Active calibrations

### Creating a New Session

**Steps:**
1. Go to **File** → **New Session**
2. Confirm clearing current data
3. All displays reset to defaults

**When to Use:**
- Starting a new experiment
- After completing previous test
- To clear old data

### Saving a Session

**Quick Save:**
1. Go to **File** → **Save Session**
2. Saves to current file (if opened)
3. Or prompts for filename if new

**Save As:**
1. Go to **File** → **Save Session As...**
2. Choose location and filename
3. Format: JSON (*.json)

**What's Saved:**
- All configuration parameters
- Sample data
- Timestamps
- Valve states
- Session metadata

### Opening a Session

**Steps:**
1. Go to **File** → **Open Session...**
2. Browse to session file (*.json)
3. Click Open

**What Happens:**
- Previous session data cleared
- Saved configuration restored
- Saved data loaded
- Ready to continue or review

---

## Tips and Best Practices

### Data Acquisition

**Sample Rate Selection:**
- Start with 1000 Hz for general use
- Increase for high-frequency phenomena
- Decrease for slow-changing signals
- Consider storage and processing capacity

**Samples Per Read:**
- 100 is a good default
- Larger = more efficient, slight delay
- Smaller = lower latency, more overhead

**Before Starting:**
- Verify device connections
- Check configuration settings
- Clear old data if needed
- Set up logging path

### Data Management

**Regular Saves:**
- Save sessions after important acquisitions
- Use descriptive filenames with dates
- Export data in multiple formats
- Keep backup copies of critical data

**File Organization:**
- Create folders for each experiment
- Use consistent naming conventions
- Include date and experiment number
- Document session parameters

### Performance Tips

**For Long Acquisitions:**
- Enable CSV logging for continuous recording
- Clear data table periodically
- Monitor disk space
- Check for errors in status bar

**Multiple Sensors:**
- Start pressure acquisition first
- Then start temperature monitoring
- Or use toolbar "Start Acquisition" for all

### Calibration Best Practices

**When to Calibrate:**
- Before critical experiments
- Periodically (monthly/quarterly)
- After sensor replacement
- If readings seem off

**Calibration Tips:**
- Use certified reference standards
- Take multiple calibration points
- Span full measurement range
- Record calibration date and conditions
- Save calibration files with descriptive names

### Troubleshooting

**If Data Looks Wrong:**
1. Check device connections
2. Verify channel configuration
3. Check sample rate settings
4. Look for error messages
5. Try stopping and restarting

**If Application Freezes:**
- Check for dialog boxes behind main window
- Wait for long operations to complete
- Close and restart if necessary

**For Best Results:**
- Keep drivers up to date
- Close unnecessary applications
- Monitor system resources
- Regular calibration checks

---

## Keyboard Shortcuts

**Standard Windows Shortcuts:**
- **Alt+F4**: Exit application
- **Tab**: Move between controls
- **Enter**: Activate focused button
- **Ctrl+C**: Copy from data table (when focused)

**Menu Access:**
- **Alt+F**: Open File menu
- **Alt+E**: Open Edit menu
- **Alt+V**: Open View menu
- **Alt+T**: Open Tools menu
- **Alt+H**: Open Help menu

---

## Getting Help

**Built-in Help:**
- **Help** → **User Guide**: Quick reference
- **Help** → **About**: Version information

**Documentation Files:**
- `README.md`: Installation and setup
- `USER_GUIDE.md`: This document
- `ARCHITECTURE.md`: Technical details
- `QUICKSTART.md`: 5-minute tutorial

**Status Bar:**
- Watch for messages and confirmations
- Error messages appear here first
- Shows current operation status

**Support:**
- GitHub Issues: Report bugs
- NI Forums: Hardware questions
- Microsoft Docs: .NET/WPF questions

---

## Appendix: Data Models

### PressureSample
```csharp
Timestamp: DateTime  // When sample was acquired
Value: double        // Voltage reading
```

### TemperatureSample
```csharp
Timestamp: DateTime     // When sample was acquired
Value: double           // Temperature in Celsius
ChannelName: string     // "TC1", "TC2", etc.
ValueFahrenheit: double // Calculated
ValueKelvin: double     // Calculated
```

### ValveState
```csharp
ValveId: string         // "V1", "V2", etc.
Name: string            // Display name
IsOpen: bool            // Open/closed state
LastChanged: DateTime   // When last toggled
Description: string     // Notes
```

### CalibrationData
```csharp
SensorId: string              // Sensor identifier
SensorType: string            // "Pressure" or "Temperature"
Offset: double                // Calibration offset
Gain: double                  // Calibration gain
CalibrationDate: DateTime     // When calibrated
CalibrationNotes: string      // User notes
CalibrationPoints: List<>     // Reference points
```

---

## Version History

**Version 2.0** (Current)
- Added menu bar and toolbar
- Multi-sensor support (pressure, temperature, valves)
- Tab-based interface
- Calibration management framework
- Enhanced export options
- Session management
- Improved dashboard layout

**Version 1.0**
- Basic pressure acquisition
- CSV logging
- Simple UI
- NI-DAQmx integration stubs

---

*For the latest updates and detailed technical documentation, see the project repository.*
