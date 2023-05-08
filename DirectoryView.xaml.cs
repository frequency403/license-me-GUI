using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualBasic;

namespace LicenseMe;

public partial class DirectoryView : Window
{
    private IAsyncEnumerable<GitDirectory?> CurrentDirectories { get; set; }

    public DirectoryView()
    {
        InitializeComponent();
    }

    public async Task UpdateListView(IAsyncEnumerable<GitDirectory?> dirs)
    {
        CurrentDirectories = dirs;
        var waitingWindow = new Waiting("Searching for Git-Repositories on your Computer");
        waitingWindow.Show();
        await foreach (var dir in CurrentDirectories)
        {
            GitGrid.Items.Add(dir);
        }

        waitingWindow.Close();
    }

    private void AddLicenseC(object sender, RoutedEventArgs a)
    {
        var dc = GitGrid.SelectedItem as GitDirectory;
        MessageBox.Show($"Funktioniert! Id: {dc.Id}");
    }

    private void AddReadmeC(object sender, RoutedEventArgs e)
    {
        
    }

    private void RemoveLicenseC(object sender, RoutedEventArgs e)
    {
        var gd = GitGrid.SelectedItem as GitDirectory;
        File.Delete(gd.LicensePath);
    }
    
    private void RemoveReadmeC(object sender, RoutedEventArgs e)
    {
        var gd = GitGrid.SelectedItem as GitDirectory;
        File.Delete(gd.ReadmePath);
    }
}