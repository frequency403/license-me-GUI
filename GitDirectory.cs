using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace LicenseMe;

public class GitDirectory
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
    
    

    public GitDirectory(int id, string path)
    {
        Id = id;
        Path = path;
        BeautifyPath();
        Name = Path.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault("UNKNOWN");
        FileChecker();
        if(HasLicense) DetermineLicense();
    }

    public GitDirectory()
    {
        
    }

    private void BeautifyPath()
    {
        var pathSplit = Path.Split(System.IO.Path.DirectorySeparatorChar);
        try
        {
            DisplayPath = pathSplit.Length >= 5 ? $"{pathSplit[0]}{System.IO.Path.DirectorySeparatorChar}{pathSplit[1]}{System.IO.Path.DirectorySeparatorChar}....{System.IO.Path.DirectorySeparatorChar}{pathSplit[^2]}{System.IO.Path.DirectorySeparatorChar}{pathSplit[^1]}" : Path;
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
        if(LicenseHolder.Licenses is null) LicenseHolder.InitLicenses();
        var fileAsString = Encoding.ASCII.GetString(file);
        await foreach (var license in LicenseHolder.Licenses)
        {
            if (fileAsString.Contains(license.Key) || fileAsString.Contains(license.Name) || fileAsString.Contains(license.SpdxId))
            {
                LicenseType = license;
            }
        }
    }

    private async Task AddLicense(BasicLicense license)
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
    }

}