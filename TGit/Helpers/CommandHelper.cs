using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

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

        public static async Task<bool> SaveFiles()
        {
            var options = await General.GetLiveInstanceAsync();

            if (!options.SaveFiles)
            {
                return false;
            }

            return await KnownCommands.File_SaveAll.ExecuteAsync();
        }
    }
}
