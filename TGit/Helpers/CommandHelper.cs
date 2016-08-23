using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit.Helpers
{
    public class CommandHelper
    {
        private readonly OleMenuCommandService _mcs;

        public CommandHelper(OleMenuCommandService mcs)
        {
            _mcs = mcs;
        }

        public void AddCommand(EventHandler handler, uint commandId)
        {
            _mcs.AddCommand(CreateCommand(handler, commandId));
        }

        public void AddCommand(MenuCommand command)
        {
            _mcs.AddCommand(command);
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
            ((OleMenuCommand) sender).Enabled = EnvHelper.HasStash;
        }

        public void Feature_BeforeQueryStatus(object sender, EventArgs e)
        {        
            ((OleMenuCommand)sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasSolutionDir() && EnvHelper.BranchName.StartsWith(EnvHelper.FlowOptions.FeaturePrefix);
        }

        public void Hotfix_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasSolutionDir() && EnvHelper.BranchName.StartsWith(EnvHelper.FlowOptions.HotfixPrefix);
        }

        public void Release_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasSolutionDir() && EnvHelper.BranchName.StartsWith(EnvHelper.FlowOptions.ReleasePrefix);
        }

        public void Solution_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Enabled = EnvHelper.HasSolutionDir();
        }

        public void SolutionVisibility_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasSolutionDir();
        }

        public void GitFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
        }

        public void GitHubFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasSolutionDir() && !EnvHelper.IsGitFlow;
        }
    }
}
