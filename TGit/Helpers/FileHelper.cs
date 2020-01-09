using EnvDTE;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SamirBoulema.TGit.Helpers
{
    public static class FileHelper
    {
        public static string GetTortoiseGitProc()
        {
            return (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\TortoiseGit", "ProcPath", @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe");
        }

        public static string GetTortoiseGitPlink()
        {
            return (string)Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\TortoiseGit", "ProcPath", @"C:\Program Files\TortoiseGit\bin\TortoiseGitPlink.exe");
        }

        public static string GetMSysGit()
        {
            var regPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\TortoiseGit", "MSysGit", @"C:\Program Files (x86)\Git\bin\git.exe");
            return Path.Combine(regPath, "git.exe");
        }

        public static void SaveAllFiles(DTE dte)
        {
            dte.ExecuteCommand("File.SaveAll");
        }

        /// <summary>
        /// Check if we have an open solution or a folder and try to find the base repository/solution dir
        /// </summary>
        /// <param name="dte"></param>
        /// <returns>Path to the base repository/solution dir</returns>
        public static string GetSolutionDir(DTE dte)
        {
            var fullName = dte.Solution.FullName;

            if (string.IsNullOrEmpty(fullName))
            {
                return string.Empty;
            }

            if (File.Exists(fullName))
            {
                var path = Path.GetDirectoryName(fullName);
                return FindGitdir(path);
            }

            if (Directory.Exists(fullName))
            {
                return FindGitdir(fullName);
            }

            return string.Empty;
        }

        /// <summary>
        /// Start at the solution dir and traverse up to find a .git folder or file
        /// </summary>
        /// <param name="path">Path to start traversing from.</param>
        /// <returns>Path to the .git folder or file.</returns>
        private static string FindGitdir(string path)
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
                    return FindGitdir(di.Parent.FullName);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "TGIT error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    }
}
