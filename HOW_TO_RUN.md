# How to View and Run the Lab Data Acquisition System

## Quick Answer: 3 Ways to See the Application

### Option 1: Visual Studio (Easiest)
1. Open `PressureDataLogger.sln` with Visual Studio 2022
2. Press **F5** (or click the green "Start" button)
3. The application window will open automatically

### Option 2: Command Line
```bash
cd PressureDataLogger
dotnet run
```
The application will build and launch.

### Option 3: Run the Executable
After building, run the .exe directly:
```
PressureDataLogger/bin/Debug/net6.0-windows/PressureDataLogger.exe
```

---

## What You'll See

When the application opens, you'll see:

### 1. **Menu Bar** (Top)
```
File | Edit | View | Tools | Help
```
- **File**: New, Open, Save, Export
- **View**: Switch between Dashboard, Graphs, Data Table, Calibrations

### 2. **Toolbar** (Below menu)
Quick-access buttons for common operations

### 3. **Main Window with 5 Tabs**

#### **Tab 1: Dashboard** (Default view)
```
┌─ Pressure Transducers ─────────────────┐
│ Device/Channel: [Dev1/ai0]             │
│ Sample Rate:    [1000] Hz              │
│ [Start] [Stop]                         │
│                                        │
│ Latest Pressure: 5.234567 V            │
└────────────────────────────────────────┘

┌─ Thermocouples ────────────────────────┐
│ [Start] [Stop]                         │
│                                        │
│ TC1: 28.5°C  TC2: 33.2°C              │
│ TC3: 38.7°C  TC4: 43.1°C              │
└────────────────────────────────────────┘

┌─ Valve Controls ───────────────────────┐
│ [Valve 1]  [Valve 2]                   │
│  CLOSED     CLOSED                     │
│                                        │
│ [Valve 3]  [Valve 4]                   │
│  CLOSED     CLOSED                     │
└────────────────────────────────────────┘
```

#### **Tab 2: Graphs**
Placeholder for real-time pressure and temperature charts

#### **Tab 3: Data Table**
Scrollable list of all acquired data samples

#### **Tab 4: Calibrations**
Manage sensor calibrations

#### **Tab 5: CSV Logging**
Configure data logging to CSV files

---

## Try It Now (5 Minutes)

### Step 1: Start the Application
Choose one of the 3 options above. Visual Studio (F5) is recommended.

### Step 2: Try Pressure Acquisition
1. Go to **Dashboard** tab (already selected by default)
2. In the **Pressure Transducers** section:
   - Click the green **Start** button
   - Watch the "Latest Pressure" value update in real-time
   - Values will be around 5.0V (simulated data)

### Step 3: Try Temperature Monitoring
1. Still in **Dashboard** tab
2. In the **Thermocouples** section:
   - Click the green **Start** button
   - See 4 temperature readings (TC1-TC4) update every second
   - Values will be between 25-45°C (simulated data)

### Step 4: Try Valve Controls
1. Still in **Dashboard** tab
2. In the **Valve Controls** section:
   - Click any valve button (e.g., "Valve 1")
   - Button turns **GREEN** and shows "OPEN"
   - Click again to close (turns gray, shows "CLOSED")

### Step 5: Explore Other Tabs
- Click **Data Table** tab to see all pressure samples
- Click **Graphs** tab to see placeholders for charts
- Click **Calibrations** tab to manage calibrations

### Step 6: Try Data Export
1. Click **File** menu → **Export Data** → **Export to CSV...**
2. Choose a location and filename
3. Click Save
4. Open the CSV file to see your data!

---

## Simulation Mode

The application runs in **simulation mode** by default, which means:
- ✅ **No hardware required** - Perfect for testing and learning
- ✅ **Realistic data** - Simulated pressure (~5V) and temperature (25-45°C)
- ✅ **All features work** - You can test everything without hardware
- ✅ **Safe to experiment** - No risk to equipment

---

## Requirements to Run

### Minimum
- Windows 10 or Windows 11
- .NET 6.0 Runtime (or Visual Studio 2022)

### To Build from Source
- Visual Studio 2022, OR
- .NET 6.0 SDK (for command-line)

---

## Troubleshooting

### "Application won't start"
**Check if .NET 6.0 is installed:**
```bash
dotnet --version
```
Should show 6.0 or higher.

### "Build errors"
**Restore dependencies:**
```bash
cd PressureDataLogger
dotnet restore
dotnet build
```

### "Can't see the window"
- Make sure you're running on Windows (not Linux/Mac)
- WPF applications only work on Windows
- Check if the window opened behind other windows

---

## Screenshots

Since this is a Windows application, here's what you'll see:

**Main Window:**
- Professional menu bar at top
- Toolbar with buttons
- Tabbed interface with 5 tabs
- Dashboard showing all sensors
- Status bar at bottom

**Dashboard Tab:**
- Pressure section with configuration and display
- Temperature section with 4 channels
- Valve controls with toggle buttons
- Real-time updates when acquisition is running

**What the Data Looks Like:**
```
Pressure: 5.234567 V (updates continuously)
TC1: 28.5°C, TC2: 33.2°C, TC3: 38.7°C, TC4: 43.1°C (updates every second)
Valves: Visual buttons (gray=closed, green=open)
```

---

## Next Steps

After running the application:

1. **Read the User Guide**: See `USER_GUIDE.md` for detailed instructions (17,000+ words!)
2. **Try all features**: Explore each tab and menu
3. **Export data**: Try exporting to CSV and JSON
4. **Connect hardware**: When ready, follow `NIDAQMX_INTEGRATION.md`

---

## Quick Reference

| Action | How To |
|--------|--------|
| Build & Run | Press F5 in Visual Studio |
| Start Pressure | Dashboard → Pressure → Start |
| Start Temperature | Dashboard → Thermocouples → Start |
| Toggle Valve | Dashboard → Click any valve button |
| View Data | Click "Data Table" tab |
| Export Data | File → Export Data → Export to CSV |
| Stop Everything | Click red Stop buttons |

---

## Summary

**To see the application:**
1. Open in Visual Studio and press F5, OR
2. Run `dotnet run` from command line
3. The main window opens with Dashboard tab
4. Click green Start buttons to see simulated data
5. All features work in simulation mode!

**You don't need any hardware to see and test the application!**

---

For more detailed information, see:
- `README.md` - Full documentation
- `USER_GUIDE.md` - Comprehensive user manual
- `QUICKSTART.md` - 5-minute tutorial
- `RELEASE_NOTES.md` - What's new in Version 2.0
