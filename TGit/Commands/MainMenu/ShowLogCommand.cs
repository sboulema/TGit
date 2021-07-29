using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using System.ComponentModel.Design;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.ShowLog)]
    internal sealed class ShowLogCommand : BaseCommand<ShowLogCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await KnownCommands.File_SaveSelectedItems.ExecuteAsync();

            await ProcessHelper.RunTortoiseGitCommand(Package, "log");
        }
    }
}
