using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseMe;

public partial class ViewLicensesToAdd : Window
{

    /// <summary>
    /// The directory to modify
    /// </summary>
    private GitDirectory DirToModify { get; }
    
    /// <summary>
    /// Inits the License-Window and customizes the Header with the Repositoryname
    /// </summary>
    /// <param name="directory"></param>
    public ViewLicensesToAdd(GitDirectory directory)
    {
        InitializeComponent();
        DirToModify = directory;
        Title = $"Add license to \"{DirToModify.Name}\" repository";
        
    }

    /// <summary>
    /// Loads all Licenses from the GitHub API
    /// </summary>
    public async Task LoadLicenses()
    {
        await foreach (var license in GithubApiCommunicator.GetLicenses())
        {
            LicenseView.Items.Add(license);
        }

        LicenseView.SelectedItem = LicenseView.Items[0];
    }
    
    
    /// <summary>
    /// Feeds the Elements with Information given by the License
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void LicenseViewSelection(object sender, RoutedEventArgs e)
    {
        var license = LicenseView.SelectedItem as BasicLicense;
        KeyText.Text = license.Key;
        NameText.Text = license.Name;
        SpdxIdText.Text = license.SpdxId;
        NodeIdText.Text = license.NodeId;
        var advanced = await license.GetAdvancedLicenseInformation();
        HyperlinkText.Text = advanced.HtmlUrl;
        LicenseUrlLink.NavigateUri = new Uri(advanced.HtmlUrl);
        LicenseDescription.Text = advanced.Description;
        PermsList.ItemsSource = advanced.Permissions;
        CondList.ItemsSource = advanced.Conditions;
        LimsList.ItemsSource = advanced.Limitations;
    }
    
    /// <summary>
    /// Starts the Process of Adding a Chosen License
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private async void ChosenLicenseClick(object sender, RoutedEventArgs e)
    {
        await DirToModify.AddLicense(LicenseView.SelectedItem as BasicLicense);
        Close();
    }
    
    /// <summary>
    /// Closes this window.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void AbortLicensingClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}