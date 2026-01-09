# NI-DAQmx Integration Guide

This guide provides detailed instructions for integrating the Pressure Data Logger application with National Instruments DAQmx hardware.

## Overview

The application is designed to work with NI-DAQmx compatible devices for acquiring pressure transducer data. The current implementation includes simulation stubs that allow development and testing without physical hardware.

## Current Implementation Status

### Completed
- ✅ Application architecture and UI
- ✅ CSV logging with async writes
- ✅ Configuration management
- ✅ Error handling framework
- ✅ Simulation mode for testing
- ✅ Service layer interfaces

### To Be Implemented (when NI-DAQmx hardware is available)
- ⚠️ NI-DAQmx Task initialization
- ⚠️ Analog input channel configuration
- ⚠️ Sample clock timing setup
- ⚠️ Continuous data acquisition
- ⚠️ Hardware-specific error handling

## Integration Steps

### Step 1: Install NI-DAQmx Drivers

1. Download NI-DAQmx from National Instruments:
   - Visit: https://www.ni.com/en-us/support/downloads/drivers/download.ni-daqmx.html
   - Download the latest version compatible with your OS
   - Install with default options

2. Verify installation:
   - Open NI Measurement & Automation Explorer (NI MAX)
   - Check that your devices appear under "Devices and Interfaces"
   - Run a self-test on your device

### Step 2: Add NI-DAQmx .NET Reference

Update `PressureDataLogger.csproj`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net6.0-windows</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <UseWPF>true</UseWPF>
    <EnableWindowsTargeting>true</EnableWindowsTargeting>
  </PropertyGroup>

  <ItemGroup>
    <!-- Add this reference -->
    <Reference Include="NationalInstruments.DAQmx">
      <HintPath>C:\Program Files (x86)\National Instruments\MeasurementStudioVS2012\DotNET\Assemblies\Current\NationalInstruments.DAQmx.dll</HintPath>
      <Private>false</Private>
    </Reference>
    
    <!-- Or use NuGet package if available -->
    <!-- <PackageReference Include="NationalInstruments.DAQmx" Version="x.x.x" /> -->
  </ItemGroup>
</Project>
```

**Note**: The exact path may vary based on your NI-DAQmx installation. Common paths include:
- Visual Studio 2012: `C:\Program Files (x86)\National Instruments\MeasurementStudioVS2012\DotNET\Assemblies\Current\`
- Visual Studio 2015: `C:\Program Files (x86)\National Instruments\MeasurementStudioVS2015\DotNET\Assemblies\Current\`
- Visual Studio 2019+: `C:\Program Files (x86)\National Instruments\MeasurementStudio\DotNET\Assemblies\Current\`

### Step 3: Update DAQManager.cs

Open `Services/DAQManager.cs` and make the following changes:

#### 3.1 Add Using Directive

At the top of the file, uncomment:

```csharp
using NationalInstruments.DAQmx;
```

#### 3.2 Declare DAQmx Objects

Replace the comment section with actual declarations:

```csharp
private Task? _daqTask;
private AnalogSingleChannelReader? _reader;
```

#### 3.3 Initialize DAQmx Task (in Start method)

Replace the TODO section with:

```csharp
_daqTask = new Task();

// Create analog input voltage channel
_daqTask.AIChannels.CreateVoltageChannel(
    deviceChannel,
    "",
    AITerminalConfiguration.Differential,  // or .Rse, .Nrse depending on your setup
    AppConfiguration.MinVoltage,
    AppConfiguration.MaxVoltage,
    AIVoltageUnits.Volts);

// Configure timing
_daqTask.Timing.ConfigureSampleClock(
    "",  // Use default onboard clock
    sampleRate,
    SampleClockActiveEdge.Rising,
    SampleQuantityMode.ContinuousSamples,
    samplesPerRead);

// Create reader
_reader = new AnalogSingleChannelReader(_daqTask.Stream);

// Start the task
_daqTask.Start();
```

#### 3.4 Update Error Handling (in Start method)

Change the catch block:

```csharp
catch (DaqException ex)
{
    _isRunning = false;
    OnErrorOccurred($"DAQmx Error: {ex.Message} (Error Code: {ex.Error})");
}
```

#### 3.5 Implement Data Reading (in AcquisitionLoop)

Replace the simulation code with:

```csharp
// Read data from NI-DAQmx
double[] data = _reader.ReadMultiSample(samplesPerRead);

