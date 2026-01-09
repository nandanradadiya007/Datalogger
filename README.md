# Lab Data Acquisition System

A comprehensive Windows desktop application (WPF, .NET) for laboratory data acquisition and control. Records data from pressure transducers, thermocouples via NI-DAQmx, and provides valve control, real-time visualization, and calibration management.

## Features

### Multi-Sensor Support
- **Pressure Transducers**: Real-time acquisition from NI-DAQmx compatible devices
- **Thermocouples**: Temperature monitoring with 4 independent channels
- **Valve Controls**: Digital control and monitoring of up to 4 valves

### Professional User Interface
- **Menu Bar**: Complete File, Edit, View, Tools, and Help menus following Windows conventions
- **Dashboard View**: Unified control panel for all sensors and devices
- **Graph View**: Real-time visualization of pressure and temperature data
- **Data Table**: Tabular view of all acquired data with export capabilities
- **Calibration Manager**: Load, save, and manage sensor calibrations

### Data Management
- **Multiple Export Formats**: CSV, JSON, and Excel-ready
- **Session Management**: Save and load complete acquisition sessions
- **Real-time Logging**: Timestamped data logging with async, non-blocking writes
- **Calibration System**: Apply calibration curves to raw sensor data

### Configuration & Control
- **Flexible Configuration**: Configure device/channel, sample rate, and samples per read
- **Real-time Display**: View latest values and recent sample history
- **Valve Control**: Toggle valves on/off with status indicators
- **Temperature Display**: Multi-channel thermocouple readings in Celsius

## Technology Stack

- **Framework**: .NET 6.0 (WPF)
- **Language**: C#
- **Target OS**: Windows 10/11
- **DAQ API**: NI-DAQmx .NET (when hardware is available)

## Project Structure

```
PressureDataLogger/
├── Models/
│   ├── AppConfiguration.cs       # Application configuration and defaults
│   ├── PressureSample.cs          # Data model for pressure samples
│   ├── TemperatureSample.cs       # Data model for temperature samples
│   ├── ValveState.cs              # Data model for valve states
│   └── CalibrationData.cs         # Calibration data and coefficients
├── Interfaces/
│   ├── IDAQManager.cs             # DAQ service interface
│   └── ICSVLogger.cs              # CSV logging interface
├── Services/
│   ├── DAQManager.cs              # NI-DAQmx acquisition service
│   └── CSVLogger.cs               # CSV logging service
├── MainWindow.xaml                # Main UI layout with menu bar and tabs
└── MainWindow.xaml.cs             # Main UI code-behind
```

## Prerequisites

### Required Software

1. **Visual Studio 2022** (or later) with:
   - .NET desktop development workload
   - Windows 10/11 SDK
   
   OR
   
   **.NET 6.0 SDK** (or later) for command-line builds

