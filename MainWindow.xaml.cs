using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        // await foreach (var license in GetLicenses())
        // {
        //     if (license is null) continue;
        //     var advanced = await license.GetAdvancedLicenseInformation();
        //     advanced?.Personalize("Oliver Schantz");
        //     MessageBox.Show(advanced?.Body, license.Name);
        // }
        var gitViewWindow = new DirectoryView();
        gitViewWindow.Show();
        
        await gitViewWindow.UpdateListView( DirectoryCrawler.GetGitDirectories());
        Close();
    }

    private void CloseProgramClick(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }
}