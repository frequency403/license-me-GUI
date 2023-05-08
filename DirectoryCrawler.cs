using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseMe;

public class GitDirectory
{
    public string Path { get; }
    public string Name { get; }
    public bool HasLicense { get; set; }
    public bool HasReadme { get; set; }

    public GitDirectory(string path)
    {
        Path = path;
        Name = Path.Split(System.IO.Path.DirectorySeparatorChar).LastOrDefault("UNKNOWN");
        HasLicense = File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}LICENSE.md") || File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}License.md") || File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}license.md") || File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}LICENSE") || File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}License") || File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}license");
        HasReadme = File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}README.md") || File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}Readme.md") || File.Exists($"{Path}{System.IO.Path.DirectorySeparatorChar}readme.md");
    }

    public GitDirectory()
    {
        
    }
    
}

public static class DirectoryCrawler
{
    public static IEnumerable<T> Traverse<T>(
        this IEnumerable<T> source
        , Func<T, IEnumerable<T>> childrenSelector)
    {
        var stack = new Stack<T>(source);
        while (stack.Any())
        {
            var next = stack.Pop();
            yield return next;
            foreach (var child in childrenSelector(next))
                stack.Push(child);
        }
    }
    
    public static IEnumerable<DirectoryInfo> GetDirectoriesWithoutThrowing(
        DirectoryInfo dir)
    {
        try
        {
            return dir.GetDirectories();
        }
        catch (Exception)//if possible catch a more derived exception
        {
            //TODO consider logging the exception
            return Enumerable.Empty<DirectoryInfo>();
        }
    }
    
    public static async IAsyncEnumerable<GitDirectory> GetGitDirectories()
    {
        foreach (var drive in DriveInfo.GetDrives())
            {
                foreach (var directory in new[] { new DirectoryInfo(drive.Name) }
                             .Traverse(dir => GetDirectoriesWithoutThrowing(dir)).Where(e =>
                                 e.Name.Contains(".git") &&
                                 !(e.Name.Contains(".github") || e.Name.Contains("AppData"))))
                {
                    yield return new GitDirectory(directory.Parent.FullName);
                }
            }
    }
    
}