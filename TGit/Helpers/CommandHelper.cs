using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit.Helpers
{
    public static class CommandHelper
    {
        public static void AddCommand(OleMenuCommandService mcs, EventHandler handler, uint commandId)
        {
            mcs.AddCommand(CreateCommand(handler, commandId));
        }

        public static void AddCommand(OleMenuCommandService mcs, MenuCommand command)
        {
            mcs.AddCommand(command);
        }

        public static OleMenuCommand CreateCommand(EventHandler handler, uint commandId)
        {
            var menuCommandId = new CommandID(GuidList.GuidTgitCmdSet, (int)commandId);
            var menuItem = new OleMenuCommand(handler, menuCommandId);
            menuItem.BeforeQueryStatus += Solution_BeforeQueryStatus;
            return menuItem;
        }

        public static OleMenuCommand CreateCommand(uint commandId)
        {
            return CreateCommand(null, commandId);
        }

        public static void ApplyStash_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Enabled = EnvHelper.HasStash;
        }

        public static void Feature_BeforeQueryStatus(object sender, EventArgs e)
        {        
            ((OleMenuCommand)sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasSolutionDir() && EnvHelper.BranchName.StartsWith(EnvHelper.FlowOptions.FeaturePrefix);
        }

        public static void Hotfix_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasSolutionDir() && EnvHelper.BranchName.StartsWith(EnvHelper.FlowOptions.HotfixPrefix);
        }

        public static void Release_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand)sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasSolutionDir() && EnvHelper.BranchName.StartsWith(EnvHelper.FlowOptions.ReleasePrefix);
        }

        public static void Solution_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Enabled = EnvHelper.HasSolutionDir();
        }

        public static void SolutionVisibility_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasSolutionDir();
        }

        public static void GitFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasSolutionDir() && EnvHelper.IsGitFlow;
        }

        public static void GitHubFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasSolutionDir() && !EnvHelper.IsGitFlow;
        }
    }
}
