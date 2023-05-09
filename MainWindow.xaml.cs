using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using static LicenseMe.GithubAPICommunicator;

namespace LicenseMe;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindow()
    {
        
        InitializeComponent();
        WindowStyle = WindowStyle.ToolWindow;
    }

    private async void SearchForGitDirectories(object sender, RoutedEventArgs e)
    {
        await Settings.InitSettings();
        var gitViewWindow = new DirectoryView();
        gitViewWindow.Show();
        await gitViewWindow.UpdateListView();
        Close();
    }

    private void CloseProgramClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void OpenGithubLink(object sender, RequestNavigateEventArgs e)
    {
        Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) {UseShellExecute = true});
        e.Handled = true;
    }

    private async void OpenSettings(object sender, RoutedEventArgs e)
    {
        if (Settings.SettingValues is null) await Settings.InitSettings();
        var sw = new SettingsWindow();
        sw.Show();
    }
}