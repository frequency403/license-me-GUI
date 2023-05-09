using System.Threading.Tasks;
using System.Windows;

namespace LicenseMe;

public partial class DirectoryView : Window
{
    public DirectoryView()
    {
        InitializeComponent();
    }

    public async Task UpdateListView()
    {
        //CurrentDirectories = dirs;
        var waitingWindow = new Waiting("Searching for Git-Repositories on your Computer");
        waitingWindow.Show();
        GitGrid.ItemsSource = await DirectoryCrawler.GetGitDirectories();
        waitingWindow.Close();
    }

    private async void AddLicenseC(object sender, RoutedEventArgs a)
    {
        var dc = GitGrid.SelectedItem as GitDirectory;
        var chooseWindow = new ViewLicensesToAdd(dc);
        chooseWindow.Show();
        var ww = new Waiting("Loading licenses from github...");
        ww.Show();
        await chooseWindow.LoadLicenses();
        ww.Close();
    }

    private async void AddReadmeC(object sender, RoutedEventArgs e)
    {
        var dc = GitGrid.SelectedItem as GitDirectory;
        await dc.AddReadme();
    }

    private void RemoveLicenseC(object sender, RoutedEventArgs e)
    {
        var gd = GitGrid.SelectedItem as GitDirectory;
        gd.RemoveLicense();
    }

    private void RemoveReadmeC(object sender, RoutedEventArgs e)
    {
        var gd = GitGrid.SelectedItem as GitDirectory;
        gd.RemoveReadme();

    }
}