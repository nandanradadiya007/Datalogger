using System.Windows;
using System.Windows.Controls;

namespace PressureDataLogger.Views;

public partial class MenuBarControl : UserControl
{
    // Events for menu actions
    public event EventHandler? FileNew;
    public event EventHandler? FileOpen;
    public event EventHandler? FileSave;
    public event EventHandler? FileSaveAs;
    public event EventHandler? ExportCsv;
    public event EventHandler? ExportExcel;
    public event EventHandler? ExportJson;
    public event EventHandler? FileExit;
    public event EventHandler? EditConfig;
    public event EventHandler? EditClear;
    public event EventHandler? ViewDashboard;
    public event EventHandler? ViewGraphs;
    public event EventHandler? ViewDataTable;
    public event EventHandler? ViewCalibrations;
    public event EventHandler? ToolsDeviceConfig;
    public event EventHandler? ToolsCalibration;
    public event EventHandler? ToolsValves;
    public event EventHandler? ToolsSettings;
    public event EventHandler? HelpGuide;
    public event EventHandler? HelpAbout;

    public MenuBarControl()
    {
        InitializeComponent();
    }

    // Public methods to update menu checkmarks from parent
    public void UpdateViewCheckmarks(string activeView)
    {
        menuViewDashboard.IsChecked = activeView == "Dashboard";
        menuViewGraphs.IsChecked = activeView == "Graphs";
        menuViewDataTable.IsChecked = activeView == "DataTable";
        menuViewCalibrations.IsChecked = activeView == "Calibrations";
    }

    // Menu event handlers
    private void MenuFileNew_Click(object sender, RoutedEventArgs e) => FileNew?.Invoke(this, EventArgs.Empty);
    private void MenuFileOpen_Click(object sender, RoutedEventArgs e) => FileOpen?.Invoke(this, EventArgs.Empty);
    private void MenuFileSave_Click(object sender, RoutedEventArgs e) => FileSave?.Invoke(this, EventArgs.Empty);
    private void MenuFileSaveAs_Click(object sender, RoutedEventArgs e) => FileSaveAs?.Invoke(this, EventArgs.Empty);
    private void MenuExportCsv_Click(object sender, RoutedEventArgs e) => ExportCsv?.Invoke(this, EventArgs.Empty);
    private void MenuExportExcel_Click(object sender, RoutedEventArgs e) => ExportExcel?.Invoke(this, EventArgs.Empty);
    private void MenuExportJson_Click(object sender, RoutedEventArgs e) => ExportJson?.Invoke(this, EventArgs.Empty);
    private void MenuFileExit_Click(object sender, RoutedEventArgs e) => FileExit?.Invoke(this, EventArgs.Empty);
    private void MenuEditConfig_Click(object sender, RoutedEventArgs e) => EditConfig?.Invoke(this, EventArgs.Empty);
    private void MenuEditClear_Click(object sender, RoutedEventArgs e) => EditClear?.Invoke(this, EventArgs.Empty);
    private void MenuViewDashboard_Click(object sender, RoutedEventArgs e) => ViewDashboard?.Invoke(this, EventArgs.Empty);
    private void MenuViewGraphs_Click(object sender, RoutedEventArgs e) => ViewGraphs?.Invoke(this, EventArgs.Empty);
    private void MenuViewDataTable_Click(object sender, RoutedEventArgs e) => ViewDataTable?.Invoke(this, EventArgs.Empty);
    private void MenuViewCalibrations_Click(object sender, RoutedEventArgs e) => ViewCalibrations?.Invoke(this, EventArgs.Empty);
    private void MenuToolsDeviceConfig_Click(object sender, RoutedEventArgs e) => ToolsDeviceConfig?.Invoke(this, EventArgs.Empty);
    private void MenuToolsCalibration_Click(object sender, RoutedEventArgs e) => ToolsCalibration?.Invoke(this, EventArgs.Empty);
    private void MenuToolsValves_Click(object sender, RoutedEventArgs e) => ToolsValves?.Invoke(this, EventArgs.Empty);
    private void MenuToolsSettings_Click(object sender, RoutedEventArgs e) => ToolsSettings?.Invoke(this, EventArgs.Empty);
    private void MenuHelpGuide_Click(object sender, RoutedEventArgs e) => HelpGuide?.Invoke(this, EventArgs.Empty);
    private void MenuHelpAbout_Click(object sender, RoutedEventArgs e) => HelpAbout?.Invoke(this, EventArgs.Empty);
}