// Process each sample
foreach (var value in data)
{
    if (token.IsCancellationRequested) break;
    
    var sample = new PressureSample(DateTime.Now, value);
    OnDataAcquired(sample);
}
```

#### 3.6 Update Stop Method

Uncomment the DAQmx cleanup code:

```csharp
try
{
    _daqTask?.Stop();
    _daqTask?.Dispose();
    _daqTask = null;
    _reader = null;
}
catch (DaqException ex)
{
    OnErrorOccurred($"Error stopping task: {ex.Message}");
}
```

#### 3.7 Remove Simulation Code

Delete or comment out the `SimulateDataAcquisition` method and its call in the acquisition loop.

### Step 4: Configure Hardware Settings

#### Terminal Configuration

The terminal configuration depends on your sensor wiring:

- **Differential (recommended for pressure transducers)**:
  ```csharp
  AITerminalConfiguration.Differential
  ```
  - Uses two wires per channel (AI+ and AI-)
  - Better noise rejection
  - Use when sensor provides differential output

- **Referenced Single-Ended (RSE)**:
  ```csharp
  AITerminalConfiguration.Rse
  ```
  - Uses one wire per channel + common ground
  - All measurements referenced to AI GND

- **Non-Referenced Single-Ended (NRSE)**:
  ```csharp
  AITerminalConfiguration.Nrse
  ```
  - Floating measurements

#### Voltage Range

Adjust based on your pressure transducer specifications:

```csharp
// Example for 0-10V transducer
_daqTask.AIChannels.CreateVoltageChannel(
    deviceChannel,
    "",
    AITerminalConfiguration.Differential,
    0.0,      // Min voltage
    10.0,     // Max voltage
    AIVoltageUnits.Volts);

// Example for ±5V transducer
_daqTask.AIChannels.CreateVoltageChannel(
    deviceChannel,
    "",
    AITerminalConfiguration.Differential,
    -5.0,     // Min voltage
    5.0,      // Max voltage
    AIVoltageUnits.Volts);
```

### Step 5: Testing

1. **Verify Device Connection**:
   ```
   - Open NI MAX
   - Locate your device
   - Run Test Panels to verify connectivity
   ```

2. **Test Single Channel**:
   ```
   - Start with one channel (e.g., Dev1/ai0)
   - Use moderate sample rate (1000 Hz)
   - Verify data appears in the application
   ```

3. **Calibrate Voltage to Pressure**:
   - Record voltage readings at known pressures
   - Create calibration curve
   - Add conversion logic if needed

## Common Configuration Examples

### Example 1: Single Pressure Transducer

```
Device/Channel: Dev1/ai0
Sample Rate: 1000 Hz
Samples Per Read: 100
Voltage Range: 0-10V (configure in code)
Terminal Config: Differential
```

### Example 2: Multiple Transducers

For multiple channels, modify `DAQManager.cs` to support multiple channels:

```csharp
// Instead of CreateVoltageChannel, use:
_daqTask.AIChannels.CreateVoltageChannel(
    "Dev1/ai0:3",  // Channels 0-3
    "",
    AITerminalConfiguration.Differential,
    0.0,
    10.0,
    AIVoltageUnits.Volts);

// Use AnalogMultiChannelReader instead of AnalogSingleChannelReader
_reader = new AnalogMultiChannelReader(_daqTask.Stream);

// Reading returns 2D array [channels, samples]
double[,] data = _reader.ReadMultiSample(samplesPerRead);
```

### Example 3: High-Speed Acquisition

```
Device/Channel: Dev1/ai0
Sample Rate: 10000 Hz (or higher)
Samples Per Read: 1000
Note: Ensure your device supports the requested sample rate
```

## Troubleshooting

### Error: "Device not found"
- Verify device is connected
- Check device name in NI MAX
- Ensure drivers are installed
- Try resetting the device in NI MAX

### Error: "Sample rate not supported"
- Check device specifications
- Reduce sample rate
- Verify no other applications are using the device

### Error: "Buffer overflow"
- Increase samples per read
- Reduce processing time in data handler
- Use hardware-timed acquisition

### Poor Data Quality
- Check grounding
- Use shielded cables
- Enable differential mode
- Verify voltage range settings
- Check for noise sources

### Data Reading Errors
- Verify channel configuration
- Check terminal configuration
- Ensure task is started before reading
- Verify timeout settings

## Performance Optimization

### Buffering
- Use appropriate buffer sizes (samples per read)
- Larger buffers = fewer CPU interrupts but higher latency
- Smaller buffers = more responsive but higher CPU usage

### Threading
- Data acquisition runs on background thread (already implemented)
- CSV logging is async (already implemented)
- UI updates are marshaled to UI thread (already implemented)

### Memory Management
- Dispose of tasks properly (already implemented)
- Monitor memory usage during long runs
- Consider periodic log file rotation for 24/7 operation

## Converting Voltage to Engineering Units

If you need to convert voltage readings to pressure units:

### Option 1: Linear Scaling

Add to `PressureSample` or create a new property:

```csharp
public double PressurePSI
{
    get
    {
        // Example: 0-10V = 0-100 PSI
        return Value * 10.0;
    }
}
```

### Option 2: Calibration Curve

Create a calibration service:

```csharp
public class CalibrationService
{
    public double VoltageToPressure(double voltage)
    {
        // Implement your calibration equation
        // Example: Linear with offset
        double slope = 10.0;      // PSI per Volt
        double offset = -50.0;    // PSI at 0V
        return voltage * slope + offset;
    }
}
```

## Additional Resources

- **NI-DAQmx .NET Help**: Available in NI-DAQmx installation
- **NI-DAQmx Examples**: Located in `C:\Users\Public\Documents\National Instruments\NI-DAQ\Examples`
- **NI Community Forums**: https://forums.ni.com/
- **DAQmx API Reference**: https://www.ni.com/docs/

## Support

For NI-DAQmx specific issues:
- Contact National Instruments support
- Visit NI Community forums
- Check NI-DAQmx documentation

For application issues:
- Open an issue in this repository
- Refer to main README.md
