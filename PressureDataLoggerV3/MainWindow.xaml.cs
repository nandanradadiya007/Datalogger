using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using PressureDataLogger.Services;
using PressureDataLogger.Models;
using PressureDataLogger.Interfaces;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;

namespace PressureDataLogger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly IDAQManager _daqManager;
    private readonly ICSVLogger _csvLogger;
    private readonly ObservableCollection<string> _recentSamples;
    private int _sampleCount = 0;
    private readonly List<ValveState> _valves;
    private readonly List<TemperatureSample> _temperatureSamples;
    private readonly Dictionary<string, CalibrationData> _calibrations;
    private bool _temperatureAcquisitionRunning = false;
    private System.Threading.Timer? _temperatureTimer;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize services
        _daqManager = new DAQManager();
        _csvLogger = new CSVLogger();
        _recentSamples = new ObservableCollection<string>();
        _valves = new List<ValveState>();
        _temperatureSamples = new List<TemperatureSample>();
        _calibrations = new Dictionary<string, CalibrationData>();

        // Initialize valves
        for (int i = 1; i <= 4; i++)
        {
            _valves.Add(new ValveState($"V{i}", $"Valve {i}", false, $"Lab valve {i}"));
        }

        // Wire up DAQ event handlers
        _daqManager.DataAcquired += OnDataAcquired;
        _daqManager.ErrorOccurred += OnErrorOccurred;

        // Wire up Menu Bar events
        menuBar.FileNew += (s, e) => MenuFileNew_Click(s, e);
        menuBar.FileOpen += (s, e) => MenuFileOpen_Click(s, e);
        menuBar.FileSave += (s, e) => MenuFileSave_Click(s, e);
        menuBar.FileSaveAs += (s, e) => MenuFileSaveAs_Click(s, e);
        menuBar.ExportCsv += (s, e) => MenuExportCsv_Click(s, e);
        menuBar.ExportExcel += (s, e) => MenuExportExcel_Click(s, e);
        menuBar.ExportJson += (s, e) => MenuExportJson_Click(s, e);
        menuBar.FileExit += (s, e) => MenuFileExit_Click(s, e);
        menuBar.EditConfig += (s, e) => MenuEditConfig_Click(s, e);
        menuBar.EditClear += (s, e) => MenuEditClear_Click(s, e);
        menuBar.ViewDashboard += (s, e) => MenuViewDashboard_Click(s, e);
        menuBar.ViewGraphs += (s, e) => MenuViewGraphs_Click(s, e);
        menuBar.ViewDataTable += (s, e) => MenuViewDataTable_Click(s, e);
        menuBar.ViewCalibrations += (s, e) => MenuViewCalibrations_Click(s, e);
        menuBar.ToolsDeviceConfig += (s, e) => MenuToolsDeviceConfig_Click(s, e);
        menuBar.ToolsCalibration += (s, e) => MenuToolsCalibration_Click(s, e);
        menuBar.ToolsValves += (s, e) => MenuToolsValves_Click(s, e);
        menuBar.ToolsSettings += (s, e) => MenuToolsSettings_Click(s, e);
        menuBar.HelpGuide += (s, e) => MenuHelpGuide_Click(s, e);
        menuBar.HelpAbout += (s, e) => MenuHelpAbout_Click(s, e);

        // Wire up Toolbar events
        toolBar.NewSession += (s, e) => MenuFileNew_Click(s, e);
        toolBar.OpenSession += (s, e) => MenuFileOpen_Click(s, e);
        toolBar.SaveSession += (s, e) => MenuFileSave_Click(s, e);
        toolBar.ShowDashboard += (s, e) => MenuViewDashboard_Click(s, e);
        toolBar.ShowGraphs += (s, e) => MenuViewGraphs_Click(s, e);
        toolBar.OpenCalibration += (s, e) => MenuToolsCalibration_Click(s, e);
        toolBar.StartAllAcquisition += (s, e) => ToolbarStartAll_Click(s, e);
        toolBar.StopAllAcquisition += (s, e) => ToolbarStopAll_Click(s, e);

        // Wire up Dashboard events
        dashboardControl.PressureStartRequested += (s, e) => BtnStart_Click(s, new RoutedEventArgs());
        dashboardControl.PressureStopRequested += (s, e) => BtnStop_Click(s, new RoutedEventArgs());
        dashboardControl.TemperatureStartRequested += (s, e) => BtnStartTemp_Click(s, new RoutedEventArgs());
        dashboardControl.TemperatureStopRequested += (s, e) => BtnStopTemp_Click(s, new RoutedEventArgs());
        dashboardControl.ValveToggleRequested += (s, valveId) => HandleValveToggle(valveId);
        dashboardControl.DeviceSelectionChanged += (s, deviceName) => OnDeviceSelected(deviceName);
        dashboardControl.RefreshDevicesRequested += (s, e) => RefreshDeviceList();

        // Wire up Data Table events
        dataTableControl.ExportToCsvRequested += (s, e) => MenuExportCsv_Click(s, e);
        dataTableControl.ClearDataRequested += (s, e) => MenuEditClear_Click(s, e);

        // Wire up Calibrations events
        calibrationsControl.NewCalibrationRequested += (s, e) => MenuToolsCalibration_Click(s, e);
        calibrationsControl.LoadCalibrationRequested += (s, e) => BtnLoadCalibration_Click(s, e);
        calibrationsControl.SaveCalibrationRequested += (s, e) => BtnSaveCalibration_Click(s, e);

        // Wire up CSV Logging events
        csvLoggingControl.BrowseRequested += (s, e) => BtnBrowse_Click(s, e);
        csvLoggingControl.StartLoggingRequested += (s, e) => BtnStartLogging_Click(s, e);
        csvLoggingControl.StopLoggingRequested += (s, e) => BtnStopLogging_Click(s, e);

        // Set up UI
        dataTableControl.SetDataSource(_recentSamples);
        
        // Set default log file path
        string defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            AppConfiguration.DefaultLogFileName);
        csvLoggingControl.LogFilePath = defaultPath;

        // Populate the device dropdown on startup
        RefreshDeviceList();

        UpdateStatus("Ready - Lab Data Acquisition System");
    }

    /// <summary>
    /// Start DAQ acquisition
    /// </summary>
    private void BtnStart_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(dashboardControl.DeviceChannel))
            {
                MessageBox.Show("Please enter a device/channel (e.g., Dev1/ai0)", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(dashboardControl.SampleRate, out int sampleRate) || sampleRate <= 0)
            {
                MessageBox.Show("Please enter a valid sample rate (positive integer)", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(dashboardControl.SamplesPerRead, out int samplesPerRead) || samplesPerRead <= 0)
            {
                MessageBox.Show("Please enter a valid samples per read (positive integer)", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Clear previous data
            _recentSamples.Clear();
            _sampleCount = 0;

            // Start acquisition
            _daqManager.Start(dashboardControl.DeviceChannel, sampleRate, samplesPerRead);

            // Update UI
            dashboardControl.SetPressureAcquisitionRunning(true);

            UpdateStatus($"Acquisition started - {dashboardControl.DeviceChannel} @ {sampleRate} Hz");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start acquisition: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            UpdateStatus($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Stop DAQ acquisition
    /// </summary>
    private void BtnStop_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            _daqManager.Stop();

            // Update UI
            dashboardControl.SetPressureAcquisitionRunning(false);

            UpdateStatus($"Acquisition stopped - Total samples: {_sampleCount}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error stopping acquisition: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Browse for log file location
    /// </summary>
    private void BtnBrowse_Click(object? sender, EventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            DefaultExt = ".csv",
            FileName = AppConfiguration.DefaultLogFileName,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (saveDialog.ShowDialog() == true)
        {
            csvLoggingControl.LogFilePath = saveDialog.FileName;
        }
    }

    /// <summary>
    /// Start CSV logging
    /// </summary>
    private void BtnStartLogging_Click(object? sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(csvLoggingControl.LogFilePath))
            {
                MessageBox.Show("Please select a log file path", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _csvLogger.StartLogging(csvLoggingControl.LogFilePath);

            // Update UI
            csvLoggingControl.SetLoggingState(true);

            UpdateStatus($"Logging started - {csvLoggingControl.LogFilePath}");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to start logging: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Stop CSV logging
    /// </summary>
    private void BtnStopLogging_Click(object? sender, EventArgs e)
    {
        try
        {
            _csvLogger.StopLogging();

            // Update UI
            csvLoggingControl.SetLoggingState(false);

            UpdateStatus("Logging stopped");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error stopping logging: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Handle new data from DAQ
    /// </summary>
    private void OnDataAcquired(object? sender, PressureSample sample)
    {
        // Marshal to UI thread
        Dispatcher.Invoke(() =>
        {
            // Update latest value in dashboard
            dashboardControl.UpdateLatestPressureValue($"{sample.Value:F6} V");

            // Add to recent samples list (keep last N samples)
            _recentSamples.Insert(0, sample.ToString());
            if (_recentSamples.Count > AppConfiguration.MaxDisplayedSamples)
            {
                _recentSamples.RemoveAt(_recentSamples.Count - 1);
            }

            _sampleCount++;
        });

        // Log to CSV (async, non-blocking)
        if (_csvLogger.IsLogging)
        {
            _ = _csvLogger.LogSampleAsync(sample);
        }
    }

    /// <summary>
    /// Handle DAQ errors
    /// </summary>
    private void OnErrorOccurred(object? sender, string error)
    {
        Dispatcher.Invoke(() =>
        {
            UpdateStatus($"Error: {error}");
            MessageBox.Show(error, "DAQ Error", MessageBoxButton.OK, MessageBoxImage.Error);
        });
    }

    /// <summary>
    /// Update status bar
    /// </summary>
    private void UpdateStatus(string message)
    {
        txtStatus.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
    }

    /// <summary>
    /// Clean up on window closing
    /// </summary>
    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try
        {
            // Stop acquisition and logging
            if (_daqManager.IsRunning)
            {
                _daqManager.Stop();
            }

            if (_csvLogger.IsLogging)
            {
                _csvLogger.StopLogging();
            }

            // Stop temperature acquisition
            _temperatureTimer?.Dispose();

            // Dispose services
            if (_daqManager is IDisposable daqDisposable)
            {
                daqDisposable.Dispose();
            }

            if (_csvLogger is IDisposable loggerDisposable)
            {
                loggerDisposable.Dispose();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error during cleanup: {ex.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    // ============= Menu Handlers =============

    private void MenuFileNew_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Create new session? This will clear all current data.", "New Session", 
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            _recentSamples.Clear();
            _sampleCount = 0;
            _temperatureSamples.Clear();
            UpdateStatus("New session created");
        }
    }

    private void MenuFileOpen_Click(object? sender, EventArgs e)
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "Session files (*.json)|*.json|All files (*.*)|*.*",
            Title = "Open Session"
        };

        if (openDialog.ShowDialog() == true)
        {
            try
            {
                // TODO: Implement session loading
                UpdateStatus($"Session opened: {openDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open session: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void MenuFileSave_Click(object? sender, EventArgs e)
    {
        // TODO: Implement session save
        UpdateStatus("Session saved");
    }

    private void MenuFileSaveAs_Click(object? sender, EventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "Session files (*.json)|*.json|All files (*.*)|*.*",
            DefaultExt = ".json",
            Title = "Save Session As"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                // TODO: Implement session save as
                UpdateStatus($"Session saved: {saveDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save session: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void MenuExportCsv_Click(object? sender, EventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv",
            DefaultExt = ".csv",
            Title = "Export Data to CSV"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                File.WriteAllLines(saveDialog.FileName, _recentSamples);
                MessageBox.Show($"Data exported successfully to:\n{saveDialog.FileName}", "Export Complete", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateStatus($"Data exported: {saveDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void MenuExportExcel_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Excel export functionality will be available in a future update.", 
            "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuExportJson_Click(object? sender, EventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "JSON files (*.json)|*.json",
            DefaultExt = ".json",
            Title = "Export Data to JSON"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                var jsonData = JsonSerializer.Serialize(_recentSamples, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(saveDialog.FileName, jsonData);
                MessageBox.Show($"Data exported successfully to:\n{saveDialog.FileName}", "Export Complete", 
                    MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateStatus($"Data exported: {saveDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to export data: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void MenuFileExit_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void MenuEditConfig_Click(object? sender, EventArgs e)
    {
        mainTabControl.SelectedItem = tabDashboard;
        UpdateStatus("Showing configuration");
    }

    private void MenuEditClear_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Clear all data?", "Confirm Clear", 
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            _recentSamples.Clear();
            _sampleCount = 0;
            UpdateStatus("Data cleared");
        }
    }

    private void MenuViewDashboard_Click(object? sender, EventArgs e)
    {
        mainTabControl.SelectedItem = tabDashboard;
    }

    private void MenuViewGraphs_Click(object? sender, EventArgs e)
    {
        mainTabControl.SelectedItem = tabGraphs;
    }

    private void MenuViewDataTable_Click(object? sender, EventArgs e)
    {
        mainTabControl.SelectedItem = tabDataTable;
    }

    private void MenuViewCalibrations_Click(object? sender, EventArgs e)
    {
        mainTabControl.SelectedItem = tabCalibrations;
    }

    private void MenuToolsDeviceConfig_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Device configuration window will open here.\n\n" +
            "Configure:\n• NI-DAQmx devices\n• Thermocouple channels\n• Valve assignments", 
            "Device Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuToolsCalibration_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Calibration wizard will open here.\n\n" +
            "Steps:\n1. Select sensor\n2. Apply known reference values\n3. Calculate calibration coefficients\n4. Save calibration", 
            "Calibration Wizard", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuToolsValves_Click(object? sender, EventArgs e)
    {
        mainTabControl.SelectedItem = tabDashboard;
        UpdateStatus("Showing valve controls");
    }

    private void MenuToolsSettings_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Application settings window will open here.\n\n" +
            "Configure:\n• Default sample rates\n• Display preferences\n• File paths\n• Units", 
            "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuHelpGuide_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Lab Data Acquisition System - User Guide\n\n" +
            "Features:\n" +
            "• Pressure transducer data acquisition\n" +
            "• Thermocouple temperature monitoring\n" +
            "• Valve control system\n" +
            "• Real-time data visualization\n" +
            "• Calibration management\n" +
            "• Data export in multiple formats\n\n" +
            "For detailed documentation, see README.md", 
            "User Guide", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuHelpAbout_Click(object? sender, EventArgs e)
    {
        MessageBox.Show("Lab Data Acquisition System\n" +
            "Version 2.0\n\n" +
            "A comprehensive multi-sensor data acquisition application\n" +
            "for laboratory environments.\n\n" +
            "Features:\n" +
            "• NI-DAQmx integration for pressure transducers\n" +
            "• Thermocouple temperature monitoring\n" +
            "• Valve control and monitoring\n" +
            "• Real-time graphing and data logging\n" +
            "• Calibration management\n\n" +
            "Built with .NET 6.0 and WPF", 
            "About", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // ============= Toolbar Handlers =============

    private void ToolbarStartAll_Click(object? sender, EventArgs e)
    {
        BtnStart_Click(sender, new RoutedEventArgs());
        BtnStartTemp_Click(sender, new RoutedEventArgs());
    }

    private void ToolbarStopAll_Click(object? sender, EventArgs e)
    {
        BtnStop_Click(sender, new RoutedEventArgs());
        BtnStopTemp_Click(sender, new RoutedEventArgs());
    }

    // ============= Tab Control Handler =============

    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string activeView = "";
        if (mainTabControl.SelectedItem == tabDashboard)
        {
            activeView = "Dashboard";
        }
        else if (mainTabControl.SelectedItem == tabGraphs)
        {
            activeView = "Graphs";
        }
        else if (mainTabControl.SelectedItem == tabDataTable)
        {
            activeView = "DataTable";
        }
        else if (mainTabControl.SelectedItem == tabCalibrations)
        {
            activeView = "Calibrations";
        }
        
        menuBar.UpdateViewCheckmarks(activeView);
    }

    // ============= Temperature Control Handlers =============

    private void BtnStartTemp_Click(object? sender, RoutedEventArgs e)
    {
        if (_temperatureAcquisitionRunning) return;

        _temperatureAcquisitionRunning = true;
        dashboardControl.SetTemperatureAcquisitionRunning(true);

        // Simulate temperature acquisition with a timer
        _temperatureTimer = new System.Threading.Timer(_ =>
        {
            SimulateTemperatureData();
        }, null, 0, 1000); // Update every second

        UpdateStatus("Temperature acquisition started");
    }

    private void BtnStopTemp_Click(object? sender, RoutedEventArgs e)
    {
        if (!_temperatureAcquisitionRunning) return;

        _temperatureAcquisitionRunning = false;
        _temperatureTimer?.Dispose();
        _temperatureTimer = null;

        Dispatcher.Invoke(() =>
        {
            dashboardControl.SetTemperatureAcquisitionRunning(false);
        });

        UpdateStatus("Temperature acquisition stopped");
    }

    private void SimulateTemperatureData()
    {
        var random = new Random();
        var baseTemp = 25.0; // Room temperature base

        Dispatcher.Invoke(() =>
        {
            dashboardControl.UpdateTemperatureValues(
                $"{baseTemp + random.NextDouble() * 10:F1}°C",
                $"{baseTemp + 5 + random.NextDouble() * 10:F1}°C",
                $"{baseTemp + 10 + random.NextDouble() * 10:F1}°C",
                $"{baseTemp + 15 + random.NextDouble() * 10:F1}°C"
            );
        });
    }

    // ============= Valve Control Handlers =============

    private void HandleValveToggle(int valveId)
    {
        var valve = _valves[valveId - 1];
        
        valve.IsOpen = !valve.IsOpen;
        valve.LastChanged = DateTime.Now;

        dashboardControl.UpdateValveState(valveId, valve);

        UpdateStatus($"Valve {valveId} {valve.Status}");
    }

    // ============= Calibration Handlers =============

    private void BtnLoadCalibration_Click(object? sender, EventArgs e)
    {
        var openDialog = new OpenFileDialog
        {
            Filter = "Calibration files (*.cal)|*.cal|JSON files (*.json)|*.json",
            Title = "Load Calibration"
        };

        if (openDialog.ShowDialog() == true)
        {
            try
            {
                // TODO: Implement calibration loading
                UpdateStatus($"Calibration loaded: {openDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load calibration: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    private void BtnSaveCalibration_Click(object? sender, EventArgs e)
    {
        var saveDialog = new SaveFileDialog
        {
            Filter = "Calibration files (*.cal)|*.cal|JSON files (*.json)|*.json",
            DefaultExt = ".cal",
            Title = "Save Calibration"
        };

        if (saveDialog.ShowDialog() == true)
        {
            try
            {
                // TODO: Implement calibration saving
                UpdateStatus($"Calibration saved: {saveDialog.FileName}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save calibration: {ex.Message}", "Error", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    // ============= Device Discovery Helpers =============

    /// <summary>
    /// Re-enumerates connected DAQ devices and loads them into the dashboard dropdown.
    /// </summary>
    private void RefreshDeviceList()
    {
        try
        {
            var devices = _daqManager.GetAvailableDevices();
            dashboardControl.LoadDevices(devices);
            UpdateStatus($"Found {devices.Count} device(s)");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Device enumeration error: {ex.Message}");
        }
    }

    /// <summary>
    /// Called when the user selects a device in the dropdown.
    /// Loads the device type and its analog-input channels.
    /// </summary>
    private void OnDeviceSelected(string deviceName)
    {
        try
        {
            var deviceType = _daqManager.GetDeviceType(deviceName);
            var channels   = _daqManager.GetDeviceChannels(deviceName);
            dashboardControl.LoadChannels(channels, deviceType);
            UpdateStatus($"Device '{deviceName}' selected – {deviceType} – {channels.Count} channel(s)");
        }
        catch (Exception ex)
        {
            UpdateStatus($"Error loading device info: {ex.Message}");
        }
    }
}