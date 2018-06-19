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
        private readonly EnvHelper _envHelper;

        public MainMenuCommands(OleMenuCommandService mcs, DTE dte, OptionPageGrid generalOptions, EnvHelper envHelper)
        {
            _dte = dte;
            _mcs = mcs;
            _generalOptions = generalOptions;
            _envHelper = envHelper;
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
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:repostatus /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PullCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:pull /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:fetch /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            PreCommand();
            var commitMessage = GitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte, _envHelper);
            var bugId = GitHelper.GetCommitMessage(_generalOptions.BugId, _dte, _envHelper);
            var gitConfig = GitHelper.GetGitConfig(_envHelper);
            ProcessHelper.StartTortoiseGitProc(_envHelper,
                $"/command:commit /path:\"{_envHelper.GetSolutionDir()}\" " +
                $"{(string.IsNullOrEmpty(commitMessage) ? string.Empty : $"/logmsg:\"{commitMessage}\"")} " +
                $"{(!string.IsNullOrEmpty(bugId) && !string.IsNullOrEmpty(gitConfig.BugTraqMessage) ? $"/bugid:\"{bugId}\"" : string.Empty)} " +
                $"/closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PushCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:push /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:log /path:\"{_envHelper.GetSolutionDir()}\" /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.Start(_envHelper.GetSolutionDir());
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:repobrowser /path:\"{_envHelper.GetSolutionDir()}\"");
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:stashsave /path:\"{_envHelper.GetSolutionDir()}\"");
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:reflog /ref:refs/stash /path:\"{_envHelper.GetSolutionDir()}\"");
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:branch /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void SwitchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:switch /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void MergeCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:merge /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void AbortMergeCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:merge /abort /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void RevertCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:revert /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void CleanupCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:cleanup /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void ResolveCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:resolve /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void SyncCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:sync /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void BrowseRefCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:refbrowse /path:\"{_envHelper.GetSolutionDir()}\"");
        }

        private void TagCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:tag /path:\"{_envHelper.GetSolutionDir()}\"");
        }
    }
}
