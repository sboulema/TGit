using EnvDTE;
using SamirBoulema.TGit.Helpers;
using Microsoft.VisualStudio.Shell;
using System;

namespace SamirBoulema.TGit.Commands
{
    public class MainMenuCommands
    {
        private readonly ProcessHelper _processHelper;
        private readonly CommandHelper _commandHelper;
        private readonly GitHelper _gitHelper;
        private readonly DTE _dte;
        private readonly OptionPageGrid _generalOptions;
        private readonly OleMenuCommandService _mcs;

        public MainMenuCommands(ProcessHelper processHelper, CommandHelper commandHelper, GitHelper gitHelper, 
            DTE dte, OptionPageGrid generalOptions, OleMenuCommandService mcs)
        {
            _processHelper = processHelper;
            _commandHelper = commandHelper;
            _gitHelper = gitHelper;
            _dte = dte;
            _generalOptions = generalOptions;
            _mcs = mcs;
        }

        public void AddCommands()
        {
            _commandHelper.AddCommand(ShowChangesCommand, PkgCmdIDList.ShowChanges);
            _commandHelper.AddCommand(PullCommand, PkgCmdIDList.Pull);
            _commandHelper.AddCommand(FetchCommand, PkgCmdIDList.Fetch);
            _commandHelper.AddCommand(CommitCommand, PkgCmdIDList.Commit);
            //OleMenuCommand commit = commandHelper.CreateCommand(CommitCommand, PkgCmdIDList.Commit);
            //commit.BeforeQueryStatus += Diff_BeforeQueryStatus;
            //mcs.AddCommand(commit);
            _commandHelper.AddCommand(PushCommand, PkgCmdIDList.Push);

            _commandHelper.AddCommand(ShowLogCommand, PkgCmdIDList.ShowLog);
            _commandHelper.AddCommand(DiskBrowserCommand, PkgCmdIDList.DiskBrowser);
            _commandHelper.AddCommand(RepoBrowserCommand, PkgCmdIDList.RepoBrowser);

            _commandHelper.AddCommand(CreateStashCommand, PkgCmdIDList.CreateStash);
            OleMenuCommand applyStash = _commandHelper.CreateCommand(ApplyStashCommand, PkgCmdIDList.ApplyStash);
            applyStash.BeforeQueryStatus += _commandHelper.ApplyStash_BeforeQueryStatus;
            _mcs.AddCommand(applyStash);

            _commandHelper.AddCommand(BranchCommand, PkgCmdIDList.Branch);
            _commandHelper.AddCommand(SwitchCommand, PkgCmdIDList.Switch);
            _commandHelper.AddCommand(MergeCommand, PkgCmdIDList.Merge);

            _commandHelper.AddCommand(RevertCommand, PkgCmdIDList.Revert);
            _commandHelper.AddCommand(ResolveCommand, PkgCmdIDList.Resolve);
            _commandHelper.AddCommand(SyncCommand, PkgCmdIDList.Sync);
            _commandHelper.AddCommand(CleanupCommand, PkgCmdIDList.Cleanup);
        }

        private void ShowChangesCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:repostatus /path:\"{solutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PullCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:pull /path:\"{solutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:fetch /path:\"{solutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc(
                $"/command:commit /path:\"{solutionDir}\" /logmsg:\"{_gitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PushCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:push /path:\"{solutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:log /path:\"{solutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.Start(solutionDir);
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:repobrowser /path:\"{solutionDir}\"");
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:stashsave /path:\"{solutionDir}\"");
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:reflog /ref:refs/stash /path:\"{solutionDir}\"");
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:branch /path:\"{solutionDir}\"");
        }
        private void SwitchCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:switch /path:\"{solutionDir}\"");
        }
        private void MergeCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:merge /path:\"{solutionDir}\"");
        }
        private void RevertCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            FileHelper.SaveAllFiles(_dte);
            _processHelper.StartTortoiseGitProc($"/command:revert /path:\"{solutionDir}\"");
        }
        private void CleanupCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:cleanup /path:\"{solutionDir}\"");
        }

        private void ResolveCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:resolve /path:\"{solutionDir}\"");
        }

        private void SyncCommand(object sender, EventArgs e)
        {
            string solutionDir = FileHelper.GetSolutionDir(_dte);
            if (string.IsNullOrEmpty(solutionDir)) return;
            _processHelper.StartTortoiseGitProc($"/command:sync /path:\"{solutionDir}\"");
        }
    }
}
