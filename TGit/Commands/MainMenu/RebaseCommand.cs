using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.rebase)]
    internal sealed class RebaseCommand : BaseCommand<RebaseCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await CommandHelper.SaveFiles();

            await ProcessHelper.RunTortoiseGitCommand("rebase");
        }
    }
}
