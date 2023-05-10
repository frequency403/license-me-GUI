using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LicenseMe;

public static class DirectoryCrawler
{
    /// <summary>
    /// Traverses down the Directories
    /// </summary>
    /// <param name="source"></param>
    /// <param name="childrenSelector"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    private static IEnumerable<T> Traverse<T>(
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
    
    /// <summary>
    /// Enumerates the Directories that dont throw an exception
    /// </summary>
    /// <param name="dir"></param>
    /// <returns></returns>

    private static IEnumerable<DirectoryInfo> GetDirectoriesWithoutThrowing(
        DirectoryInfo dir)
    {
        try
        {
            return dir.GetDirectories();
        }
        catch (Exception) //if possible catch a more derived exception
        {
            return Enumerable.Empty<DirectoryInfo>();
        }
    }
    
    /// <summary>
    /// Searches the whole machine for directories, that have a ".git" folder in them
    /// </summary>
    /// <returns>An ObservableCollection of GitDirectory Objects</returns>
    public static async Task<ObservableCollection<GitDirectory>> GetGitDirectories()
    {
        return await Task.Run(() =>
        {
            var counter = 0;
            var list = new ObservableCollection<GitDirectory>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                foreach (var directory in new[] { new DirectoryInfo(drive.Name) }
                             .Traverse(GetDirectoriesWithoutThrowing).Where(e =>
                                 e.Name.Contains(".git") &&
                                 !(e.Name.Contains(".github") || e.Name.Contains("AppData"))))
                {
                    list.Add(new GitDirectory(counter, directory.Parent.FullName));
                    counter++;
                }
            }

            return list;
        });
    }
}