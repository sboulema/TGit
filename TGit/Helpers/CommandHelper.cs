using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;

namespace SamirBoulema.TGit.Helpers
{
    public static class CommandHelper
    {
        public static void AddMenuCommand(IMenuCommandService menuCommandService, int commandId, EventHandler eventHandler)
        {
            var menuCommand = new OleMenuCommand(null, new CommandID(GuidList.GuidTgitCmdSet, commandId));
            menuCommand.BeforeQueryStatus += eventHandler;
            menuCommandService.AddCommand(menuCommand);
        }

        public static void SolutionVisibility_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = FileHelper.HasSolutionDir().Result;
        }

        public static void GitHubFlow_BeforeQueryStatus(object sender, EventArgs e)
        {
            ((OleMenuCommand) sender).Visible = FileHelper.HasSolutionDir().Result && !GitHelper.IsGitFlow().Result;
        }

        public static void GitSvn_BeforeQueryStatus(object sender, EventArgs e) 
            => ((OleMenuCommand)sender).Visible = FileHelper.HasSolutionDir().Result && GitHelper.IsGitSvn().Result;
    }
}
