using EnvDTE;
using System;
using System.Runtime.Caching;

namespace SamirBoulema.TGit.Helpers
{
    /// <summary>
    /// Provide cached access to a number of environment variables
    /// </summary>
    public class EnvHelper
    {
        private readonly DTE _dte;
        private MemoryCache _cache;

        public EnvHelper(DTE dte)
        {
            _dte = dte;
            _cache = new MemoryCache("TGIT");
        }

        /// <summary>
        /// Check if GitFlow is initialized
        /// </summary>
        /// <remarks>Getting the actual flow config is cached for 1m</remarks>
        /// <returns></returns>
        public bool IsGitFlow()
        {
            var gitConfig = GetGitConfig();
            return !string.IsNullOrEmpty(gitConfig.MasterBranch);
        }

        /// <summary>
        /// Get the Git config
        /// </summary>
        /// <remarks>Cached for 1m</remarks>
        /// <returns></returns>
        public GitConfig GetGitConfig()
        {
            if (_cache.Contains("GitConfig"))
            {
                return (GitConfig)_cache.Get("GitConfig");
            }

            var gitConfig = GitHelper.GetGitConfig(this);
            if (gitConfig != null)
            {
                _cache.Set("GitConfig", gitConfig, DateTimeOffset.Now.AddMinutes(1));
            }
            return gitConfig;
        }

        /// <summary>
        /// Get the root of the solution path (where the .git folder resides)
        /// </summary>
        /// <remarks>Cached for 30s</remarks>
        /// <returns></returns>
        public string GetSolutionDir()
        {
            if (_cache.Contains("SolutionDir"))
            {
                return _cache.Get("SolutionDir").ToString();
            }

            var solutionDir = FileHelper.GetSolutionDir(_dte);
            if (!string.IsNullOrEmpty(solutionDir))
            {
                _cache.Set("SolutionDir", solutionDir, DateTimeOffset.Now.AddSeconds(30));
            }
            return solutionDir;
        }

        /// <summary>
        /// Do we have any stashes available
        /// </summary>
        /// <remarks>Cached for 1m</remarks>
        /// <returns></returns>
        public bool HasStash()
        {
            if (_cache.Contains("HasStash"))
            {
                return bool.Parse(_cache.Get("HasStash").ToString());
            }

            var hasStash = ProcessHelper.StartProcessGit(this, "stash list");
            _cache.Set("HasStash", hasStash, DateTimeOffset.Now.AddMinutes(1));
            return hasStash;
        }

        /// <summary>
        /// Get the path to the TortoiseGit process
        /// </summary>
        /// <remarks>Cached for 1h</remarks>
        /// <returns></returns>
        public string GetTortoiseGitProc()
        {
            if (_cache.Contains("TortoiseGitProc"))
            {
                return _cache.Get("TortoiseGitProc").ToString();
            }

            var tortoiseGitProc = FileHelper.GetTortoiseGitProc();
            if (!string.IsNullOrEmpty(tortoiseGitProc))
            {
                _cache.Set("TortoiseGitProc", tortoiseGitProc, DateTimeOffset.Now.AddHours(1));
            }
            return tortoiseGitProc;
        }

        /// <summary>
        /// Get the path to the Git process
        /// </summary>
        /// <remarks>Cached for 1h</remarks>
        /// <returns></returns>
        public string GetGit()
        {
            if (_cache.Contains("Git"))
            {
                return _cache.Get("Git").ToString();
            }

            var git = FileHelper.GetMSysGit();
            if (!string.IsNullOrEmpty(git))
            {
                _cache.Set("Git", git, DateTimeOffset.Now.AddHours(1));
            }
            return git; 
        }

        /// <summary>
        /// Check if we have a path to a solution directory
        /// </summary>
        /// <remarks>Actual solution dir path is cached for 30s</remarks>
        /// <returns></returns>
        public bool HasSolutionDir() => !string.IsNullOrEmpty(GetSolutionDir());

        /// <summary>
        /// Get the Git branch name eg. 'feature/tgit'
        /// </summary>
        /// <remarks>Cached for 15s</remarks>
        /// <returns></returns>
        private string GetBranchName()
        {
            if (_cache.Contains("BranchName"))
            {
                return _cache.Get("BranchName").ToString();
            }

            var branchName = GitHelper.GetCurrentBranchName(false, this);
            if (!string.IsNullOrEmpty(branchName))
            {
                _cache.Set("BranchName", branchName, DateTimeOffset.Now.AddSeconds(15));
            }
            return branchName;
        }

        /// <summary>
        /// Check if a branch name starts with a given value
        /// </summary>
        /// <remarks>Actual branch name is cached for 15s</remarks>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool BranchNameStartsWith(string name) => GetBranchName().StartsWith(name);
    }
}
