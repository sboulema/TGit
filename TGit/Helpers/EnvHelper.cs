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

        public bool IsGitFlow()
        {
            var gitConfig = GetGitConfig();
            return !string.IsNullOrEmpty(gitConfig.MasterBranch);
        }

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

        public bool HasStash()
        {
            return ProcessHelper.StartProcessGit(this, "stash list");
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

        public bool HasSolutionDir() => !string.IsNullOrEmpty(GetSolutionDir());

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

        public bool BranchNameStartsWith(string name) => GetBranchName().StartsWith(name);
    }
}
