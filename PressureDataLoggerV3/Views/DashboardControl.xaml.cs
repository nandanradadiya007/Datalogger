using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using System.Linq;
using PressureDataLogger.Models;

namespace PressureDataLogger.Views;

public partial class DashboardControl : UserControl
{
    // Events for pressure control
    public event EventHandler? PressureStartRequested;
    public event EventHandler? PressureStopRequested;
    
    // Events for temperature control
    public event EventHandler? TemperatureStartRequested;
    public event EventHandler? TemperatureStopRequested;
    
    // Event for valve changes
    public event EventHandler<int>? ValveToggleRequested;

    // Event raised when the user changes the selected device so the parent can
    // supply the list of channels and device type.
    public event EventHandler<string>? DeviceSelectionChanged;

    // Event raised when the user clicks Refresh to re-enumerate connected devices.
    public event EventHandler? RefreshDevicesRequested;

    public DashboardControl()
    {
        InitializeComponent();
    }

    // Public properties to access configuration values
    public string DeviceChannel
    {
        get
        {
            var channel = cmbChannel.SelectedItem as string;
            if (!string.IsNullOrEmpty(channel))
                return channel;
            var device = cmbDevice.SelectedItem as string ?? string.Empty;
            return string.IsNullOrEmpty(device) ? AppConfiguration.DefaultDeviceChannel : $"{device}/ai0";
        }
    }
    public string SampleRate => txtSampleRate.Text;
    public string SamplesPerRead => txtSamplesPerRead.Text;

    // ---- Device / channel population helpers called by the parent window ----

    /// <summary>
    /// Populates the device drop-down with the supplied list.
    /// </summary>
    public void LoadDevices(IEnumerable<string> devices)
    {
        cmbDevice.ItemsSource = devices.ToList();
        if (cmbDevice.Items.Count > 0)
            cmbDevice.SelectedIndex = 0;
    }

    /// <summary>
    /// Populates the channel drop-down and updates the device-type label.
    /// </summary>
    public void LoadChannels(IEnumerable<string> channels, string deviceType)
    {
        txtDeviceType.Text = deviceType;
        txtDeviceType.FontStyle = FontStyles.Normal;
        txtDeviceType.Foreground = new SolidColorBrush(Color.FromRgb(33, 150, 243)); // blue
        cmbChannel.ItemsSource = channels.ToList();
        if (cmbChannel.Items.Count > 0)
            cmbChannel.SelectedIndex = 0;
    }

    // Public methods to update UI from parent
    public void SetPressureAcquisitionRunning(bool isRunning)
    {
        btnStart.IsEnabled = !isRunning;
        btnStop.IsEnabled = isRunning;
        cmbDevice.IsEnabled = !isRunning;
        cmbChannel.IsEnabled = !isRunning;
        btnRefreshDevices.IsEnabled = !isRunning;
        txtSampleRate.IsEnabled = !isRunning;
        txtSamplesPerRead.IsEnabled = !isRunning;
    }

    public void SetTemperatureAcquisitionRunning(bool isRunning)
    {
        btnStartTemp.IsEnabled = !isRunning;
        btnStopTemp.IsEnabled = isRunning;
    }

    public void UpdateLatestPressureValue(string value)
    {
        txtLatestValue.Text = value;
    }

    public void UpdateTemperatureValues(string temp1, string temp2, string temp3, string temp4)
    {
        txtTemp1.Text = temp1;
        txtTemp2.Text = temp2;
        txtTemp3.Text = temp3;
        txtTemp4.Text = temp4;
    }

    public void UpdateValveState(int valveNumber, ValveState valve)
    {
        Button? btn = valveNumber switch
        {
            1 => btnValve1,
            2 => btnValve2,
            3 => btnValve3,
            4 => btnValve4,
            _ => null
        };

        if (btn != null)
        {
            btn.Content = valve.Status;
            btn.Background = valve.IsOpen ? 
                new SolidColorBrush(Color.FromRgb(76, 175, 80)) : // Green
                new SolidColorBrush(Color.FromRgb(204, 204, 204)); // Gray
        }
    }

    // Event handlers
    private void BtnStart_Click(object sender, RoutedEventArgs e)
    {
        PressureStartRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnStop_Click(object sender, RoutedEventArgs e)
    {
        PressureStopRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnStartTemp_Click(object sender, RoutedEventArgs e)
    {
        TemperatureStartRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnStopTemp_Click(object sender, RoutedEventArgs e)
    {
        TemperatureStopRequested?.Invoke(this, EventArgs.Empty);
    }

    private void BtnValve_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string valveIdStr)
        {
            int valveId = int.Parse(valveIdStr);
            ValveToggleRequested?.Invoke(this, valveId);
        }
    }

    private void CmbDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cmbDevice.SelectedItem is string deviceName)
        {
            cmbChannel.ItemsSource = null;
            txtDeviceType.Text = "-- loading --";
            txtDeviceType.FontStyle = FontStyles.Italic;
            txtDeviceType.Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102));
            DeviceSelectionChanged?.Invoke(this, deviceName);
        }
    }

    private void BtnRefreshDevices_Click(object sender, RoutedEventArgs e)
    {
        RefreshDevicesRequested?.Invoke(this, EventArgs.Empty);
    }
}
