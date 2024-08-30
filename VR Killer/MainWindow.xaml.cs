using Microsoft.Win32;
using System;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VR_Killer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private NotifyIcon _notifyIcon;

    private bool IsOriginalName => System.IO.Directory.Exists(_config.OpenvrPath);

    private const string StartupKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
    private const string AppName = "VR Killer";

    private const string ConfigFilePath = "config.json";
    private readonly AppConfig _config;

    public MainWindow()
    {
        InitializeComponent();

        _config = AppConfig.Load(ConfigFilePath);

        ConfigureNotifyIcon();
        Hide();

        StartWithWindows.Checked += StartWithWindows_Checked;
        StartWithWindows.Unchecked += StartWithWindows_Unchecked;

        AutoPath.Checked += AutoPath_Checked;
        AutoPath.Unchecked += AutoPath_Unchecked;

        LoadConfig();
    }

    private void LoadConfig()
    {
        AutoPath.IsChecked = _config.AutoPath;
        StartWithWindows.IsChecked = _config.StartWithWindows;
        OpenvrPath.Text = _config.OpenvrPath;

        if (_config.AutoPath)
        {
            OpenvrPath.IsEnabled = false;
            BrowseButton.IsEnabled = false;
        }
    }

    private void SaveConfig()
    {
        _config.AutoPath = AutoPath.IsChecked ?? false;
        _config.StartWithWindows = StartWithWindows.IsChecked ?? false;
        _config.OpenvrPath = OpenvrPath.Text;

        AppConfig.Save(_config, ConfigFilePath);
    }

    private void ConfigureNotifyIcon()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = new Icon("vrkiller.ico"),
            Visible = true,
            Text = "VR Killer"
        };

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add(IsOriginalName ? "Desativar VR" : "Ativar VR", null, VrKillerAlternate);
        contextMenu.Items.Add("Configurações", null, Open_Click);
        contextMenu.Items.Add("Sair", null, Exit_Click);

        _notifyIcon.ContextMenuStrip = contextMenu;
        _notifyIcon.DoubleClick += VrKillerAlternate;
    }

    private void UpdateContextMenu()
    {
        if (_notifyIcon.ContextMenuStrip != null)
        {
            _notifyIcon.ContextMenuStrip.Items[0].Text = IsOriginalName ? "Desativar VR" : "Ativar VR";
        }
    }

    private void VrKillerAlternate(object? sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(_config.OpenvrPath))
        {
            System.Windows.MessageBox.Show("Caminho do OpenVR não definido", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            if (System.IO.Directory.Exists(_config.OpenvrPath))
            {
                System.IO.Directory.Move(_config.OpenvrPath, _config.OpenvrPath + "_killed");
                _notifyIcon.Icon = new Icon("vrkiller.ico");
            }
            else if (System.IO.Directory.Exists(_config.OpenvrPath + "_killed"))
            {
                System.IO.Directory.Move(_config.OpenvrPath + "_killed", _config.OpenvrPath);
                _notifyIcon.Icon = new Icon("vrkiller.ico");
            }
            else
            {
                System.Windows.MessageBox.Show("Caminho do OpenVR não encontrado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                _notifyIcon.Icon = new Icon("vrkiller.ico");
            }

            UpdateContextMenu();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Erro ao renomear o caminho: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void Open_Click(object? sender, EventArgs e)
    {
        Show();
        WindowState = WindowState.Normal;
    }

    private void Exit_Click(object? sender, EventArgs e)
    {
        _notifyIcon.Visible = false;
        System.Windows.Application.Current.Shutdown();
    }

    protected override void OnStateChanged(EventArgs e)
    {
        if (WindowState == WindowState.Minimized)
        {
            Hide();
        }
        base.OnStateChanged(e);
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }

    private void StartWithWindows_Checked(object sender, RoutedEventArgs e)
    {
        SetStartup(true);

        SaveConfig();
    }

    private void StartWithWindows_Unchecked(object sender, RoutedEventArgs e)
    {
        SetStartup(false);

        SaveConfig();
    }

    private void AutoPath_Checked(object sender, RoutedEventArgs e)
    {
        OpenvrPath.IsEnabled = false;
        BrowseButton.IsEnabled = false;
        _config.OpenvrPath = Environment.ExpandEnvironmentVariables("%localappdata%\\openvr");
        OpenvrPath.Text = _config.OpenvrPath;

        SaveConfig();
    }

    private void AutoPath_Unchecked(object sender, RoutedEventArgs e)
    {
        OpenvrPath.IsEnabled = true;
        BrowseButton.IsEnabled = true;
        _config.OpenvrPath = string.Empty;
        OpenvrPath.Text = _config.OpenvrPath;

        SaveConfig();
    }

    private void SetStartup(bool enable)
    {
        using RegistryKey? key = Registry.CurrentUser.OpenSubKey(StartupKey, true);
        if (enable)
            key?.SetValue(AppName, System.Reflection.Assembly.GetExecutingAssembly().Location);
        else
            key?.DeleteValue(AppName, false);
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        using var folderBrowserDialog = new FolderBrowserDialog();
        DialogResult result = folderBrowserDialog.ShowDialog();
        if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
        {
            OpenvrPath.Text = folderBrowserDialog.SelectedPath;
            SaveConfig();
        }

    }
}