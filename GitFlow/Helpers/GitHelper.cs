using EnvDTE;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace SamirBoulema.TGIT.Helpers
{
    public class GitHelper
    {
        private FileHelper fileHelper;
        private ProcessHelper processHelper;
        private string featureBranch, releaseBranch, hotfixBranch;

        public GitHelper(FileHelper fileHelper, ProcessHelper processHelper, string featureBranch, string releaseBranch, string hotfixBranch)
        {
            this.fileHelper = fileHelper;
            this.processHelper = processHelper;
            this.featureBranch = featureBranch;
            this.releaseBranch = releaseBranch;
            this.hotfixBranch = hotfixBranch;
        }

        public string GetCommitMessage(string commitMessageTemplate, DTE dte)
        {
            //var projectDir = _dte.Solution.Projects.Item(1).FullName;
            //var projectFileName = _dte.Solution.Projects.Item(1).FileName;

            string commitMessage = commitMessageTemplate;
            commitMessage = commitMessage.Replace("$(BranchName)", GetCurrentBranchName(false));
            commitMessage = commitMessage.Replace("$(FeatureName)", GetCurrentBranchName(true));
            commitMessage = commitMessage.Replace("$(Configuration)", dte.Solution.SolutionBuild.ActiveConfiguration?.Name);
            //commitMessage = commitMessage.Replace("$(Platform)", (string)_dte.Solution.Projects.Item(1).ConfigurationManager.PlatformNames);
            commitMessage = commitMessage.Replace("$(DevEnvDir)", (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\SxS\\VS7\\", dte.Version, ""));
            //commitMessage = commitMessage.Replace("$(ProjectDir)", Path.GetDirectoryName(_dte.Solution.Projects.Item(1).FullName));
            //commitMessage = commitMessage.Replace("$(ProjectPath)", Path.GetFullPath(_dte.Solution.Projects.Item(1).FullName));
            //commitMessage = commitMessage.Replace("$(ProjectName)", _dte.Solution.Projects.Item(1).FullName);
            //commitMessage = commitMessage.Replace("$(ProjectFileName)", _dte.Solution.Projects.Item(1).FileName);
            //commitMessage = commitMessage.Replace("$(ProjectExt)", Path.GetExtension(_dte.Solution.Projects.Item(1).FileName));
            commitMessage = commitMessage.Replace("$(SolutionDir)", Path.GetDirectoryName(dte.Solution.FullName));
            commitMessage = commitMessage.Replace("$(SolutionPath)", Path.GetFullPath(dte.Solution.FullName));
            commitMessage = commitMessage.Replace("$(SolutionName)", dte.Solution.FullName);
            commitMessage = commitMessage.Replace("$(SolutionFileName)", dte.Solution.FileName);
            commitMessage = commitMessage.Replace("$(SolutionExt)", Path.GetExtension(dte.Solution.FileName));
            commitMessage = commitMessage.Replace("$(VSInstallDir)", (string)Registry.GetValue(string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{0}", dte.Version), "InstallDir", ""));
            commitMessage = commitMessage.Replace("$(FxCopDir)", (string)Registry.GetValue(string.Format("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{0}\\Edev", dte.Version), "FxCopDir", ""));
            return commitMessage;
        }

        public string GetCurrentBranchName(bool trimPrefix)
        {
            string solutionDir = fileHelper.GetSolutionDir();

            if (string.IsNullOrEmpty(solutionDir))
            {
                return string.Empty;
            }

            string branchName = string.Empty;
            string error = string.Empty;
            string drive = Path.GetPathRoot(solutionDir).TrimEnd('\\');
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = string.Format("/c cd \"{0}\" && {1} && \"{2}\" symbolic-ref -q --short HEAD", solutionDir, drive, fileHelper.GetMSysGit()),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                branchName = proc.StandardOutput.ReadLine();
            }
            while (!proc.StandardError.EndOfStream)
            {
                error += proc.StandardError.ReadLine();
            }
            if (branchName != null)
            {
                if (branchName.StartsWith(featureBranch) && trimPrefix)
                {
                    return branchName.Substring(featureBranch.Length + 1);
                }
                else if (branchName.StartsWith(releaseBranch) && trimPrefix)
                {
                    return branchName.Substring(releaseBranch.Length + 1);
                }
                else if (branchName.StartsWith(hotfixBranch) && trimPrefix)
                {
                    return branchName.Substring(hotfixBranch.Length + 1);
                }
                return branchName;
            }
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Unable to detect branch name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return branchName;
        }

        public bool BranchExists(string branchName)
        {
            return processHelper.StartProcessGit($"rev-parse --verify origin/{branchName}", false);
        }
    }
}