2. **NI-DAQmx Drivers** (for hardware integration):
   - Download from: [NI-DAQmx Download Page](https://www.ni.com/en-us/support/downloads/drivers/download.ni-daqmx.html)
   - Required for production use with actual NI hardware
   - The .NET API is typically located at:
     ```
     C:\Program Files (x86)\National Instruments\MeasurementStudioVS2012\DotNET\Assemblies\Current\NationalInstruments.DAQmx.dll
     ```

### Optional (for Development without Hardware)

The application includes simulation stubs that allow testing without physical NI hardware. When running in simulation mode, the application generates synthetic pressure data for demonstration purposes.

## Building the Application

### Using Visual Studio

1. Open the project:
   ```
   PressureDataLogger/PressureDataLogger.csproj
   ```

2. Restore NuGet packages (automatic in Visual Studio)

3. Build the solution:
   - Press `Ctrl+Shift+B` or
   - Menu: Build → Build Solution

### Using Command Line (dotnet CLI)

1. Navigate to the project directory:
   ```bash
   cd PressureDataLogger
   ```

2. Restore dependencies:
   ```bash
   dotnet restore
   ```

3. Build the project:
   ```bash
   dotnet build
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

### Build Output

The compiled application will be located at:
```
PressureDataLogger/bin/Debug/net6.0-windows/PressureDataLogger.exe
```

## Running the Application

### Application Layout

The application features a professional Windows-style interface with:

1. **Menu Bar**
   - **File**: New session, Open, Save, Export (CSV/JSON/Excel), Exit
   - **Edit**: Configuration, Clear data
   - **View**: Switch between Dashboard, Graphs, Data Table, and Calibrations
   - **Tools**: Device configuration, Calibration wizard, Valve controls, Settings
   - **Help**: User guide and About information

2. **Toolbar**: Quick access buttons for common actions

3. **Main Tabs**
   - **Dashboard**: Control panel for all sensors and devices
   - **Graphs**: Real-time visualization (placeholders for charting)
   - **Data Table**: Tabular view of acquired data
   - **Calibrations**: Calibration management
   - **CSV Logging**: Configure and control data logging

### Using Pressure Transducers

1. Navigate to the **Dashboard** tab
2. In the **Pressure Transducers** section:
   - **Device/Channel**: Enter your device channel (e.g., `Dev1/ai0`)
   - **Sample Rate**: Set acquisition rate in Hz (default: 1000)
   - **Samples Per Read**: Set buffer size (default: 100)
3. Click **Start** to begin pressure data collection
4. Latest pressure reading displays in the blue box
5. Click **Stop** to end acquisition

### Using Thermocouples

1. Navigate to the **Dashboard** tab
2. In the **Thermocouples** section:
   - Click **Start** to begin temperature monitoring
   - View real-time temperature from 4 channels (TC1-TC4)
   - Temperatures displayed in Celsius
3. Click **Stop** to end temperature acquisition

### Controlling Valves

1. Navigate to the **Dashboard** tab
2. In the **Valve Controls** section:
   - Click any valve button (Valve 1-4) to toggle state
   - **Gray button** = CLOSED
   - **Green button** = OPEN
   - Status updates display in the status bar

### With NI-DAQmx Hardware

1. Ensure NI-DAQmx drivers are installed
2. Connect your NI DAQ device
3. Launch the application
4. Configure settings:
   - **Device/Channel**: Enter your device channel (e.g., `Dev1/ai0`)
   - **Sample Rate**: Set acquisition rate in Hz (default: 1000)
   - **Samples Per Read**: Set buffer size (default: 100)
5. Click **Start Acquisition** to begin data collection

### Without Hardware (Simulation Mode)

The application will automatically run in simulation mode if NI-DAQmx is not available:

1. Launch the application
2. Use default or custom settings
3. Click **Start Acquisition**
4. The application will generate simulated pressure data (~5V ± 0.5V)

## Using CSV Logging

1. Navigate to the **CSV Logging** tab (or use Dashboard controls)
2. Click **Browse** to select a log file location
3. Click **Start Logging** to begin recording data
4. Data is saved with format: `Timestamp,Value`
5. Example CSV output:
   ```
   Timestamp,Value
   2024-01-09 14:30:15.123,5.234567
   2024-01-09 14:30:15.124,5.189234
   ```

## Exporting Data

### Export to CSV
1. Go to **File** → **Export Data** → **Export to CSV...**
2. Choose location and filename
3. All current session data is exported

### Export to JSON
1. Go to **File** → **Export Data** → **Export to JSON...**
2. Choose location and filename
3. Data is exported in JSON format with indentation

### Export to Excel
Coming in a future update - currently shows notification dialog

## Calibration Management

### Loading Calibrations
1. Navigate to the **Calibrations** tab
2. Click **Load Calibration**
3. Select a calibration file (*.cal or *.json)

### Saving Calibrations
1. Navigate to the **Calibrations** tab
2. Click **Save Calibration**
3. Choose location and filename

### Using Calibration Wizard
1. Go to **Tools** → **Calibration Wizard...**
2. Follow the wizard to:
   - Select sensor to calibrate
   - Apply known reference values
   - Calculate calibration coefficients
   - Save calibration data

## Session Management

### New Session
- **File** → **New Session** - Clears all current data and starts fresh

### Save Session
- **File** → **Save Session** - Saves current configuration and data
- **File** → **Save Session As...** - Save with a new filename

### Open Session
- **File** → **Open Session...** - Load a previously saved session

## Configuration Defaults

Default configuration values are defined in `Models/AppConfiguration.cs`:

- **Device/Channel**: `Dev1/ai0`
- **Sample Rate**: `1000 Hz`
- **Samples Per Read**: `100`
- **Voltage Range**: `-10V` to `+10V`
- **Max Displayed Samples**: `100` (in UI list)

## Integrating with NI-DAQmx Hardware

The application is designed to work with NI-DAQmx .NET API. Currently, the DAQ manager includes stubs with clear TODO markers indicating where hardware-specific code should be added.

### Steps to Integrate NI-DAQmx:

1. **Add NI-DAQmx Reference**:
   
   In `PressureDataLogger.csproj`, add:
   ```xml
   <ItemGroup>
     <Reference Include="NationalInstruments.DAQmx">
       <HintPath>C:\Program Files (x86)\National Instruments\MeasurementStudioVS2012\DotNET\Assemblies\Current\NationalInstruments.DAQmx.dll</HintPath>
     </Reference>
   </ItemGroup>
   ```

2. **Update DAQManager.cs**:
   
   Uncomment the TODO sections in `Services/DAQManager.cs`:
   - Add `using NationalInstruments.DAQmx;`
   - Uncomment NI-DAQmx Task initialization code
   - Uncomment analog channel configuration
   - Uncomment timing configuration
   - Uncomment data reading code
   - Remove simulation stub code

3. **Test with Hardware**:
   - Connect your NI DAQ device
   - Verify device name with NI MAX (Measurement & Automation Explorer)
   - Run the application and configure your device/channel

### Key Integration Points

All hardware integration points are marked with `TODO` comments in:
- `Services/DAQManager.cs` - Lines with DAQmx API calls

## Troubleshooting

### Build Issues

**Error: "To build a project targeting Windows on this operating system..."**
- This is expected on non-Windows systems
- The application is Windows-only and requires Windows to build/run

**Warning: "The target framework 'net6.0-windows' is out of support..."**
- This is a warning about .NET 6 reaching end-of-life
- The application will still build and run
- Consider upgrading to .NET 8 for long-term support

### Runtime Issues

**"Failed to start acquisition" error:**
- Check that your device name is correct (use NI MAX)
- Verify NI-DAQmx drivers are installed
- Ensure no other application is using the device
- Check that the channel configuration matches your hardware

**Data not appearing:**
- Verify acquisition is started (green status indicator)
- Check sample rate is appropriate for your application
- Ensure the device is properly connected

**Temperature readings showing "-- °C":**
- Click **Start** button in Thermocouples section
- Simulated data will appear if no hardware is connected

**Valves not responding:**
- Click directly on valve buttons to toggle
- Button color changes: Gray (closed) or Green (open)
- Check status bar for valve state confirmations

### CSV Logging Issues

**"Failed to start logging" error:**
- Check that the file path is valid
- Ensure you have write permissions to the directory
- Verify the disk has sufficient space

## New Features in Version 2.0

### User Interface Enhancements
- ✅ Professional menu bar (File, Edit, View, Tools, Help)
- ✅ Toolbar with quick action buttons
- ✅ Tab-based navigation system
- ✅ Improved dashboard layout
- ✅ Status bar with contextual messages

### Multi-Sensor Support
- ✅ 4-channel thermocouple temperature monitoring
- ✅ Simulated temperature acquisition
- ✅ Temperature display in Celsius
- ✅ Future: Fahrenheit and Kelvin conversions

### Valve Control System
- ✅ 4 digital valve controls
- ✅ Toggle on/off functionality
- ✅ Visual status indicators (color-coded)
- ✅ State tracking and logging

### Data Management
- ✅ Export to CSV
- ✅ Export to JSON
- ✅ Session save/load framework
- ✅ Clear data functionality
- ⏳ Excel export (coming soon)

### Calibration System
- ✅ Calibration data models
- ✅ Calibration tab interface
- ✅ Load/Save calibration files
- ⏳ Calibration wizard (coming soon)
- ⏳ Apply calibrations to readings (coming soon)

### Graph Visualization
- ✅ Graph tab with placeholders
- ⏳ Real-time pressure plotting (coming soon)
- ⏳ Real-time temperature plotting (coming soon)
- ⏳ Charting library integration (coming soon)

## Troubleshooting (Legacy)

### Build Issues

**Error: "To build a project targeting Windows on this operating system..."**
- This is expected on non-Windows systems
- The application is Windows-only and requires Windows to build/run

**Warning: "The target framework 'net6.0-windows' is out of support..."**
- This is a warning about .NET 6 reaching end-of-life
- The application will still build and run
- Consider upgrading to .NET 8 for long-term support

### Runtime Issues

**"Failed to start acquisition" error:**
- Check that your device name is correct (use NI MAX)
- Verify NI-DAQmx drivers are installed
- Ensure no other application is using the device
- Check that the channel configuration matches your hardware

**Data not appearing:**
- Verify acquisition is started (green status indicator)
- Check sample rate is appropriate for your application
- Ensure the device is properly connected

### CSV Logging Issues

**"Failed to start logging" error:**
- Check that the file path is valid
- Ensure you have write permissions to the directory
- Verify the disk has sufficient space

## License

This project is open source. See the repository for license details.

## Contributing

Contributions are welcome! Please feel free to submit issues or pull requests.

## Support

For issues related to:
- **Application bugs**: Open an issue in this repository
- **NI-DAQmx hardware/drivers**: Contact National Instruments support
- **.NET/WPF questions**: Refer to Microsoft documentation
