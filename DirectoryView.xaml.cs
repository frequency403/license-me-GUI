using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseMe;

public partial class DirectoryView : Window
{
    public DirectoryView()
    {
        InitializeComponent();
    }

    public async Task UpdateListView(IAsyncEnumerable<GitDirectory?> dirs)
    {
        var waitingWindow = new Waiting("Searching for Git-Repositories on your Computer");
        waitingWindow.Show();
        await foreach (var dir in dirs)
        {
            GitGrid.Items.Add(dir);
        }
        waitingWindow.Close();
    }
}