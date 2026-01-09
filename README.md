# Datalogger

A Windows desktop application (WPF, .NET) that records pressure data from NI pressure transducers via NI-DAQmx and supports logging to CSV files.

## Features

- **Data Acquisition**: Real-time acquisition from NI-DAQmx compatible devices
- **Flexible Configuration**: Configure device/channel, sample rate, and samples per read
- **CSV Logging**: Timestamped data logging with async, non-blocking writes
- **Real-time Display**: View latest values and recent sample history
- **User-friendly UI**: Clean WPF interface with status indicators

## Technology Stack

- **Framework**: .NET 6.0 (WPF)
- **Language**: C#
- **Target OS**: Windows 10/11
- **DAQ API**: NI-DAQmx .NET (when hardware is available)

## Project Structure

```
PressureDataLogger/
├── Models/
│   ├── AppConfiguration.cs    # Application configuration and defaults
│   └── PressureSample.cs       # Data model for pressure samples
├── Interfaces/
│   ├── IDAQManager.cs          # DAQ service interface
│   └── ICSVLogger.cs           # CSV logging interface
├── Services/
│   ├── DAQManager.cs           # NI-DAQmx acquisition service
│   └── CSVLogger.cs            # CSV logging service
├── MainWindow.xaml             # Main UI layout
└── MainWindow.xaml.cs          # Main UI code-behind
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

1. Click **Browse** to select a log file location
2. Click **Start Logging** to begin recording data
3. Data is saved with format: `Timestamp,Value`
4. Example CSV output:
   ```
   Timestamp,Value
   2024-01-09 14:30:15.123,5.234567
   2024-01-09 14:30:15.124,5.189234
   ```

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
