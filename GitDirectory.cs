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

public class GitDirectory : INotifyPropertyChanged
{
    public int Id { get; }
    public string Path { get; }

    public string DisplayPath { get; set; }
    public string Name { get; }
    public bool HasLicense { get; set; }

    public string? LicensePath { get; set; }

    public BasicLicense? LicenseType { get; set; }
    public bool HasReadme { get; set; }

    public string? ReadmePath { get; set; }


    public event PropertyChangedEventHandler PropertyChanged;

    public void NotifyPropertyChanged(string propName)
    {
        if(this.PropertyChanged != null)
            this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
    }
    
    public GitDirectory(int id, string path)
    {
        Id = id;
        Path = path;
        BeautifyPath();
        Name = Path.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault("UNKNOWN");
        FileChecker();
        if (HasLicense) DetermineLicense();
    }

    public GitDirectory()
    {
    }

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

    private async void DetermineLicense()
    {
        await using var licenseFile = new FileStream(LicensePath, FileMode.Open);
        var file = new byte[licenseFile.Length];
        await licenseFile.ReadAsync(file);
        var fileAsString = Encoding.ASCII.GetString(file);
        await foreach (var license in GithubAPICommunicator.GetLicenses())
        {
            if (fileAsString.Contains(license.Key) || fileAsString.Contains(license.Name) ||
                fileAsString.Contains(license.SpdxId))
            {
                LicenseType = license;
            }
        }
    }

    public async Task AddLicense(BasicLicense license)
    {
        var username = Interaction.InputBox("Please enter your full name:");
        //TODO UsernameCheck and Verification
        var lFilePath = LicensePath ?? Path + System.IO.Path.DirectorySeparatorChar + "LICENSE";
        await using var newLicenseFile = new FileStream(lFilePath, FileMode.OpenOrCreate);
        var alicense = await license.GetAdvancedLicenseInformation();
        alicense?.Personalize(username);
        await newLicenseFile.WriteAsync(Encoding.ASCII.GetBytes(alicense.Body));
        HasLicense = true;
        LicensePath = lFilePath;
        LicenseType = license;
        NotifyPropertyChanged("HasLicense");
        NotifyPropertyChanged("LicenseType");
    }

    public async Task AddReadme()
    {
        switch (MessageBox.Show("This will generate a Readme-Template with the given project name. Continue?",
                    "Genereate README.md", MessageBoxButton.OKCancel, MessageBoxImage.Question))
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

    private async Task<string> ReadmeText()
    {
        using var client = new HttpClient();
        var text = await client.GetStringAsync(
            Settings.SettingValues.ReadmeUrl);
        return text.Replace(Settings.SettingValues.ReplaceInReadme, $"# {Name.ToUpper()}");
    }
}