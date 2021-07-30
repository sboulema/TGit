using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using SamirBoulema.TGit.Helpers;
using Task = System.Threading.Tasks.Task;

namespace SamirBoulema.TGit.Commands
{
    [Command(PackageGuids.guidTGitCmdSetString, PackageIds.diskBrowserContext)]
    internal sealed class DiskBrowserFileCommand : BaseCommand<DiskBrowserFileCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            ProcessHelper.Start(await FileHelper.GetActiveDocumentFilePath());
        }
    }
}
