using System.Windows;
using System.Windows.Controls;

namespace PressureDataLogger.Views;

public partial class CalibrationsControl : UserControl
{
    // Events
    public event EventHandler? NewCalibrationRequested;
    public event EventHandler? LoadCalibrationRequested;
    public event EventHandler? SaveCalibrationRequested;

    public CalibrationsControl()
    {
        InitializeComponent();
    }

    // Public method to access calibration panel
    public Panel CalibrationPanel => pnlCalibrations;

    // Event handlers
    private void ButtonNewCalibration_Click(object sender, RoutedEventArgs e)
    {
        NewCalibrationRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ButtonLoadCalibration_Click(object sender, RoutedEventArgs e)
    {
        LoadCalibrationRequested?.Invoke(this, EventArgs.Empty);
    }

    private void ButtonSaveCalibration_Click(object sender, RoutedEventArgs e)
    {
        SaveCalibrationRequested?.Invoke(this, EventArgs.Empty);
    }
}
