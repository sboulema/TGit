using EnvDTE;
using Microsoft.Win32;
using System.IO;

namespace SamirBoulema.TGit.Helpers
{
    public class GitHelper
    {
        private readonly ProcessHelper _processHelper;

        public GitHelper(ProcessHelper processHelper)
        {
            _processHelper = processHelper;
        }

        public string GetCommitMessage(string commitMessageTemplate, DTE dte)
        {
            var commitMessage = commitMessageTemplate;
            commitMessage = commitMessage.Replace("$(BranchName)", GetCurrentBranchName(false));
            commitMessage = commitMessage.Replace("$(FeatureName)", GetCurrentBranchName(true));
            commitMessage = commitMessage.Replace("$(Configuration)", dte.Solution.SolutionBuild.ActiveConfiguration?.Name);
            commitMessage = commitMessage.Replace("$(DevEnvDir)", (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\SxS\\VS7\\", dte.Version, ""));
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
            var flowOptions = GetFlowOptions();
            var branchName = _processHelper.StartProcessGitResult("symbolic-ref -q --short HEAD");

            if (branchName == null) return string.Empty;

            if (branchName.StartsWith(flowOptions.FeaturePrefix) && trimPrefix)
            {
                return branchName.Substring(flowOptions.FeaturePrefix.Length);
            }
            if (branchName.StartsWith(flowOptions.ReleasePrefix) && trimPrefix)
            {
                return branchName.Substring(flowOptions.ReleasePrefix.Length);
            }
            if (branchName.StartsWith(flowOptions.HotfixPrefix) && trimPrefix)
            {
                return branchName.Substring(flowOptions.HotfixPrefix.Length);
            }

            return branchName;
        }

        public string GetSshSetup()
        {
            var remoteOriginPuttyKeyfile = _processHelper.StartProcessGitResult("config --get remote.origin.puttykeyfile");
            if (string.IsNullOrEmpty(remoteOriginPuttyKeyfile)) return string.Empty;

            _processHelper.Start("pageant", remoteOriginPuttyKeyfile);
            return $"set GIT_SSH={FileHelper.GetTortoiseGitPlink()} && ";
        }

        public FlowOptions GetFlowOptions()
        {
            return new FlowOptions
            {
                MasterBranch = _processHelper.StartProcessGitResult("config --get gitflow.branch.master"),
                DevelopBranch = _processHelper.StartProcessGitResult("config --get gitflow.branch.develop"),
                FeaturePrefix = _processHelper.StartProcessGitResult("config --get gitflow.prefix.feature"),
                ReleasePrefix = _processHelper.StartProcessGitResult("config --get gitflow.prefix.release"),
                HotfixPrefix = _processHelper.StartProcessGitResult("config --get gitflow.prefix.hotfix")
            };
        }

        public bool RemoteBranchExists(string branch)
        {
            return _processHelper.StartProcessGit($"show-ref refs/remotes/origin/{branch}");
        }
    }
}
