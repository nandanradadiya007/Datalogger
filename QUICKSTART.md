# Quick Start Guide - Pressure Data Logger

This guide will help you get started with the Pressure Data Logger application quickly.

## 5-Minute Setup

### 1. Prerequisites Check
- ✅ Windows 10 or Windows 11
- ✅ .NET 6.0 SDK or Visual Studio 2022+
- ⚠️ NI-DAQmx drivers (optional for simulation mode)

### 2. Build & Run

**Option A: Using Visual Studio**
```
1. Double-click: PressureDataLogger.sln
2. Press F5 to build and run
```

**Option B: Using Command Line**
```bash
cd PressureDataLogger
dotnet run
```

### 3. First Acquisition (Simulation Mode)

The application runs in simulation mode by default (no hardware required):

1. **Configure Settings** (or use defaults):
   - Device/Channel: `Dev1/ai0` ← Already filled in
   - Sample Rate: `1000` Hz ← Already filled in
   - Samples Per Read: `100` ← Already filled in

2. **Start Acquisition**:
   - Click the green **"Start Acquisition"** button
   - Watch the latest value update in real-time
   - See sample history populate in the list

3. **Start Logging** (Optional):
   - Click **"Browse"** to choose where to save CSV file
   - Click blue **"Start Logging"** button
   - Data will be saved with timestamps

4. **Stop Everything**:
   - Click **"Stop Logging"** (orange button)
   - Click **"Stop Acquisition"** (red button)

## Understanding the UI

### Top Section - DAQ Configuration
```
┌─────────────────────────────────────────┐
│ Device/Channel:   [Dev1/ai0    ]        │
│ Sample Rate (Hz): [1000        ]        │
│ Samples Per Read: [100         ]        │
│ [Start Acquisition] [Stop Acquisition]  │
└─────────────────────────────────────────┘
```

### Middle Section - CSV Logging
```
┌─────────────────────────────────────────┐
│ Log File Path: [path...] [Browse...]    │
│ [Start Logging] [Stop Logging]          │
└─────────────────────────────────────────┘
```

### Bottom Section - Data Display
```
┌─────────────────────────────────────────┐
│ Latest Value:                           │
│    5.234567 V  ← Real-time update       │
│                                         │
│ Recent Samples:                         │
│  2024-01-09 14:30:15.123, 5.234567     │
│  2024-01-09 14:30:15.124, 5.189234     │
│  ...                                    │
└─────────────────────────────────────────┘
```

### Status Bar
```
14:30:15 - Acquisition started - Dev1/ai0 @ 1000 Hz
```

## What You'll See (Simulation Mode)

In simulation mode, you'll see:
- **Voltage values** around 5.0V ± 0.5V
- **Sample rate** matches your configuration
- **Real-time updates** in the UI
- **CSV file** with timestamp and value columns

Example output:
```
Latest Value: 5.234567 V

Recent Samples:
2024-01-09 14:30:15.123, 5.234567
2024-01-09 14:30:15.124, 5.189234
2024-01-09 14:30:15.125, 4.987654
```

## CSV File Format

When logging is enabled, data is saved as:

```csv
Timestamp,Value
2024-01-09 14:30:15.123,5.234567
2024-01-09 14:30:15.124,5.189234
2024-01-09 14:30:15.125,4.987654
```

You can open this file in:
- Excel
- MATLAB
- Python (pandas)
- Any text editor

## Common Use Cases

### Use Case 1: Quick Data Collection
```
1. Start application
2. Click "Start Acquisition"
3. Click "Start Logging"
4. Wait for desired duration
5. Click "Stop Logging"
6. Click "Stop Acquisition"
7. Open CSV file
```

### Use Case 2: Real-time Monitoring Only
```
1. Start application
2. Click "Start Acquisition"
3. Watch latest value
4. No logging needed
5. Click "Stop Acquisition" when done
```

### Use Case 3: Continuous 24/7 Logging
```
1. Start application
2. Browse and select log file location
3. Click "Start Logging"
4. Click "Start Acquisition"
5. Minimize window
6. Let run continuously
```

