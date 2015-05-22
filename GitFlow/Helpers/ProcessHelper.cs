using EnvDTE;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace FundaRealEstateBV.TGIT.Helpers
{
    public class ProcessHelper
    {
        private FileHelper _fileHelper;
        private string _solutionDir;

        public ProcessHelper(DTE dte)
        {
            _fileHelper = new FileHelper(dte);
        }

        public bool StartProcessGit(string gitCommands)
        {
            _solutionDir = _fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(_solutionDir)) return false;

            string output = string.Empty;
            string error = string.Empty;
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = string.Format("/c {0} && cd {1} && git {2}", Path.GetPathRoot(_solutionDir).TrimEnd('\\'), _solutionDir, gitCommands),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                output = proc.StandardOutput.ReadLine();
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }
            if (!string.IsNullOrEmpty(output))
            {
                return true;
            }
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "TGIT error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
    }
}
