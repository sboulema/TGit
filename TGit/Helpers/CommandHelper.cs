using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit.Helpers
{
    public class CommandHelper
    {
        private readonly ProcessHelper _processHelper;
        private readonly FileHelper _fileHelper;
        private readonly GitHelper _gitHelper;
        private readonly OleMenuCommandService _mcs;

        public CommandHelper(ProcessHelper processHelper, FileHelper fileHelper, GitHelper gitHelper, OleMenuCommandService mcs)
        {
            _processHelper = processHelper;
            _fileHelper = fileHelper;
            _gitHelper = gitHelper;
            _mcs = mcs;
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
            var hasSolution = !string.IsNullOrEmpty(_fileHelper.GetSolutionDir());
            ((OleMenuCommand)sender).Visible = hasSolution && _gitHelper.IsGitFlow();
            ((OleMenuCommand)sender).Enabled = hasSolution && _gitHelper.IsFeatureBranch();
        }

        public void Hotfix_BeforeQueryStatus(object sender, EventArgs e)
        {
            var hasSolution = !string.IsNullOrEmpty(_fileHelper.GetSolutionDir());
            ((OleMenuCommand)sender).Visible = hasSolution && _gitHelper.IsGitFlow();
            ((OleMenuCommand)sender).Enabled = hasSolution && _gitHelper.IsHotfixBranch();
        }

        public void Release_BeforeQueryStatus(object sender, EventArgs e)
        {
            var hasSolution = !string.IsNullOrEmpty(_fileHelper.GetSolutionDir());
            ((OleMenuCommand)sender).Visible = hasSolution && _gitHelper.IsGitFlow();
            ((OleMenuCommand)sender).Enabled = hasSolution && _gitHelper.IsReleaseBranch();
        }

        public void Solution_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Enabled = !string.IsNullOrEmpty(_fileHelper.GetSolutionDir());
        }

        public void SolutionVisibility_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = !string.IsNullOrEmpty(_fileHelper.GetSolutionDir());
        }

        public void GitFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = !string.IsNullOrEmpty(_fileHelper.GetSolutionDir()) && _gitHelper.IsGitFlow();
        }

        public void GitHubFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = !string.IsNullOrEmpty(_fileHelper.GetSolutionDir()) && _gitHelper.IsGitHubFlow();
        }
    }
}
