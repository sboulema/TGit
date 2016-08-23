using EnvDTE;

namespace SamirBoulema.TGit.Helpers
{
    public static class EnvHelper
    {
        public static FlowOptions FlowOptions;
        public static bool IsGitFlow;
        public static string BranchName;
        public static string SolutionDir;
        public static bool HasStash;
        public static string TortoiseGitProc;
        public static string Git;

        public static void GetFlowOptions()
        {
            FlowOptions = GitHelper.GetFlowOptions();
            IsGitFlow = !string.IsNullOrEmpty(FlowOptions.MasterBranch);
        }

        public static void GetBranchName()
        {
            BranchName = GitHelper.GetCurrentBranchName(false);
        }

        public static void GetSolutionDir(DTE dte)
        {
            SolutionDir = FileHelper.GetSolutionDir(dte);
        }

        public static void GetStash()
        {
            HasStash = ProcessHelper.StartProcessGit("stash list");
        }

        public static void GetTortoiseGitProc()
        {
            TortoiseGitProc = FileHelper.GetTortoiseGitProc();
        }

        public static void GetGit()
        {
            Git = FileHelper.GetMSysGit();
        }

        public static bool HasSolutionDir()
        {
            return !string.IsNullOrEmpty(SolutionDir);
        }

        public static void Clear()
        {
            FlowOptions = null;
            IsGitFlow = false;
            BranchName = string.Empty;
            SolutionDir = string.Empty;
            TortoiseGitProc = string.Empty;
            HasStash = false;
            Git = string.Empty;
        }
    }
}
