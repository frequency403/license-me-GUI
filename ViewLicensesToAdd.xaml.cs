using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseMe;

public partial class ViewLicensesToAdd : Window
{

    private GitDirectory DirToModify { get; }
    
    public ViewLicensesToAdd(GitDirectory directory)
    {
        InitializeComponent();
        DirToModify = directory;
        Title = $"Add license to \"{DirToModify.Name}\" repository";
        
    }

    public async Task LoadLicenses()
    {
        await foreach (var license in GithubApiCommunicator.GetLicenses())
        {
            LicenseView.Items.Add(license);
        }

        LicenseView.SelectedItem = LicenseView.Items[0];
    }

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

    private async void ChosenLicenseClick(object sender, RoutedEventArgs e)
    {
        await DirToModify.AddLicense(LicenseView.SelectedItem as BasicLicense);
        Close();
    }

    private void AbortLicensingClick(object sender, RoutedEventArgs e)
    {
        Close();
    }
}