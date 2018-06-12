using EnvDTE;
using Microsoft.Win32;
using System.IO;

namespace SamirBoulema.TGit.Helpers
{
    public static class GitHelper
    {
        public static string GetCommitMessage(string commitMessageTemplate, DTE dte)
        {
            if (string.IsNullOrEmpty(commitMessageTemplate)) return string.Empty;

            var commitMessage = commitMessageTemplate;
            commitMessage = commitMessage.Replace("$(BranchName)", GetCurrentBranchName(false, dte));
            commitMessage = commitMessage.Replace("$(FeatureName)", GetCurrentBranchName(true, dte));
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

        public static string GetCurrentBranchName(bool trimPrefix, DTE dte)
        {
            var branchName = ProcessHelper.StartProcessGitResult(dte, "symbolic-ref -q --short HEAD");

            if (branchName == null) return string.Empty;

            var gitConfig = GetGitConfig(dte);

            if (branchName.StartsWith(gitConfig.FeaturePrefix) && trimPrefix)
            {
                return branchName.Substring(gitConfig.FeaturePrefix.Length);
            }
            if (branchName.StartsWith(gitConfig.ReleasePrefix) && trimPrefix)
            {
                return branchName.Substring(gitConfig.ReleasePrefix.Length);
            }
            if (branchName.StartsWith(gitConfig.HotfixPrefix) && trimPrefix)
            {
                return branchName.Substring(gitConfig.HotfixPrefix.Length);
            }

            return branchName;
        }

        public static string GetSshSetup(DTE dte = null)
        {
            if (dte == null)
            {
                dte = EnvHelper.Dte;
            }

            var remoteOriginPuttyKeyfile = ProcessHelper.StartProcessGitResult(dte, "config --get remote.origin.puttykeyfile");
            if (string.IsNullOrEmpty(remoteOriginPuttyKeyfile)) return string.Empty;

            ProcessHelper.Start("pageant", remoteOriginPuttyKeyfile);
            return $"set GIT_SSH={FileHelper.GetTortoiseGitPlink()} && ";
        }

        public static GitConfig GetGitConfig(DTE dte = null)
        {
            if (dte == null)
            {
                dte = EnvHelper.Dte;
            }

            return new GitConfig(ProcessHelper.StartProcessGitResult(dte, "config --get-regexp gitflow"));
        }

        public static bool RemoteBranchExists(DTE dte, string branch)
        {
            return ProcessHelper.StartProcessGit(dte, $"show-ref refs/remotes/origin/{branch}");
        }
    }
}
