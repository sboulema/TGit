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
            CommandHelper.AddCommand(_mcs, ApplyStashCommand, PkgCmdIDList.ApplyStash, CommandHelper.ApplyStash_BeforeQueryStatus);

            CommandHelper.AddCommand(_mcs, BranchCommand, PkgCmdIDList.Branch);
            CommandHelper.AddCommand(_mcs, SwitchCommand, PkgCmdIDList.Switch);
            CommandHelper.AddCommand(_mcs, MergeCommand, PkgCmdIDList.Merge);
            CommandHelper.AddCommand(_mcs, RebaseCommand, PkgCmdIDList.Rebase);

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
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:repostatus /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PullCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:pull /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void FetchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:fetch /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void CommitCommand(object sender, EventArgs e)
        {
            PreCommand();
            var commitMessage = GitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte, _envHelper);
            var bugId = GitHelper.GetCommitMessage(_generalOptions.BugId, _dte, _envHelper);
            var gitConfig = GitHelper.GetGitConfig(_envHelper);
            ProcessHelper.StartTortoiseGitProc(_envHelper,
                $"/command:commit " +
                $"{(string.IsNullOrEmpty(commitMessage) ? string.Empty : $"/logmsg:\"{commitMessage}\"")} " +
                $"{(!string.IsNullOrEmpty(bugId) && !string.IsNullOrEmpty(gitConfig.BugTraqMessage) ? $"/bugid:\"{bugId}\"" : string.Empty)} " +
                $"/closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void PushCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:push /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void ShowLogCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:log /closeonend:{_generalOptions.CloseOnEnd}");
        }
        private void DiskBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.Start(_envHelper.GetGitRoot());
        }
        private void RepoBrowserCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:repobrowser");
        }
        private void CreateStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:stashsave");
        }
        private void ApplyStashCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:reflog /ref:refs/stash");
        }
        private void BranchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:branch");
        }

        private void SwitchCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:switch");
        }

        private void MergeCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:merge");
        }

        private void RebaseCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:rebase");
        }

        private void AbortMergeCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:merge /abort");
        }

        private void RevertCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:revert");
        }

        private void CleanupCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:cleanup");
        }

        private void ResolveCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:resolve");
        }

        private void SyncCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:sync");
        }

        private void BrowseRefCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:refbrowse");
        }

        private void TagCommand(object sender, EventArgs e)
        {
            PreCommand();
            ProcessHelper.StartTortoiseGitProc(_envHelper, $"/command:tag");
        }
    }
}
