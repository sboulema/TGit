using EnvDTE;
using SamirBoulema.TGit.Helpers;
using System;
using Microsoft.VisualStudio.Shell;

namespace SamirBoulema.TGit.Commands
{
    public class MainMenuCommands
    {
        private readonly DTE _dte;
        private readonly OleMenuCommandService _mcs;
        private readonly OptionPageGrid _generalOptions;

        public MainMenuCommands(OleMenuCommandService mcs, DTE dte, OptionPageGrid generalOptions)
        {
            _dte = dte;
            _mcs = mcs;
            _generalOptions = generalOptions;
        }

        public void AddCommands()
        {
            CommandHelper.AddCommand(_mcs, ShowChangesCommand, PkgCmdIDList.ShowChanges);
            CommandHelper.AddCommand(_mcs, PullCommand, PkgCmdIDList.Pull);
            CommandHelper.AddCommand(_mcs, FetchCommand, PkgCmdIDList.Fetch);
            CommandHelper.AddCommand(_mcs, CommitCommand, PkgCmdIDList.Commit);
            CommandHelper.AddCommand(_mcs, PushCommand, PkgCmdIDList.Push);

            CommandHelper.AddCommand(_mcs, ShowLogCommand, PkgCmdIDList.ShowLog);
            CommandHelper.AddCommand(_mcs, DiskBrowserCommand, PkgCmdIDList.DiskBrowser);
            CommandHelper.AddCommand(_mcs, RepoBrowserCommand, PkgCmdIDList.RepoBrowser);

            CommandHelper.AddCommand(_mcs, CreateStashCommand, PkgCmdIDList.CreateStash);
            var applyStash = CommandHelper.CreateCommand(ApplyStashCommand, PkgCmdIDList.ApplyStash);
            applyStash.BeforeQueryStatus += CommandHelper.ApplyStash_BeforeQueryStatus;
            CommandHelper.AddCommand(_mcs, applyStash);

            CommandHelper.AddCommand(_mcs, BranchCommand, PkgCmdIDList.Branch);
            CommandHelper.AddCommand(_mcs, SwitchCommand, PkgCmdIDList.Switch);
            CommandHelper.AddCommand(_mcs, MergeCommand, PkgCmdIDList.Merge);

            CommandHelper.AddCommand(_mcs, RevertCommand, PkgCmdIDList.Revert);
            CommandHelper.AddCommand(_mcs, ResolveCommand, PkgCmdIDList.Resolve);
            CommandHelper.AddCommand(_mcs, TagCommand, PkgCmdIDList.Tag);
            CommandHelper.AddCommand(_mcs, SyncCommand, PkgCmdIDList.Sync);
            CommandHelper.AddCommand(_mcs, CleanupCommand, PkgCmdIDList.Cleanup);
            CommandHelper.AddCommand(_mcs, BrowseRefCommand, PkgCmdIDList.BrowseRef);
            CommandHelper.AddCommand(_mcs, AbortMergeCommand, PkgCmdIDList.AbortMerge);

        }

        private void PreCommand()
        {
            FileHelper.SaveAllFiles(_dte);
        }

        private void ShowChangesCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:repostatus /path:\"{EnvHelper.GetSolutionDir(_dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PullCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:pull /path:\"{EnvHelper.GetSolutionDir(_dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:fetch /path:\"{EnvHelper.GetSolutionDir(_dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            PreCommand();
            var commitMessage = GitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte);
            var bugId = GitHelper.GetCommitMessage(_generalOptions.BugId, _dte);
            var gitConfig = GitHelper.GetGitConfig(_dte);
            ProcessHelper.StartTortoiseGitProc(
                $"/command:commit /path:\"{EnvHelper.GetSolutionDir(_dte)}\" " +
                $"{(string.IsNullOrEmpty(commitMessage) ? string.Empty : $"/logmsg:\"{commitMessage}\"")} " +
                $"{(!string.IsNullOrEmpty(bugId) && !string.IsNullOrEmpty(gitConfig.BugTraqMessage) ? $"/bugid:\"{bugId}\"" : string.Empty)} " +
                $"/closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PushCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:push /path:\"{EnvHelper.GetSolutionDir(_dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:log /path:\"{EnvHelper.GetSolutionDir(_dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.Start(EnvHelper.GetSolutionDir(_dte));
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:repobrowser /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:stashsave /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:reflog /ref:refs/stash /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:branch /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void SwitchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:switch /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void MergeCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:merge /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void AbortMergeCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:merge /abort /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void RevertCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:revert /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void CleanupCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:cleanup /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void ResolveCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:resolve /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void SyncCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:sync /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void BrowseRefCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:refbrowse /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }

        private void TagCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:tag /path:\"{EnvHelper.GetSolutionDir(_dte)}\"");
        }
    }
}
