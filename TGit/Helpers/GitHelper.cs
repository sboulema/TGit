using Community.VisualStudio.Toolkit;
using Microsoft.Win32;
using SamirBoulema.TGit.Models;
using System.IO;
using System.Threading.Tasks;

namespace SamirBoulema.TGit.Helpers
{
    public static class GitHelper
    {
        private static GitConfig _gitConfig;
        private static string _solutionFullPath;

        public static async Task<string> GetCommitMessage(string commitMessageTemplate)
        {
            if (string.IsNullOrEmpty(commitMessageTemplate))
            {
                return string.Empty;
            }

            var solution = await VS.Solutions.GetCurrentSolutionAsync();
            var version = await VS.Shell.GetVsVersionAsync();

            var commitMessage = commitMessageTemplate;
            commitMessage = commitMessage.Replace("$(BranchName)", await GetCurrentBranchName(false));
            commitMessage = commitMessage.Replace("$(FeatureName)", await GetCurrentBranchName(true));
            commitMessage = commitMessage.Replace("$(DevEnvDir)", (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\SxS\\VS7\\", version.ToString(), string.Empty));
            commitMessage = commitMessage.Replace("$(SolutionDir)", Path.GetDirectoryName(solution?.FullPath));
            commitMessage = commitMessage.Replace("$(SolutionPath)", Path.GetFullPath(solution?.FullPath));
            commitMessage = commitMessage.Replace("$(SolutionName)", solution?.Name);
            commitMessage = commitMessage.Replace("$(SolutionFileName)", Path.GetFileName(solution?.FullPath));
            commitMessage = commitMessage.Replace("$(SolutionExt)", Path.GetExtension(solution?.FullPath));
            commitMessage = commitMessage.Replace("$(VSInstallDir)", (string)Registry.GetValue($"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{version}", "InstallDir", string.Empty));
            commitMessage = commitMessage.Replace("$(FxCopDir)", (string)Registry.GetValue($"HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\VisualStudio\\{version}\\Edev", "FxCopDir", string.Empty));
            return commitMessage;
        }

        public static async Task<string> GetCurrentBranchName(bool trimPrefix)
        {
            var branchName = await ProcessHelper.StartProcessGitResult("symbolic-ref -q --short HEAD");

            if (branchName == null)
            {
                return string.Empty;
            }

            var gitConfig = await GetGitConfig();

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

        public static async Task<string> GetSshSetup()
        {
            var remoteOriginPuttyKeyfile = await ProcessHelper.StartProcessGitResult("config --get remote.origin.puttykeyfile");

            if (string.IsNullOrEmpty(remoteOriginPuttyKeyfile))
            {
                return string.Empty;
            }

            ProcessHelper.Start("pageant", remoteOriginPuttyKeyfile);

            return $"set GIT_SSH={FileHelper.GetTortoiseGitPlink()} && ";
        }

        /// <summary>
        /// Get GIT config.
        /// </summary>
        /// <remarks>
        /// - Cache received config based on the currently opened solution file path.
        /// - If the solution file path is different from last time we got the git config,
        ///   it means we have a new solution and should also get new git config.
        /// </remarks>
        /// <returns>Strong typed GIT config about gitflow, svn and bugtraq.</returns>
        public static async Task<GitConfig> GetGitConfig()
        {
            var solution = await VS.Solutions.GetCurrentSolutionAsync();

            if (_gitConfig == null ||
                string.IsNullOrEmpty(_solutionFullPath) ||
                !_solutionFullPath.Equals(solution?.FullPath))
            {
                _gitConfig = new GitConfig(await ProcessHelper.StartProcessGitResult("config --get-regexp \"gitflow|bugtraq|svn-remote\""));
                _solutionFullPath = solution?.FullPath;
            }

            return _gitConfig;
        }

        public static async Task<bool> RemoteBranchExists(string branch)
        {
            return await ProcessHelper.StartProcessGit($"show-ref refs/remotes/origin/{branch}");
        }

        public static string FormatCliCommand(string gitCommand, bool appendNextLine = true)
        {
            var git = FileHelper.GetMSysGit();
            return $"echo ^> {Path.GetFileNameWithoutExtension(git)} {gitCommand} && \"{git}\" {gitCommand}{(appendNextLine ? " && " : string.Empty)}";
        }

        /// <summary>
        /// Check if GitFlow is initialized
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsGitFlow()
        {
            var gitConfig = await GetGitConfig();
            return !string.IsNullOrEmpty(gitConfig.MasterBranch);
        }

        /// <summary>
        /// Check if Git SVN is used for this repo
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsGitSvn()
        {
            var gitConfig = await GetGitConfig();
            return !string.IsNullOrEmpty(gitConfig.SvnUrl);
        }

        /// <summary>
        /// Do we have any stashes available
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> HasStash()
            => await ProcessHelper.StartProcessGit("stash list");
    }
}
