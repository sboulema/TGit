using EnvDTE;
using SamirBoulema.TGIT.Helpers;
using Microsoft.VisualStudio.Shell;
using System;

namespace SamirBoulema.TGIT.Commands
{
    public class MainMenuCommands
    {
        private ProcessHelper processHelper;
        private CommandHelper commandHelper;
        private FileHelper fileHelper;
        private GitHelper gitHelper;
        private DTE dte;
        private OptionPageGrid options;
        private OleMenuCommandService mcs;

        public MainMenuCommands(ProcessHelper processHelper, CommandHelper commandHelper, GitHelper gitHelper, FileHelper fileHelper,
            DTE dte, OptionPageGrid options, OleMenuCommandService mcs)
        {
            this.processHelper = processHelper;
            this.commandHelper = commandHelper;
            this.gitHelper = gitHelper;
            this.fileHelper = fileHelper;
            this.dte = dte;
            this.options = options;
            this.mcs = mcs;
        }

        public void AddCommands()
        {
            commandHelper.AddCommand(ShowChangesCommand, PkgCmdIDList.ShowChanges);
            commandHelper.AddCommand(PullCommand, PkgCmdIDList.Pull);
            commandHelper.AddCommand(FetchCommand, PkgCmdIDList.Fetch);
            commandHelper.AddCommand(CommitCommand, PkgCmdIDList.Commit);
            //OleMenuCommand commit = commandHelper.CreateCommand(CommitCommand, PkgCmdIDList.Commit);
            //commit.BeforeQueryStatus += Diff_BeforeQueryStatus;
            //mcs.AddCommand(commit);
            commandHelper.AddCommand(PushCommand, PkgCmdIDList.Push);

            commandHelper.AddCommand(ShowLogCommand, PkgCmdIDList.ShowLog);
            commandHelper.AddCommand(DiskBrowserCommand, PkgCmdIDList.DiskBrowser);
            commandHelper.AddCommand(RepoBrowserCommand, PkgCmdIDList.RepoBrowser);

            commandHelper.AddCommand(CreateStashCommand, PkgCmdIDList.CreateStash);
            OleMenuCommand applyStash = commandHelper.CreateCommand(ApplyStashCommand, PkgCmdIDList.ApplyStash);
            applyStash.BeforeQueryStatus += commandHelper.ApplyStash_BeforeQueryStatus;
            mcs.AddCommand(applyStash);

            commandHelper.AddCommand(BranchCommand, PkgCmdIDList.Branch);
            commandHelper.AddCommand(SwitchCommand, PkgCmdIDList.Switch);
            commandHelper.AddCommand(MergeCommand, PkgCmdIDList.Merge);

            commandHelper.AddCommand(RevertCommand, PkgCmdIDList.Revert);
            commandHelper.AddCommand(ResolveCommand, PkgCmdIDList.Resolve);
            commandHelper.AddCommand(SyncCommand, PkgCmdIDList.Sync);
            commandHelper.AddCommand(CleanupCommand, PkgCmdIDList.Cleanup);
        }

        private void ShowChangesCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:repostatus /path:\"{0}\" /closeonend:0", solutionDir));
        }
        private void PullCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:pull /path:\"{0}\" /closeonend:0", solutionDir));
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:fetch /path:\"{0}\" /closeonend:0", solutionDir));
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:commit /path:\"{0}\" /logmsg:\"{1}\" /closeonend:0", solutionDir,
                gitHelper.GetCommitMessage(options.CommitMessage, dte)));
        }
        private void PushCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc(string.Format("/command:push /path:\"{0}\" /closeonend:0", solutionDir));
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc(string.Format("/command:log /path:\"{0}\" /closeonend:0", solutionDir));
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.Start(solutionDir);
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc(string.Format("/command:repobrowser /path:\"{0}\"", solutionDir));
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:stashsave /path:\"{0}\"", solutionDir));
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:reflog /ref:refs/stash /path:\"{0}\"", solutionDir));
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:branch /path:\"{0}\"", solutionDir));
        }
        private void SwitchCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:switch /path:\"{0}\"", solutionDir));
        }
        private void MergeCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:merge /path:\"{0}\"", solutionDir));
        }
        private void RevertCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(string.Format("/command:revert /path:\"{0}\"", solutionDir));
        }
        private void CleanupCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc(string.Format("/command:cleanup /path:\"{0}\"", solutionDir));
        }

        private void ResolveCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc(string.Format("/command:resolve /path:\"{0}\"", solutionDir));
        }

        private void SyncCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc(string.Format("/command:sync /path:\"{0}\"", solutionDir));
        }
    }
}
