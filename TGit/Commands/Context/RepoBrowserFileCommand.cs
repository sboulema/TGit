using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(GuidList.GuidTgitCmdSetString, PkgCmdIDList.RepoBrowserContext)]
    internal sealed class RepoBrowserFileCommand : BaseCommand<RepoBrowserFileCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ProcessHelper.RunTortoiseGitFileCommand(Package, "repobrowser");
        }
    }
}
