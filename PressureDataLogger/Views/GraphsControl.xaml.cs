using System.Windows.Controls;

namespace PressureDataLogger.Views;

public partial class GraphsControl : UserControl
{
    public GraphsControl()
    {
        InitializeComponent();
    }

    // Future implementation: Add methods to update charts with real-time data
    // public void UpdatePressureChart(IEnumerable<PressureSample> data) { }
    // public void UpdateTemperatureChart(IEnumerable<TemperatureSample> data) { }
}
