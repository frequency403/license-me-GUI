using System;
using System.Threading.Tasks;
using System.Windows;

namespace LicenseMe;

public partial class UserInput : Window
{
    /// <summary>
    /// A Custom Dialog Window. Needs to be called with ".ShowDialog()"
    /// </summary>
    /// <param name="text"></param>
    /// <param name="head"></param>
    public UserInput(string text, string head = "InputPrompt")
    {
        InitializeComponent();
        UserInputText.Text = text;
        Title = head;
    }

    public string InputOfUser => UserInputBox.Text;

    private void OkayButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        UserInputBox.SelectAll();
        UserInputBox.Focus();
    }
}