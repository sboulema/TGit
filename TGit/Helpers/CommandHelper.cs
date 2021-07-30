using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit.Helpers
{
    public static class CommandHelper
    {
        public static void AddMenuCommand(IMenuCommandService menuCommandService, int commandId, EventHandler eventHandler)
        {
            var menuCommand = new OleMenuCommand(null, new CommandID(PackageGuids.guidTGitCmdSet, commandId));
            menuCommand.BeforeQueryStatus += eventHandler;
            menuCommandService.AddCommand(menuCommand);
        }

        public static async void SolutionVisibility_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = await FileHelper.HasSolutionDir();
        }

        public static async void GitHubFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = await FileHelper.HasSolutionDir() && !await GitHelper.IsGitFlow();
        }

        public static async void GitSvn_BeforeQueryStatus(object sender, EventArgs e) 
            => ((OleMenuCommand)sender).Visible = await FileHelper.HasSolutionDir() && await GitHelper.IsGitSvn();
    }
}
