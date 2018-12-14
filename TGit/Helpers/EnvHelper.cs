using EnvDTE;
using SamirBoulema.TGit.Models;
using System;
using System.IO;
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
        /// <remarks>Getting the actual git config is cached for 1m</remarks>
        /// <returns></returns>
        public bool IsGitFlow()
        {
            var gitConfig = GetGitConfig();
            return !string.IsNullOrEmpty(gitConfig.MasterBranch);
        }

        /// <summary>
        /// Check if Git SVN is used for this repo
        /// </summary>
        /// <remarks>Getting the actual git config is cached for 1m</remarks>
        /// <returns></returns>
        public bool IsGitSvn()
        {
            var gitConfig = GetGitConfig();
            return !string.IsNullOrEmpty(gitConfig.SvnUrl);
        }

        /// <summary>
        /// Get the Git config
        /// </summary>
        /// <remarks>Cached for 1m</remarks>
        /// <returns></returns>
        public GitConfig GetGitConfig()
        {
            if (_cache.Contains(CacheKeyEnum.GitConfig.ToString()))
            {
                return (GitConfig)_cache.Get(CacheKeyEnum.GitConfig.ToString());
            }

            var gitConfig = GitHelper.GetGitConfig(this);
            if (gitConfig != null)
            {
                _cache.Set(CacheKeyEnum.GitConfig.ToString(), gitConfig, DateTimeOffset.Now.AddMinutes(1));
            }
            return gitConfig;
        }

        /// <summary>
        /// Do we have any stashes available
        /// </summary>
        /// <remarks>Cached for 1m</remarks>
        /// <returns></returns>
        public bool HasStash()
        {
            if (_cache.Contains(CacheKeyEnum.HasStash.ToString()))
            {
                return bool.Parse(_cache.Get(CacheKeyEnum.HasStash.ToString()).ToString());
            }

            var hasStash = ProcessHelper.StartProcessGit(this, "stash list");
            _cache.Set(CacheKeyEnum.HasStash.ToString(), hasStash, DateTimeOffset.Now.AddMinutes(1));
            return hasStash;
        }

        /// <summary>
        /// Get the path to the TortoiseGit process
        /// </summary>
        /// <remarks>Cached for 1h</remarks>
        /// <returns></returns>
        public string GetTortoiseGitProc()
        {
            if (_cache.Contains(CacheKeyEnum.TortoiseGitProc.ToString()))
            {
                return _cache.Get(CacheKeyEnum.TortoiseGitProc.ToString()).ToString();
            }

            var tortoiseGitProc = FileHelper.GetTortoiseGitProc();
            if (!string.IsNullOrEmpty(tortoiseGitProc))
            {
                _cache.Set(CacheKeyEnum.TortoiseGitProc.ToString(), tortoiseGitProc, DateTimeOffset.Now.AddHours(1));
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
            if (_cache.Contains(CacheKeyEnum.Git.ToString()))
            {
                return _cache.Get(CacheKeyEnum.Git.ToString()).ToString();
            }

            var git = FileHelper.GetMSysGit();
            if (!string.IsNullOrEmpty(git))
            {
                _cache.Set(CacheKeyEnum.Git.ToString(), git, DateTimeOffset.Now.AddHours(1));
            }
            return git; 
        }

        /// <summary>
        /// Get the Git branch name eg. 'feature/tgit'
        /// </summary>
        /// <remarks>Cached for 15s</remarks>
        /// <returns></returns>
        private string GetBranchName()
        {
            if (_cache.Contains(CacheKeyEnum.BranchName.ToString()))
            {
                return _cache.Get(CacheKeyEnum.BranchName.ToString()).ToString();
            }

            var branchName = GitHelper.GetCurrentBranchName(false, this);
            if (!string.IsNullOrEmpty(branchName))
            {
                _cache.Set(CacheKeyEnum.BranchName.ToString(), branchName, DateTimeOffset.Now.AddSeconds(15));
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

        /// <summary>
        /// Clear key from the cache
        /// </summary>
        /// <param name="key"></param>
        public void ClearCache(CacheKeyEnum key) => _cache.Remove(key.ToString());

        public string GetGitRoot(string workingDir = null)
        {
            if (string.IsNullOrEmpty(workingDir))
            {
                workingDir = _dte.Solution.FullName;
            }

            if (File.Exists(workingDir))
            {
                workingDir = Path.GetDirectoryName(workingDir);
            }

            var gitRoot = GitHelper.GetGitRoot(this, workingDir);

            if (!Directory.Exists(gitRoot))
            {
                return string.Empty;
            }

            return gitRoot;
        }

        public bool HasGitRoot() => !string.IsNullOrEmpty(GetGitRoot());
    }
}
