using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit.Helpers
{
    public class CommandHelper
    {
        private readonly ProcessHelper _processHelper;
        private readonly OleMenuCommandService _mcs;
        private readonly TGitPackage _package;

        public CommandHelper(ProcessHelper processHelper, OleMenuCommandService mcs, TGitPackage package)
        {
            _processHelper = processHelper;
            _mcs = mcs;
            _package = package;
        }

        public void AddCommand(EventHandler handler, uint commandId)
        {
            _mcs.AddCommand(CreateCommand(handler, commandId));
        }

        public OleMenuCommand CreateCommand(EventHandler handler, uint commandId)
        {
            var menuCommandId = new CommandID(GuidList.GuidTgitCmdSet, (int)commandId);
            var menuItem = new OleMenuCommand(handler, menuCommandId);
            menuItem.BeforeQueryStatus += Solution_BeforeQueryStatus;
            return menuItem;
        }

        public OleMenuCommand CreateCommand(uint commandId)
        {
            return CreateCommand(null, commandId);
        }

        public void ApplyStash_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = _processHelper.StartProcessGit("stash list");
        }

        private void Diff_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Enabled = _processHelper.StartProcessGit("diff");
        }

        public void Feature_BeforeQueryStatus(object sender, EventArgs e)
        {         
            ((OleMenuCommand)sender).Visible = _package.HasSolutionDir() && _package.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = _package.HasSolutionDir() && _package.BranchName.StartsWith(_package.FlowOptions.FeaturePrefix);
        }

        public void Hotfix_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Visible = _package.HasSolutionDir() && _package.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = _package.HasSolutionDir() && _package.BranchName.StartsWith(_package.FlowOptions.HotfixPrefix);
        }

        public void Release_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Visible = _package.HasSolutionDir() && _package.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = _package.HasSolutionDir() && _package.BranchName.StartsWith(_package.FlowOptions.ReleasePrefix);
        }

        public void Solution_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Enabled = _package.HasSolutionDir();
        }

        public void SolutionVisibility_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = _package.HasSolutionDir();
        }

        public void GitFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = _package.HasSolutionDir() && _package.IsGitFlow;
        }

        public void GitHubFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = _package.HasSolutionDir() && !_package.IsGitFlow;
        }
    }
}
