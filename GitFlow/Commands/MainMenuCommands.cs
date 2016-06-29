using EnvDTE;
using SamirBoulema.TGIT.Helpers;
using Microsoft.VisualStudio.Shell;
using System;

namespace SamirBoulema.TGIT.Commands
{
    public class MainMenuCommands
    {
        private readonly ProcessHelper processHelper;
        private readonly CommandHelper commandHelper;
        private readonly FileHelper fileHelper;
        private readonly GitHelper gitHelper;
        private readonly DTE dte;
        private readonly OptionPageGrid options;
        private readonly OleMenuCommandService mcs;

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
            processHelper.StartTortoiseGitProc($"/command:repostatus /path:\"{solutionDir}\" /closeonend:0");
        }
        private void PullCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:pull /path:\"{solutionDir}\" /closeonend:0");
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:fetch /path:\"{solutionDir}\" /closeonend:0");
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc(
                $"/command:commit /path:\"{solutionDir}\" /logmsg:\"{gitHelper.GetCommitMessage(options.CommitMessage, dte)}\" /closeonend:0");
        }
        private void PushCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc($"/command:push /path:\"{solutionDir}\" /closeonend:0");
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc($"/command:log /path:\"{solutionDir}\" /closeonend:0");
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
            processHelper.StartTortoiseGitProc($"/command:repobrowser /path:\"{solutionDir}\"");
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:stashsave /path:\"{solutionDir}\"");
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:reflog /ref:refs/stash /path:\"{solutionDir}\"");
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:branch /path:\"{solutionDir}\"");
        }
        private void SwitchCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:switch /path:\"{solutionDir}\"");
        }
        private void MergeCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:merge /path:\"{solutionDir}\"");
        }
        private void RevertCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            fileHelper.SaveAllFiles();
            processHelper.StartTortoiseGitProc($"/command:revert /path:\"{solutionDir}\"");
        }
        private void CleanupCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc($"/command:cleanup /path:\"{solutionDir}\"");
        }

        private void ResolveCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc($"/command:resolve /path:\"{solutionDir}\"");
        }

        private void SyncCommand(object sender, EventArgs e)
        {
            string solutionDir = fileHelper.GetSolutionDir();
            if (string.IsNullOrEmpty(solutionDir)) return;
            processHelper.StartTortoiseGitProc($"/command:sync /path:\"{solutionDir}\"");
        }
    }
}
