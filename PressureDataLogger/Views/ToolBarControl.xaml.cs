using System.Windows;
using System.Windows.Controls;

namespace PressureDataLogger.Views;

public partial class ToolBarControl : UserControl
{
    // Events for toolbar actions
    public event EventHandler? NewSession;
    public event EventHandler? OpenSession;
    public event EventHandler? SaveSession;
    public event EventHandler? ShowDashboard;
    public event EventHandler? ShowGraphs;
    public event EventHandler? OpenCalibration;
    public event EventHandler? StartAllAcquisition;
    public event EventHandler? StopAllAcquisition;

    public ToolBarControl()
    {
        InitializeComponent();
    }

    // Toolbar event handlers
    private void ButtonNew_Click(object sender, RoutedEventArgs e) => NewSession?.Invoke(this, EventArgs.Empty);
    private void ButtonOpen_Click(object sender, RoutedEventArgs e) => OpenSession?.Invoke(this, EventArgs.Empty);
    private void ButtonSave_Click(object sender, RoutedEventArgs e) => SaveSession?.Invoke(this, EventArgs.Empty);
    private void ButtonDashboard_Click(object sender, RoutedEventArgs e) => ShowDashboard?.Invoke(this, EventArgs.Empty);
    private void ButtonGraphs_Click(object sender, RoutedEventArgs e) => ShowGraphs?.Invoke(this, EventArgs.Empty);
    private void ButtonCalibration_Click(object sender, RoutedEventArgs e) => OpenCalibration?.Invoke(this, EventArgs.Empty);
    private void ButtonStartAll_Click(object sender, RoutedEventArgs e) => StartAllAcquisition?.Invoke(this, EventArgs.Empty);
    private void ButtonStopAll_Click(object sender, RoutedEventArgs e) => StopAllAcquisition?.Invoke(this, EventArgs.Empty);
}
