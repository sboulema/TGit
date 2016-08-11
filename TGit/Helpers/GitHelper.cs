using EnvDTE;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Process = System.Diagnostics.Process;

namespace SamirBoulema.TGit.Helpers
{
    public class GitHelper
    {
        private readonly FileHelper _fileHelper;
        private readonly ProcessHelper _processHelper;
        private readonly string _featureBranch, _releaseBranch, _hotfixBranch;

        public GitHelper(FileHelper fileHelper, ProcessHelper processHelper, string featureBranch, string releaseBranch, string hotfixBranch)
        {
            _fileHelper = fileHelper;
            _processHelper = processHelper;
            _featureBranch = featureBranch;
            _releaseBranch = releaseBranch;
            _hotfixBranch = hotfixBranch;
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
            commitMessage = commitMessage.Replace("$(VSInstallDir)", (string)Registry.GetValue($"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{dte.Version}", "InstallDir", ""));
            commitMessage = commitMessage.Replace("$(FxCopDir)", (string)Registry.GetValue($"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{dte.Version}\\Edev", "FxCopDir", ""));
            return commitMessage;
        }

        public string GetCurrentBranchName(bool trimPrefix)
        {
            var solutionDir = _fileHelper.GetSolutionDir();

            if (string.IsNullOrEmpty(solutionDir))
            {
                return string.Empty;
            }

            var branchName = string.Empty;
            var error = string.Empty;
            var drive = Path.GetPathRoot(solutionDir).TrimEnd('\\');
            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c cd \"{solutionDir}\" && {drive} && \"{_fileHelper.GetMSysGit()}\" symbolic-ref -q --short HEAD",
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
                if (branchName.StartsWith(_featureBranch) && trimPrefix)
                {
                    return branchName.Substring(_featureBranch.Length + 1);
                }
                if (branchName.StartsWith(_releaseBranch) && trimPrefix)
                {
                    return branchName.Substring(_releaseBranch.Length + 1);
                }
                if (branchName.StartsWith(_hotfixBranch) && trimPrefix)
                {
                    return branchName.Substring(_hotfixBranch.Length + 1);
                }
                return branchName;
            }
            if (!string.IsNullOrEmpty(error))
            {
                MessageBox.Show(error, "Unable to detect branch name", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return branchName;
        }

        public bool IsGitFlow()
        {
            return _processHelper.StartProcessGit("config --get gitflow.branch.master", false);
        }

        public bool IsGitHubFlow()
        {
            return !IsGitFlow();
        }

        public string GetSshSetup()
        {
            var remoteOriginPuttyKeyfile = _processHelper.StartProcessGitResult("config --get remote.origin.puttykeyfile");
            if (string.IsNullOrEmpty(remoteOriginPuttyKeyfile)) return string.Empty;

            _processHelper.Start("pageant", remoteOriginPuttyKeyfile);
            return $"set GIT_SSH={_fileHelper.GetTortoiseGitPlink()} && ";
        }
    }
}
