using EnvDTE;
using SamirBoulema.TGit.Helpers;
using System;

namespace SamirBoulema.TGit.Commands
{
    public class MainMenuCommands
    {
        private readonly CommandHelper _commandHelper;
        private readonly DTE _dte;
        private readonly OptionPageGrid _generalOptions;

        public MainMenuCommands(CommandHelper commandHelper, DTE dte, OptionPageGrid generalOptions)
        {
            _commandHelper = commandHelper;
            _dte = dte;
            _generalOptions = generalOptions;
        }

        public void AddCommands()
        {
            _commandHelper.AddCommand(ShowChangesCommand, PkgCmdIDList.ShowChanges);
            _commandHelper.AddCommand(PullCommand, PkgCmdIDList.Pull);
            _commandHelper.AddCommand(FetchCommand, PkgCmdIDList.Fetch);
            _commandHelper.AddCommand(CommitCommand, PkgCmdIDList.Commit);
            _commandHelper.AddCommand(PushCommand, PkgCmdIDList.Push);

            _commandHelper.AddCommand(ShowLogCommand, PkgCmdIDList.ShowLog);
            _commandHelper.AddCommand(DiskBrowserCommand, PkgCmdIDList.DiskBrowser);
            _commandHelper.AddCommand(RepoBrowserCommand, PkgCmdIDList.RepoBrowser);

            _commandHelper.AddCommand(CreateStashCommand, PkgCmdIDList.CreateStash);
            var applyStash = _commandHelper.CreateCommand(ApplyStashCommand, PkgCmdIDList.ApplyStash);
            applyStash.BeforeQueryStatus += _commandHelper.ApplyStash_BeforeQueryStatus;
            _commandHelper.AddCommand(applyStash);

            _commandHelper.AddCommand(BranchCommand, PkgCmdIDList.Branch);
            _commandHelper.AddCommand(SwitchCommand, PkgCmdIDList.Switch);
            _commandHelper.AddCommand(MergeCommand, PkgCmdIDList.Merge);

            _commandHelper.AddCommand(RevertCommand, PkgCmdIDList.Revert);
            _commandHelper.AddCommand(ResolveCommand, PkgCmdIDList.Resolve);
            _commandHelper.AddCommand(SyncCommand, PkgCmdIDList.Sync);
            _commandHelper.AddCommand(CleanupCommand, PkgCmdIDList.Cleanup);
        }

        private void PreCommand()
        {
            EnvHelper.GetFlowOptions();
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
            ProcessHelper.StartTortoiseGitProc(
                $"/command:commit /path:\"{EnvHelper.SolutionDir}\" /logmsg:\"{GitHelper.GetCommitMessage(_generalOptions.CommitMessage, _dte)}\" /closeonend:{_generalOptions.CloseOnEnd}");
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
    }
}
