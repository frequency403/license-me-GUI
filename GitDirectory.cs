using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualBasic;

namespace LicenseMe;

/// <summary>
/// An Object representing a Git-Directory
/// </summary>
public class GitDirectory : INotifyPropertyChanged
{
    public int Id { get; }
    
    /// <summary>
    /// The Absolute Path
    /// </summary>
    public string Path { get; }
    
    /// <summary>
    /// The beautified Path
    /// </summary>
    public string DisplayPath { get; set; }
    
    /// <summary>
    /// The name of the repository, i.e. the parent-folder name
    /// </summary>
    public string Name { get; }
    
    /// <summary>
    /// Does it contain a License file?
    /// </summary>
    public bool HasLicense { get; set; }

    /// <summary>
    /// If it contains a license file, this is the path to it.
    /// </summary>
    public string? LicensePath { get; set; }

    /// <summary>
    /// If the type could be figured out, the appropriate license is saved here
    /// </summary>
    public BasicLicense? LicenseType { get; set; }
    
    /// <summary>
    /// Does it contain a readme file?
    /// </summary>
    public bool HasReadme { get; set; }
    
    /// <summary>
    /// If it does, this is the path to it.
    /// </summary>

    public string? ReadmePath { get; set; }


    /// <summary>
    /// The event for the UI, needed for recognizing data-changes
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    /// Triggers the event
    /// </summary>
    /// <param name="propName">The Property that has changed</param>
    private void NotifyPropertyChanged(string propName)
    {
        PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }
    
    public GitDirectory(int id, string path)
    {
        Id = id;
        Path = path;
        BeautifyPath(); //Beautification for display reasons
        Name = Path.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault("UNKNOWN");
        FileChecker(); //Has the directoriy already a license or readme file?
        if (HasLicense) DetermineLicense();
    }

    public GitDirectory()
    {
    }

    /// <summary>
    /// Splits the Path if it is too long, and replaces the overflow with a "/.../"
    /// </summary>
    private void BeautifyPath()
    {
        var pathSplit = Path.Split(System.IO.Path.DirectorySeparatorChar);
        try
        {
            DisplayPath = pathSplit.Length >= 5
                ? $"{pathSplit[0]}{System.IO.Path.DirectorySeparatorChar}{pathSplit[1]}{System.IO.Path.DirectorySeparatorChar}....{System.IO.Path.DirectorySeparatorChar}{pathSplit[^2]}{System.IO.Path.DirectorySeparatorChar}{pathSplit[^1]}"
                : Path;
        }
        catch (Exception)
        {
            DisplayPath = Path;
        }
    }

