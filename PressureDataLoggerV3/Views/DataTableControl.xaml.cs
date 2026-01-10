using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace PressureDataLogger.Views;

public partial class DataTableControl : UserControl
{
    // Events
    public event EventHandler? ExportToCsvRequested;
    public event EventHandler? ClearDataRequested;

    public DataTableControl()
    {
        InitializeComponent();
    }

    // Public method to set data source
    public void SetDataSource(ObservableCollection<string> recentSamples)
    {
        lstRecentSamples.ItemsSource = recentSamples;
    }

    // Event handlers
    private void ButtonExportCsv_Click(object sender, RoutedEventArgs e)
    {
        ExportToCsvRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ButtonClearData_Click(object sender, RoutedEventArgs e)
    {
        ClearDataRequested?.Invoke(this, EventArgs.Empty);
    }
}
