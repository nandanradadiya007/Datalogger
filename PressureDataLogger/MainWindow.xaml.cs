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

    public MainWindow()
    {
        InitializeComponent();

        // Initialize services
        _daqManager = new DAQManager();
        _csvLogger = new CSVLogger();
        _recentSamples = new ObservableCollection<string>();

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

        UpdateStatus("Ready - Configure DAQ settings and click Start Acquisition");
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
}