using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;
using static LicenseMe.GithubApiCommunicator;

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
        if (Settings.SettingValues is null) await Settings.InitSettings();
        if (RateLimitReached(out var date) is null or true)
        {
            MessageBox.Show(
                $"The GitHub API Limit for requests has been reached.\nThe Program can not continue until the limit is reset!\n\nThe Limit resets at {date:G}\n\nConsider using a Token in the program settings\nand get your Token from\n\nhttps://github.com/settings/tokens",
                "API-Limit Reached", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
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