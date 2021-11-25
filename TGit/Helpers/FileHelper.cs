using Community.VisualStudio.Toolkit;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using File = System.IO.File;

namespace SamirBoulema.TGit.Helpers
{
    public static class FileHelper
    {
        public static string GetTortoiseGitProc()
            => (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\TortoiseGit",
                "ProcPath", @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe");

        public static string GetTortoiseGitPlink()
            => (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\TortoiseGit",
                "ProcPath", @"C:\Program Files\TortoiseGit\bin\TortoiseGitPlink.exe");

        public static string GetMSysGit()
        {
            var regPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\TortoiseGit",
                "MSysGit", @"C:\Program Files (x86)\Git\bin\git.exe");
            return Path.Combine(regPath, "git.exe");
        }

        /// <summary>
        /// Check if we have an open solution or a folder and try to find the base repository/solution dir
        /// </summary>
        /// <param name="dte"></param>
        /// <returns>Path to the base repository/solution dir</returns>
        public static async Task<string> GetSolutionDir()
        {
            var solution = await VS.Solutions.GetCurrentSolutionAsync();

            var filePath = solution?.FullPath;

            if (string.IsNullOrEmpty(filePath))
            {
                return string.Empty;
            }

            if (File.Exists(filePath))
            {
                var path = Path.GetDirectoryName(filePath);
                return await FindGitdir(path);
            }

            if (Directory.Exists(filePath))
            {
                return await FindGitdir(filePath);
            }

            return string.Empty;
        }

        /// <summary>
        /// Check if we have a path to a solution directory
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> HasSolutionDir()
            => !string.IsNullOrEmpty(await GetSolutionDir());

        /// <summary>
        /// Start at the solution dir and traverse up to find a .git folder or file
        /// </summary>
        /// <param name="path">Path to start traversing from.</param>
        /// <returns>Path to the .git folder or file.</returns>
        private static async Task<string> FindGitdir(string path)
        {
            try
            {
                var di = new DirectoryInfo(path);
                if (di.GetDirectories().Any(d => d.Name.Equals(".git")))
                {
                    return di.FullName;
                }

                var gitFilePath = Path.Combine(path, ".git");
                if (File.Exists(gitFilePath))
                {
                    var text = File.ReadAllText(gitFilePath);
                    var match = Regex.Match(text, "gitdir:(.*)");
                    if (match.Success)
                    {
                        var gitDirPath = match.Groups[1].Value.Trim();

                        if (Directory.Exists(gitDirPath))
                        {
                            return gitDirPath;
                        }

                        if (File.Exists(gitDirPath))
                        {
                            return File.ReadAllText(gitDirPath);
                        }
                    }
                }

                if (di.Parent != null)
                {
                    return await FindGitdir(di.Parent.FullName);
                }
            }
            catch (Exception e)
            {
                await VS.MessageBox.ShowErrorAsync("TGIT error", e.Message);
            }
            return string.Empty;
        }

        /// <summary>
        /// Get case sensitive path.
        /// http://stackoverflow.com/questions/325931/getting-actual-file-name-with-proper-casing-on-windows-with-net
        /// </summary>
        public static string GetExactFileName(string pathName)
        {
            if (!(File.Exists(pathName) || Directory.Exists(pathName)))
                return pathName;

            var di = new DirectoryInfo(pathName);

            if (di.Parent != null)
            {
                return di.Parent.GetFileSystemInfos(di.Name)[0].Name;
            }
            return di.Name.ToUpper();
        }

        public static string GetExactPathName(string pathName)
        {
            if (!(File.Exists(pathName) || Directory.Exists(pathName)))
                return pathName;

            var di = new DirectoryInfo(pathName);

            if (di.Parent != null)
            {
                return Path.Combine(
                    GetExactPathName(di.Parent.FullName),
                    di.Parent.GetFileSystemInfos(di.Name)[0].Name);
            }
            return di.Name.ToUpper();
        }

        /// <summary>
        /// Get the path of the file on which to act upon. 
        /// This can be different depending on where the TGit context menu was used
        /// </summary>
        /// <returns>File path</returns>
        public static async Task<string> GetActiveDocumentFilePath()
        {
            var windowFrame = await VS.Windows.GetCurrentWindowAsync();
            var solutionExplorerIsActive = windowFrame.Guid == new Guid(WindowGuids.SolutionExplorer);

            // Context menu in the Solution Explorer
            if (solutionExplorerIsActive)
            {
                var selectedItem = await VS.Solutions.GetActiveItemAsync();

                if (selectedItem != null)
                {
                    if (selectedItem.Type == SolutionItemType.Project ||
                        selectedItem.Type == SolutionItemType.Solution)
                    {
                        return Path.GetDirectoryName(selectedItem.FullPath);
                    }
                    else if (selectedItem.Type == SolutionItemType.PhysicalFile)
                    {
                        return selectedItem.FullPath;
                    }
                }
            }

            var documentView = await VS.Documents.GetActiveDocumentViewAsync();
            return documentView?.FilePath;
        }

        public static async Task<int?> GetActiveDocumentCurrentLine()
        {
            var documentView = await VS.Documents.GetActiveDocumentViewAsync();
            return documentView?.TextView?.Selection.ActivePoint.Position.GetContainingLine().LineNumber;
        }
    }
}
