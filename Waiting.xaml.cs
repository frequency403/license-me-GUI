using System.Windows;

namespace LicenseMe;

public partial class Waiting : Window
{
    public Waiting(string text)
    {
        InitializeComponent();
        WaitingText.Text = text;
        WaitingBar.IsIndeterminate = true;
        WindowStyle = WindowStyle.None;
    }
}