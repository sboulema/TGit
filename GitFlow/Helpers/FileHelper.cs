using EnvDTE;
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
