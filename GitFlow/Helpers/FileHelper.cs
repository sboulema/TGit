using EnvDTE;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FundaRealEstateBV.TGIT.Helpers
{
    public class FileHelper
    {
        private DTE _dte;

        public FileHelper(DTE dte)
        {
            _dte = dte;
        }

        public string GetTortoiseGitProc()
        {
            return (string) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\TortoiseGit", "ProcPath", @"C:\Program Files\TortoiseGit\bin\TortoiseGitProc.exe");
        }

        public string GetMSysGit()
        {
            string regPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\TortoiseGit", "MSysGit", null);

            if (string.IsNullOrEmpty(regPath))
            {
                return @"C:\Program Files (x86)\Git\bin\git.exe";
            }
            return Path.Combine(regPath, "git.exe");
        }

        public void SaveAllFiles()
        {
            _dte.ExecuteCommand("File.SaveAll");
        }

        public string GetSolutionDir()
        {
            string fileName = _dte.Solution.FullName;
            if (!string.IsNullOrEmpty(fileName))
            {
                var path = Path.GetDirectoryName(fileName);
                return FindGitdir(path);
            }
            return string.Empty;
        }

        private static string FindGitdir(string path)
        {
            try
            {
                var di = new DirectoryInfo(path);
                if (di.GetDirectories().Any(d => d.Name.Equals(".git")))
                {
                    return di.FullName;
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
    }
}
