using EnvDTE;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SamirBoulema.TGit.Helpers
{
    public static class FileHelper
    {
        public static string GetTortoiseGitProc()
        {
            return (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\TortoiseGit", "ProcPath", @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe");
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
