using System.Windows;
using System.Windows.Controls;

namespace PressureDataLogger.Views;

public partial class CsvLoggingControl : UserControl
{
    // Events
    public event EventHandler? BrowseRequested;
    public event EventHandler? StartLoggingRequested;
    public event EventHandler? StopLoggingRequested;

    public CsvLoggingControl()
    {
        InitializeComponent();
    }

    // Public properties to access UI values
    public string LogFilePath
    {
        get => txtLogFilePath.Text;
        set => txtLogFilePath.Text = value;
    }

    // Public methods to update UI state
    public void SetLoggingState(bool isLogging)
    {
        btnStartLogging.IsEnabled = !isLogging;
        btnStopLogging.IsEnabled = isLogging;
        btnBrowse.IsEnabled = !isLogging;
    }

    // Event handlers
    private void BtnBrowse_Click(object sender, RoutedEventArgs e)
    {
        BrowseRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnStartLogging_Click(object sender, RoutedEventArgs e)
    {
        StartLoggingRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnStopLogging_Click(object sender, RoutedEventArgs e)
    {
        StopLoggingRequested?.Invoke(this, EventArgs.Empty);
    }
}
