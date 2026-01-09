# Architecture Overview

## Application Architecture

The Pressure Data Logger follows a layered architecture pattern with clear separation of concerns.

### Layer Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                      │
│  ┌───────────────────────────────────────────────────────┐  │
│  │              MainWindow (WPF)                         │  │
│  │  - User Interface                                     │  │
│  │  - Input Validation                                   │  │
│  │  - Display Updates                                    │  │
│  │  - Event Handling                                     │  │
│  └───────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ uses
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                      Service Layer                          │
│  ┌─────────────────────┐        ┌─────────────────────┐    │
│  │   DAQManager        │        │    CSVLogger        │    │
│  │  - NI-DAQmx Comm   │        │  - File I/O         │    │
│  │  - Data Acquisition │        │  - Async Writes     │    │
│  │  - Thread Management│        │  - Buffering        │    │
│  └─────────────────────┘        └─────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ uses
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                       Model Layer                           │
│  ┌─────────────────────┐        ┌─────────────────────┐    │
│  │  PressureSample     │        │ AppConfiguration    │    │
│  │  - Data Structure   │        │ - Constants         │    │
│  │  - Timestamp        │        │ - Defaults          │    │
│  │  - Value            │        │                     │    │
│  └─────────────────────┘        └─────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                            │
                            │ uses
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    Interface Layer                          │
│  ┌─────────────────────┐        ┌─────────────────────┐    │
│  │   IDAQManager       │        │   ICSVLogger        │    │
│  │  - Abstractions     │        │  - Abstractions     │    │
│  │  - Events           │        │  - Contract         │    │
│  └─────────────────────┘        └─────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
```

## Component Interactions

### Data Flow

```
┌──────────────┐
│  NI-DAQmx    │  Hardware Layer
│   Hardware   │
└──────┬───────┘
       │ Voltage Samples
       ↓
┌──────────────┐
│  DAQManager  │  Acquisition Service
│  (Service)   │  - Reads voltage data
└──────┬───────┘  - Creates PressureSample objects
       │          - Fires DataAcquired events
       │ Events
       ├─────────────────────────┐
       ↓                         ↓
┌──────────────┐          ┌──────────────┐
│  MainWindow  │          │  CSVLogger   │  Logging Service
│     (UI)     │          │  (Service)   │  - Async file writes
└──────────────┘          └──────────────┘  - Buffering
   Display Data              Save to CSV
```

### Threading Model

```
┌───────────────────────────────────────────────────────────┐
│                      Main Thread (UI)                     │
│  - Handles user input                                     │
│  - Updates display                                        │
│  - Processes events from background threads               │
└───────────────────────────────────────────────────────────┘
                            ↑
                            │ Dispatcher.Invoke()
                            │
┌───────────────────────────────────────────────────────────┐
│                  Background Thread (DAQ)                  │
│  - Reads data from NI-DAQmx                              │
│  - Continuous acquisition loop                            │
│  - Fires DataAcquired events                             │
└───────────────────────────────────────────────────────────┘
                            │
                            │ Async calls
                            ↓
┌───────────────────────────────────────────────────────────┐
│                 Thread Pool (Logging)                     │
│  - Async CSV writes                                       │
│  - Non-blocking I/O                                       │
│  - Buffered writes                                        │
└───────────────────────────────────────────────────────────┘
```

## Key Design Patterns

### 1. Interface Segregation
- `IDAQManager`: Defines acquisition contract
- `ICSVLogger`: Defines logging contract
- Enables testing and future extensibility

### 2. Observer Pattern
- DAQManager fires events for new data
- MainWindow subscribes to data events
- CSVLogger subscribes to data events
- Loose coupling between components

### 3. Single Responsibility
- **DAQManager**: Only handles data acquisition
- **CSVLogger**: Only handles file I/O
- **MainWindow**: Only handles UI logic
- **Models**: Only hold data structures

### 4. Dependency Injection (Constructor-based)
```csharp
public MainWindow()
{
    _daqManager = new DAQManager();
    _csvLogger = new CSVLogger();
    // Services are injected at construction
}
```

## Component Details

### MainWindow (UI Layer)

**Responsibilities:**
- Display configuration controls
- Validate user input
- Start/stop acquisition
- Start/stop logging
- Display real-time data
- Handle errors and status updates

**Key Methods:**
- `BtnStart_Click()`: Starts acquisition
- `BtnStop_Click()`: Stops acquisition
- `OnDataAcquired()`: Handles new data from DAQ
- `OnErrorOccurred()`: Handles errors from DAQ

### DAQManager (Service Layer)

**Responsibilities:**
- Initialize NI-DAQmx tasks
- Configure analog input channels
- Read data continuously
- Manage acquisition thread
- Handle hardware errors

**Key Methods:**
- `Start()`: Initializes and starts acquisition
- `Stop()`: Stops and cleans up resources
- `AcquisitionLoop()`: Background data reading
- `OnDataAcquired()`: Fires data event

**Threading:**
- Runs on dedicated background thread
- Thread-safe event firing
- Proper cleanup on stop

### CSVLogger (Service Layer)

**Responsibilities:**
- Open/close CSV files
- Write headers
- Async data writes
- Buffer management
- File I/O error handling

**Key Methods:**
- `StartLogging()`: Opens CSV file
- `StopLogging()`: Closes CSV file
- `LogSampleAsync()`: Writes data asynchronously

**Performance:**
- Uses `SemaphoreSlim` for thread safety
- Async writes for non-blocking I/O
- Periodic flushing for data safety

### Models

**PressureSample:**
- Timestamp: When sample was acquired
- Value: Voltage reading
- ToString(): Formatted output

**AppConfiguration:**
- Static constants for defaults
- Centralized configuration
- Easy to modify

## Data Flow Example

### Complete Acquisition Cycle

```
1. User clicks "Start Acquisition"
   ↓
