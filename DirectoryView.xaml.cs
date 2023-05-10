using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseMe;

public partial class DirectoryView : Window
{
    public DirectoryView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Inits the Search for the Git-Directories
    /// </summary>
    public async Task UpdateListView()
    {
        var waitingWindow = new Waiting("Searching for Git-Repositories on your Computer");
        waitingWindow.Show();
        GitGrid.ItemsSource = await DirectoryCrawler.GetGitDirectories();
        waitingWindow.Close();
    }

    /// <summary>
    /// Button-Click from the Context-Menu when clicking "AddLicense"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="a"></param>
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

    /// <summary>
    /// Button-Click from the Context-Menu when clicking "AddReadme"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private async void AddReadmeC(object sender, RoutedEventArgs e)
    {
        var dc = GitGrid.SelectedItem as GitDirectory;
        await dc.AddReadme();
    }

    /// <summary>
    /// Button-Click from the Context-Menu when clicking "RemoveLicense"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RemoveLicenseC(object sender, RoutedEventArgs e)
    {
        var gd = GitGrid.SelectedItem as GitDirectory;
        gd.RemoveLicense();
    }
    /// <summary>
    /// Button-Click from the Context-Menu when clicking "RemoveReadme"
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    private void RemoveReadmeC(object sender, RoutedEventArgs e)
    {
        var gd = GitGrid.SelectedItem as GitDirectory;
        gd.RemoveReadme();

    }

    /// <summary>
    /// Opens the File-Explorer on the DirectoyPath selected
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OpenExplorerClick(object sender, RoutedEventArgs e)
    {
        var gd = GitGrid.SelectedItem as GitDirectory;
        Process.Start("explorer.exe", gd.Path);
    }
}