# UI Layout Reference

This document provides a visual reference of the Pressure Data Logger UI layout.

## Main Window Layout

```
┌────────────────────────────────────────────────────────────────────────────┐
│ Pressure Data Logger - NI-DAQmx                                      [_][□][X]│
├────────────────────────────────────────────────────────────────────────────┤
│                                                                            │
│  Pressure Data Logger - NI-DAQmx                                          │
│  ══════════════════════════════════════                                   │
│                                                                            │
│  ┌─ DAQ Configuration ─────────────────────────────────────────────────┐  │
│  │                                                                      │  │
│  │  Device/Channel:    [Dev1/ai0            ]                          │  │
│  │                                                                      │  │
│  │  Sample Rate (Hz):  [1000                ]                          │  │
│  │                                                                      │  │
│  │  Samples Per Read:  [100                 ]                          │  │
│  │                                                                      │  │
│  │  [ Start Acquisition ]  [ Stop Acquisition ]                        │  │
│  │    (Green button)          (Red button)                             │  │
│  │                                                                      │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                            │
│  ┌─ CSV Logging ────────────────────────────────────────────────────────┐  │
│  │                                                                      │  │
│  │  Log File Path:  [C:\Users\...\PressureData.csv] [ Browse... ]     │  │
│  │                                                                      │  │
│  │  [ Start Logging ]  [ Stop Logging ]                                │  │
│  │   (Blue button)      (Orange button)                                │  │
│  │                                                                      │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                            │
│  ┌─ Data Display ───────────────────────────────────────────────────────┐  │
│  │                                                                      │  │
│  │  ┌─────────────────────────────────────────────────────────────────┐│  │
│  │  │ Latest Value:                                                   ││  │
│  │  │                                                                  ││  │
│  │  │      5.234567 V                                                 ││  │
│  │  │                                                                  ││  │
│  │  └─────────────────────────────────────────────────────────────────┘│  │
│  │                                                                      │  │
│  │  Recent Samples:                                                    │  │
│  │  ┌────────────────────────────────────────────────────────────────┐ │  │
│  │  │ 2024-01-09 14:30:15.123, 5.234567                             │ │  │
│  │  │ 2024-01-09 14:30:15.124, 5.189234                             │ │  │
│  │  │ 2024-01-09 14:30:15.125, 4.987654                             │ │  │
│  │  │ 2024-01-09 14:30:15.126, 5.123456                             │ │  │
│  │  │ 2024-01-09 14:30:15.127, 5.098765                             │ │  │
│  │  │ 2024-01-09 14:30:15.128, 5.234567                             │ │  │
│  │  │ 2024-01-09 14:30:15.129, 5.176543                             │ │  │
│  │  │ 2024-01-09 14:30:15.130, 4.998765                             │ │  │
│  │  │ ...                                                             │↕│  │
│  │  └────────────────────────────────────────────────────────────────┘ │  │
│  │                                                                      │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                                                            │
├────────────────────────────────────────────────────────────────────────────┤
│ 14:30:15 - Acquisition started - Dev1/ai0 @ 1000 Hz                       │
└────────────────────────────────────────────────────────────────────────────┘
```

## UI Elements Description

### Title Bar
- **Application Name**: "Pressure Data Logger - NI-DAQmx"
- **Standard Windows Controls**: Minimize, Maximize, Close

### DAQ Configuration Panel
```
┌─────────────────────────────────────────────────┐
│ Device/Channel:     [Text Input]               │
│ - Default: Dev1/ai0                            │
│ - User can enter any valid NI device/channel   │
│                                                │
│ Sample Rate (Hz):   [Text Input]               │
│ - Default: 1000                                 │
│ - Accepts positive integers                     │
│                                                │
│ Samples Per Read:   [Text Input]               │
│ - Default: 100                                  │
│ - Accepts positive integers                     │
│                                                │
│ [Start Acquisition] [Stop Acquisition]         │
│  - Start: Green background, enabled initially   │
│  - Stop: Red background, disabled initially     │
│  - Toggle state when clicked                    │
└─────────────────────────────────────────────────┘
```

### CSV Logging Panel
```
┌─────────────────────────────────────────────────┐
│ Log File Path: [Read-only display] [Browse]    │
│ - Shows selected file path                      │
│ - Browse opens SaveFileDialog                   │
│ - Default: Documents\PressureData.csv          │
│                                                │
│ [Start Logging] [Stop Logging]                 │
│  - Start: Blue background, enabled initially    │
│  - Stop: Orange background, disabled initially  │
│  - Toggle state when clicked                    │
└─────────────────────────────────────────────────┘
```