2. MainWindow.BtnStart_Click()
   - Validates inputs
   - Calls _daqManager.Start()
   ↓
3. DAQManager.Start()
   - Creates NI-DAQmx task (or stub)
   - Configures channels and timing
   - Starts background thread
   ↓
4. DAQManager.AcquisitionLoop() [Background Thread]
   - Reads samples from hardware
   - Creates PressureSample objects
   - Fires DataAcquired events
   ↓
5. MainWindow.OnDataAcquired() [UI Thread via Dispatcher]
   - Updates latest value display
   - Adds to recent samples list
   - Calls CSVLogger if logging enabled
   ↓
6. CSVLogger.LogSampleAsync() [Thread Pool]
   - Writes to CSV file asynchronously
   - Returns immediately (non-blocking)
   ↓
7. Loop continues until user clicks "Stop Acquisition"
```

## Error Handling Strategy

### Layers of Error Handling

```
┌─────────────────────────────────────────────┐
│ UI Layer (MainWindow)                       │
│ - Catches all exceptions                    │
│ - Shows user-friendly messages              │
│ - Updates status bar                        │
└─────────────────────────────────────────────┘
                    ↑
                    │ propagates
                    │
┌─────────────────────────────────────────────┐
│ Service Layer (DAQManager, CSVLogger)       │
│ - Catches specific exceptions              │
│ - Fires ErrorOccurred events               │
│ - Attempts graceful recovery               │
└─────────────────────────────────────────────┘
                    ↑
                    │ throws
                    │
┌─────────────────────────────────────────────┐
│ External (NI-DAQmx, File System)           │
│ - DaqException (hardware errors)           │
│ - IOException (file errors)                │
└─────────────────────────────────────────────┘
```

## Resource Management

### Lifecycle Management

```
Application Start
    ↓
Initialize Services (Constructor)
    - Create DAQManager
    - Create CSVLogger
    - Wire up events
    ↓
Running State
    - Acquisition may start/stop
    - Logging may start/stop
    ↓
Application Close (Window_Closing)
    - Stop acquisition if running
    - Stop logging if running
    - Dispose DAQManager
    - Dispose CSVLogger
    ↓
Application Exit
```

### IDisposable Pattern

Both service classes implement IDisposable:
- DAQManager: Stops threads, cleans up NI-DAQmx tasks
- CSVLogger: Closes files, releases locks

## Extensibility Points

### Easy to Add

1. **Multiple Channels**: Modify DAQManager to support multiple AI channels
2. **Data Processing**: Add processing pipeline between acquisition and display
3. **Unit Conversion**: Extend PressureSample with pressure units
4. **Database Logging**: Implement ILogger for database writes
5. **Real-time Plotting**: Add charting control to MainWindow
6. **Alarm Conditions**: Add threshold checking in OnDataAcquired

### Example: Adding Pressure Conversion

```csharp
// In PressureSample.cs
public double PressurePSI => Value * 10.0;  // Convert voltage to PSI

// In MainWindow.xaml
<TextBlock Text="{Binding PressurePSI, StringFormat={0:F2} PSI}"/>
```

## Testing Strategy

### Unit Testing
- Test models in isolation
- Mock IDAQManager for UI testing
- Mock ICSVLogger for acquisition testing

### Integration Testing
- Test DAQManager with simulation stubs
- Test CSVLogger with temp files
- Test UI with mock services

### Hardware Testing
- Test with actual NI-DAQmx devices
- Verify data accuracy
- Performance testing under load

## Performance Considerations

### Bottlenecks to Monitor

1. **UI Updates**: Dispatcher calls can be expensive
   - Solution: Update at limited rate (e.g., 10 Hz max)

2. **CSV Writes**: Disk I/O can be slow
   - Solution: Async writes, buffering (already implemented)

3. **Memory Usage**: Large sample history
   - Solution: Limit displayed samples (already implemented)

4. **Thread Synchronization**: Lock contention
   - Solution: Minimize critical sections

### Typical Performance

- **Sample Rate**: Up to 10,000+ Hz (hardware dependent)
- **UI Update Rate**: ~10-30 Hz (adequate for display)
- **CSV Write Rate**: Limited by disk speed (~1000s of samples/sec)
- **Memory Usage**: ~50 MB typical, ~100 MB with large history

## Security Considerations

### Current Implementation

- ✅ Input validation (sample rate, samples per read)
- ✅ Safe file I/O (directory creation, error handling)
- ✅ No network exposure
- ✅ No credential storage

### Future Enhancements

- Consider file encryption for sensitive data
- Add audit logging for compliance
- Implement access controls if networked

## Conclusion

This architecture provides:
- ✅ Clean separation of concerns
- ✅ Testable components
- ✅ Extensible design
- ✅ Maintainable codebase
- ✅ Good performance
- ✅ Robust error handling

The design is ready for both simulation testing and real hardware integration.
