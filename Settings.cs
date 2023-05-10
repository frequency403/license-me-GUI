using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LicenseMe;

public static class Settings
{
    /// <summary>
    /// Path to Settingsfile
    /// </summary>
    private static readonly string SettingsFilePath = Environment.CurrentDirectory + Path.DirectorySeparatorChar + "settings.json";
    /// <summary>
    /// The File Handle
    /// </summary>
    private static FileStream? _settingsFileHandle;
    /// <summary>
    /// An object, representing the Values of the Settings
    /// </summary>
    public static SettingValues? SettingValues;
    
    
    /// <summary>
    /// Inits the Settings-file, creates a new one with sample data if no file exists.
    /// </summary>
    public static async Task InitSettings()
    {
        _settingsFileHandle = !File.Exists(SettingsFilePath)
            ? new FileStream(SettingsFilePath, FileMode.CreateNew)
            : new FileStream(SettingsFilePath, FileMode.OpenOrCreate);
        var b = new byte[_settingsFileHandle.Length];
        await _settingsFileHandle.ReadAsync(b);
        if (Encoding.UTF8.GetString(b) == "")
        {
            
            await _settingsFileHandle.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(new SettingValues
            {
                GitHubUser = "frequency403",
                GitHubToken = null,
                ReadmeUrl = "https://raw.githubusercontent.com/PurpleBooth/a-good-readme-template/main/README.md",
                ReplaceInReadme = "# Project Title"
            }));
            await _settingsFileHandle.FlushAsync();
        }
        SettingValues = JsonSerializer.Deserialize<SettingValues>(Encoding.UTF8.GetString(b));
    }
    
    /// <summary>
    /// Saves the Settings changed. Overwrites the whole file
    /// </summary>
    public static async Task SaveSettings()
    {
        _settingsFileHandle.Position = 0;
        _settingsFileHandle.SetLength(0);
        await _settingsFileHandle.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(SettingValues));
        await _settingsFileHandle.FlushAsync();
    }
}