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
        }

        private void PreCommand()
        {
            EnvHelper.GetGitConfig();
            EnvHelper.GetBranchName();
            EnvHelper.GetStash();
            FileHelper.SaveAllFiles(_dte);
        }

        private void ShowChangesCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:repostatus /path:\"{EnvHelper.SolutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PullCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:pull /path:\"{EnvHelper.SolutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:fetch /path:\"{EnvHelper.SolutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            PreCommand();
            var commitMessage = GitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte);
            var bugId = GitHelper.GetCommitMessage(_generalOptions.BugId, _dte);
            ProcessHelper.StartTortoiseGitProc(
                $"/command:commit /path:\"{EnvHelper.SolutionDir}\" " +
                $"{(string.IsNullOrEmpty(commitMessage) ? string.Empty : $"/logmsg:\"{commitMessage}\"")} " +
                $"{(!string.IsNullOrEmpty(bugId) && !string.IsNullOrEmpty(EnvHelper.GitConfig.BugTraqMessage) ? $"/bugid:\"{bugId}\"" : string.Empty)} " +
                $"/closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PushCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:push /path:\"{EnvHelper.SolutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:log /path:\"{EnvHelper.SolutionDir}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.Start(EnvHelper.SolutionDir);
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:repobrowser /path:\"{EnvHelper.SolutionDir}\"");
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:stashsave /path:\"{EnvHelper.SolutionDir}\"");
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:reflog /ref:refs/stash /path:\"{EnvHelper.SolutionDir}\"");
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:branch /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void SwitchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:switch /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void MergeCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:merge /path:\"{EnvHelper.SolutionDir}\"");
        }
        private void RevertCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:revert /path:\"{EnvHelper.SolutionDir}\"");
        }
        private void CleanupCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:cleanup /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void ResolveCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:resolve /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void SyncCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:sync /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void BrowseRefCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:refbrowse /path:\"{EnvHelper.SolutionDir}\"");
        }

        private void TagCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc($"/command:tag /path:\"{EnvHelper.SolutionDir}\"");
        }
    }
}
