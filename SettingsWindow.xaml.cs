using System.Windows;

namespace LicenseMe;

public partial class SettingsWindow : Window
{
    /// <summary>
    /// Loads the Settings-Window with the Values provided by the Settings-Object
    /// </summary>
    public SettingsWindow()
    {
        InitializeComponent();
        WindowStyle = WindowStyle.ThreeDBorderWindow;
        ResizeMode = ResizeMode.NoResize;
        GhUsername.Text = Settings.SettingValues.GitHubUser;
        GhToken.Text = Settings.SettingValues.GitHubToken;
        GhReadmeLink.Text = Settings.SettingValues.ReadmeUrl;
        GhPhraseToReplace.Text = Settings.SettingValues.ReplaceInReadme;
    }

    /// <summary>
    /// Saves the Settings
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void SaveSettings(object sender, RoutedEventArgs e)
    {
        Settings.SettingValues.GitHubUser = GhUsername.Text;
        Settings.SettingValues.GitHubToken = GhToken.Text==""?null:GhToken.Text;
        Settings.SettingValues.ReadmeUrl = GhReadmeLink.Text;
        Settings.SettingValues.ReplaceInReadme = GhPhraseToReplace.Text;
        await Settings.SaveSettings();
        Close();
    }
    
    /// <summary>
    /// Closes the Window
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CloseWindow(object sender, RoutedEventArgs e)
    {
        Close();
    }
}