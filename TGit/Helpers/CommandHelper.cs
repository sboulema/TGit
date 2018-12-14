using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit.Helpers
{
    public static class CommandHelper
    {
        public static EnvHelper EnvHelper;

        public static void AddCommand(OleMenuCommandService mcs, EventHandler handler, uint commandId, EventHandler eventHandler)
        {
            mcs.AddCommand(CreateCommand(handler, commandId, eventHandler));
        }

        public static void AddCommand(OleMenuCommandService mcs, EventHandler handler, uint commandId)
        {
            mcs.AddCommand(CreateCommand(handler, commandId));
        }

        public static void AddCommand(OleMenuCommandService mcs, MenuCommand command)
        {
            mcs.AddCommand(command);
        }

        public static OleMenuCommand CreateCommand(EventHandler handler, uint commandId, EventHandler eventHandler = null)
        {
            var menuCommandId = new CommandID(GuidList.GuidTgitCmdSet, (int)commandId);
            var menuItem = new OleMenuCommand(handler, menuCommandId);

            if (eventHandler != null)
            {
                menuItem.BeforeQueryStatus += eventHandler;
            }
            
            return menuItem;
        }

        public static OleMenuCommand CreateCommand(uint commandId)
        {
            return CreateCommand(null, commandId);
        }

        public static void ApplyStash_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Enabled = EnvHelper.HasStash();
        }

        public static void Feature_BeforeQueryStatus(object sender, EventArgs e)
        {
            var gitConfig = EnvHelper.GetGitConfig();
            ((OleMenuCommand)sender).Visible = EnvHelper.HasGitRoot() && EnvHelper.IsGitFlow();
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasGitRoot() && EnvHelper.BranchNameStartsWith(gitConfig.FeaturePrefix);
        }

        public static void Hotfix_BeforeQueryStatus(object sender, EventArgs e)
        {
            var gitConfig = EnvHelper.GetGitConfig();
            ((OleMenuCommand)sender).Visible = EnvHelper.HasGitRoot() && EnvHelper.IsGitFlow();
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasGitRoot() && EnvHelper.BranchNameStartsWith(gitConfig.HotfixPrefix);
        }

        public static void Release_BeforeQueryStatus(object sender, EventArgs e)
        {
            var gitConfig = EnvHelper.GetGitConfig();
            ((OleMenuCommand)sender).Visible = EnvHelper.HasGitRoot() && EnvHelper.IsGitFlow();
            ((OleMenuCommand)sender).Enabled = EnvHelper.HasGitRoot() && EnvHelper.BranchNameStartsWith(gitConfig.ReleasePrefix);
        }

        public static void SolutionVisibility_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasGitRoot();
        }

        public static void GitFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasGitRoot() && EnvHelper.IsGitFlow();
        }

        public static void GitHubFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = EnvHelper.HasGitRoot() && !EnvHelper.IsGitFlow();
        }

        public static void GitSvn_BeforeQueryStatus(object sender, EventArgs e) 
            => ((OleMenuCommand)sender).Visible = EnvHelper.HasGitRoot() && EnvHelper.IsGitSvn();
    }
}
