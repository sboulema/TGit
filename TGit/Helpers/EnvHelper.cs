using EnvDTE;

namespace SamirBoulema.TGit.Helpers
{
    public static class EnvHelper
    {
        private static string _solutionDir;
        private static string _tortoiseGitProc;
        private static string _git;
        public static DTE Dte;

        public static bool IsGitFlow(DTE dte = null)
        {
            if (dte == null)
            {
                dte = Dte;
            }
            var gitConfig = GitHelper.GetGitConfig(dte);
            return !string.IsNullOrEmpty(gitConfig.MasterBranch);
        }

        public static string GetSolutionDir(DTE dte = null)
        {
            if (dte == null)
            {
                dte = Dte;
            }
            if (string.IsNullOrEmpty(_solutionDir))
            {
                _solutionDir = FileHelper.GetSolutionDir(dte);
            }
            return _solutionDir;
        }

        public static bool HasStash(DTE dte = null)
        {
            if (dte == null)
            {
                dte = Dte;
            }
            return ProcessHelper.StartProcessGit(dte, "stash list");
        }

        public static string GetTortoiseGitProc()
        {
            if (string.IsNullOrEmpty(_tortoiseGitProc))
            {
                _tortoiseGitProc = FileHelper.GetTortoiseGitProc();
            }
            return _tortoiseGitProc;
        }

        public static string GetGit()
        {
            if (string.IsNullOrEmpty(_git))
            {
                _git = FileHelper.GetMSysGit();
            }
            return _git; 
        }

        public static bool HasSolutionDir(DTE dte = null) => !string.IsNullOrEmpty(GetSolutionDir(dte));

        public static bool BranchNameStartsWith(string name, DTE dte = null)
        {
            if (dte == null)
            {
                dte = Dte;
            }
            return GitHelper.GetCurrentBranchName(false, dte).StartsWith(name);
        }

        public static void Clear()
        {
            _solutionDir = string.Empty;
            _tortoiseGitProc = string.Empty;
            _git = string.Empty;
        }
    }
}
