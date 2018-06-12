using EnvDTE;

namespace SamirBoulema.TGit.Helpers
{
    public static class EnvHelper
    {
        public static string BranchName;
        private static string _solutionDir;
        public static string TortoiseGitProc;
        public static string Git;
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

        public static void GetTortoiseGitProc() => TortoiseGitProc = FileHelper.GetTortoiseGitProc();

        public static void GetGit() => Git = FileHelper.GetMSysGit();

        public static bool HasSolutionDir(DTE dte = null) => !string.IsNullOrEmpty(GetSolutionDir(dte));

        public static void Clear()
        {
            BranchName = string.Empty;
            _solutionDir = string.Empty;
            TortoiseGitProc = string.Empty;
            Git = string.Empty;
        }
    }
}