    /// <summary>
    /// This function checks, if certain versions of the readme or license already exist
    /// </summary>
    private void FileChecker()
    {
        switch (Path)
        {
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}LICENSE.md"):
                HasLicense = true;
                LicensePath = $"{x}{System.IO.Path.DirectorySeparatorChar}LICENSE.md";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}License.md"):
                HasLicense = true;
                LicensePath = $"{x}{System.IO.Path.DirectorySeparatorChar}License.md";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}license.md"):
                HasLicense = true;
                LicensePath = $"{x}{System.IO.Path.DirectorySeparatorChar}license.md";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}LICENSE"):
                HasLicense = true;
                LicensePath = $"{x}{System.IO.Path.DirectorySeparatorChar}LICENSE";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}License"):
                HasLicense = true;
                LicensePath = $"{x}{System.IO.Path.DirectorySeparatorChar}License";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}license"):
                HasLicense = true;
                LicensePath = $"{x}{System.IO.Path.DirectorySeparatorChar}license";
                break;
            default:
                HasLicense = false;
                LicensePath = null;
                break;
        }

        switch (Path)
        {
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}README.md"):
                HasReadme = true;
                ReadmePath = $"{x}{System.IO.Path.DirectorySeparatorChar}README.md";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}Readme.md"):
                HasReadme = true;
                ReadmePath = $"{x}{System.IO.Path.DirectorySeparatorChar}Readme.md";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}readme.md"):
                HasReadme = true;
                ReadmePath = $"{x}{System.IO.Path.DirectorySeparatorChar}readme.md";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}README"):
                HasReadme = true;
                ReadmePath = $"{x}{System.IO.Path.DirectorySeparatorChar}README";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}Readme"):
                HasReadme = true;
                ReadmePath = $"{x}{System.IO.Path.DirectorySeparatorChar}Readme";
                break;
            case { } x when File.Exists($"{x}{System.IO.Path.DirectorySeparatorChar}readme"):
                HasReadme = true;
                ReadmePath = $"{x}{System.IO.Path.DirectorySeparatorChar}readme";
                break;
            default:
                HasReadme = false;
                ReadmePath = null;
                break;
        }
    }
    
    /// <summary>
    /// This function gets called, when the property "HasLicense" is true.
    /// It reads the existing license file and tries to recognize the license-type
    /// </summary>
    private async void DetermineLicense()
    {
        await using var licenseFile = new FileStream(LicensePath, FileMode.Open);
        var file = new byte[licenseFile.Length];
        await licenseFile.ReadAsync(file);
        var fileAsString = Encoding.ASCII.GetString(file);
        await foreach (var license in GithubApiCommunicator.GetLicenses())
        {
            try
            {
                var advanced = await license.GetAdvancedLicenseInformation();
                var range = 11;
                if (advanced.Body.StartsWith("\t\t"))
                {
                    range *= 2;
                }
                if (fileAsString.StartsWith(advanced.Body[..range]))
                {
                    LicenseType = license;
                }
            }
            catch (Exception)
            {
                LicenseType = null;
            }
        }
    }
    
    /// <summary>
    /// This functions adds a License to the Repository, changing "HasLicense" and "LicensePath" if successful.
    /// </summary>
    /// <param name="license">The License chosen in UI</param>

    public async Task AddLicense(BasicLicense license)
    {
        var userDialog = new UserInput("Please enter your full name:", "Enter your first and lastname");
        if (userDialog.ShowDialog() is false or null)
        {
            MessageBox.Show("Adding aborted!", "Aborted", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        //TODO UsernameCheck and Verification
        var advancedLicenseInformation = await license.GetAdvancedLicenseInformation();
        advancedLicenseInformation?.Personalize(userDialog.InputOfUser);
        await using (var newLicenseFile = new FileStream(LicensePath ?? Path + System.IO.Path.DirectorySeparatorChar + "LICENSE", FileMode.OpenOrCreate))
        {
            await newLicenseFile.WriteAsync(Encoding.ASCII.GetBytes(advancedLicenseInformation.Body));
            await newLicenseFile.FlushAsync();
        }
        HasLicense = true;
        LicensePath ??= Path + System.IO.Path.DirectorySeparatorChar + "LICENSE";
        LicenseType = license;
        if (MessageBox.Show(
                "Do you want to add the License to a ReadmeFile\nor generate a new one with the chosen license?",
                "Add License", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            var ww = new Waiting("Processing your request....");
            ww.Show();
            await PhraseReplacer.InsertLicenseToReadme(this);
            ww.Close();
            NotifyPropertyChanged("HasReadme");
        }
        NotifyPropertyChanged("HasLicense");
        NotifyPropertyChanged("LicenseType");
    }

    /// <summary>
    /// This function adds a dummy README.md file.
    /// </summary>
    public async Task AddReadme()
    {
        switch (MessageBox.Show("This will generate a Readme-Template with the given project name. Continue?",
                    "Generate README.md", MessageBoxButton.OKCancel, MessageBoxImage.Question))
        {
            case MessageBoxResult.OK:
                try
                {
                    await using (var fileStream = new FileStream($"{Path}{System.IO.Path.DirectorySeparatorChar}README.md",
                               FileMode.CreateNew))
                    {
                        await fileStream.WriteAsync(Encoding.Default.GetBytes(await ReadmeText()));
                    }
                    HasReadme = true;
                    ReadmePath = $"{Path}{System.IO.Path.DirectorySeparatorChar}README.md";
                    NotifyPropertyChanged("HasReadme");
                    MessageBox.Show("Readme was successfully created!", "Creation successful", MessageBoxButton.OK, MessageBoxImage.Information);
                    
                }
                catch (Exception e)
                {
                    MessageBox.Show($"Something went wrong while creating the Readme.md\n\nReason: {e.Message}");
                }
                break;
            case MessageBoxResult.Cancel:
                break;
            case MessageBoxResult.Yes:
            case MessageBoxResult.No:
            case MessageBoxResult.None:
            default:
                break;
        }
    }
    
    /// <summary>
    /// If "HasReadme" is true, this function deletes the README file under the given "ReadmePath"
    /// </summary>

    public void RemoveReadme()
    {
        if (HasReadme)
        {
            File.Delete(ReadmePath);
            HasReadme = false;
            ReadmePath = null;
            NotifyPropertyChanged("HasReadme");
            MessageBox.Show("Readme was successfully removed!", "Deleted readme file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        else
        {
            MessageBox.Show("Cannot remove a non-existing readme file!", "Error removing readme file", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// if "HasLicense" is true, this function deleted the LICENSE file under the given "LicensePath"
    /// </summary>
    public void RemoveLicense()
    {
        if (HasLicense)
        {
            File.Delete(LicensePath);
            HasLicense = false;
            LicensePath = null;
            LicenseType = null;
            NotifyPropertyChanged("HasLicense");
            NotifyPropertyChanged("LicenseType");
            MessageBox.Show("License was successfully removed!", "Deleted license file", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }
        else
        {
            MessageBox.Show("Cannot remove a non-existing license!", "Error deleting license file", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    
    /// <summary>
    /// This function gets a readmetext from the URL in the Settings-file, and replaces the "Headline" with the current Directory Name
    /// </summary>
    /// <returns></returns>

    private async Task<string> ReadmeText()
    {
        using var client = new HttpClient();
        var text = await client.GetStringAsync(
            Settings.SettingValues.ReadmeUrl);
        return text.Replace(Settings.SettingValues.ReplaceInReadme, $"# {Name.ToUpper()}");
    }
}