### Data Display Panel
```
┌─────────────────────────────────────────────────┐
│ ┌─ Latest Value ─────────────────────────────┐ │
│ │ Latest Value:                              │ │
│ │                                            │ │
│ │   5.234567 V                               │ │
│ │   (Large, blue text)                       │ │
│ │   (Updates in real-time)                   │ │
│ └────────────────────────────────────────────┘ │
│                                                │
│ Recent Samples:                                │
│ ┌──────────────────────────────────────────┐  │
│ │ [ListBox with scrollbar]                 │  │
│ │ - Shows last 100 samples                 │  │
│ │ - Newest at top                          │  │
│ │ - Format: timestamp, value               │  │
│ │ - Monospace font (Consolas)              │  │
│ └──────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

### Status Bar
```
┌─────────────────────────────────────────────────┐
│ [Timestamp] - [Status Message]                 │
│ Examples:                                       │
│ - "Ready"                                       │
│ - "Acquisition started - Dev1/ai0 @ 1000 Hz"   │
│ - "Logging started - C:\path\to\file.csv"      │
│ - "Error: Failed to start acquisition"         │
└─────────────────────────────────────────────────┘
```

## Color Scheme

### Buttons
- **Start Acquisition**: #4CAF50 (Green) - Success/Go action
- **Stop Acquisition**: #F44336 (Red) - Stop/Danger action
- **Start Logging**: #2196F3 (Blue) - Primary action
- **Stop Logging**: #FF9800 (Orange) - Warning/Pause action
- **Browse**: Default gray

### Display Areas
- **Latest Value Box**: Light blue background (#F0F8FF)
- **Border**: Blue (#2196F3)
- **Text**: Blue (#2196F3)

### Status Bar
- **Background**: Light gray (#F5F5F5)
- **Border**: Gray (#CCCCCC)
- **Text**: Black

## UI States

### Initial State (Ready)
```
Configuration Panel:
  ✓ All inputs enabled
  ✓ Start Acquisition: Enabled
  ✗ Stop Acquisition: Disabled

Logging Panel:
  ✓ Browse: Enabled
  ✓ Start Logging: Enabled
  ✗ Stop Logging: Disabled

Data Display:
  - Latest Value: "-- V"
  - Sample List: Empty

Status: "Ready"
```

### During Acquisition
```
Configuration Panel:
  ✗ All inputs disabled (locked)
  ✗ Start Acquisition: Disabled
  ✓ Stop Acquisition: Enabled

Logging Panel:
  ✓ Browse: Enabled (if not logging)
  ✓ Start/Stop Logging: Active

Data Display:
  - Latest Value: Updating (e.g., "5.234567 V")
  - Sample List: Populating with data

Status: "Acquisition started - Dev1/ai0 @ 1000 Hz"
```

### During Acquisition + Logging
```
Configuration Panel:
  ✗ All inputs disabled
  ✗ Start Acquisition: Disabled
  ✓ Stop Acquisition: Enabled

Logging Panel:
  ✗ Browse: Disabled
  ✗ Start Logging: Disabled
  ✓ Stop Logging: Enabled

Data Display:
  - Latest Value: Updating
  - Sample List: Populating

Status: "Logging started - C:\path\to\file.csv"
```

## User Interactions

### Starting Acquisition
1. User enters/verifies configuration
2. User clicks "Start Acquisition"
3. Inputs become disabled
4. Start button becomes disabled
5. Stop button becomes enabled
6. Status updates
7. Data starts appearing

### Starting Logging
1. User clicks "Browse" (optional)
2. User selects file location
3. User clicks "Start Logging"
4. Browse becomes disabled
5. Start Logging becomes disabled
6. Stop Logging becomes enabled
7. Status updates
8. Data is saved to CSV

### Stopping Everything
1. User clicks "Stop Logging"
2. Logging UI resets
3. User clicks "Stop Acquisition"
4. Acquisition UI resets
5. All inputs re-enabled
6. Status shows total samples

## Error Display

### Error Dialog Box
```
┌─ Error ──────────────────────────────┐
│                                      │
│  [!]  Failed to start acquisition    │
│                                      │
│  Detailed error message appears here │
│                                      │
│           [ OK ]                     │
│                                      │
└──────────────────────────────────────┘
```

### Status Bar Error
```
Status: "Error: Failed to start acquisition - Device not found"
```

## Window Properties
- **Width**: 900 pixels
- **Height**: 600 pixels
- **Resizable**: Yes
- **Minimum Size**: 800x550
- **Default Position**: Center screen

## Font Specifications
- **Title**: 20pt, Bold
- **Labels**: 12pt, Regular
- **Input Fields**: 12pt, Regular
- **Latest Value**: 24pt, Bold
- **Sample List**: 11pt, Consolas (monospace)
- **Status Bar**: 12pt, Regular

## Accessibility Features
- Clear labels for all inputs
- Tab order follows logical flow
- Enter key submits in text boxes
- Status messages for screen readers
- High contrast color scheme
- Large click targets (buttons)

## Keyboard Shortcuts
While not explicitly implemented, standard WPF shortcuts work:
- **Tab**: Move between controls
- **Enter**: Activate focused button
- **Alt+F4**: Close window
- **Ctrl+C**: Copy from list (when focused)

## File Dialog (Browse)
```
┌─ Save As ────────────────────────────────────────┐
│                                                  │
│ Save in: [Documents              ] [▼]          │
│                                                  │
│ ┌──────────────────────────────────────────────┐ │
│ │ 📁 Desktop                                   │ │
│ │ 📁 Downloads                                 │ │
│ │ 📁 Documents                                 │ │
│ │ 📄 PressureData.csv                          │ │
│ └──────────────────────────────────────────────┘ │
│                                                  │
│ File name: [PressureData.csv      ]             │
│ Save as type: [CSV files (*.csv)  ] [▼]        │
│                                                  │
│             [ Save ]  [ Cancel ]                 │
│                                                  │
└──────────────────────────────────────────────────┘
```

## This UI Provides
- ✅ Clear visual hierarchy
- ✅ Intuitive control grouping
- ✅ Immediate feedback
- ✅ Error prevention (disabled controls)
- ✅ Status visibility
- ✅ Professional appearance
- ✅ Responsive design
- ✅ Standard Windows patterns
