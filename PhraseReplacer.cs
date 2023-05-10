using System;
using System.IO;
using System.Threading.Tasks;

namespace LicenseMe;

public static class PhraseReplacer
{
    public static async Task InsertLicenseToReadme(GitDirectory directory)
    {
        if (!directory.HasReadme)
        {
            await directory.AddReadme();
        }
        var file = await File.ReadAllTextAsync(directory.ReadmePath);
        foreach (var section in file.Split("## "))
        {
            if (section.StartsWith("License") || section.StartsWith("license") || section.StartsWith("LICENSE"))
            {
                var advancedInformation = await directory.LicenseType?.GetAdvancedLicenseInformation();
                file = file.Replace(section, $"License\n\nThis project is licensed under the [{advancedInformation.Name}]({advancedInformation.HtmlUrl}) - see the [LICENSE](LICENSE) file for details\n\n##");
            }
        }

        await File.WriteAllTextAsync(directory.ReadmePath, file);

    }
}