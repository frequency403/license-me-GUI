using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LicenseMe;

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
            return Enumerable.Empty<DirectoryInfo>();
        }
    }
    
    public static async IAsyncEnumerable<GitDirectory> GetGitDirectories()
    {
        var counter = 0;
        foreach (var drive in DriveInfo.GetDrives())
            {
                foreach (var directory in new[] { new DirectoryInfo(drive.Name) }
                             .Traverse(dir => GetDirectoriesWithoutThrowing(dir)).Where(e =>
                                 e.Name.Contains(".git") &&
                                 !(e.Name.Contains(".github") || e.Name.Contains("AppData"))))
                {
                    yield return new GitDirectory(counter, directory.Parent.FullName);
                    counter++;
                }
            }
    }
    
}