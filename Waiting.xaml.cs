using System.Windows;

namespace LicenseMe;

public partial class Waiting : Window
{
    /// <summary>
    /// An Indeterminate waiting window with custom text
    /// </summary>
    /// <param name="text"></param>
    public Waiting(string text)
    {
        InitializeComponent();
        WaitingText.Text = text;
        WaitingBar.IsIndeterminate = true;
        WindowStyle = WindowStyle.None;
    }
}