## Tips & Tricks

### Tip 1: Sample Rate Selection
- **High rate (10,000+ Hz)**: Fast transient events
- **Medium rate (1,000 Hz)**: General purpose, good default
- **Low rate (10-100 Hz)**: Slow processes, conserve disk space

### Tip 2: Log File Management
- Choose a descriptive filename (e.g., `PressureTest_2024-01-09.csv`)
- Save to a location with plenty of disk space
- Consider date/time in filename for multiple tests

### Tip 3: Performance
- Larger "Samples Per Read" = more efficient, slightly higher latency
- Smaller "Samples Per Read" = more responsive, higher CPU usage
- Default (100) works well for most applications

### Tip 4: Data Review
- Stop acquisition before opening CSV file in Excel
- Use Excel, MATLAB, or Python for data analysis
- CSV format is universal and easy to process

## Troubleshooting

### Problem: Application won't start
**Solution**: Check that .NET 6.0+ is installed
```bash
dotnet --version
```

### Problem: Build errors
**Solution**: Restore NuGet packages
```bash
dotnet restore
dotnet build
```

### Problem: Can't select log file
**Solution**: 
- Ensure folder exists
- Check write permissions
- Try selecting a different location

### Problem: No data appearing
**Solution**: 
- Check that "Start Acquisition" button was clicked
- Verify status bar shows "Acquisition started"
- Try stopping and starting again

## Next Steps

### Ready for Real Hardware?

1. **Install NI-DAQmx drivers**:
   - Download from: https://www.ni.com/en-us/support/downloads/drivers/download.ni-daqmx.html
   - Follow installation wizard

2. **Connect your device**:
   - Use NI MAX to verify device
   - Note your device name (e.g., Dev1)

3. **Update application**:
   - See: `NIDAQMX_INTEGRATION.md` for detailed instructions
   - Uncomment hardware-specific code in `Services/DAQManager.cs`

4. **Configure for your sensor**:
   - Set appropriate voltage range
   - Choose terminal configuration
   - Adjust sample rate

### Want to Customize?

- **Change default values**: Edit `Models/AppConfiguration.cs`
- **Add features**: Extend `Services/DAQManager.cs`
- **Modify UI**: Edit `MainWindow.xaml`
- **Add calculations**: Update `Models/PressureSample.cs`

## Support & Documentation

- **Full documentation**: See `README.md`
- **Hardware integration**: See `NIDAQMX_INTEGRATION.md`
- **Issues/bugs**: Open an issue on GitHub

## Example Workflow

Here's a complete example workflow:

```
1. Launch application
   ✓ Application opens with default settings

2. Configure acquisition
   ✓ Keep defaults: Dev1/ai0, 1000 Hz, 100 samples

3. Select log file
   ✓ Click "Browse"
   ✓ Navigate to Documents folder
   ✓ Enter filename: "PressureTest_Jan09.csv"
   ✓ Click "Save"

4. Start logging
   ✓ Click "Start Logging" (blue button)
   ✓ Status bar: "Logging started - Documents/PressureTest_Jan09.csv"

5. Start acquisition
   ✓ Click "Start Acquisition" (green button)
   ✓ Latest value starts updating
   ✓ Sample list starts populating
   ✓ Status bar: "Acquisition started - Dev1/ai0 @ 1000 Hz"

6. Monitor for desired duration
   ✓ Watch real-time updates
   ✓ Verify data looks reasonable
   ✓ Note the sample count in status bar

7. Stop acquisition
   ✓ Click "Stop Acquisition" (red button)
   ✓ Status bar: "Acquisition stopped - Total samples: 10000"

8. Stop logging
   ✓ Click "Stop Logging" (orange button)
   ✓ Status bar: "Logging stopped"

9. Review data
   ✓ Open CSV file in Excel
   ✓ Create graphs, analyze trends
   ✓ Export results
```

## Success!

You're now ready to use the Pressure Data Logger! 🎉

For more advanced features and hardware integration, check out the other documentation files.
