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

        // Wire up event handlers
        _daqManager.DataAcquired += OnDataAcquired;
        _daqManager.ErrorOccurred += OnErrorOccurred;

        // Set up UI
        lstRecentSamples.ItemsSource = _recentSamples;
        
        // Set default log file path
        string defaultPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            AppConfiguration.DefaultLogFileName);
        txtLogFilePath.Text = defaultPath;

        UpdateStatus("Ready - Lab Data Acquisition System");
    }

    /// <summary>
    /// Start DAQ acquisition
    /// </summary>
    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(txtDeviceChannel.Text))
            {
                MessageBox.Show("Please enter a device/channel (e.g., Dev1/ai0)", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSampleRate.Text, out int sampleRate) || sampleRate <= 0)
            {
                MessageBox.Show("Please enter a valid sample rate (positive integer)", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(txtSamplesPerRead.Text, out int samplesPerRead) || samplesPerRead <= 0)
            {
                MessageBox.Show("Please enter a valid samples per read (positive integer)", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Clear previous data
            _recentSamples.Clear();
            _sampleCount = 0;

            // Start acquisition
            _daqManager.Start(txtDeviceChannel.Text, sampleRate, samplesPerRead);

            // Update UI
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;
            txtDeviceChannel.IsEnabled = false;
            txtSampleRate.IsEnabled = false;
            txtSamplesPerRead.IsEnabled = false;

            UpdateStatus($"Acquisition started - {txtDeviceChannel.Text} @ {sampleRate} Hz");
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
    private void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _daqManager.Stop();

            // Update UI
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
            txtDeviceChannel.IsEnabled = true;
            txtSampleRate.IsEnabled = true;
            txtSamplesPerRead.IsEnabled = true;

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
    private void BtnBrowse_Click(object sender, RoutedEventArgs e)
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
            txtLogFilePath.Text = saveDialog.FileName;
        }
    }

    /// <summary>
    /// Start CSV logging
    /// </summary>
    private void BtnStartLogging_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtLogFilePath.Text))
            {
                MessageBox.Show("Please select a log file path", 
                    "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _csvLogger.StartLogging(txtLogFilePath.Text);

            // Update UI
            btnStartLogging.IsEnabled = false;
            btnStopLogging.IsEnabled = true;
            btnBrowse.IsEnabled = false;

            UpdateStatus($"Logging started - {txtLogFilePath.Text}");
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
    private void BtnStopLogging_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            _csvLogger.StopLogging();

            // Update UI
            btnStartLogging.IsEnabled = true;
            btnStopLogging.IsEnabled = false;
            btnBrowse.IsEnabled = true;

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
            // Update latest value
            txtLatestValue.Text = $"{sample.Value:F6} V";

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

    private void MenuFileNew_Click(object sender, RoutedEventArgs e)
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

    private void MenuFileOpen_Click(object sender, RoutedEventArgs e)
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

    private void MenuFileSave_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Implement session save
        UpdateStatus("Session saved");
    }

    private void MenuFileSaveAs_Click(object sender, RoutedEventArgs e)
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

    private void MenuExportCsv_Click(object sender, RoutedEventArgs e)
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

    private void MenuExportExcel_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Excel export functionality will be available in a future update.", 
            "Feature Coming Soon", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuExportJson_Click(object sender, RoutedEventArgs e)
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

    private void MenuFileExit_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void MenuEditConfig_Click(object sender, RoutedEventArgs e)
    {
        mainTabControl.SelectedItem = tabDashboard;
        UpdateStatus("Showing configuration");
    }

    private void MenuEditClear_Click(object sender, RoutedEventArgs e)
    {
        if (MessageBox.Show("Clear all data?", "Confirm Clear", 
            MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            _recentSamples.Clear();
            _sampleCount = 0;
            UpdateStatus("Data cleared");
        }
    }

    private void MenuViewDashboard_Click(object sender, RoutedEventArgs e)
    {
        mainTabControl.SelectedItem = tabDashboard;
    }

    private void MenuViewGraphs_Click(object sender, RoutedEventArgs e)
    {
        mainTabControl.SelectedItem = tabGraphs;
    }

    private void MenuViewDataTable_Click(object sender, RoutedEventArgs e)
    {
        mainTabControl.SelectedItem = tabDataTable;
    }

    private void MenuViewCalibrations_Click(object sender, RoutedEventArgs e)
    {
        mainTabControl.SelectedItem = tabCalibrations;
    }

    private void MenuToolsDeviceConfig_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Device configuration window will open here.\n\n" +
            "Configure:\n• NI-DAQmx devices\n• Thermocouple channels\n• Valve assignments", 
            "Device Configuration", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuToolsCalibration_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Calibration wizard will open here.\n\n" +
            "Steps:\n1. Select sensor\n2. Apply known reference values\n3. Calculate calibration coefficients\n4. Save calibration", 
            "Calibration Wizard", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuToolsValves_Click(object sender, RoutedEventArgs e)
    {
        mainTabControl.SelectedItem = tabDashboard;
        UpdateStatus("Showing valve controls");
    }

    private void MenuToolsSettings_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Application settings window will open here.\n\n" +
            "Configure:\n• Default sample rates\n• Display preferences\n• File paths\n• Units", 
            "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void MenuHelpGuide_Click(object sender, RoutedEventArgs e)
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

    private void MenuHelpAbout_Click(object sender, RoutedEventArgs e)
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

    private void ToolbarStartAll_Click(object sender, RoutedEventArgs e)
    {
        BtnStart_Click(sender, e);
        BtnStartTemp_Click(sender, e);
    }

    private void ToolbarStopAll_Click(object sender, RoutedEventArgs e)
    {
        BtnStop_Click(sender, e);
        BtnStopTemp_Click(sender, e);
    }

    // ============= Tab Control Handler =============

    private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (mainTabControl.SelectedItem == tabDashboard)
        {
            menuViewDashboard.IsChecked = true;
            menuViewGraphs.IsChecked = false;
            menuViewDataTable.IsChecked = false;
            menuViewCalibrations.IsChecked = false;
        }
        else if (mainTabControl.SelectedItem == tabGraphs)
        {
            menuViewDashboard.IsChecked = false;
            menuViewGraphs.IsChecked = true;
            menuViewDataTable.IsChecked = false;
            menuViewCalibrations.IsChecked = false;
        }
        else if (mainTabControl.SelectedItem == tabDataTable)
        {
            menuViewDashboard.IsChecked = false;
            menuViewGraphs.IsChecked = false;
            menuViewDataTable.IsChecked = true;
            menuViewCalibrations.IsChecked = false;
        }
        else if (mainTabControl.SelectedItem == tabCalibrations)
        {
            menuViewDashboard.IsChecked = false;
            menuViewGraphs.IsChecked = false;
            menuViewDataTable.IsChecked = false;
            menuViewCalibrations.IsChecked = true;
        }
    }

    // ============= Temperature Control Handlers =============

    private void BtnStartTemp_Click(object sender, RoutedEventArgs e)
    {
        if (_temperatureAcquisitionRunning) return;

        _temperatureAcquisitionRunning = true;
        btnStartTemp.IsEnabled = false;
        btnStopTemp.IsEnabled = true;

        // Simulate temperature acquisition with a timer
        _temperatureTimer = new System.Threading.Timer(_ =>
        {
            SimulateTemperatureData();
        }, null, 0, 1000); // Update every second

        UpdateStatus("Temperature acquisition started");
    }

    private void BtnStopTemp_Click(object sender, RoutedEventArgs e)
    {
        if (!_temperatureAcquisitionRunning) return;

        _temperatureAcquisitionRunning = false;
        _temperatureTimer?.Dispose();
        _temperatureTimer = null;

        Dispatcher.Invoke(() =>
        {
            btnStartTemp.IsEnabled = true;
            btnStopTemp.IsEnabled = false;
        });

        UpdateStatus("Temperature acquisition stopped");
    }

    private void SimulateTemperatureData()
    {
        var random = new Random();
        var baseTemp = 25.0; // Room temperature base

        Dispatcher.Invoke(() =>
        {
            txtTemp1.Text = $"{baseTemp + random.NextDouble() * 10:F1}°C";
            txtTemp2.Text = $"{baseTemp + 5 + random.NextDouble() * 10:F1}°C";
            txtTemp3.Text = $"{baseTemp + 10 + random.NextDouble() * 10:F1}°C";
            txtTemp4.Text = $"{baseTemp + 15 + random.NextDouble() * 10:F1}°C";
        });
    }

    // ============= Valve Control Handlers =============

    private void BtnValve_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string valveIdStr)
        {
            int valveId = int.Parse(valveIdStr);
            var valve = _valves[valveId - 1];
            
            valve.IsOpen = !valve.IsOpen;
            valve.LastChanged = DateTime.Now;

            btn.Content = valve.Status;
            btn.Background = valve.IsOpen ? 
                new SolidColorBrush(Color.FromRgb(76, 175, 80)) : // Green
                new SolidColorBrush(Color.FromRgb(204, 204, 204)); // Gray

            UpdateStatus($"Valve {valveId} {valve.Status}");
        }
    }

    // ============= Calibration Handlers =============

    private void BtnLoadCalibration_Click(object sender, RoutedEventArgs e)
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

    private void BtnSaveCalibration_Click(object sender, RoutedEventArgs e)
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
}